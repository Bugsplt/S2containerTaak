using System;
using System.Collections.Generic;
using System.Net.Mime;
using Containership.Enums;

namespace Containership.classes
{
    public class Ship
    {
        public int Width { get; private set; }
        
        private int _minLoad;
        private int _maxWeightDiff;
        private int _totalWeight = 0;
        private int _maxLoad;
        private int _length;
        private List<StackRow> _load = new();
        private Dockyard _dockedAt;
        private List<Container> _normalContainers = new();
        private List<Container> _cooledContainers = new();
        private List<Container> _valuableContainers = new();
        private List<Container> _cooledValuableContainers = new();
        
        public void SetSize(int length , int width)
        { 
            Width = width;
            _length = length;
            for (var i = 0; i < length; i++)
            {
                _load.Add(new StackRow(width));
            }
        }
        
        public void SetMaxLoad(int maxLoad)
        {
            _maxLoad = maxLoad * 100000;
            _maxWeightDiff = _maxLoad / 5;
            _minLoad = _maxLoad / 2;
        }
        
        public void Dock(Dockyard dockyard)
        {
            _dockedAt = dockyard;
        }

        private bool FreeSpot(int lengthIndex, int widthIndex, int heightIndex)
        {
            var containerList = _load[lengthIndex].GetStacks()[widthIndex].GetContainers();
            if (containerList.Count > heightIndex || !_load[lengthIndex].GetStacks()[widthIndex].IsAvailable())
            {
                return false;
            }
            return true;
        }

        public void PutLoad()
        {
            //todo make it impossible to load too much weight
            PutCooledValuableContainers();
            //todo put valuable containers per length instead of per row
            PutValuableContainers();
            PutCooledContainers();
            //todo try to place lowest weight cooled containers if some are left
            PutNormalContainers();
            //todo try to place lowest weight normal containers if some are left
            CheckBalance();
            //todo swap left and right most containers in all arrays
            foreach (var stackRow in _load)
            {
                stackRow.ReArrange();
            }
            Console.WriteLine($"Normal containers left: {_dockedAt.GetNormalContainers().Count}         Normal containers placed: {_normalContainers.Count}");
            Console.WriteLine($"Cooled containers left: {_dockedAt.GetCooledContainers().Count}         Cooled containers placed: {_cooledContainers.Count}");
            Console.WriteLine($"Valuable containers left: {_dockedAt.GetValuableContainers().Count}         Valuable containers placed: {_valuableContainers.Count}");
            Console.WriteLine($"CooledValuable containers left: {_dockedAt.GetCooledValuableContainers().Count}         CooledValuable containers placed: {_cooledValuableContainers.Count}");
        }

       private void CheckBalance()
        {
            var leftWeight = 0;
            var rightWeight = 0;
            var totalWeight = 0;
            foreach (var stackRow in _load)
            {
                foreach (var stack in stackRow.GetStacks())
                {
                    totalWeight += stack.Weight;
                    if ((stackRow.GetStackIndex(stack) + 1) * 2 < Width + 1)
                    {
                        leftWeight += stack.Weight;
                    } 
                    else if ((stackRow.GetStackIndex(stack) + 1) * 2 > Width + 1)
                    {
                        rightWeight += stack.Weight;
                    }
                }
            }

            if (leftWeight + _maxWeightDiff > rightWeight && rightWeight + _maxWeightDiff > leftWeight)
            {
                Console.WriteLine("Balanced");
            }
            else
            {
                Console.WriteLine("Not balanced");
            }

            if (totalWeight < _minLoad)
            {
                Console.WriteLine("Not enough weight");
            }
            else if (totalWeight > _maxLoad)
            {
                Console.WriteLine("Too heavy");
            }
            else
            {
                Console.WriteLine("valid load weight");
            }

        }
        
