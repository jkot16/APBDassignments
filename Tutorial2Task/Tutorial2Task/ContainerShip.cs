

namespace ContainersApp
{
    public class ContainerShip
    {
        public string Name { get; }
        public double MaxSpeed { get; }
        public int MaxContainerCount { get; }
        public double MaxWeightTons { get; }
        private List<Container> _containers = new List<Container>();

        public ContainerShip(string name, double maxSpeed, int maxContainerCount, double maxWeightTons)
        {
            Name = name;
            MaxSpeed = maxSpeed;
            MaxContainerCount = maxContainerCount;
            MaxWeightTons = maxWeightTons;
        }

        public void LoadContainer(Container container)
        {
            if (_containers.Count >= MaxContainerCount) throw new InvalidOperationException("Maximum container count exceeded");
            double totalWeight = _containers.Sum(c => c.TareWeight + c.CargoMass);
            double newContainerWeight = container.TareWeight + container.CargoMass;
            if (totalWeight + newContainerWeight > MaxWeightTons * 1000) throw new InvalidOperationException("Maximum ship weight exceeded");
            _containers.Add(container);
        }

        public void LoadContainers(IEnumerable<Container> containers)
        {
            foreach (var container in containers) LoadContainer(container);
        }

        public void RemoveContainer(Container container)
        {
            _containers.Remove(container);
        }

        public void UnloadContainer(Container container)
        {
            container.EmptyCargo();
        }

        public void ReplaceContainer(string serialNumber, Container newContainer)
        {
            var oldContainer = _containers.FirstOrDefault(c => c.SerialNumber == serialNumber);
            if (oldContainer == null) throw new ArgumentException("Container not found");
            RemoveContainer(oldContainer);
            LoadContainer(newContainer);
        }

        public void TransferContainer(Container container, ContainerShip otherShip)
        {
            RemoveContainer(container);
            otherShip.LoadContainer(container);
        }

        public double GetTotalWeightKg()
        {
            return _containers.Sum(c => c.TareWeight + c.CargoMass);
        }

        public void PrintShipInfo()
        {
            Console.WriteLine("Name: " + Name);
            Console.WriteLine("Max speed (knots): " + MaxSpeed);
            Console.WriteLine("Max containers: " + MaxContainerCount);
            Console.WriteLine("Max weight (tons): " + MaxWeightTons);
            Console.WriteLine("Current containers: " + _containers.Count);
            Console.WriteLine("Current total weight (kg): " + GetTotalWeightKg());
            if (_containers.Count == 0) return;
            for (int i = 0; i < _containers.Count; i++)
            {
                var container = _containers[i];
                Console.WriteLine(container.SerialNumber + " | CargoMass: " + container.CargoMass + " kg | TareWeight: " + container.TareWeight + " kg");
            }
        }
    }
}
