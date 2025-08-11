using System.Text.Json;

namespace BlackjackLogic
{
    public static class GameSaves
    {
        private static string SaveFilePath => Path.Combine(FileSystem.AppDataDirectory, "blackjack_save.json");

        public static void SaveGame(BlackjackGameLogic game)
        {
            var json = JsonSerializer.Serialize(game);
            File.WriteAllText(SaveFilePath, json);
        }

        public static BlackjackGameLogic LoadGame()
        {
            if (!File.Exists(SaveFilePath))
            {
                return null;
            }

            var json = File.ReadAllText(SaveFilePath);
            return JsonSerializer.Deserialize<BlackjackGameLogic>(json);
        }

        public static bool SaveFileExists()
        {
            return File.Exists(SaveFilePath);
        }
    }
}
