﻿using FaasNet.EventMesh.Common;

namespace FaasNet.EventMesh.FirstConsole
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Console.Title = "First console";
            ConsoleHelper.Start(4000, 5672, 2803).Wait();
        }
    }
}