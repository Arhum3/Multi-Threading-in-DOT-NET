
namespace CancellationTokenAndTaskDelay
{
    public class RepeatingTaskRunner
    {
        public RepeatingTaskRunner() { }
        public async Task RunningTaskRepeatedly(int invl, CancellationToken token, int count)
        {
            Console.WriteLine("Task Running...");
            try
            {
                while (!token.IsCancellationRequested)
                {
                    Console.WriteLine("Same Task Running for {0}th time", count++);
                    await Task.Delay(invl, token);
                }
            }

            catch (TaskCanceledException tce)
            {
                Console.WriteLine("Unable to proceed further because " + tce.Message);
            }
        }
    }
}
