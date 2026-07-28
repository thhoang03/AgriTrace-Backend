namespace AgriTrace.Domain.Common;

public static class EventPermissionRules
{
    private static readonly IReadOnlyDictionary<string, HashSet<string>> _matrix = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase)
    {
        ["FARM"] = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "HARVEST" },
        ["PROCESSOR"] = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "RECEIVE", "PROCESSING", "PACKAGING", "SPLIT", "MERGE" },
        ["DISTRIBUTOR"] = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "RECEIVE", "TRANSPORT", "DISTRIBUTION", "SPLIT", "MERGE" },
        ["RETAILER"] = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "RECEIVE", "RETAIL", "SPLIT" },
        ["INSPECTION"] = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "INSPECTION" },
        ["SYSTEM"] = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "HARVEST", "RECEIVE", "PROCESSING", "PACKAGING", "TRANSPORT", "DISTRIBUTION", "RETAIL", "INSPECTION", "SPLIT", "MERGE", "RECALL" }
    };

    public static bool IsAllowed(string orgTypeCode, string eventTypeCode)
    {
        if (string.IsNullOrWhiteSpace(orgTypeCode) || string.IsNullOrWhiteSpace(eventTypeCode))
            return false;

        if (_matrix.TryGetValue(orgTypeCode, out var allowed))
        {
            return allowed.Contains(eventTypeCode);
        }

        return false;
    }
}
