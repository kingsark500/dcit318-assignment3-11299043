using System;
using System.Collections.Generic;
using System.IO;

// a. Student class
public class Student
{
    public int Id { get; }
    public string FullName { get; }
    public int Score { get; }

    public Student(int id, string fullName, int score)
    {
        Id = id;
        FullName = fullName;
        Score = score;
    }

    public string GetGrade()
    {
        return Score switch
        {
            >= 80 => "A",
            >= 70 => "B",
            >= 60 => "C",
            >= 50 => "D",
            _ => "F"
        };
    }
}

// b. Custom exceptions
public class InvalidScoreFormatException : Exception
{
    public InvalidScoreFormatException(string message) : base(message) { }
}

public class MissingFieldException : Exception
{
    public MissingFieldException(string message) : base(message) { }
}

// d. StudentResultProcessor class
public class StudentResultProcessor
{
    public List<Student> ReadStudentsFromFile(string inputFilePath)
    {
        List<Student> students = new();

        using (StreamReader reader = new StreamReader(inputFilePath))
        {
            string? line;
            int lineNumber = 0;

            while ((line = reader.ReadLine()) != null)
            {
                lineNumber++;
                if (string.IsNullOrWhiteSpace(line)) continue;

                var parts = line.Split(',');

                if (parts.Length != 3)
                {
                    throw new MissingFieldException($"Line {lineNumber}: Missing fields. Expected 3 values.");
                }

                try
                {
                    int id = int.Parse(parts[0].Trim());
                    string name = parts[1].Trim();
                    int score = int.Parse(parts[2].Trim());

                    students.Add(new Student(id, name, score));
                }
                catch (FormatException)
                {
                    throw new InvalidScoreFormatException($"Line {lineNumber}: Invalid score format for student: {line}");
                }
            }
        }

        return students;
    }

    public void WriteReportToFile(List<Student> students, string outputFilePath)
    {
        using (StreamWriter writer = new StreamWriter(outputFilePath))
        {
            foreach (var student in students)
            {
                writer.WriteLine($"{student.FullName} (ID: {student.Id}): Score = {student.Score}, Grade = {student.GetGrade()}");
            }
        }
    }
}

// e. Main program
public class Program
{
    public static void Main()
    {
        string inputFile = "students.txt";         // Adjust filename if needed
        string outputFile = "report.txt";

        try
        {
            StudentResultProcessor processor = new StudentResultProcessor();

            var students = processor.ReadStudentsFromFile(inputFile);

            processor.WriteReportToFile(students, outputFile);

            Console.WriteLine("Student report generated successfully!");
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine("Error: Input file not found.");
        }
        catch (InvalidScoreFormatException ex)
        {
            Console.WriteLine($"Invalid Score Error: {ex.Message}");
        }
        catch (MissingFieldException ex)
        {
            Console.WriteLine($"Missing Field Error: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected Error: {ex.Message}");
        }
        Console.ReadLine();
    }
}