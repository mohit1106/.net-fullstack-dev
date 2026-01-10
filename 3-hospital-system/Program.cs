using System;

class Doctor
{
    public static int TotalDoctors = 0;
    public string Name;
    public int Age;

    public Doctor(string name, int age)
    {
        Name = name;
        Age = age;
        TotalDoctors++;
    }
}

class Cardiologist : Doctor
{
    public string Specialty;

    public Cardiologist(string name, int age, string specialty)
        : base(name, age)
    {
        Specialty = specialty;
    }

    public void PrintDetails()
    {
        Console.WriteLine("Total Doctors: " + Doctor.TotalDoctors);
        Console.WriteLine("Name: " + Name);
        Console.WriteLine("Age: " + Age);
        Console.WriteLine("Specialty: " + Specialty);
    }
}

class Program
{
    static void Main()
    {
        Cardiologist c1 = new Cardiologist("tfgcvh", 45455, "some Specialist");
        Cardiologist c2 = new Cardiologist("shygufy", 5456, "another Specialist");
        c1.PrintDetails();
        Console.WriteLine(Cardiologist.TotalDoctors);
        Console.WriteLine(Doctor.TotalDoctors);
    }
}
