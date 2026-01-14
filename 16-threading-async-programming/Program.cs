Using System.Threading;

class Program {
    static void Main() {
        Thread thread = new Thread( new ParameterizedThreadStart(PrintMessage));
        thread.Start("heelo from thread");
    }
    static void PrintMessage(object message) {
        Console.WriteLine(message);
    }
}