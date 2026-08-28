namespace IdeioMarketing.MarketingFeature.Entities
{
    public static class MarketingPipelineVisibility
    {
        private const string HiddenMarker = "\u001Eideio:pipeline:hidden\u001E";
        private const string HasContractMarker = "\u001Eideio:contract:yes\u001E";
        private const string ContractDeferredMarker = "\u001Eideio:contract:deferred\u001E";
        private const string ContractStartMarkerPrefix = "\u001Eideio:contract:start:";
        private const string ContractEndMarkerPrefix = "\u001Eideio:contract:end:";
        private const string LossReasonMarkerPrefix = "\u001Eideio:loss:reason:";
        private const string MarkerTerminator = "\u001E";

        public static bool Resolve(bool? value)
        {
            // Eski istemciler bu alanı göndermez. Mevcut otomatik ekleme davranışını
            // korumak için eksik değer pipeline'a dahil kabul edilir.
            return value ?? true;
        }

        public static string EncodeNote(string? note, bool isInPipeline)
        {
            return EncodeNote(
                note,
                isInPipeline,
                ResolveHasContractFromNote(note),
                ResolveContractStartDateFromNote(note),
                ResolveContractEndDateFromNote(note),
                ResolveContractDeferredFromNote(note),
                ResolveLossReasonFromNote(note));
        }

        public static string EncodeNote(string? note, bool isInPipeline, bool hasContract)
        {
            return EncodeNote(
                note,
                isInPipeline,
                hasContract,
                ResolveContractStartDateFromNote(note),
                ResolveContractEndDateFromNote(note),
                false,
                ResolveLossReasonFromNote(note));
        }

        public static string EncodeNote(
            string? note,
            bool isInPipeline,
            bool hasContract,
            string? contractStartDate,
            string? contractEndDate)
        {
            return EncodeNote(note, isInPipeline, hasContract, contractStartDate, contractEndDate, false, null);
        }

        public static string EncodeNote(
            string? note,
            bool isInPipeline,
            bool hasContract,
            string? contractStartDate,
            string? contractEndDate,
            bool contractDeferred)
        {
            return EncodeNote(note, isInPipeline, hasContract, contractStartDate, contractEndDate, contractDeferred, null);
        }

        public static string EncodeNote(
            string? note,
            bool isInPipeline,
            bool hasContract,
            string? contractStartDate,
            string? contractEndDate,
            bool contractDeferred,
            string? lossReason)
        {
            var cleanNote = DecodeNote(note);
            var normalizedStartDate = NormalizeContractDate(contractStartDate);
            var normalizedEndDate = NormalizeContractDate(contractEndDate);
            var normalizedLossReason = NormalizeLossReason(lossReason);
            return (isInPipeline ? string.Empty : HiddenMarker)
                + (hasContract ? HasContractMarker : string.Empty)
                + (!hasContract && contractDeferred ? ContractDeferredMarker : string.Empty)
                + CreateDateMarker(ContractStartMarkerPrefix, normalizedStartDate)
                + CreateDateMarker(ContractEndMarkerPrefix, normalizedEndDate)
                + CreateValueMarker(LossReasonMarkerPrefix, normalizedLossReason)
                + cleanNote;
        }

        public static bool ResolveFromNote(string? note)
        {
            return note?.StartsWith(HiddenMarker, StringComparison.Ordinal) != true;
        }

        public static bool ResolveHasContractFromNote(string? note)
        {
            var value = note ?? string.Empty;
            if (value.StartsWith(HiddenMarker, StringComparison.Ordinal))
            {
                value = value[HiddenMarker.Length..];
            }

            return value.StartsWith(HasContractMarker, StringComparison.Ordinal);
        }

        public static string? ResolveContractStartDateFromNote(string? note)
        {
            return ExtractDateMarker(note, ContractStartMarkerPrefix);
        }

        public static string? ResolveContractEndDateFromNote(string? note)
        {
            return ExtractDateMarker(note, ContractEndMarkerPrefix);
        }

        public static bool ResolveContractDeferredFromNote(string? note)
        {
            return (note ?? string.Empty).Contains(ContractDeferredMarker, StringComparison.Ordinal);
        }

        public static string? ResolveLossReasonFromNote(string? note)
        {
            return ExtractValueMarker(note, LossReasonMarkerPrefix);
        }

        public static string DecodeNote(string? note)
        {
            var value = note ?? string.Empty;
            if (value.StartsWith(HiddenMarker, StringComparison.Ordinal))
            {
                value = value[HiddenMarker.Length..];
            }

            if (value.StartsWith(HasContractMarker, StringComparison.Ordinal))
            {
                value = value[HasContractMarker.Length..];
            }


            if (value.StartsWith(ContractDeferredMarker, StringComparison.Ordinal))
            {
                value = value[ContractDeferredMarker.Length..];
            }

            value = RemoveDateMarker(value, ContractStartMarkerPrefix);
            value = RemoveDateMarker(value, ContractEndMarkerPrefix);
            value = RemoveDateMarker(value, LossReasonMarkerPrefix);

            return value;
        }

        private static string? NormalizeContractDate(string? value)
        {
            return DateTime.TryParse(value, out var date) ? date.ToString("yyyy-MM-dd") : null;
        }

        private static string CreateDateMarker(string prefix, string? value)
        {
            return CreateValueMarker(prefix, value);
        }

        private static string? ExtractDateMarker(string? note, string prefix)
        {
            return ExtractValueMarker(note, prefix);
        }

        private static string CreateValueMarker(string prefix, string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? string.Empty : prefix + value + MarkerTerminator;
        }

        private static string? ExtractValueMarker(string? note, string prefix)
        {
            var value = note ?? string.Empty;
            var start = value.IndexOf(prefix, StringComparison.Ordinal);
            if (start < 0)
            {
                return null;
            }

            start += prefix.Length;
            var end = value.IndexOf(MarkerTerminator, start, StringComparison.Ordinal);
            return end > start ? value[start..end] : null;
        }

        private static string? NormalizeLossReason(string? value)
        {
            return value?.Trim() switch
            {
                "Müşteri Vazgeçti" => "Müşteri Vazgeçti",
                "Sözleşme Yenilenmedi" => "Sözleşme Yenilenmedi",
                "Sözleşme Müşteri Tarafından Fesedildi" => "Sözleşme Müşteri Tarafından Fesedildi",
                "Sözleşme Firma Tarafından Fes Edildi" => "Sözleşme Firma Tarafından Fes Edildi",
                _ => null
            };
        }

        private static string RemoveDateMarker(string value, string prefix)
        {
            if (!value.StartsWith(prefix, StringComparison.Ordinal))
            {
                return value;
            }

            var end = value.IndexOf(MarkerTerminator, prefix.Length, StringComparison.Ordinal);
            return end >= 0 ? value[(end + MarkerTerminator.Length)..] : value;
        }
    }
}