        private void PutCooledValuableContainers()
        { 
            var end = false;
            var placedCounter = 0;
            for (var i = 0; _dockedAt.GetCooledValuableContainers().Count > 0; i++)
            {
                var container = _dockedAt.GetCooledValuableContainers()[0];
                if (_totalWeight + container.Weight > _maxLoad)
                {
                    end = true;
                    foreach (var item in _dockedAt.GetCooledValuableContainers())
                    {
                        if (_totalWeight + item.Weight <= _maxLoad)
                        {
                            container = item;
                            end = false;
                            break;
                        }
                    }
                }

                if (end)
                {
                    Console.WriteLine("Max load reached");
                    break;
                }
                var widthIndex = i;
                
                // lower x to fit within index range
                for (; widthIndex >= Width;)
                {
                    widthIndex -= Width;
                    //keep track of height
                }

                // uneven gets put inwards from the right
                if (widthIndex != 0 && widthIndex % 2 != 0)
                {
                    widthIndex = Width - (widthIndex + 1) / 2;
                }

                // even gets put inwards from the left
                else if (widthIndex != 0)
                {
                    widthIndex = widthIndex / 2;
                }

                //check if the spot is free
                if (FreeSpot(0, widthIndex, 0))
                {
                    _cooledValuableContainers.Add(container);
                    _load[0].GetStacks()[widthIndex].Add(container);
                    _dockedAt.RemoveCooledValuableContainer(container);
                    //keep track of placed containers
                    placedCounter++;
                }

                //stop infinite recursion
                if (placedCounter == Width)
                {
                    if (_dockedAt.GetCooledValuableContainers().Count != 0)
                    {
                          Console.WriteLine("Too many cooled valuable containers!");
                    }
                    break;
                }
            }
        }
        
        private void PutValuableContainers()
        { 
            var end = false;
            var rowIndex = 0;
            var unavailableNr = 0;
            for (var i = 0; _dockedAt.GetValuableContainers().Count > 0; i++)
            {
                var container = _dockedAt.GetValuableContainers()[0];
                    var widthIndex = i;

                    if (_totalWeight + container.Weight > _maxLoad)
                    {
                        end = true;
                        foreach (var item in _dockedAt.GetValuableContainers())
                        {
                            if (_totalWeight + item.Weight <= _maxLoad)
                            {
                                container = item;
                                end = false;
                                break;
                            }
                        }
                    }

                    if (end)
                    {
                        Console.WriteLine("Max load reached");
                        break;
                    }
                    
                    // lower x to fit within index range
                    for (; widthIndex >= Width;)
                    {
                        widthIndex -= Width;
                    }

                    // uneven gets put inwards from the right
                    if (widthIndex != 0 && widthIndex % 2 != 0)
                    {
                        widthIndex = Width - (widthIndex + 1) / 2;
                    }

                    // even gets put inwards from the left
                    else if (widthIndex != 0)
                    {
                        widthIndex = widthIndex / 2;
                    }

                    //check if the spot is free
                    if (FreeSpot(rowIndex, widthIndex, 0))
                    {
                        _valuableContainers.Add(container);
                        _load[rowIndex].GetStacks()[widthIndex].Add(container);
                        _dockedAt.RemoveValuableContainer(container);
                        //keep track of nr of unavailable spots
                        unavailableNr++;
                        if ((rowIndex + 2) % 3 == 0 && rowIndex + 1 < _load.Count)
                        {
                            _load[rowIndex+1].GetStacks()[widthIndex].SetUnavailable();
                        }
                    }
                    else
                    {
                        unavailableNr++;
                    }


                    //keep track of row
                    if (unavailableNr >= Width * (rowIndex + 1))
                    {
                        rowIndex++;
                    }

                    //prevent infinite recursion
                    if (rowIndex == _length)
                    {
                        Console.WriteLine("Too many valuable containers");
                        break;
                    }
                
            }
        }
        
