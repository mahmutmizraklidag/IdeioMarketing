using System.Text.Json;
using IdeioMarketing.MarketingFeature.Content;
using IdeioMarketing.MarketingFeature.Entities;

var tests = new (string Name, Action Run)[]
{
    ("Yeni müşteri pipeline'a otomatik eklenir", NewLeadStartsInPipeline),
    ("Eski payload pipeline'da görünür kalır", LegacyPayloadDefaultsToPipeline),
    ("Pipeline'dan kaldırma değeri korunur", RemovedPayloadStaysOutOfPipeline),
    ("Arayüz kaldırma ve yeniden ekleme kontrollerini içerir", HtmlContainsPipelineControls),
    ("HTML enjeksiyonu kaynak içeriği korur", HtmlEnhancementPreservesSource),
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

static void HtmlContainsPipelineControls()
{
    const string source = "<html><head></head><body><main>kaynak</main></body></html>";
    var result = MarketingPipelineHtmlEnhancement.Apply(source);

    Contains(result, "data-pipeline-remove");
    Contains(result, "data-pipeline-add");
    Contains(result, "inPipeline:lead?isInPipeline(lead):true");
    Contains(result, "S.leads.filter(l=>isInPipeline(l)&&l.stage===s.id)");
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
