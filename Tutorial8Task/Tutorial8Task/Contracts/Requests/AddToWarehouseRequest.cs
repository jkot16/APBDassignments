namespace Tutorial8Task.Contracts.Requests;

public class AddToWarehouseRequest
{
    public int ProductId    { get; set; }
    public int WarehouseId  { get; set; }
    public int Amount       { get; set; }
    public DateTime CreatedAt { get; set; }
}