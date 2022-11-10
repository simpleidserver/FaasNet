namespace FaasNet.EventMesh.Performance
{
    public static class Constants
    {
        private static string BaseDirectoryPath = Directory.GetCurrentDirectory();
        public static string RecordsFilePath = Path.Combine(BaseDirectoryPath, "records.txt");
        public static string SummaryFilePath = Path.Combine(BaseDirectoryPath, "summary.txt");
        public static char Separator = ';';
    }
}
