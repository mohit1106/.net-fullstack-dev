// using System;

// class Student
// {
//     public int Id { get; set; }
//     public string Name { get; set; }

//     public void Deconstruct(out int id, out string name)
//     {
//         id = Id;
//         name = Name;
//     }
// }
// class Program1 {
//     public static void Main(){
//         var s = new Student { Id = 1, Name = "Amit" };
//         s.GetType();
//         var (sid, sname) = s;

//         Console.WriteLine(sid);
//         Console.WriteLine(sname);
//     }
// }
