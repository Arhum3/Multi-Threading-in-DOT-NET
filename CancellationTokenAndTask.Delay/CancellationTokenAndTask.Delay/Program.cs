using CancellationTokenAndTaskDelay;
//changes: move cancellation token in RepeatingTaskrunner class, thus implementing Single Responsibility Principle
//Add an action method that would simulate some actual work with delay DONE!
//moving all local variables to RepeatingTaskRunner Class DONE!
//also exposing proper start and stop events DONE!
//extracted the base class from RepeatingTaskRunner Class and also some optimization changes DONE!

//TODO: Implement Logging
internal class Program
{
    private static async Task Main(string[] args)
    {
        string s = "";
        CancellationTokenSource ParenTokenSource = new CancellationTokenSource();
        Console.WriteLine("Enter the interval (in seconds)");
        int invl = int.Parse(Console.ReadLine());
        invl = invl * 1000;

        Console.WriteLine("Enter the name of the Task: ");
        string TaskName = Console.ReadLine();

        Console.WriteLine("Press c to cancel the task.\n");
        var taskRunner = new CustomRepeatingTaskRunner(invl, TaskName, ParenTokenSource.Token);
        taskRunner.Start();
        
        s = Console.ReadLine();

        if (s == "c")
            taskRunner.Stop();


        Console.ReadLine();
    }
}