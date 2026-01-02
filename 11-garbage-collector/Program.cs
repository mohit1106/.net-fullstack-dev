using System;
class Program{
    public static void Main() {
        // Console.WriteLine("starting");
        // for(int i=0; i<3; i++){
        //     MyClass obj = new MyClass();
        // }

        // var student = (id: 101, name: "amit");
        // Console.WriteLine(GetType(student));

        // Console.WriteLine("forcing gc");
        // GC.Collect();
        // GC.WaitForPendingFinalizers();
        // Console.WriteLine("finally");

        // static (int sum, int diff, int avg) calculate(int a, int b){
        //     return (a +b, a-b, (a+b)/2);
        // }
        // Console.WriteLine(calculate(5, 5));

        static (bool IsValid, string Message) ValidateUser(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return (false, "Username is required");
            }

            return (true, "Valid user");
        }

        var response = ValidateUser("Admin");
        Console.WriteLine(response.Message);
    }
}
// class MyClass {
//     ~MyClass() {
//         Console.WriteLine("finalizer");
//     }
// }