namespace IdeioMarketing.MarketingFeature.Entities
{
    public static class MarketingPipelineVisibility
    {
        public static bool Resolve(bool? value)
        {
            // Eski istemciler bu alanı göndermez. Mevcut otomatik ekleme davranışını
            // korumak için eksik değer pipeline'a dahil kabul edilir.
            return value ?? true;
        }
    }
}