        private void PutCooledContainers()
        { 
            var end = false;
            var takenCounter = 0;
            for (var i = 0; _dockedAt.GetCooledContainers().Count > 0; i++)
            {
               
                var container = _dockedAt.GetCooledContainers()[0];
                var widthIndex = i;

                if (_totalWeight + container.Weight > _maxLoad)
                {
                    end = true;
                    foreach (var item in _dockedAt.GetCooledContainers())
                    {
                        if (_totalWeight + item.Weight <= _maxLoad)
                        {
                            container = item;
                            end = false;
                            break;
                        }
                    }
                }

                if (end)
                {
                    Console.WriteLine("Max load reached");
                    break;
                }
                
                var heightIndex = 0;
                // lower x to fit within index range
                for (; widthIndex >= Width;)
                {
                    widthIndex -= Width;
                    //keep track of height
                    heightIndex++;
                }

                // uneven gets put inwards from the right
                if (widthIndex != 0 && widthIndex % 2 != 0)
                {
                    widthIndex = Width - (widthIndex + 1) / 2;
                }

                // even gets put inwards from the left
                else if (widthIndex != 0)
                {
                    widthIndex = widthIndex / 2;
                }

                //check if the spot is free
                if (FreeSpot(0, widthIndex, heightIndex) && _load[0].GetStacks()[widthIndex].CanCarry(container.Weight))
                {
                    _cooledContainers.Add(container);
                    _load[0].GetStacks()[widthIndex].Add(container);
                    _dockedAt.RemoveCooledContainer(container);
                    //
                    takenCounter = 0;
                }
                else
                {
                    //keep track of nr of concurrent impossible spots
                    takenCounter++;
                }

                //stop infinite recursion
                if (takenCounter > Width)
                {
                    Console.WriteLine("Too many cooled containers!");
                    break;
                }
            }
        }

        private bool CanPlaceWeight(int weight)
        {
            foreach (var stackRow in _load)
            {
                foreach (var stack in stackRow.GetStacks())
                {
                    if (stack.IsAvailable() && stack.CanCarry(weight))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        
        private void PutNormalContainers()
        {
           //loop through heights until containers are out or no available spots are left
            for (var heightIndex = 0; _dockedAt.GetNormalContainers().Count > 0; heightIndex++)
            {
                if (CanPlaceWeight(_dockedAt.GetNormalContainers()[0].Weight))
                {
                    //keep track of available spots in layer
                    var layerTakenCounter = 0;
                    //loop through each row of the ship
                    for (var lengthIndex = 0; layerTakenCounter < Width * _length &&  _dockedAt.GetNormalContainers().Count > 0; lengthIndex++)
                    {
                        //loop through each container in a row
                        for (var i = 0; i < Width &&  _dockedAt.GetNormalContainers().Count > 0; i++)
                        {
                            var container = _dockedAt.GetNormalContainers()[0];
                            var widthIndex = i;
                            
                            // uneven gets put inwards from the right
                            if (widthIndex != 0 && widthIndex % 2 != 0)
                            {
                                widthIndex = Width - (widthIndex + 1) / 2;
                            }

                            // even gets put inwards from the left
                            else if (widthIndex != 0)
                            {
                                widthIndex = widthIndex / 2;
                            }

                            //check if the spot is free
                            if (FreeSpot(lengthIndex, widthIndex, heightIndex) &&
                                _load[lengthIndex].GetStacks()[widthIndex].CanCarry(container.Weight))
                            {
                                _normalContainers.Add(container);
                                _load[lengthIndex].GetStacks()[widthIndex].Add(container);
                                _dockedAt.RemoveNormalContainer(container);
                            }
                        }

                        layerTakenCounter += Width;
                    }
                }
                else
                {
                    Console.WriteLine("Max load reached");
                    break;
                }
            }
        }

        public List<List<Container>> GetLayer(int layer)
        {
            var returnList = new List<List<Container>>();
            var i = 0;
            foreach (var stackRow in _load)
            {
                returnList.Add(new List<Container>());
                foreach (var stack in stackRow.GetStacks())
                {
                    if (stack.GetContainers().Count > layer)
                    {
                        returnList[i].Add(stack.GetContainers()[layer]);
                    }
                    else
                    {
                        returnList[i].Add(new Container(false));
                    }
                }
                i++;
            }
            return returnList;
        }
    }
}