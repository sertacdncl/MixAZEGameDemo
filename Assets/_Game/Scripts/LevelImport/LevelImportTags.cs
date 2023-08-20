namespace Game
{
    public static class LevelImportTags
    {
        public const string GeneratedLevel = "MixAZE GeneratedLevel_";
        public const string LevelInfo = "MixAZE LevelInfo";
        public static string GetLevelName(int level) => GeneratedLevel + level;
    }
}
