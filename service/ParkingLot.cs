using System;
using System.Linq;
using System.Collections.Generic;
using ParkingLot.Models

namespace ParkingLot.services
public interface IParkingLotUseCase
{
    void CreateParkingLot(int totalSlots);
    int ParkVehicle(string registrationNumber, string color, string vehicleType);
    void LeaveVehicle(int slotNumber);
    ParkingReport GetParkingReport();
}

public class ParkingLotUseCase : IParkingLotUseCase
{
    private readonly List<ParkingSlot> parkingSlots;

    public ParkingLotUseCase()
    {
        parkingSlots = new List<ParkingSlot>();
    }

    public void CreateParkingLot(int totalSlots)
    {
        parkingSlots.Clear();
        for (int i = 1; i <= totalSlots; i++)
        {
            parkingSlots.Add(new ParkingSlot { SlotNumber = i, IsOccupied = false });
        }
    }

    public int ParkVehicle(string registrationNumber, string color, string vehicleType)
    {
        var availableSlot = parkingSlots.FirstOrDefault(slot => !slot.IsOccupied);
        if (availableSlot != null)
        {
            availableSlot.IsOccupied = true;
            availableSlot.RegistrationNumber = registrationNumber;
            availableSlot.Color = color;
            availableSlot.VehicleType = vehicleType;
            return availableSlot.SlotNumber;
        }
        return -1;
    }

    public void LeaveVehicle(int slotNumber)
    {
        var slotToLeave = parkingSlots.FirstOrDefault(slot => slot.SlotNumber == slotNumber);
        if (slotToLeave != null)
        {
            slotToLeave.IsOccupied = false;
            slotToLeave.RegistrationNumber = null;
            slotToLeave.Color = null;
            slotToLeave.VehicleType = null;
        }
    }

    public ParkingReport GetParkingReport()
    {
        var report = new ParkingReport
        {
            OccupiedSlots = parkingSlots.Count(slot => slot.IsOccupied),
            AvailableSlots = parkingSlots.Count(slot => !slot.IsOccupied),
            VehicleCountByType = parkingSlots
                .Where(slot => slot.IsOccupied)
                .GroupBy(slot => slot.VehicleType)
                .ToDictionary(group => group.Key, group => group.Count()),
            OddPlateNumbers = GetPlateNumbersByType("odd"),
            EvenPlateNumbers = GetPlateNumbersByType("even"),
            VehicleCountByColor = parkingSlots
                .Where(slot => slot.IsOccupied)
                .GroupBy(slot => slot.Color)
                .ToDictionary(group => group.Key, group => group.Count())
        };
        return report;
    }

    private List<string> GetPlateNumbersByType(string plateType)
    {
        int lastDigitPredicate = plateType.ToLower() == "odd" ? 1 : 0;
        return parkingSlots
            .Where(slot => slot.IsOccupied && int.Parse(slot.RegistrationNumber.Last().ToString()) % 2 == lastDigitPredicate)
            .Select(slot => slot.RegistrationNumber)
            .ToList();
    }
}