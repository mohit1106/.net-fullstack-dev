// using System;
// class Program{
//     public static void Main() {
//         // Console.WriteLine("starting");
//         // for(int i=0; i<3; i++){
//         //     MyClass obj = new MyClass();
//         // }

//         // var student = (id: 101, name: "amit");
//         // Console.WriteLine(GetType(student));

//         // Console.WriteLine("forcing gc");
//         // GC.Collect();
//         // GC.WaitForPendingFinalizers();
//         // Console.WriteLine("finally");

//         // static (int sum, int diff, int avg) calculate(int a, int b){
//         //     return (a +b, a-b, (a+b)/2);
//         // }
//         // Console.WriteLine(calculate(5, 5));

//         // static (bool IsValid, string Message) ValidateUser(string username)
//         // {
//         //     if (string.IsNullOrEmpty(username))
//         //     {
//         //         return (false, "Username is required");
//         //     }

//         //     return (true, "Valid user");
//         // }

//         // var response = ValidateUser("Admin");
//         // Console.WriteLine(response.Message);

//     }
// }
// // class MyClass {
// //     ~MyClass() {
// //         Console.WriteLine("finalizer");
// //     }
// // }



using System;
using System.Linq;
using System.Collections.Generic;

class Student
{
    public string Name { get; set; }
    public int Marks { get; set; }
}

class Program
{
    static void Main()
    {
        // var students = new List<Student>
        // {
        //     new Student { Name = "John", Marks = 75 },
        //     new Student { Name = "Jane", Marks = 50 }
        // };
        
        // var result = students.Select(s => new {
        //     s.Name,
        //     Grade = s.Marks > 60 ? "Pass" : "Fail"
        // });
        
        // Console.WriteLine(result.GetType());

        List<int> numbers = new List<int>{5,2,4,8,7};
        var asc = numbers.OrderBy(n => n);
        var desc = numbers.OrderByDescending(n => n);
        foreach(var n in asc) Console.Write(n + " ");
        Console.WriteLine();
        foreach(var n in desc) Console.Write(n + " ");
        Console.WriteLine();



        

        List<Employee> employees = new List<Employee>
        {
            new Employee { Name = "Amit", Salary = 50000 },
            new Employee { Name = "Ravi", Salary = 70000 },
            new Employee { Name = "Neha", Salary = 60000 }
        };

        var sortedBySalary = employees.OrderBy(e => e.Salary);
        foreach(var n in sortedBySalary) Console.Write(n.Name + " ");
    }
    class Employee
    {
        public string Name { get; set; }
        public int Salary { get; set; }
    }
}