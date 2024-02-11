namespace ParkingLot.Models

public class ParkingReport
{
    public int OccupiedSlots { get; set; }
    public int AvailableSlots { get; set; }
    public Dictionary<string, int> VehicleCountByType { get; set; }
    public List<string> OddPlateNumbers { get; set; }
    public List<string> EvenPlateNumbers { get; set; }
    public Dictionary<string, int> VehicleCountByColor { get; set; }
}