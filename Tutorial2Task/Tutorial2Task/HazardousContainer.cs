

namespace ContainersApp.Containers;

public class HazardousContainer : Container, IHazardNotifier
{
    public HazardousContainer(
        double height,
        double depth,
        double tareWeight,
        double maxPayload,
        char containerType) 
        : base(height, depth, tareWeight, maxPayload, containerType)
    {
    }

    public override void LoadCargo(double mass)
    {
        if (mass <= 0) throw new ArgumentException("Mass must be greater than 0");

        if (!CanLoadCargo(mass))
        {
            NotifyHazard("Container is overloaded");
            throw new OverfillException();
        }

        CargoMass += mass;
    }
    
    public void NotifyHazard(string message)
    {
        Console.WriteLine($"Hazardous situation occured for container {SerialNumber}: " + $"\n Message: {message}");
    }
}