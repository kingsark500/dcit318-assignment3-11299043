using System;

using System.Collections.Generic;
using System.IO;
using System.Text.Json;

// a. Define immutable InventoryItem using record
public record InventoryItem(int Id, string Name, int Quantity, DateTime DateAdded) : IInventoryEntity;

// b. Marker interface
public interface IInventoryEntity
{
    int Id { get; }
}

// c. Generic InventoryLogger<T>
public class InventoryLogger<T> where T : IInventoryEntity
{
    private List<T> _log = new();
    private readonly string _filePath;

    public InventoryLogger(string filePath)
    {
        _filePath = filePath;
    }

    public void Add(T item)
    {
        _log.Add(item);
    }

    public List<T> GetAll()
    {
        return new List<T>(_log);
    }

    public void SaveToFile()
    {
        try
        {
            string json = JsonSerializer.Serialize(_log, new JsonSerializerOptions { WriteIndented = true });
            using StreamWriter writer = new(_filePath);

            writer.Write(json);

            Console.WriteLine("Data saved successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving data: {ex.Message}");
        }
    }

    public void LoadFromFile()
    {
        try
        {
            if (!File.Exists(_filePath))
            {
                Console.WriteLine("File not found.");
                return;
            }

            using StreamReader reader = new(_filePath);
            string json = reader.ReadToEnd();

            var loadedItems = JsonSerializer.Deserialize<List<T>>(json);
            _log = loadedItems ?? new List<T>();

            Console.WriteLine("Data loaded successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading data: {ex.Message}");
        }
    }
}

// f. Integration layer
public class InventoryApp
{
    private InventoryLogger<InventoryItem> _logger;

    public InventoryApp(string filePath)
    {
        _logger = new InventoryLogger<InventoryItem>(filePath);
    }

    public void SeedSampleData()
    {
        _logger.Add(new InventoryItem(1, "Headset", 10, DateTime.Now));
        _logger.Add(new InventoryItem(2, "Mouse", 50, DateTime.Now));

        _logger.Add(new InventoryItem(3, "hpLaptop", 30, DateTime.Now));
        _logger.Add(new InventoryItem(4, "Smartphone", 20, DateTime.Now));

        _logger.Add(new InventoryItem(5, "Printer", 15, DateTime.Now));
    }


    public void SaveData()
    {

        _logger.SaveToFile();
    }

    public void LoadData()
    {
        _logger.LoadFromFile();
    }

    public void PrintAllItems()
    {
        Console.WriteLine("\n--- Inventory Items ---");
        foreach (var item in _logger.GetAll())
        {
            Console.WriteLine($"ID: {item.Id}, Name: {item.Name}, Quantity: {item.Quantity}, Date Added: {item.DateAdded:yyyy-MM-dd}");
        }
    }
}

// g. Main Method
public class Program
{
    public static void Main()
    {
        string filePath = "inventory.json";

        // First session: Seed and save
        InventoryApp app = new(filePath);
        app.SeedSampleData();

        app.SaveData();

        // Simulate new session: reload data
        Console.WriteLine("\n--- Simulating new session ---");
        InventoryApp newApp = new(filePath);

        newApp.LoadData();

        newApp.PrintAllItems();

        Console.ReadLine();
    }
    
}