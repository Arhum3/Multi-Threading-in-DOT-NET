
using CancellationTokenAndTask.Delay;

namespace CancellationTokenAndTaskDelay
{
    public class CustomRepeatingTaskRunner : RepeatingTaskRunnerBase
    {
        private CancellationTokenSource _cancellationTokenSource;
        private int invl;
        private string TaskName;
        
        //for testing purposes, will be removed
        private int instanceCount;
        public CustomRepeatingTaskRunner(int interval, string taskName, CancellationToken parentToken) {

            _cancellationTokenSource = new CancellationTokenSource();
            TaskName = taskName;
            invl = interval;

            if (parentToken == CancellationToken.None)
                _cancellationTokenSource = new CancellationTokenSource();
            else
                _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(parentToken);

        }
        public override async void Start()
        {
            PreStart();
            Console.WriteLine("Task {0} is starting....", TaskName);
            PostStart();

            var repeatingTask = RunRepeatedlyAsync();
            await repeatingTask;
        }
        public override void WorkAction(CancellationToken cancellationToken)
        {
            Console.WriteLine("Long running work started....");

            try
            {
                //simulating long work
                Thread.Sleep(3000);
                Console.WriteLine("Long running work completed....");
            }
            catch (TaskCanceledException tce)
            {
                Console.WriteLine(tce.Message + " occured during long work, thrown from action method");
            }
        }
        
        
        public async Task RunRepeatedlyAsync()
        {
            Console.WriteLine("Repeating Action Running...");
            try
            {
                while (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    Console.WriteLine("\nTask {1} Running for {0}th time", instanceCount++, TaskName);
                    WorkAction(_cancellationTokenSource.Token);
                    await Task.Delay(invl, _cancellationTokenSource.Token);
                }
            }

            catch (TaskCanceledException tce)
            {
                Console.WriteLine("Task {0} Unable to proceed further because " + tce.Message + " This Exception was thrown at RepeatingTaskRunner Class", TaskName);
            }
        }


        public override void Stop()
        {
            PreStop();
            CancelTask();
            PostStop();
        }
        private void CancelTask()
        {
            _cancellationTokenSource?.Cancel();
        }
    }
}
