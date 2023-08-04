using CancellationTokenAndTaskDelay;

internal class Program
{
    private static async Task Main(string[] args)
    {
        Console.WriteLine("Enter the interval(in seconds): ");
        int invl = int.Parse(Console.ReadLine());
        //converting the interval to milliseconds
        invl = invl * 1000;

        string s = "";

        Console.WriteLine("Press c to cancel the task\n");

        CancellationTokenSource source = new CancellationTokenSource();
        CancellationToken token = source.Token;

        var taskRunner = new RepeatingTaskRunner();

        var repeatingTask = taskRunner.RunningTaskRepeatedly(invl, token, 0);

        s = Console.ReadLine();

        if (s == "c")
            source.Cancel();

        await repeatingTask;

        Console.ReadLine();
    }
}