using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CancellationTokenAndTask.Delay
{
    public abstract class RepeatingTaskRunnerBase : IDisposable
    {
        private CancellationTokenSource? _cancellationTokenSource;
        public abstract void WorkAction(CancellationToken cancellationToken);

        public virtual void PreStart()
        {
            return;
        }
        public virtual void PostStart()
        {
            return;
        }
        public virtual void PreStop()
        {
            return;
        }
        public virtual void PostStop()
        {
            return;
        }
        public abstract void Start();
        public abstract void Stop();

        public void Dispose()
        {
            if(_cancellationTokenSource!=null)
            {
                _cancellationTokenSource.Dispose();
            }
        }
    }
}
