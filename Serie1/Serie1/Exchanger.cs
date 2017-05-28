using System;
using System.Threading;

namespace Serie1 {
    public class Exchanger<T> where T : class {
        public T Exchange(T mine, int timeout) {
            lock (this) {
                int lastTime = (timeout != Timeout.Infinite) ? Environment.TickCount : 0;
                do {
                    try {
                        Monitor.Wait(this, timeout);
                    }
                    catch (ThreadInterruptedException) {
                        
                        throw;
                    }
                    if (SyncUtils.AdjustTimeout(ref lastTime, ref timeout) == 0)
                    {
                        Monitor.PulseAll(this);
                        return null;
                    }
                } while (true);
            }
        }

    }
}