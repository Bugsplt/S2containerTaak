using System.Collections.Generic;
using System.Linq;
using Containership.Enums;

namespace Containership.Classes
{
    public class Dockyard
    {
        private List<Container> _normalContainers = new();
        private List<Container> _cooledContainers = new();
        private List<Container> _valuableContainers = new();
        private List<Container> _cooledValuableContainers = new();

        public IReadOnlyList<Container> GetNormalContainers()
        {
            return _normalContainers;
        }
        public IReadOnlyList<Container> GetCooledContainers()
        {
            return _cooledContainers;
        }
        public IReadOnlyList<Container> GetValuableContainers()
        {
            return _valuableContainers;
        }
        public IReadOnlyList<Container> GetCooledValuableContainers()
        {
            return _cooledValuableContainers;
        }

        public void RemoveNormalContainer(Container container)
        {
            _normalContainers.Remove(container);
        }

        public void RemoveCooledContainer(Container container)
        {
            _cooledContainers.Remove(container);
        }

        public void RemoveValuableContainer(Container container)
        {
            _valuableContainers.Remove(container);
        }

        public void RemoveCooledValuableContainer(Container container)
        {
            _cooledValuableContainers.Remove(container);
        }

        public void NewShipment(int amount)
        {
            ClearOldShipment();
            CreateContainers(amount);
            SortWeights();
        }

        private void SortWeights()
        {
            _normalContainers = _normalContainers.OrderBy(x => -x.Weight).ToList();
            _cooledContainers = _cooledContainers.OrderBy(x => -x.Weight).ToList();
            _cooledValuableContainers = _cooledValuableContainers.OrderBy(x => -x.Weight).ToList();
            _valuableContainers = _valuableContainers.OrderBy(x => -x.Weight).ToList();
        }

        private void CreateContainers(int amount)
        {
            for (var i = 0; i < amount; i++)
            {
                var container = new Container();
                switch (container.Type)
                {
                    case ContainerType.Normal:
                        _normalContainers.Add(container);
                        break;
                    case ContainerType.Cooled:
                        _cooledContainers.Add(container);
                        break;
                    case ContainerType.Valuable:
                        _valuableContainers.Add(container);
                        break;
                    case ContainerType.ValuableCooled:
                        _cooledValuableContainers.Add(container);
                        break;
                }
            }
        }

        private void ClearOldShipment()
        {
            _normalContainers.Clear();
            _cooledContainers.Clear();
            _valuableContainers.Clear();
            _cooledValuableContainers.Clear();
        }
    }
}