
namespace CancellationTokenAndTaskDelay
{
    public class RepeatingTaskRunner
    {
        private CancellationTokenSource _cancellationTokenSource;
        private int invl;
        private string TaskName;
        private int instanceCount;
        public RepeatingTaskRunner() {

            _cancellationTokenSource = new CancellationTokenSource();
            TaskName = "";
            instanceCount = 0;
            invl = 0;
        }
        private void onStart() {

            Console.WriteLine("Enter the interval(in seconds): ");
            invl = int.Parse(Console.ReadLine());
            //converting the interval to milliseconds
            invl = invl * 1000;

            Console.WriteLine("Enter the name of the task: ");
            TaskName = Console.ReadLine();
        }
        public void Start() {
            onStart();
        }
        public async Task RunningTaskRepeatedly(Action<CancellationToken>workAction)
        {
            Console.WriteLine("Repeating Action Running...");
            try
            {
                while (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    Console.WriteLine("\nTask {1} Running for {0}th time", instanceCount++, TaskName);
                    workAction(_cancellationTokenSource.Token);
                    await Task.Delay(invl, _cancellationTokenSource.Token);
                }
            }

            catch (TaskCanceledException tce)
            {
                Console.WriteLine("Task {0} Unable to proceed further because " + tce.Message + " This Exception was thrown at RepeatingTaskRunner Class", TaskName);
            }
        }

        public void Stop()
        {
            CancelTask();
        }
        private void CancelTask()
        {
           _cancellationTokenSource?.Cancel(); 
        }
    }
}
