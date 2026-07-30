using System.Text.Json;
using IdeioMarketing.MarketingFeature.Content;
using IdeioMarketing.MarketingFeature.Entities;

var tests = new (string Name, Action Run)[]
{
    ("Yeni müşteri pipeline'a otomatik eklenir", NewLeadStartsInPipeline),
    ("Eski payload pipeline'da görünür kalır", LegacyPayloadDefaultsToPipeline),
    ("Pipeline'dan kaldırma değeri korunur", RemovedPayloadStaysOutOfPipeline),
    ("Pipeline görünürlüğü mevcut not alanında korunur", PipelineVisibilityRoundTripsThroughNote),
    ("Arayüz kaldırma ve yeniden ekleme kontrollerini içerir", HtmlContainsPipelineControls),
    ("Pipeline filtreleri birlikte ve anlık çalışır", HtmlContainsLinkedPipelineFilters),
    ("Pipeline kartı tüm sorumluların baş harflerini gösterir", HtmlShowsEveryPipelineOwner),
    ("HTML enjeksiyonu kaynak içeriği korur", HtmlEnhancementPreservesSource),
    ("Marketing kaydı tarayıcı storage'ına düşmez", PersistenceUsesDatabaseOnly),
    ("Mobil menü açma ve kapatma kontrollerini içerir", MobileNavigationProvidesToggle),
};

var failed = 0;
foreach (var test in tests)
{
    try
    {
        test.Run();
        Console.WriteLine($"PASS  {test.Name}");
    }
    catch (Exception exception)
    {
        failed++;
        Console.Error.WriteLine($"FAIL  {test.Name}: {exception.Message}");
    }
}

Console.WriteLine($"{tests.Length - failed}/{tests.Length} test başarılı.");
return failed == 0 ? 0 : 1;

static void NewLeadStartsInPipeline()
{
    Equal(true, new MarketingLead().IsInPipeline);
}

static void LegacyPayloadDefaultsToPipeline()
{
    var payload = JsonSerializer.Deserialize<TestPayload>("{\"company\":\"Eski Müşteri\"}");
    Equal(true, MarketingPipelineVisibility.Resolve(payload?.InPipeline));
}

static void RemovedPayloadStaysOutOfPipeline()
{
    var payload = JsonSerializer.Deserialize<TestPayload>("{\"company\":\"Müşteri\",\"inPipeline\":false}", new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    });
    Equal(false, MarketingPipelineVisibility.Resolve(payload?.InPipeline));
}

static void PipelineVisibilityRoundTripsThroughNote()
{
    var stored = MarketingPipelineVisibility.EncodeNote("Müşteri notu", false);

    Equal(false, MarketingPipelineVisibility.ResolveFromNote(stored));
    Equal("Müşteri notu", MarketingPipelineVisibility.DecodeNote(stored));
    Equal("Müşteri notu", MarketingPipelineVisibility.EncodeNote(stored, true));
}

static void HtmlContainsPipelineControls()
{
    const string source = "<html><head></head><body><main>kaynak</main></body></html>";
    var result = MarketingPipelineHtmlEnhancement.Apply(source);

    Contains(result, "data-pipeline-remove");
    Contains(result, "data-pipeline-add");
    Contains(result, "inPipeline:lead?isInPipeline(lead):true");
    Contains(result, "filtered.filter(l=>l.stage===s.id)");
}

static void HtmlContainsLinkedPipelineFilters()
{
    const string source = "<html><head></head><body><main>kaynak</main></body></html>";
    var result = MarketingPipelineHtmlEnhancement.Apply(source);

    Contains(result, "pipelineOwnerFilter");
    Contains(result, "pipelineStatusFilter");
    Contains(result, "pipelineMonthFilter");
    Contains(result, "pipelineOwnersOf(l).includes(pipelineFilters.owner)");
    Contains(result, "l.status === pipelineFilters.status");
    Contains(result, "monthKey(l) === pipelineFilters.month");
    Contains(result, "pipelineFilters.owner=e.target.value;render();");
    Contains(result, "pipelineFilters.status=e.target.value;render();");
    Contains(result, "pipelineFilters.month=e.target.value;render();");
}

