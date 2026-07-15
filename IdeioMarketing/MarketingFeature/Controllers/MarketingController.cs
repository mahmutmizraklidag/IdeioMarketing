using System.Text.Json;
using IdeioMarketing.MarketingFeature.Data;
using IdeioMarketing.MarketingFeature.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdeioMarketing.Controllers
{
    public class MarketingController : Controller
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly IConfiguration _configuration;

        public MarketingController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View("~/MarketingFeature/Views/Marketing/Index.cshtml");
        }

        [HttpGet]
        public async Task<IActionResult> StorageGet(string? key)
        {
            await using var context = CreateContext();
            var leads = await context.MarketingLeads
                .AsNoTracking()
                .Include(x => x.Source)
                .Include(x => x.Status)
                .Include(x => x.Temperature)
                .Include(x => x.Stage)
                .Include(x => x.LeadOwners)
                    .ThenInclude(x => x.MarketingOwner)
                .OrderBy(x => x.SortOrder)
                .ThenBy(x => x.Id)
                .ToListAsync();

            var payload = leads.Select(ToStoragePayload).ToList();
            return Json(new { value = JsonSerializer.Serialize(payload, JsonOptions) });
        }

        [HttpPost]
        public async Task<IActionResult> StorageSet([FromBody] MarketingStorageSetRequest request)
        {
            await using var context = CreateContext();
            var incoming = string.IsNullOrWhiteSpace(request.Value)
                ? new List<MarketingLeadStoragePayload>()
                : JsonSerializer.Deserialize<List<MarketingLeadStoragePayload>>(request.Value, JsonOptions) ?? new List<MarketingLeadStoragePayload>();

            await SyncStorageLeads(context, incoming);
            await context.SaveChangesAsync();

            return Json(new { success = true });
        }

        private MarketingDatabaseContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<MarketingDatabaseContext>()
                .UseSqlServer(GetMarketingConnectionString(_configuration))
                .Options;

            return new MarketingDatabaseContext(options);
        }

        private static string GetMarketingConnectionString(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("SqlServer") ?? string.Empty;
            return connectionString.Contains("Encrypt=", StringComparison.OrdinalIgnoreCase)
                ? connectionString
                : connectionString.TrimEnd(';') + ";Encrypt=False;";
        }

        private static async Task SyncStorageLeads(MarketingDatabaseContext context, List<MarketingLeadStoragePayload> incoming)
        {
            var now = DateTime.Now;
            var stages = await context.MarketingStages.ToDictionaryAsync(x => x.Key);
            var statuses = await context.MarketingLeadStatuses.ToDictionaryAsync(x => x.Label);
            var temperatures = await context.MarketingLeadTemperatures.ToDictionaryAsync(x => x.Key);
            var sources = await context.MarketingSources.ToDictionaryAsync(x => x.Name);
            var owners = await context.MarketingOwners.ToDictionaryAsync(x => x.Name);

            var existing = await context.MarketingLeads
                .Include(x => x.LeadOwners)
                .ToDictionaryAsync(x => x.ExternalId);

            var incomingIds = incoming
                .Where(x => !string.IsNullOrWhiteSpace(x.Id))
                .Select(x => x.Id!.Trim())
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var deleted = existing.Values.Where(x => !incomingIds.Contains(x.ExternalId)).ToList();
            context.MarketingLeads.RemoveRange(deleted);

            for (var i = 0; i < incoming.Count; i++)
            {
                var item = incoming[i];
                if (string.IsNullOrWhiteSpace(item.Company))
                {
                    continue;
                }

                var externalId = string.IsNullOrWhiteSpace(item.Id)
                    ? "l" + Guid.NewGuid().ToString("N")[..7]
                    : item.Id.Trim();

                if (!existing.TryGetValue(externalId, out var lead))
                {
                    lead = new MarketingLead
                    {
                        ExternalId = externalId,
                        CreatedAt = now
                    };
                    context.MarketingLeads.Add(lead);
                    existing[externalId] = lead;
                }

                lead.Company = item.Company.Trim();
                lead.Contact = item.Contact?.Trim() ?? string.Empty;
                lead.Email = item.Email?.Trim() ?? string.Empty;
                lead.SourceId = ResolveSourceId(item.Source, sources);
                lead.StatusId = ResolveStatusId(item.Status, statuses);
                lead.TemperatureId = ResolveTemperatureId(item.Temp, temperatures);
                lead.StageId = ResolveStageId(item.Stage, stages);
                lead.Value = item.Value;
                lead.Date = ResolveDate(item.Date);
                lead.Note = item.Note?.Trim() ?? string.Empty;
                lead.SortOrder = i + 1;
                lead.IsInPipeline = MarketingPipelineVisibility.Resolve(item.InPipeline);
                lead.UpdatedAt = now;

                var ownerNames = (item.Owners ?? new List<string>())
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(x => x.Trim())
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                if (ownerNames.Count == 0)
                {
                    ownerNames.Add(owners.Keys.OrderBy(x => owners[x].SortOrder).First());
                }

                var desiredOwners = new List<(MarketingOwner Owner, int SortOrder)>();
                foreach (var ownerName in ownerNames.Select((name, index) => new { name, index }))
                {
                    if (!owners.TryGetValue(ownerName.name, out var owner))
                    {
                        owner = new MarketingOwner
                        {
                            Name = ownerName.name,
                            Color = "#7E7C86",
                            SortOrder = owners.Count + 1
                        };
                        context.MarketingOwners.Add(owner);
                        owners[ownerName.name] = owner;
                    }

                    desiredOwners.Add((owner, ownerName.index + 1));
                }

                var currentLinks = lead.LeadOwners.ToList();
                var desiredExistingOwnerIds = desiredOwners
                    .Where(x => x.Owner.Id > 0)
                    .Select(x => x.Owner.Id)
                    .ToHashSet();

                foreach (var removedLink in currentLinks.Where(x => !desiredExistingOwnerIds.Contains(x.MarketingOwnerId)))
                {
                    context.MarketingLeadOwners.Remove(removedLink);
                    lead.LeadOwners.Remove(removedLink);
                }

                foreach (var desiredOwner in desiredOwners)
                {
                    var currentLink = desiredOwner.Owner.Id > 0
                        ? currentLinks.FirstOrDefault(x => x.MarketingOwnerId == desiredOwner.Owner.Id)
                        : null;

                    if (currentLink != null)
                    {
                        currentLink.SortOrder = desiredOwner.SortOrder;
                        continue;
                    }

                    lead.LeadOwners.Add(new MarketingLeadOwner
                    {
                        MarketingLead = lead,
                        MarketingOwner = desiredOwner.Owner,
                        SortOrder = desiredOwner.SortOrder
                    });
                }
            }
        }

        private static object ToStoragePayload(MarketingLead lead)
        {
            return new
            {
                id = lead.ExternalId,
                company = lead.Company,
                contact = lead.Contact ?? string.Empty,
                email = lead.Email ?? string.Empty,
                source = lead.Source.Name,
                status = lead.Status.Label,
                temp = lead.Temperature.Key,
                value = lead.Value,
                owners = lead.LeadOwners
                    .OrderBy(x => x.SortOrder)
                    .Select(x => x.MarketingOwner.Name)
                    .ToList(),
                stage = lead.Stage.Key,
                inPipeline = lead.IsInPipeline,
                date = lead.Date.ToString("yyyy-MM-dd"),
                note = lead.Note ?? string.Empty
            };
        }

        private static int ResolveSourceId(string? value, Dictionary<string, MarketingSource> sources)
        {
            return !string.IsNullOrWhiteSpace(value) && sources.TryGetValue(value.Trim(), out var source)
                ? source.Id
                : sources["Referans"].Id;
        }

        private static int ResolveStatusId(string? value, Dictionary<string, MarketingLeadStatus> statuses)
        {
            return !string.IsNullOrWhiteSpace(value) && statuses.TryGetValue(value.Trim(), out var status)
                ? status.Id
                : statuses["Düzenli İş"].Id;
        }

        private static int ResolveTemperatureId(string? value, Dictionary<string, MarketingLeadTemperature> temperatures)
        {
            return !string.IsNullOrWhiteSpace(value) && temperatures.TryGetValue(value.Trim(), out var temperature)
                ? temperature.Id
                : temperatures["sicak"].Id;
        }

        private static int ResolveStageId(string? value, Dictionary<string, MarketingStage> stages)
        {
            return !string.IsNullOrWhiteSpace(value) && stages.TryGetValue(value.Trim(), out var stage)
                ? stage.Id
                : stages["new"].Id;
        }

        private static DateTime ResolveDate(string? value)
        {
            return DateTime.TryParse(value, out var date)
                ? date.Date
                : DateTime.Today;
        }

        public class MarketingStorageSetRequest
        {
            public string? Key { get; set; }
            public string? Value { get; set; }
        }

        public class MarketingLeadStoragePayload
        {
            public string? Id { get; set; }
            public string? Company { get; set; }
            public string? Contact { get; set; }
            public string? Email { get; set; }
            public string? Source { get; set; }
            public string? Status { get; set; }
            public string? Temp { get; set; }
            public decimal Value { get; set; }
            public List<string>? Owners { get; set; }
            public string? Stage { get; set; }
            public bool? InPipeline { get; set; }
            public string? Date { get; set; }
            public string? Note { get; set; }
        }
    }
}
