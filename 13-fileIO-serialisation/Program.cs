using System;
using System.IO;
using System.Text.Json;

class Program{
    static void Main() {
        FileInfo file = new FileInfo("sample.txt");
        if(!file.Exists) {
            using (StreamWriter writer = file.CreateText()) {
                writer.WriteLine("Hello fileInfo class");
            }
        }
        Console.WriteLine("name : " + file.Name);
        Console.WriteLine("length : " + file.Length);
        Console.WriteLine("time  : " + file.CreationTime);

        // Directory
        Directory.CreateDirectory("Logs");
        if(Directory.Exists("Logs")) {
            Console.WriteLine("logs directory created");
        }

        // DirectoryInfo
        DirectoryInfo dir = new DirectoryInfo("Logs");
        if(!dir.Exists) {
            dir.Create();
        }
        Console.WriteLine("Name: "+ dir.Name);
        Console.WriteLine("full path: "+ dir.FullName);



        // User user = new User { Id = 1, Name = "Alice" };
        // string json = JsonSerializer.Serialize(user);
        // File.WriteAllText("user.json", json);
        // Console.WriteLine("User serialized successfully.");



        // string json = File.ReadAllText("user.json");
        // User user = JsonSerializer.Deserialize<User>(json);
        // Console.WriteLine($"User Loaded: {user.Id}, {user.Name}");


        User user = new User { Id = 1, Name = "Alice" };
        XmlSerializer serializer = new XmlSerializer(typeof(User));
        using (FileStream fs = new FileStream("user.xml", FileMode.Create))
        {
            serializer.Serialize(fs, user);
        }

        Console.WriteLine("XML Serialized");

    }
    [Serializable]
    public class User
    {
        public int Id;
        public string Name;
    }

}