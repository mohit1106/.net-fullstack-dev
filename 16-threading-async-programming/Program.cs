using System;
using System.Threading;

class Program {
    static void Main() {
        Thread worker = new Thread(DoWork);
        worker.Start();
        Console.WriteLine("New Thread continues...");
    }

    static void DoWork() {
        for (int i = 0; i <= 5; i++) {
            Console.WriteLine("worker thread: " + i);
            Thread.Sleep(1000);
        }
    }
}
