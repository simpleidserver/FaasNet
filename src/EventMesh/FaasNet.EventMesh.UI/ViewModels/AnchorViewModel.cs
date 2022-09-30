namespace FaasNet.EventMesh.UI.ViewModels
{
    public class AnchorViewModel
    {
        public AnchorPositions Position { get; set; }
        public string ClassName
        {
            get
            {
                switch (Position)
                {
                    case AnchorPositions.LEFT:
                        return "anchor l";
                    case AnchorPositions.TOP:
                        return "anchor t";
                    case AnchorPositions.RIGHT:
                        return "anchor r";
                    default:
                        return "anchor b";
                }
            }
        }
    }

    public enum AnchorPositions
    {
        LEFT = 0,
        TOP = 1,
        RIGHT = 2,
        BOTTOM = 3
    }
}
