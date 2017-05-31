using System;
using System.Threading;

namespace Serie1 {
    public class Exchanger<T> where T : class {

        private ThreadPair<T> pair;

        public T Exchange(T mine, int timeout) {
            lock (this) {

                if (timeout == 0) {
                    return null;
                }
                if (pair == null) {
                    pair = new ThreadPair<T>(mine);
                }

                if (!pair.Exchanging) { //do swap
                    T val = swap(pair,mine);
                    Monitor.PulseAll(this);
                    return val;
                }


                int lastTime = (timeout != Timeout.Infinite) ? Environment.TickCount : 0;

                do {
                    try {
                        Monitor.Wait(this, timeout);
                    }
                    catch (ThreadInterruptedException) {
                        if (pair.Completed) {
                            return pair.Second;
                        }
                        throw;
                    }
                    if (SyncUtils.AdjustTimeout(ref lastTime, ref timeout) == 0) {
                        Monitor.PulseAll(this);
                        return null;
                    }
                } while (true);
            }
        }

        private T swap(ThreadPair<T> threadPair, T mine) {
            T val = threadPair.First;
            threadPair.Second = mine;
            threadPair.Exchanging = true;
            threadPair.Completed = true;
            return val;
        }
    }
}