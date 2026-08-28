using System.Text.Json;
using IdeioMarketing.MarketingFeature.Content;
using IdeioMarketing.MarketingFeature.Data;
using IdeioMarketing.MarketingFeature.Entities;

var tests = new (string Name, Action Run)[]
{
    ("Yeni müşteri pipeline'a otomatik eklenir", NewLeadStartsInPipeline),
    ("Eski payload pipeline'da görünür kalır", LegacyPayloadDefaultsToPipeline),
    ("Pipeline'dan kaldırma değeri korunur", RemovedPayloadStaysOutOfPipeline),
    ("Pipeline görünürlüğü mevcut not alanında korunur", PipelineVisibilityRoundTripsThroughNote),
    ("Sözleşme bilgisi mevcut not alanında korunur", ContractStatusRoundTripsThroughNote),
    ("Sözleşme tarihleri ve daha sonra durumu korunur", ContractDatesAndDeferredStatusRoundTrip),
    ("Kaybedilme sebebi mevcut not alanında korunur", LossReasonRoundTripsThroughNote),
    ("Arayüz kaldırma ve yeniden ekleme kontrollerini içerir", HtmlContainsPipelineControls),
    ("Satış tamamlandı kayıtlarında sözleşme kontrolü gösterilir", HtmlContainsContractControls),
    ("Pipeline satış tamamlandı popup akışını içerir", PipelineShowsWonContractPrompt),
    ("Pipeline kaybedildi sebebi popup akışını içerir", PipelineShowsLostReasonPrompt),
    ("Ekip performansı kaybedilen müşteri istatistiklerini içerir", TeamPerformanceShowsLostCustomerStats),
    ("Ana sayfa yaklaşan sözleşme kartı ve listesini içerir", DashboardShowsExpiringContracts),
    ("Pipeline filtreleri birlikte ve anlık çalışır", HtmlContainsLinkedPipelineFilters),
    ("Pipeline kartı tüm sorumluların baş harflerini gösterir", HtmlShowsEveryPipelineOwner),
    ("Reddedildi aşaması form ve pipeline'da bulunur", RejectedStageIsAvailableEverywhere),
    ("Pipeline dar ekranlara uyumlu yatay akış içerir", PipelineIsResponsive),
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

static void ContractStatusRoundTripsThroughNote()
{
    var stored = MarketingPipelineVisibility.EncodeNote("Müşteri notu", false, true);

    Equal(false, MarketingPipelineVisibility.ResolveFromNote(stored));
    Equal(true, MarketingPipelineVisibility.ResolveHasContractFromNote(stored));
    Equal("Müşteri notu", MarketingPipelineVisibility.DecodeNote(stored));
    Equal(false, MarketingPipelineVisibility.ResolveHasContractFromNote(
        MarketingPipelineVisibility.EncodeNote(stored, true, false)));
}

static void ContractDatesAndDeferredStatusRoundTrip()
{
    var stored = MarketingPipelineVisibility.EncodeNote(
        "Müşteri notu",
        true,
        true,
        "2026-09-01",
        "2027-08-31",
        false);

    Equal("2026-09-01", MarketingPipelineVisibility.ResolveContractStartDateFromNote(stored));
    Equal("2027-08-31", MarketingPipelineVisibility.ResolveContractEndDateFromNote(stored));
    Equal(false, MarketingPipelineVisibility.ResolveContractDeferredFromNote(stored));
    Equal("Müşteri notu", MarketingPipelineVisibility.DecodeNote(stored));

    var datesWithoutContract = MarketingPipelineVisibility.EncodeNote(
        "Müşteri notu",
        true,
        false,
        "2026-10-01",
        "2027-09-30",
        false);
    Equal(false, MarketingPipelineVisibility.ResolveHasContractFromNote(datesWithoutContract));
    Equal("2026-10-01", MarketingPipelineVisibility.ResolveContractStartDateFromNote(datesWithoutContract));
    Equal("2027-09-30", MarketingPipelineVisibility.ResolveContractEndDateFromNote(datesWithoutContract));

    var deferred = MarketingPipelineVisibility.EncodeNote("Müşteri notu", true, false, null, null, true);
    Equal(false, MarketingPipelineVisibility.ResolveHasContractFromNote(deferred));
    Equal(true, MarketingPipelineVisibility.ResolveContractDeferredFromNote(deferred));
    Equal("Müşteri notu", MarketingPipelineVisibility.DecodeNote(deferred));
}

static void LossReasonRoundTripsThroughNote()
{
    const string reason = "Sözleşme Yenilenmedi";
    var stored = MarketingPipelineVisibility.EncodeNote(
        "Müşteri notu",
        true,
        false,
        null,
        null,
        false,
        reason);

    Equal(reason, MarketingPipelineVisibility.ResolveLossReasonFromNote(stored));
    Equal("Müşteri notu", MarketingPipelineVisibility.DecodeNote(stored));

    var invalid = MarketingPipelineVisibility.EncodeNote("Not", true, false, null, null, false, "Geçersiz sebep");
    Equal<string?>(null, MarketingPipelineVisibility.ResolveLossReasonFromNote(invalid));
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

static void HtmlContainsContractControls()
{
    const string source = "<html><head></head><body><main>kaynak</main></body></html>";
    var result = MarketingPipelineHtmlEnhancement.Apply(source);

    Contains(result, "id=\"f-has-contract\"");
    Contains(result, "Sözleşmesi var mı?");
    Contains(result, "f.stage===\"won\"");
    Contains(result, "id=\"f-contract-start\"");
    Contains(result, "id=\"f-contract-end\"");
    Contains(result, "contractStartDate,contractEndDate,contractDeferred:false");
    Contains(result, "contractStartDate=stage===\"won\"?g(\"f-contract-start\")");
    Contains(result, "<th>Sözleşme</th>");
    Contains(result, "contractCell(l)");
    Contains(result, "Sözleşme var");
    Contains(result, "Sözleşme yok");
    Contains(result, "contractDatesAreValid");
}

static void PipelineShowsWonContractPrompt()
{
    const string source = "<html><head></head><body><main>kaynak</main></body></html>";
    var result = MarketingPipelineHtmlEnhancement.Apply(source);

    Contains(result, "openWonContractPrompt(l)");
    Contains(result, "id=\"contract-prompt-has-contract\"");
    Contains(result, "id=\"contract-prompt-start\"");
    Contains(result, "id=\"contract-prompt-end\"");
    Contains(result, "Daha sonra gireceğim");
    Contains(result, "lead.contractDeferred=deferred");
    Contains(result, "if(col.dataset.col===\"won\")");
    Contains(result, "Daha sonra girilecek");
    Contains(result, "contractStartDate=deferred?\"\":startInput.value");
    DoesNotContain(result, "id=\"contract-prompt-start\" type=\"date\" disabled");
    DoesNotContain(result, "checkbox.onchange=syncDateInputs");
}

static void PipelineShowsLostReasonPrompt()
{
    const string source = "<html><head></head><body><main>kaynak</main></body></html>";
    var result = MarketingPipelineHtmlEnhancement.Apply(source);

    Contains(result, "openLostReasonPrompt(l)");
    Contains(result, "id=\"loss-prompt-reason\"");
    Contains(result, "if(col.dataset.col===\"lost\")");
    Contains(result, "if(!lossReason)return");
    Contains(result, "lead.lossReason=lossReason");
    Contains(result, "id=\"f-loss-reason\"");
    Contains(result, "Müşteri Vazgeçti");
    Contains(result, "Sözleşme Yenilenmedi");
    Contains(result, "Sözleşme Müşteri Tarafından Fesedildi");
    Contains(result, "Sözleşme Firma Tarafından Fes Edildi");
    Contains(result, "lossReasonBadge(l)");
}

static void TeamPerformanceShowsLostCustomerStats()
{
    const string source = "<html><head></head><body><main id=\"content\"></main></body></html>";
    var result = MarketingPipelineHtmlEnhancement.Apply(source);

    Contains(result, "data-team-loss-mode=\"true\"");
    Contains(result, "Kaybedilen Müşteriler</button>");
    Contains(result, "const viewTeamLosses");
    Contains(result, "lead.stage===\"lost\"");
    Contains(result, "Kaybedilme sebeplerine göre müşteriler");
    Contains(result, "Sorumluya göre kaybedilen müşteriler");
    Contains(result, "Hangi müşteri neden kaybedildi?");
    Contains(result, "Sebep girilmemiş");
    Contains(result, "S.teamLossMode=true;render();");
    Contains(result, "bindTeamPerformanceModes()");
}

static void DashboardShowsExpiringContracts()
{
    const string source = "<html><head></head><body><main id=\"content\"></main><div id=\"modal-root\"></div></body></html>";
    var result = MarketingPipelineHtmlEnhancement.Apply(source);

    Contains(result, "const contractsExpiringSoon");
    Contains(result, "lead.stage===\"won\"&&lead.contractEndDate");
    Contains(result, "limit.setMonth(limit.getMonth()+1)");
    Contains(result, "item.endDate>=today&&item.endDate<=limit");
    Contains(result, "id=\"contractExpiryKpi\"");
    Contains(result, "count>0?\"has-expiring\"");
    Contains(result, "contract-alarm-dot");
    Contains(result, "contract-alarm-pulse");
    Contains(result, "Sözleşmesi yakında bitecek");
    Contains(result, "openExpiringContractsPrompt");
    Contains(result, "id=\"contract-expiry-overlay\"");
    Contains(result, "Bitiş tarihi");
    Contains(result, "Kalan");
    Contains(result, "if(S.view===\"dashboard\")addDashboardContractExpiryCard();");
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

static void RejectedStageIsAvailableEverywhere()
{
    const string source = "<html><head></head><body><main>kaynak</main></body></html>";
    var result = MarketingPipelineHtmlEnhancement.Apply(source);
    var rejected = MarketingSeedData.Stages.Single(x => x.Key == "rejected");

    Equal("Reddedildi", rejected.Label);
    Equal(7, rejected.SortOrder);
    Contains(result, "STAGES.push({id:\"rejected\",label:\"Reddedildi\"");
    Contains(result, "<option value=\"rejected\" ${f.stage===\"rejected\"?\"selected\":\"\"}>Reddedildi</option>");
    Contains(result, "const cols=STAGES.map");
}

static void PipelineIsResponsive()
{
    const string source = "<html><head></head><body><main>kaynak</main></body></html>";
    var result = MarketingPipelineHtmlEnhancement.Apply(source);

    Contains(result, "pipeline-board-wrap");
    Contains(result, "pipeline-scroll-hint");
    Contains(result, "id=\"pipelineScrollPrev\"");
    Contains(result, "id=\"pipelineScrollNext\"");
    Contains(result, "board.scrollBy({left:distance(),behavior:\"smooth\"})");
    Contains(result, "event.target.closest(\".pcard,button,a,input,select,textarea\")");
    Contains(result, "board.setPointerCapture(event.pointerId)");
    Contains(result, "board.scrollLeft=pan.startScrollLeft+(pan.startX-event.clientX)");
    Contains(result, "scroll-snap-type:x proximity");
    Contains(result, "@media(max-width:720px)");
    Contains(result, "width:min(84vw,320px)");
    Contains(result, "classList.toggle(\"pipeline-page\",S.view===\"pipeline\")");
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
