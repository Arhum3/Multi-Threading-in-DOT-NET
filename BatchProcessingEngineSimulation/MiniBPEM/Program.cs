using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MiniBPEM
{
    class Program
    {
        //static void OddNums(string taskName, CancellationToken token)
        //{
        //    for (int n = 1; n < (20 + 1); n++)
        //    {
        //        if (n % 2 != 0)
        //        {
        //            if (!token.IsCancellationRequested)
        //            {
        //                Console.WriteLine($"{taskName}: {n}");
        //            }
        //        }
        //    }
        //}


        static void RunBatches(List<int> batchData, int batch, CancellationToken linkedToken)
        {
            try
            {
                var po = new ParallelOptions() { CancellationToken = linkedToken };
                var result = Parallel.ForEach(batchData, po, (item, state) =>
                   {
                       linkedToken.ThrowIfCancellationRequested();
                       RunTask(item, batch, linkedToken);
                   });
            }
            catch (AggregateException aex)
            {
                throw;
            }
        }
        static void RunTask(int data, int batchNumber, CancellationToken token)
        {
            var rnd = new Random(Guid.NewGuid().GetHashCode());
            var time = rnd.Next(2, 7);
            Console.WriteLine($"Running Batch: {batchNumber} and Task: {data}");
            Task.Delay(new TimeSpan(0, 0, time), token).Wait();
        }


        static void EvenNums(int batchInd, int breakIndex, CancellationToken token, List<int> data, CancellationTokenSource tokenSource)
        {
            Parallel.For(0, data.Count, (i) =>
            {
                if (token.IsCancellationRequested)
                {
                    Console.WriteLine("Task {0} of batch {1} canceled", i, batchInd);

                }

                else if (i == breakIndex)
                {

                    Console.WriteLine("Task {0} of batch {1} canceled automatically", i, batchInd);

                    tokenSource.Cancel();
                    tokenSource.Dispose();
                    tokenSource = new CancellationTokenSource();
                    token = tokenSource.Token;

                    //return;
                }
                else
                {
                    Console.WriteLine("task {0}", data[i]);
                    Thread.Sleep(2000);
                }

            });
        }
        static void Main(string[] args)
        {

            CancellationTokenSource cnclToken = new CancellationTokenSource();
            CancellationToken token = cnclToken.Token;

            string msg = "Currently running";
            List<int> list = Enumerable.Range(0, 150).ToList();
            CallContext.LogicalSetData("name", msg);


            var rnd = new Random();
            int breakIndex = rnd.Next(1, 11);

            var batches = list.Select((item, i) => new { Index = i, Value = item }).GroupBy(item => item.Index / 50)
                .Select(item => item.Select(val => val.Value).ToList())
                .ToList();

            var po = new ParallelOptions() { CancellationToken = token };
            var result = Task.Run(()=> Parallel.ForEach(batches, po, (batch, index) =>
            {
                //also need to pass cancelToken(token source) to cancel within task 
                //EvenNums(batches.IndexOf(batch), breakIndex, token, batch, cnclToken);
                var batchToken = new CancellationTokenSource(5 * 1000);
                try
                {
                    using (CancellationTokenSource linkedCts = CancellationTokenSource.CreateLinkedTokenSource(batchToken.Token, token))
                    {
                        RunBatches(batch, batches.IndexOf(batch), linkedCts.Token);
                    }
                }
                catch (Exception oce)
                {
                    if (oce is AggregateException)
                    {
                        var aex = oce as AggregateException;
                        foreach (var ex in aex.InnerExceptions)
                        {
                            Console.WriteLine($"{ex.InnerException.Message}");
                        }
                    }
                    if (token.IsCancellationRequested)
                        Console.WriteLine($"Cancellation requested by user");
                    else if (batchToken.Token.IsCancellationRequested)
                        Console.WriteLine($"Batch cancelled after 5 seconds");
                }
            }));

            char ch = Console.ReadKey().KeyChar;
            if (ch == 'c' || ch == 'C')
            {
                cnclToken.Cancel();
                Console.WriteLine("Task Cancellation Requested");
            }



            // Set the data in the logical call context
            //CallContext.LogicalSetData("Username", "Sikandar Mirza");             // Start three tasks in different threads
            //Task task1 = Task.Factory.StartNew(() => OddNums("OddNums", cnclToken.Token), cnclToken.Token);
            //Task task2 = Task.Factory.StartNew(() => EvenNums("EvenNums", cnclToken.Token), cnclToken.Token);
            //Task task3 = Task.Factory.StartNew(() => EvenNums("Task 3", cnclToken.Token), cnclToken.Token);             // Wait for the user to press Enter
            //Console.WriteLine("Press Enter to cancel the current task");
            //Console.ReadLine();             // Cancel the current task
            //cnclToken.Cancel();             // Wait for all tasks to complete
            try
            {
                // Task.WaitAll();
            }
            catch (AggregateException ex)
            {
                foreach (Exception inner in ex.InnerExceptions)
                {
                    Console.WriteLine("Task Exception: " + inner.Message);
                }
            }
            finally
            {
                //cnclToken.Dispose();
            }
            Console.ReadLine();             // Cancel the current task
        }
        //static void LogCallContextData(string taskName, CancellationToken token)
        //{
        //    try
        //    {
        //        while (!token.IsCancellationRequested)
        //        {
        //            // Get the data from the logical call context
        //            string username = (string)CallContext.LogicalGetData("Username");
        //            Console.WriteLine($"{taskName}: {username}");                     // Wait for 1 second
        //            token.WaitHandle.WaitOne(1000);
        //            Console.ReadLine();
        //        }
        //    }
        //    catch (OperationCanceledException)
        //    {
        //        Console.WriteLine($"{taskName}: Cancelled");
        //        Console.ReadLine();
        //        throw;
        //    }
        //    finally
        //    {
        //        Console.ReadLine();
        //    }
        //}
    }
}
