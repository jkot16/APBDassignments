namespace ContainersApp.Containers;

public class LiquidContainer : HazardousContainer
{
    private readonly double _allowedMaxPayload;

    public LiquidContainer(
        double height,
        double depth,
        double tareWeight,
        double maxPayload,
        bool isCargoHazardous)
        : base(height, depth, tareWeight, maxPayload, 'L')
    {
        IsCargoHazardous = isCargoHazardous;
        var capacityLimit = IsCargoHazardous ? 0.5 : 0.9;
        _allowedMaxPayload = maxPayload * capacityLimit;
    }

    public bool IsCargoHazardous { get; }
    
    protected override bool CanLoadCargo(double mass) => CargoMass + mass <= _allowedMaxPayload;
}