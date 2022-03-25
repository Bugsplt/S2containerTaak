using System;
using Containership.Enums;

namespace Containership.Classes
{
    public class Container
    {
        private Random _random = new();
        private int _maxWeight = 30000;
        private int _minWeight = 4000;
        
        public int MaxLoad { get; } = 120000;
        public ContainerType Type { get; }
        public int Weight { get; }

        public Container()
        {
            switch (_random.Next(1,101))
            {
                case < 3:
                    Type = ContainerType.ValuableCooled;
                    break;
                case < 14:
                    Type = ContainerType.Valuable;
                    break;
                case < 25: 
                    Type = ContainerType.Cooled;
                    break;
                default:
                    Type = ContainerType.Normal;
                    break;
            }
            Weight = _random.Next(_minWeight, _maxWeight + 1);
        }
        
        
        
        public Container(bool isEmpty)
        {
            Weight = 0;
        }
    }
}