using System;

namespace FaasNet.Gateway.Core.Domains
{
    public class ApiDefinitionOperationBlockUI : ICloneable
    {
        public int Childwidth { get; set; }
        public int Height { get; set; }
        public int Id { get; set; }
        public int Parent { get; set; }
        public int Width { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public object Clone()
        {
            return new ApiDefinitionOperationBlockUI
            {
                Childwidth = Childwidth,
                Height = Height,
                Id = Id,
                Parent = Parent,
                Width = Width,
                X = X,
                Y = Y
            };
        }
    }
}
