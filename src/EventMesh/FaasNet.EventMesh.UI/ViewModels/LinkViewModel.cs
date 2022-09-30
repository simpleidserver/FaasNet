using FaasNet.EventMesh.UI.Helpers;
using System.Diagnostics;

namespace FaasNet.EventMesh.UI.ViewModels
{
    public class LinkViewModel
    {
        public LinkPointViewModel StartPoint { get; set; }
        public LinkPointViewModel EndPoint { get; set; }
        public string Path
        {
            get
            {
                var mpx = (EndPoint.X + StartPoint.X) * 0.5;
                var mpy = (EndPoint.Y + StartPoint.Y) * 0.5;
                var theta = Math.Atan2(EndPoint.Y - StartPoint.Y, EndPoint.X - StartPoint.X) - Math.PI / 2;
                var offset = 30;
                var c1x = mpx + offset * Math.Cos(theta);
                var c1y = mpy + offset * Math.Sin(theta);
                var curve = "M" + CoordinateHelper.Sanitize(StartPoint.X) + " " + CoordinateHelper.Sanitize(StartPoint.Y) + " Q " + CoordinateHelper.Sanitize(c1x) + " " + CoordinateHelper.Sanitize(c1y) + " " + CoordinateHelper.Sanitize(EndPoint.X) + " " + CoordinateHelper.Sanitize(EndPoint.Y);
                Debug.WriteLine(curve);
                return curve;
            }
        }

        public bool IsLinkedToApplication(string applicationId)
        {
            return StartPoint.ApplicationId == applicationId || EndPoint.ApplicationId == applicationId;
        }

        public LinkViewModel Clone()
        {
            return new LinkViewModel
            {
                StartPoint = StartPoint.Clone(),
                EndPoint = EndPoint.Clone()
            };
        }

        private static int Signum(double x)
        {
            return (x < 0) ? -1 : 1;
        }

        private static double Absolute(double x)
        {
            return x < 0 ? -x : x;
        }
    }

    public class LinkPointViewModel : PointViewModel
    {
        public string ApplicationId { get; set; }
        public AnchorPositions AnchorPosition { get; set; }

        public LinkPointViewModel Clone()
        {
            return new LinkPointViewModel
            {
                AnchorPosition = AnchorPosition,
                ApplicationId = ApplicationId,
                X = X,
                Y = Y
            };
        }
    }
}