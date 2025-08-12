using System.Text.Json;

namespace BlackjackLogic
{
    public static class PersistenceService
    {
        private static string SaveFilePath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "blackjack_stats.json");

        public static void SaveStats(PlayerStats stats)
        {
            var json = JsonSerializer.Serialize(stats);
            File.WriteAllText(SaveFilePath, json);
        }

        public static PlayerStats LoadStats()
        {
            if (!File.Exists(SaveFilePath))
            {
                return new PlayerStats(); // Return new stats if no save file
            }

            var json = File.ReadAllText(SaveFilePath);
            return JsonSerializer.Deserialize<PlayerStats>(json);
        }
    }
}
