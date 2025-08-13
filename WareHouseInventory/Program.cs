using System;
using System.Collections.Generic;

// a. Marker Interface for inventory items
public interface IInventoryItem
{
    int Id { get; }

    string Name { get; }

    int Quantity { get; set; }
}

// b. ElectronicItem class
public class ElectronicItem : IInventoryItem
{
    public int Id { get; }
    public string Name { get; }

    public int Quantity { get; set; }
    public string Brand { get; }

    public int WarrantyMonths { get; }

    public ElectronicItem(int id, string name, int quantity, string brand, int warrantyMonths)
    {
        Id = id;
        Name = name;

        Quantity = quantity;
        Brand = brand;

        WarrantyMonths = warrantyMonths;
    }

    public override string ToString()
    {

        return $"[Electronic] ID: {Id}, Name: {Name}, Brand: {Brand}, Qty: {Quantity}, Warranty: {WarrantyMonths} months";

    }
}

// b. GroceryItem class
public class GroceryItem : IInventoryItem
{
    public int Id { get; }
    public string Name { get; 

    public int Quantity { get; set; }

    public DateTime ExpiryDate { get; }

    public GroceryItem(int id, string name, int quantity, DateTime expiryDate)
    {

        Id = id;
        Name = name;
        Quantity = quantity;
        ExpiryDate = expiryDate;

    }

    public override string ToString()
    {

        return $"[Grocery] ID: {Id}, Name: {Name}, Qty: {Quantity}, Expiry: {ExpiryDate:yyyy-MM-dd}";

    }
}

// e. Custom Exceptions
public class DuplicateItemException : Exception
{

    public DuplicateItemException(string message) : base(message) { }

}

public class ItemNotFoundException : Exception
{

    public ItemNotFoundException(string message) : base(message) { }

}

public class InvalidQuantityException : Exception
{
    public InvalidQuantityException(string message) : base(message) { }
}

// c. Inventory repository
public class InventoryRepository<T> where T : IInventoryItem
{
    private Dictionary<int, T> _items = new();

    public void AddItem(T item)
    {
        if (_items.ContainsKey(item.Id))
            throw new DuplicateItemException($"Item with ID {item.Id} already exists.");
        _items[item.Id] = item;
    }

    public T GetItemById(int id)
    {
        if (_items.TryGetValue(id, out var item))
            return item;
        throw new ItemNotFoundException($"Item with ID {id} not found.");
    }

    public void RemoveItem(int id)
    {
        if (!_items.ContainsKey(id))
            throw new ItemNotFoundException($"Cannot remove. Item with ID {id} not found.");
        _items.Remove(id);
    }

    public List<T> GetAllItems() => new List<T>(_items.Values);

    public void UpdateQuantity(int id, int newQuantity)
    {
        if (newQuantity < 0)
            throw new InvalidQuantityException("Quantity cannot be negative.");

        var item = GetItemById(id);
        item.Quantity = newQuantity;
    }
}

// f. Warehouse manager
public class WareHouseManager
{
    private InventoryRepository<ElectronicItem> _electronics = new();
    private InventoryRepository<GroceryItem> _groceries = new();

    public void SeedData()
    {
        // Add electronics
        _electronics.AddItem(new ElectronicItem(1, "Laptop", 10, "Dell", 24));
        _electronics.AddItem(new ElectronicItem(2, "Smartphone", 20, "Samsung", 12));
        _electronics.AddItem(new ElectronicItem(3, "TV", 5, "LG", 36));
        _electronics.AddItem(new ElectronicItem(4, "Ipad", 13, "Apple", 36));


        // Add groceries
        _groceries.AddItem(new GroceryItem(101, "Milk", 50, DateTime.Now.AddDays(7)));
        _groceries.AddItem(new GroceryItem(102, "Bread", 30, DateTime.Now.AddDays(3)));
        _groceries.AddItem(new GroceryItem(103, "Eggs", 60, DateTime.Now.AddDays(10)));
    }

    public void PrintAllItems<T>(InventoryRepository<T> repo) where T : IInventoryItem
    {
        foreach (var item in repo.GetAllItems())
        {
            Console.WriteLine(item);
        }
    }

    public void IncreaseStock<T>(InventoryRepository<T> repo, int id, int quantity) where T : IInventoryItem
    {
        try
        {
            var item = repo.GetItemById(id);
            repo.UpdateQuantity(id, item.Quantity + quantity);
            Console.WriteLine($"Stock increased. New quantity: {item.Quantity}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    public void RemoveItemById<T>(InventoryRepository<T> repo, int id) where T : IInventoryItem
    {
        try
        {
            repo.RemoveItem(id);
            Console.WriteLine($"Item with ID {id} removed.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    public InventoryRepository<ElectronicItem> Electronics => _electronics;
    public InventoryRepository<GroceryItem> Groceries => _groceries;
}

// Main Program
public class Program
{
    public static void Main()
    {
        var manager = new WareHouseManager();

        manager.SeedData();

        Console.WriteLine("\n--- Grocery Items ---");
        manager.PrintAllItems(manager.Groceries);

        Console.WriteLine("\n--- Electronic Items ---");
        manager.PrintAllItems(manager.Electronics);

        // Exception tests
        Console.WriteLine("\n--- Testing Exceptions ---");

        try
        {
            manager.Electronics.AddItem(new ElectronicItem(1, "Duplicate Laptop", 5, "HP", 12));
        }
        catch (DuplicateItemException ex)
        {
            Console.WriteLine($"Duplicate Add Error: {ex.Message}");
        }

        manager.RemoveItemById(manager.Groceries, 999); // Non-existent item

        try
        {
            manager.Groceries.UpdateQuantity(101, -10); // Invalid quantity
        }
        catch (InvalidQuantityException ex)
        {
            Console.WriteLine($"Invalid Quantity Error: {ex.Message}");
        }
        Console.ReadLine();
    }
}