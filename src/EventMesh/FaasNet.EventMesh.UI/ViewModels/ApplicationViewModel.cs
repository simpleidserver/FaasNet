using FaasNet.EventMesh.Client.StateMachines.Client;

namespace FaasNet.EventMesh.UI.ViewModels
{
    public class ApplicationViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
        public DateTime CreateDateTime { get; set; }
        public IEnumerable<ClientPurposeTypes> Purposes { get; set; } = new List<ClientPurposeTypes>();
        public bool IsNew { get; set; } = false;
        public string ClientSecret { get; set; } = string.Empty;
        public double CoordinateX { get; set; }
        public double CoordinateY { get; set; }
        public int Width { get; set; } = 200;
        public int Height { get; set; } = 80;
        public bool IsActive { get; set; } = false;
        public ICollection<AnchorViewModel> Anchors { get; set; } = new List<AnchorViewModel>
        {
            new AnchorViewModel { Position = AnchorPositions.TOP },
            new AnchorViewModel { Position = AnchorPositions.RIGHT },
            new AnchorViewModel { Position = AnchorPositions.BOTTOM },
            new AnchorViewModel { Position = AnchorPositions.LEFT }
        };
        public string Matrix
        {
            get
            {
                return $"matrix(1 0 0 1 {CoordinateX.ToString().Replace(",",".")} {CoordinateY.ToString().Replace(",", ".")})";
            }
        }
        public string GetClassName(string selectedApplicationId)
        {
            var result = new List<string> { "application" };
            if (Id == selectedApplicationId) result.Add("moving");
            if (IsActive) result.Add("selected");
            return string.Join(" ", result);
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

        public (double, double) ComputeAnchorOffset(AnchorPositions position)
        {
            switch(position)
            {
                case AnchorPositions.TOP:
                    return (CoordinateX + (Width / 2), CoordinateY);
                case AnchorPositions.RIGHT:
                    return (CoordinateX + Width, CoordinateY + (Height / 2));
                case AnchorPositions.BOTTOM:
                    return (CoordinateX + (Width / 2), CoordinateY + Height);
                default:
                    return (CoordinateX, CoordinateY + (Height / 2));
            }
        }
    }
}
