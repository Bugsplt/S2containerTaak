using System;
using System.Collections.Generic;
using Containership.classes;

namespace Containership
{
    class Program
    {
        public static void Main(string[] args)
        {
            var ship = new Ship();
            var dockYard = new Dockyard();
            ship.Dock(dockYard);
            Console.WriteLine("Ship length: ");
            // var length = int.Parse(Console.ReadLine() ?? string.Empty);
            Console.WriteLine("Ship width: ");
            ship.SetSize(/*(length, int.Parse(Console.ReadLine() ?? string.Empty)*/10,7);
            Console.WriteLine("Ship max load: ");
            ship.SetMaxLoad(/*int.Parse(Console.ReadLine() ?? string.Empty)*/50);
            Console.WriteLine("Nr of containers: ");
            dockYard.NewShipment(/*int.Parse(Console.ReadLine() ?? string.Empty)*/300);
            ship.PutLoad();
        }
    }
}
