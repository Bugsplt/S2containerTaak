using System;
using System.Collections.Generic;
using System.Net.Mime;
using Containership.Enums;

namespace Containership.classes
{
    public class Ship
    {
        private int _minLoad;
        private int _maxWeightDiff;
        private int _maxLoad;
        private int _length;
        private int _width;
        private List<StackRow> _load = new();
        private Dockyard _dockedAt;
        private List<Container> NormalContainers = new();
        private List<Container> CooledContainers = new();
        private List<Container> ValuableContainers = new();
        private List<Container> CooledValuableContainers = new();
        
        public void SetSize(int length , int width)
        { 
            _width = width;
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

        public void PlaceContainer(Container container, int lengthIndex, int widthIndex)
        {
            _load[lengthIndex].GetStacks()[widthIndex].Add(container);
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
            PutValuableContainers();
            PutCooledContainers();
            //todo try to place lowest weight cooled containers if some are left
            PutNormalContainers();
            //todo try to place lowest weight normal containers if some are left
            CheckBalance();
            
            Console.WriteLine($"Normal containers left: {_dockedAt.GetNormalContainers().Count}         Normal containers placed: {NormalContainers.Count}");
            Console.WriteLine($"Cooled containers left: {_dockedAt.GetCooledContainers().Count}         Cooled containers placed: {CooledContainers.Count}");
            Console.WriteLine($"Valuable containers left: {_dockedAt.GetValuableContainers().Count}         Valuable containers placed: {ValuableContainers.Count}");
            Console.WriteLine($"CooledValuable containers left: {_dockedAt.GetCooledValuableContainers().Count}         CooledValuable containers placed: {CooledValuableContainers.Count}");
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
                    if ((stackRow.GetStackIndex(stack) + 1) * 2 < _width + 1)
                    {
                        leftWeight += stack.Weight;
                    } 
                    else if ((stackRow.GetStackIndex(stack) + 1) * 2 > _width + 1)
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
            var placedCounter = 0;
            for (var i = 0; _dockedAt.GetCooledValuableContainers().Count > 0; i++)
            {
                var container = _dockedAt.GetCooledValuableContainers()[0];
                var widthIndex = i;
                
                // lower x to fit within index range
                for (; widthIndex >= _width;)
                {
                    widthIndex -= _width;
                    //keep track of height
                }

                // uneven gets put inwards from the right
                if (widthIndex != 0 && widthIndex % 2 != 0)
                {
                    widthIndex = _width - (widthIndex + 1) / 2;
                }

                // even gets put inwards from the left
                else if (widthIndex != 0)
                {
                    widthIndex = widthIndex / 2;
                }

                //check if the spot is free
                if (FreeSpot(0, widthIndex, 0))
                {
                    CooledValuableContainers.Add(container);
                    _load[0].GetStacks()[widthIndex].Add(container);
                    _dockedAt.RemoveCooledValuableContainer(container);
                    //keep track of placed containers
                    placedCounter++;
                }

                //stop infinite recursion
                if (placedCounter == _width)
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
            var rowIndex = 0;
            var unavailableNr = 0;
            for (var i = 0; _dockedAt.GetValuableContainers().Count > 0; i++)
            {
                


                    var container = _dockedAt.GetValuableContainers()[0];
                    var widthIndex = i;

                    // lower x to fit within index range
                    for (; widthIndex >= _width;)
                    {
                        widthIndex -= _width;
                    }

                    // uneven gets put inwards from the right
                    if (widthIndex != 0 && widthIndex % 2 != 0)
                    {
                        widthIndex = _width - (widthIndex + 1) / 2;
                    }

                    // even gets put inwards from the left
                    else if (widthIndex != 0)
                    {
                        widthIndex = widthIndex / 2;
                    }

                    //check if the spot is free
                    if (FreeSpot(rowIndex, widthIndex, 0))
                    {
                        ValuableContainers.Add(container);
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
                    if (unavailableNr >= _width * (rowIndex + 1))
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
            var takenCounter = 0;
            for (var i = 0; _dockedAt.GetCooledContainers().Count > 0; i++)
            {
                var container = _dockedAt.GetCooledContainers()[0];
                var widthIndex = i;

                var heightIndex = 0;
                // lower x to fit within index range
                for (; widthIndex >= _width;)
                {
                    widthIndex -= _width;
                    //keep track of height
                    heightIndex++;
                }

                // uneven gets put inwards from the right
                if (widthIndex != 0 && widthIndex % 2 != 0)
                {
                    widthIndex = _width - (widthIndex + 1) / 2;
                }

                // even gets put inwards from the left
                else if (widthIndex != 0)
                {
                    widthIndex = widthIndex / 2;
                }

                //check if the spot is free
                if (FreeSpot(0, widthIndex, heightIndex) && _load[0].GetStacks()[widthIndex].CanCarry(container.Weight))
                {
                    CooledContainers.Add(container);
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
                if (takenCounter > _width)
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
                    for (var lengthIndex = 0; layerTakenCounter < _width * _length &&  _dockedAt.GetNormalContainers().Count > 0; lengthIndex++)
                    {
                        //loop through each container in a row
                        for (var i = 0; i < _width &&  _dockedAt.GetNormalContainers().Count > 0; i++)
                        {
                            var container = _dockedAt.GetNormalContainers()[0];
                            var widthIndex = i;
                            // uneven gets put inwards from the right
                            if (widthIndex != 0 && widthIndex % 2 != 0)
                            {
                                widthIndex = _width - (widthIndex + 1) / 2;
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
                                NormalContainers.Add(container);
                                _load[lengthIndex].GetStacks()[widthIndex].Add(container);
                                _dockedAt.RemoveNormalContainer(container);
                            }
                        }

                        layerTakenCounter += _width;
                    }
                }
                else
                {
                    break;
                }
            }
        }
    }
}