static void HtmlShowsEveryPipelineOwner()
{
    const string source = "<html><head></head><body><main>kaynak</main></body></html>";
    var result = MarketingPipelineHtmlEnhancement.Apply(source);

    Contains(result, "pipelineOwnerChips(l)");
    Contains(result, "pipelineOwnersOf(lead).map(owner=>");
    Contains(result, "pipeline-owner-chips");
    Contains(result, "lead.owner2");
}

static void HtmlEnhancementPreservesSource()
{
    const string source = "<html><head><title>Test</title></head><body><main>korunacak</main></body></html>";
    var result = MarketingPipelineHtmlEnhancement.Apply(source);

    Contains(result, "<title>Test</title>");
    Contains(result, "<main>korunacak</main>");
    Equal(1, Count(result, "pipeline-visibility-script"));
    Equal(1, Count(result, "pipeline-visibility-styles"));
}

static void PersistenceUsesDatabaseOnly()
{
    const string source = """
<script>
const Store={
  async load(){return null;}
};
const S={leads:SEED.slice(),view:"dashboard",q:"",fStage:"all",fOwner:"all",fStatus:"all",fMonth:"all",perfMonth:"all",salesMode:"toplam"};
let dragId=null;
function commit(){Store.save(S.leads);render();}
</script>
""";

    var result = MarketingDatabasePersistenceHtmlEnhancement.Apply(source);

    Contains(result, "await window.storage.set");
    Contains(result, "async function commit()");
    DoesNotContain(result, "localStorage");
}

static void MobileNavigationProvidesToggle()
{
    const string source = """
<html><head><title>Test</title></head><body>
<div class="app">
  <aside class="sidebar"><nav id="nav"></nav></aside>
  <main class="main"><header class="topbar"><h1>Başlık</h1></header></main>
</div>
</body></html>
""";

    var result = MarketingMobileNavigationHtmlEnhancement.Apply(source);

    Contains(result, "id=\"mobileMenuToggle\"");
    Contains(result, "aria-controls=\"marketingSidebar\"");
    Contains(result, "id=\"mobileMenuBackdrop\"");
    Contains(result, "body.mobile-menu-open .sidebar{transform:translateX(0);}");
    Contains(result, "event.target.closest(\".nav-btn\")");
    Contains(result, "event.key===\"Escape\"");
    Contains(result, "<title>Test</title>");
    Equal(1, Count(result, "mobile-navigation-script"));
    Equal(1, Count(result, "mobile-navigation-styles"));

    var renderedPage = MarketingHtmlTemplate.Render("\"test-token\"");
    Contains(renderedPage, "id=\"mobileMenuToggle\"");
    Contains(renderedPage, "id=\"marketingSidebar\"");
    Contains(renderedPage, "id=\"mobileMenuBackdrop\"");
    Equal(1, Count(renderedPage, "mobile-navigation-script"));
    Equal(1, Count(renderedPage, "mobile-navigation-styles"));
}

static void Equal<T>(T expected, T actual)
{
    if (!EqualityComparer<T>.Default.Equals(expected, actual))
    {
        throw new InvalidOperationException($"Beklenen: {expected}; gerçekleşen: {actual}");
    }
}

static void Contains(string value, string expected)
{
    if (!value.Contains(expected, StringComparison.Ordinal))
    {
        throw new InvalidOperationException($"Beklenen içerik bulunamadı: {expected}");
    }
}

static void DoesNotContain(string value, string unexpected)
{
    if (value.Contains(unexpected, StringComparison.Ordinal))
    {
        throw new InvalidOperationException($"Beklenmeyen içerik bulundu: {unexpected}");
    }
}

static int Count(string value, string expected)
{
    var count = 0;
    var index = 0;
    while ((index = value.IndexOf(expected, index, StringComparison.Ordinal)) >= 0)
    {
        count++;
        index += expected.Length;
    }

    return count;
}

sealed class TestPayload
{
    public string? Company { get; set; }
    public bool? InPipeline { get; set; }
}
