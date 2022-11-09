namespace FaasNet.EventMesh.Performance
{
    public static class Constants
    {
        private static string BaseDirectoryPath = "C:\\Projects\\FaasNet\\src\\EventMesh\\FaasNet.EventMesh.Performance";
        public static string RecordsFilePath = Path.Combine(BaseDirectoryPath, "records.txt");
        public static string SummaryFilePath = Path.Combine(BaseDirectoryPath, "summary.txt");
        public static char Separator = ';';
    }
}
