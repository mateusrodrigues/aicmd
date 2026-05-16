static class History
{
    internal static string GetPath()
    {
        var snapUserCommon = Environment.GetEnvironmentVariable("SNAP_USER_COMMON");
        return Path.Combine(
            snapUserCommon ?? Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            ".aicmd_history");
    }

    internal static List<string> Load(string historyPath)
    {
        if (!File.Exists(historyPath))
            return [];
        return [.. File.ReadLines(historyPath).Where(l => !string.IsNullOrWhiteSpace(l)).TakeLast(20)];
    }

    internal static void Save(string historyPath, List<string> history, string entry)
    {
        history.Remove(entry);
        history.Add(entry);
        if (history.Count > 20)
            history.RemoveAt(0);
        File.WriteAllLines(historyPath, history);
    }
}
