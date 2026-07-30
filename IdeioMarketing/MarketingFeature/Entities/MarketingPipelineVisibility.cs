namespace IdeioMarketing.MarketingFeature.Entities
{
    public static class MarketingPipelineVisibility
    {
        private const string HiddenMarker = "\u001Eideio:pipeline:hidden\u001E";

        public static bool Resolve(bool? value)
        {
            // Eski istemciler bu alanı göndermez. Mevcut otomatik ekleme davranışını
            // korumak için eksik değer pipeline'a dahil kabul edilir.
            return value ?? true;
        }

        public static string EncodeNote(string? note, bool isInPipeline)
        {
            var cleanNote = DecodeNote(note);
            return isInPipeline ? cleanNote : HiddenMarker + cleanNote;
        }

        public static bool ResolveFromNote(string? note)
        {
            return note?.StartsWith(HiddenMarker, StringComparison.Ordinal) != true;
        }

        public static string DecodeNote(string? note)
        {
            var value = note ?? string.Empty;
            return value.StartsWith(HiddenMarker, StringComparison.Ordinal)
                ? value[HiddenMarker.Length..]
                : value;
        }
    }
}
