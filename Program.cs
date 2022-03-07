using System;
using System.Collections.Generic;
using Containership.classes;
using Containership.Enums;

namespace Containership
{
    class Program
    {
        public static void Main(string[] args)
        {
            //todo allow for adding new ships / shipments
            var ship = new Ship();
            var dockYard = new Dockyard();
            ship.Dock(dockYard);
            Console.WriteLine("Ship length (10): ");
            var length = int.Parse(Console.ReadLine() ?? string.Empty);
            Console.WriteLine("Ship width (7): ");
            ship.SetSize(length, int.Parse(Console.ReadLine() ?? string.Empty));
            Console.WriteLine("Ship max load * 100000 (50): ");
            ship.SetMaxLoad(int.Parse(Console.ReadLine() ?? string.Empty));
            Console.WriteLine("Nr of containers (300): ");
            dockYard.NewShipment(int.Parse(Console.ReadLine() ?? string.Empty));
            ship.PutLoad();
            var continueChat = true;
            for (;continueChat;)
            {
                Console.WriteLine("Enter a number to look at a layer or type 'exit' to quit");
                var input = Console.ReadLine();
                if (input == "exit")
                {
                    continueChat = false;
                }
                else
                {
                    try
                    {
                        var containerString = "The bridge (powered back of the ship)\n";
                        for (var i = 0; i < ship.Width; i++)
                        {
                            containerString += "= == == =";
                        }
                        containerString += "\n";
                        foreach (var containers in ship.GetLayer(int.Parse(input)-1))
                        {
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
                        }
                        for (var i = 0; i < ship.Width; i++)
                        {
                            containerString += "= == == =";
                        }

                        containerString += "\n";
                        Console.Write(containerString);
                        Console.WriteLine("container types: <normal>  ;  $~valuable cooled~$  ;  ~cooled~  ;  $valuable$");
                    }
                    catch (Exception)
                    {
                      Console.WriteLine("layer not found");
                    }
                }
            }
            
        }
    }
}
