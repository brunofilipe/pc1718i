using System;
using System.Collections.Generic;
using System.Threading;

namespace ConcurrencyProgramming.serie1 {
    public class ExpirableLazy<T> where T : class {

        private readonly Func<T> provider;
        private bool isCompleted;
        private T val;
        private readonly TimeSpan timeToLive;
        private readonly object _lock = new object();
        private LinkedList<int> threadQueue;

        public ExpirableLazy(Func<T> provider, TimeSpan timeToLive) {
            this.provider = provider;
            this.timeToLive = timeToLive;
            this.threadQueue = new LinkedList<int>();
        }

        public T Value {
            get {
                lock (_lock) {
                    int threadId = Thread.CurrentThread.ManagedThreadId;
                    TimeSpan now = DateTime.Now.TimeOfDay;
                    if (timeToLive >= now && isCompleted) {
                        return val;
                    }
                    //fazer o calculo, e retornar o val , tudo na thread invocante
                    if ((!isCompleted || timeToLive < now) && threadQueue.Count == 0) {
                        try {
                            val = provider();
                            isCompleted = true;
                            return val;
                        }
                        catch (Exception) {
                            throw;
                        }
                    }
                    LinkedListNode<int> node = threadQueue.AddLast(threadId);
                    do {
                        try {
                            Monitor.Wait(_lock);
                        }
                        catch (ThreadInterruptedException) {
                            threadQueue.Remove(node);
                            Monitor.PulseAll(_lock);
                            throw;
                        }

                        if (val == null && threadQueue.First.Value == threadId) {
                            threadQueue.Remove(node);
                            try {
                                val = provider();
                                isCompleted = true;
                                return val;
                            }
                            catch (Exception) {
                                Monitor.PulseAll(_lock);
                                throw;
                            }
                        }
                        if (val != null){
                            Monitor.PulseAll(_lock);
                            threadQueue.Remove(node);
                            return val;
                        }

                    } while (true);
                }
            }
        }


    }
}