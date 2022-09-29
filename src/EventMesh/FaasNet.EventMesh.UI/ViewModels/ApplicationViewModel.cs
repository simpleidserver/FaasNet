namespace FaasNet.EventMesh.UI.ViewModels
{
    public class ApplicationViewModel
    {
        public string Id { get; set; }
        public string ClientId { get; set; }
        public double CoordinateX { get; set; }
        public double CoordinateY { get; set; }
        public int Width { get; set; } = 200;
        public int Height { get; set; } = 80;
        public string Matrix
        {
            get
            {
                return $"matrix(1 0 0 1 {CoordinateX.ToString().Replace(",",".")} {CoordinateY.ToString().Replace(",", ".")})";
            }
        }

        public ApplicationViewModel Clone()
        {
            return new ApplicationViewModel
            {
                Id = Id,
                ClientId = ClientId,
                CoordinateX = CoordinateX,
                CoordinateY = CoordinateY,
                Width = Width,
                Height = Height
            };
        }
    }
}
