using System;
using System.Collections.Generic;

namespace FaasNet.EventMesh.Runtime.Website.Services
{
    public class BreadcrumbState
    {
        public BreadcrumbState()
        {
            Items = new List<BreadcrumbItem>();
        }

        public ICollection<BreadcrumbItem> Items { get; private set; }

        public event Action StateChanged;

        public void SetItems(ICollection<BreadcrumbItem> items)
        {
            Items = items;
            StateChanged?.Invoke();
        }
    }

    public class BreadcrumbItem
    {
        public BreadcrumbItem(string text)
        {
            Text = text;
        }

        public string Text { get; private set; }
        public string Anchor { get; private set; }
    }
}
