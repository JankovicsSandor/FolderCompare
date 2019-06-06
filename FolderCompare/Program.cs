using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FolderCompare
{
    class Program
    {
        static void Main(string[] args)
        {

            StreamReader reader = File.OpenText("compare.txt");
            string line;
            string path = @"./outPut/";
            List<Task> tasks = new List<Task>();
            CancellationTokenSource cts = new CancellationTokenSource();

            List<CompareSetting> settings = new List<CompareSetting>();

            while ((line = reader.ReadLine()) != null)
            {
                string[] items = line.Split(',');
                settings.Add(new CompareSetting() { Type = items[0], FromBuild = items[1], ToBuild = items[2] });
            }

            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
            Directory.CreateDirectory(path);

            Console.WriteLine($"Start {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}");
            foreach (CompareSetting item in settings)
            {
                Task t = Task.Run(() => CompareTask.OneMeasure(item, cts.Token)).ContinueWith(resItem =>
                {
                    Console.WriteLine($"Done comparing {resItem.Result.Type}-{resItem.Result.FromBuild}-{resItem.Result.ToBuild}- {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}");
                });
                tasks.Add(t);
            }

            Task.WhenAll(tasks).ContinueWith(prevTask =>
            {
                Console.WriteLine($"Finished {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}");
            }, TaskContinuationOptions.OnlyOnRanToCompletion);
            Console.ReadLine();
            cts.Cancel();
        }
    }
}
