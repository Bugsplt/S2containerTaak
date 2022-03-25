using System;
using System.Collections.Generic;
using Containership.Classes;
using Containership.Enums;

namespace Containership
{
    public class ContainerProgram
    {
        public static void Main(string[] args)
        {
            //todo allow for adding new ships / shipments?
            var ship = new Ship();
            var dockYard = new Dockyard();
            ship.Dock(dockYard);
            AskData(ship, dockYard);
            ship.PutLoad();
            StartChat(ship);
        }

        private static void AskData(Ship ship, Dockyard dockyard)
        {
            var length = 0;
            var answered = false;
            for (; !answered;)
            {
                Console.WriteLine("Ship length (10): ");
                var input = Console.ReadLine() ?? string.Empty;
                if (!int.TryParse(input, out _))
                {
                    Console.WriteLine("Int expected");
                    continue;
                }
                length = int.Parse(input);
                if (length > 0)
                {
                    answered = true;
                }
                else
                {
                    Console.WriteLine("Give a length above 0");
                }
            }
            answered = false;
            for (; !answered;)
            {
                Console.WriteLine("Ship width (7): ");
                var input = Console.ReadLine() ?? string.Empty;
                if (!int.TryParse(input, out _))
                {
                    Console.WriteLine("Int expected");
                    continue;
                }
                var width = int.Parse(input);
                if (width > 0)
                {
                    ship.SetSize(length, width);
                    answered = true;
                }
                else
                {
                    Console.WriteLine("Give a width above 0");
                }
            }
            answered = false;
            for (; !answered;)
            {
                Console.WriteLine("Ship max load * 100000 (50): ");
                var input = Console.ReadLine() ?? string.Empty;
                if (!int.TryParse(input, out _))
                {
                    Console.WriteLine("Int expected");
                    continue;
                }
                var maxLoad = int.Parse(input);
                if (maxLoad > 0)
                {
                    ship.SetMaxLoad(maxLoad);
                    answered = true;
                }
                else
                {
                    Console.WriteLine("Give a max load above 0");
                }
            }

            answered = false;
            for (;!answered;)
            {
                Console.WriteLine("Nr of containers (300): ");
                var input = Console.ReadLine() ?? string.Empty;
                if (!int.TryParse(input, out _))
                {
                    Console.WriteLine("Int expected");
                    continue;
                }
                var amount = int.Parse(input);
                if (amount > 0)
                {
                    dockyard.NewShipment(amount);
                    answered = true;
                }
                else
                {
                    Console.WriteLine("Give a nr of containers above 0");
                }
            }

        }
        
        private static void StartChat(Ship ship)
        {
            var continueChat = true;
            for (; continueChat;)
            {
                Console.WriteLine("Enter a number to look at a layer or type 'exit' to quit");
                var input = Console.ReadLine() ?? string.Empty;
                if (input == "exit")
                {
                    continueChat = false;
                }
                else if (int.TryParse(input, out _))
                {
                    LogLayer(ship, input);
                }
                else
                {
                    Console.WriteLine("Command not recognized");
                }
            }
        }

        private static void LogLayer(Ship ship, string? input)
        {
            var containerString = "The bridge (powered back of the ship)\n";
            for (var i = 0; i < ship.Width; i++)
            {
                containerString += "= == == =";
            }

            containerString += "\n";
            foreach (var containers in ship.GetLayer(int.Parse(input ?? string.Empty) - 1))
            {
                containerString += ContainerString(containers);
            }

            for (var i = 0; i < ship.Width; i++)
            {
                containerString += "= == == =";
            }

            containerString += "\n";
            Console.Write(containerString);
            Console.WriteLine("container types: <normal>  ;  $~valuable cooled~$  ;  ~cooled~  ;  $valuable$");
        }

        private static string ContainerString(List<Container> containers)
        {
            var containerString = "";
            foreach (var container in containers)
            {
                if (container.Weight != 0)
                {
                    switch (container.Type)
                    {
                        case ContainerType.Valuable:
                            containerString += $"$ {container.Weight} $";
                            break;
                        case ContainerType.Cooled:
                            containerString += $"~ {container.Weight} ~";
                            break;
                        case ContainerType.ValuableCooled:
                            containerString += $"$~{container.Weight}~$";
                            break;
                        case ContainerType.Normal:
                            containerString += $"< {container.Weight} >";
                            break;
                    }
                }
                else
                {
                    containerString += "< empty >";
                }
            }

            containerString += "\n";
            return containerString;
        }
    }
}
