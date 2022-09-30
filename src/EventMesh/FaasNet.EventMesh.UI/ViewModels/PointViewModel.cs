namespace FaasNet.EventMesh.UI.ViewModels
{
    public class PointViewModel
    {
        public double X { get; set; }
        public double Y { get; set; }

        public PointViewModel Clone()
        {
            return new PointViewModel
            {
                X = X,
                Y = Y
            };
        }
    }
}
