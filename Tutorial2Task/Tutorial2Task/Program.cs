
using ContainersApp.Containers;

namespace ContainersApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            
            // Creating ships
            var ship1 = new ContainerShip("ShipOne", 25, 5, 50);
            var ship2 = new ContainerShip("ShipTwo", 20, 3, 20);

            // Creating containers
            var gasContainer = new GasContainer(200, 100, 1000, 5000, 2);
            var hazardousContainer = new HazardousContainer(200, 100, 1000, 5000, 'H');
            var liquidContainer = new LiquidContainer(200, 100, 1000, 5000, false);
            var refrigeratedContainer = new RefrigeratedContainer(200, 100, 1000, 5000, ProductType.Bananas, 15);

           
            
            // Loading cargo into containers
            try
            {
                gasContainer.LoadCargo(3000);
                hazardousContainer.LoadCargo(2000);
                liquidContainer.LoadCargo(1000);
                refrigeratedContainer.LoadCargo(1500);
                
            }
            catch (OverfillException e)
            {
                Console.WriteLine("OverfillException: " + e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }

            // OverfillException should work here
            try
            {
                hazardousContainer.LoadCargo(9999); 
            }
            catch (OverfillException e)
            {
                Console.WriteLine("Caught OverfillException: " + e.Message);
            }

            
            
            // Loading containers onto ship
            ship1.LoadContainer(gasContainer);
            ship1.LoadContainer(hazardousContainer);

            // Loading list of containers onto ship
            var additionalContainers = new List<Container> { liquidContainer, refrigeratedContainer };
            ship1.LoadContainers(additionalContainers);

            // Removing container
            ship1.RemoveContainer(hazardousContainer);

            // Unloading container
            ship1.UnloadContainer(gasContainer);

            
            // Replacing container
            ship1.ReplaceContainer(refrigeratedContainer.SerialNumber, hazardousContainer);

            // Transferring container between ships
            ship1.TransferContainer(gasContainer, ship2);


            Console.WriteLine(liquidContainer.SerialNumber + " | CargoMass: " + liquidContainer.CargoMass + " kg | TareWeight: " + liquidContainer.TareWeight + " kg");

         
            Console.WriteLine("\n--- ShipOne Info ---");
            ship1.PrintShipInfo();
            Console.WriteLine("\n--- ShipTwo Info ---");
            ship2.PrintShipInfo();
           
        }
    }
}
