using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AsyncFileDemo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileController : ControllerBase
    {
        // SYNCHRONOUS VERSION
        [HttpGet("read-sync")]
        public string ReadSync()
        {
            Stopwatch sw = Stopwatch.StartNew();

            string path1 = @"C:\Temp\file1.txt";
            string path2 = @"C:\Temp\file2.txt";

            // Read file1 first
            string file1Content = System.IO.File.ReadAllText(path1);

            // After file1 finishes, read file2
            string file2Content = System.IO.File.ReadAllText(path2);

            sw.Stop();
            return $"File1: {file1Content}\nFile2: {file2Content}\nTime: {sw.ElapsedTicks} ticks";

            // return $"File1: {file1Content}\nFile2: {file2Content}\nTime Taken: {sw.ElapsedMilliseconds} ms";
        }

        // ASYNCHRONOUS VERSION
        [HttpGet("read-async")]
        public async Task<string> ReadAsync()
        {
            Stopwatch sw = Stopwatch.StartNew();

            string path1 = @"C:\Temp\file1.txt";
            string path2 = @"C:\Temp\file2.txt";

            // Start both reads at the same time
            Task<string> file1Task = System.IO.File.ReadAllTextAsync(path1);
            Task<string> file2Task = System.IO.File.ReadAllTextAsync(path2);

            // Wait for both
            string file1Content = await file1Task;
            string file2Content = await file2Task;

            sw.Stop();
            return $"File1: {file1Content}\nFile2: {file2Content}\nTime: {sw.ElapsedTicks} ticks";

            //return $"File1: {file1Content}\nFile2: {file2Content}\nTime Taken: {sw.ElapsedMilliseconds} ms";
        }
    }
}