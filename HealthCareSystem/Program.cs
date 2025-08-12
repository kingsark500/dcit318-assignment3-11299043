using System;

using System.Collections.Generic;
using System.Linq;

// Generic repository
public class Repository<T>
{
    private List<T> items = new();

    public void Add(T item) => items.Add(item);

    public List<T> GetAll() => items;

    public T? GetById(Func<T, bool> predicate)
    {
        return items.FirstOrDefault(predicate);
    }

    public bool Remove(Func<T, bool> predicate)

    {
        var item = items.FirstOrDefault(predicate);

        if (item != null)
        {
            items.Remove(item);
            return true;
        }
        return false;
    }
}

// Patient class
public class Patient
{
    public int Id { get; }
    public string Name { get; }

    public int Age { get; }
    public string Gender { get; }

    public Patient(int id, string name, int age, string gender)
    {
        Id = id;
        Name = name;

        Age = age;
        Gender = gender;
    }


    public override string ToString()
    {
        return $"Patient ID: {Id}, Name: {Name}, Age: {Age}, Gender: {Gender}";
    }
}

// Prescription class
public class Prescription
{
    public int Id { get; }
    public int PatientId { get; }

    public string MedicationName { get; }
    public DateTime DateIssued { get; }

    public Prescription(int id, int patientId, string medicationName, DateTime dateIssued)
    {
        Id = id;
        PatientId = patientId;

        MedicationName = medicationName;
        DateIssued = dateIssued;
    }

    public override string ToString()
    {
        return $"Prescription ID: {Id}, Medication: {MedicationName}, Issued: {DateIssued:yyyy-MM-dd}";
    }
}

// Health system application
public class HealthSystemApp
{
    private Repository<Patient> _patientRepo = new();

    private Repository<Prescription> _prescriptionRepo = new();

    private Dictionary<int, List<Prescription>> _prescriptionMap = new();

    // Seed initial data
    public void SeedData()
    {
        _patientRepo.Add(new Patient(1, "Julius Sarkodie", 30, "Female"));

        _patientRepo.Add(new Patient(2, "Obuo Adom", 45, "Male"));

        _patientRepo.Add(new Patient(3, "Alexander Danso ", 25, "Female"));

        _patientRepo.Add(new Patient(4, "Josephine Adom", 45, "Female"));

        _prescriptionRepo.Add(new Prescription(101, 1, "Vitamin C", DateTime.Now.AddDays(-2)));

        _prescriptionRepo.Add(new Prescription(102, 1, "Ibuprofen", DateTime.Now.AddDays(-1)));

        _prescriptionRepo.Add(new Prescription(103, 2, "Amoxicillin", DateTime.Now.AddDays(-5)));

        _prescriptionRepo.Add(new Prescription(104, 2, "Cetirizine", DateTime.Now));

        _prescriptionRepo.Add(new Prescription(105, 3, "Metformin", DateTime.Now));

        _prescriptionRepo.Add(new Prescription(104, 4, "ZincTablet", DateTime.Now));
    }

    // Group prescriptions by patient
    public void BuildPrescriptionMap()
    {
        _prescriptionMap.Clear();

        var allPrescriptions = _prescriptionRepo.GetAll();

        foreach (var prescription in allPrescriptions)
        {

            if (!_prescriptionMap.ContainsKey(prescription.PatientId))
            {
                _prescriptionMap[prescription.PatientId] = new List<Prescription>();
            }

            _prescriptionMap[prescription.PatientId].Add(prescription);
        }
    }

    // Print all patients
    public void PrintAllPatients()
    {
        Console.WriteLine("\n--- All Patients ---");
        foreach (var patient in _patientRepo.GetAll())
        {
            Console.WriteLine(patient);
        }
    }

    // Print prescriptions for a specific patient
    public void PrintPrescriptionsForPatient(int id)
    {
        Console.WriteLine($"\n--- Prescriptions for Patient ID: {id} ---");

        if (_prescriptionMap.TryGetValue(id, out var prescriptions))
        {
            foreach (var p in prescriptions)
            {
                Console.WriteLine(p);
            }
        }
        else
        {
            Console.WriteLine("No prescriptions found.");
        }
    }
}

// Main Program
public class Program
{
    public static void Main()
    {
        var app = new HealthSystemApp();

        app.SeedData();

        app.BuildPrescriptionMap();

        app.PrintAllPatients();

        Console.Write("\nEnter Patient ID to view prescriptions: ");

        if (int.TryParse(Console.ReadLine(), out int patientId))
        {
            app.PrintPrescriptionsForPatient(patientId);
        }
        else
        {

            Console.WriteLine("Invalid input.");
        }
        Console.ReadLine();
    }
    
}