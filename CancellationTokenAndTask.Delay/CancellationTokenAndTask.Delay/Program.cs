using CancellationTokenAndTaskDelay;
//changes: move cancellation token in RepeatingTaskrunner class, thus implementing Single Responsibility Principle
//Add an action method that would simulate some actual work with delay
//moving all local variables to RepeatingTaskRunner Class
//also exposing proper start and stop events

//TODO: Implement Logging
internal class Program
{
    private static async Task Main(string[] args)
    {
        string s = "";
        var taskRunner = new RepeatingTaskRunner();

        Console.WriteLine("Press c to cancel the task\n");
        
        taskRunner.Start();

        Action<CancellationToken> WorkAction = (CancellationToken) => {
            
            Console.WriteLine("Long Work Simulation Started...");

            try
            {
                Thread.Sleep(3000);
                Console.WriteLine("Long Work Simulation Ended...");
            }
            catch (TaskCanceledException tce)
            {
                Console.Out.WriteLineAsync(tce.Message + " This Exception occured in long running work simulation, and it is thrown in action method");
            }
            
        };

        var repeatingTask = taskRunner.RunningTaskRepeatedly(WorkAction);

        s = Console.ReadLine();

        if (s == "c")
            taskRunner.Stop();

        await repeatingTask;

        Console.ReadLine();
    }
}