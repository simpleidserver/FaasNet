namespace FaasNet.EventMesh.UI.Helpers
{
    public static class CoordinateHelper
    {
        public static string Sanitize(IEnumerable<double> coordinates)
        {
            return string.Join(" ", coordinates.Select(c => Sanitize(c)));
        }

        public static IEnumerable<double> Parse(string coordinates)
        {
            return coordinates.Split(' ').Select(t => double.Parse(t.Replace(".", ",")));
        }

        public static string Sanitize(double d)
        {
            return d.ToString().Replace(",", ".");
        }
    }
}
