using System;
using System.Collections.Generic;
using ParkingLot.Models;
using ParkingLot.services;

class Program
{
    private readonly IParkingLotUseCase parkingLotUseCase;

    public Program(IParkingLotUseCase parkingLotUseCase)
    {
        this.parkingLotUseCase = parkingLotUseCase;
    }

    public void Run()
    {
        while (true)
        {
            Console.Write("$ ");
            string command = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(command))
                continue;

            string[] args = command.Split(' ');

            switch (args[0].ToLower())
            {
                case "create_parking_lot":
                    if (args.Length == 2 && int.TryParse(args[1], out int totalSlots))
                    {
                        parkingLotUseCase.CreateParkingLot(totalSlots);
                        Console.WriteLine($"Created a parking lot with {totalSlots} slots");
                    }
                    else
                    {
                        Console.WriteLine("Invalid command. Usage: create_parking_lot <total_slots>");
                    }
                    break;

                case "park":
                    if (args.Length == 4)
                    {
                        string registrationNumber = args[1];
                        string color = args[2];
                        string vehicleType = args[3];

                        int slotNumber = parkingLotUseCase.ParkVehicle(registrationNumber, color, vehicleType);
                        if (slotNumber != -1)
                        {
                            Console.WriteLine($"Allocated slot number: {slotNumber}");
                        }
                        else
                        {
                            Console.WriteLine("Sorry, parking lot is full");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid command. Usage: park <registration_number> <color> <vehicle_type>");
                    }
                    break;

                case "leave":
                    if (args.Length == 2 && int.TryParse(args[1], out int leaveSlotNumber))
                    {
                        parkingLotUseCase.LeaveVehicle(leaveSlotNumber);
                        Console.WriteLine($"Slot number {leaveSlotNumber} is free");
                    }
                    else
                    {
                        Console.WriteLine("Invalid command. Usage: leave <slot_number>");
                    }
                    break;

                case "status":
                    PrintParkingStatus();
                    break;

                case "type_of_vehicles":
                    if (args.Length == 2)
                    {
                        string vehicleType = args[1];
                        PrintVehicleCountByType(vehicleType);
                    }
                    else
                    {
                        Console.WriteLine("Invalid command. Usage: type_of_vehicles <vehicle_type>");
                    }
                    break;

                case "registration_numbers_for_vehicles_with_odd_plate":
                    PrintPlateNumbersByType("odd");
                    break;

                case "registration_numbers_for_vehicles_with_even_plate":
                    PrintPlateNumbersByType("even");
                    break;

                case "registration_numbers_for_vehicles_with_colour":
                    if (args.Length == 2)
                    {
                        string color = args[1];
                        PrintRegistrationNumbersByColor(color);
                    }
                    else
                    {
                        Console.WriteLine("Invalid command. Usage: registration_numbers_for_vehicles_with_colour <color>");
                    }
                    break;

                case "slot_numbers_for_vehicles_with_colour":
                    if (args.Length == 2)
                    {
                        string color = args[1];
                        PrintSlotNumbersByColor(color);
                    }
                    else
                    {
                        Console.WriteLine("Invalid command. Usage: slot_numbers_for_vehicles_with_colour <color>");
                    }
                    break;

                case "slot_number_for_registration_number":
                    if (args.Length == 2)
                    {
                        string registrationNumber = args[1];
                        PrintSlotNumberByRegistrationNumber(registrationNumber);
                    }
                    else
                    {
                        Console.WriteLine("Invalid command. Usage: slot_number_for_registration_number <registration_number>");
                    }
                    break;

                case "exit":
                    Environment.Exit(0);
                    break;

                default:
                    Console.WriteLine("Invalid command. Please try again.");
                    break;
            }
        }
    }

    private void PrintParkingStatus()
    {
        ParkingReport report = parkingLotUseCase.GetParkingReport();
        Console.WriteLine($"Occupied Slots: {report.OccupiedSlots}");
        Console.WriteLine($"Available Slots: {report.AvailableSlots}");
        Console.WriteLine("Vehicle Count by Type:");
        foreach (var kvp in report.VehicleCountByType)
        {
            Console.WriteLine($"{kvp.Key}: {kvp.Value}");
        }
        Console.WriteLine("Odd Plate Numbers: " + string.Join(", ", report.OddPlateNumbers));
        Console.WriteLine("Even Plate Numbers: " + string.Join(", ", report.EvenPlateNumbers));
        Console.WriteLine("Vehicle Count by Color:");
        foreach (var kvp in report.VehicleCountByColor)
        {
            Console.WriteLine($"{kvp.Key}: {kvp.Value}");
        }
    }

    private void PrintVehicleCountByType(string vehicleType)
    {
        int count = parkingLotUseCase.GetParkingReport().VehicleCountByType.GetValueOrDefault(vehicleType, 0);
        Console.WriteLine(count);
    }

    private void PrintPlateNumbersByType(string plateType)
    {
        List<string> plateNumbers = plateType.ToLower() == "odd"
            ? parkingLotUseCase.GetParkingReport().OddPlateNumbers
            : parkingLotUseCase.GetParkingReport().EvenPlateNumbers;

        Console.WriteLine(string.Join(", ", plateNumbers));
    }

    private void PrintRegistrationNumbersByColor(string color)
    {
        List<string> registrationNumbers = parkingLotUseCase.GetParkingReport().VehicleCountByColor
            .Where(kvp => string.Equals(kvp.Key, color, StringComparison.OrdinalIgnoreCase))
            .SelectMany(kvp => Enumerable.Repeat(kvp.Key, kvp.Value))
            .ToList();

        Console.WriteLine(string.Join(", ", registrationNumbers));
    }

    private void PrintSlotNumbersByColor(string color)
    {
        List<string> slotNumbers = parkingLotUseCase.GetParkingReport().VehicleCountByColor
            .Where(kvp => string.Equals(kvp.Key, color, StringComparison.OrdinalIgnoreCase))
            .SelectMany(kvp => Enumerable.Repeat(kvp.Key, kvp.Value))
            .ToList();

        Console.WriteLine(string.Join(", ", slotNumbers));
    }

    private void PrintSlotNumberByRegistrationNumber(string registrationNumber)
    {
        int slotNumber = parkingLotUseCase.GetParkingReport().OccupiedSlots > 0
            ? parkingLotUseCase.GetParkingReport().OccupiedSlots
            : -1;

        Console.WriteLine(slotNumber);
    }

    static void Main()
    {
        var parkingLotUseCase = new ParkingLotUseCase();
        var program = new Program(parkingLotUseCase);
        program.Run();
    }
}
