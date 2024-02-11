namespace ParkingLot.Models

public class ParkingSlot
{
    public int SlotNumber { get; set; }
    public bool IsOccupied { get; set; }
    public string RegistrationNumber { get; set; }
    public string Color { get; set; }
    public string VehicleType { get; set; }
}
