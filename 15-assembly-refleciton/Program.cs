using System;
using System.Reflection;

class Program{
    static void Main() {
        Assembly assembly = Assembly.GetExecutingAssembly();
        Console.WriteLine(assembly);

        // Type type = typeof(Employee);
        Employee employeeObject = new Employee("abc", 12);
        Type type = employeeObject.GetType();
        Console.WriteLine(type);

        // MethodInfo method = type.GetMethod("increaseId");
        // Console.WriteLine(method);
        // method.Invoke(employeeObject, new object[] { 5 });
        // Console.WriteLine(employeeObject.id);

        // PropertyInfo prop = type.GetProperty("property");
        // prop.SetValue(employeeObject, "John");
        // Console.WriteLine(employeeObject.property);

        FieldInfo field = type.GetField(
            "_salary",
            BindingFlags.NonPublic | BindingFlags.Instance
        );
        Console.WriteLine(field);
        Console.WriteLine("prev salary: " + field.GetValue(employeeObject));
        field.SetValue(employeeObject, 50000);
        Console.WriteLine("new salary: " + field.GetValue(employeeObject));

        ConstructorInfo ctor = type.GetConstructor(new Type[] {typeof(string), typeof(int)});
        Object obj = ctor.Invoke(new object[] {"John", 250});
        Employee emp = (Employee)ctor.Invoke(new object[] { "John", 250 });
        Console.WriteLine(emp.name + " : " + emp.id);

        ParameterInfo[] parameters = method.GetParameters();
        
    }
}

class Employee {
    public string property{get; set;}
    private int _salary;

    public string name ;
    public int id ;
    public Employee(string name, int id){
        this.name = name;
        this.id = id;
    }
    public void increaseId(int i){
        id += i;
    }
}