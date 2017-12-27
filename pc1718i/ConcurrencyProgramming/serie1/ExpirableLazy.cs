using System;
using System.Collections.Generic;
using System.Threading;

namespace ConcurrencyProgramming.serie1 {
    public class ExpirableLazy<T> where T : class {

        private readonly Func<T> provider;
        private bool _isCompleted;
        private T _val;
        private readonly DateTime _timeToLive;
        private readonly object _lock = new object();
        private readonly LinkedList<int> _threadQueue;

        public ExpirableLazy(Func<T> provider, TimeSpan timeToLive) {
            this.provider = provider;
            this._timeToLive = DateTime.Now.Add(timeToLive);
            this._threadQueue = new LinkedList<int>();
        }

        public T Value {
            get {
                lock (_lock) {
               
                    int threadId = Thread.CurrentThread.ManagedThreadId;
                    DateTime now = DateTime.Now;
                    if (_timeToLive <= now && _isCompleted) {
                        return _val;
                    }
                    //fazer o calculo, e retornar o val , tudo na thread invocante
                    if ((!_isCompleted || _timeToLive > now) && _threadQueue.Count == 0) {
                        try {
                            _val = provider();
                            _isCompleted = true;
                            return _val;
                        }
                        catch {
                            throw new InvalidOperationException();
                        }
                    }
                    LinkedListNode<int> node = _threadQueue.AddLast(threadId);
                    do {

                        if (_val == null && _threadQueue.First.Value == threadId) {
                            _threadQueue.Remove(node);
                            try {  
                                _val = provider();
                                _isCompleted = true;
                                return _val;
                            }
                            catch (Exception) {
                                Monitor.PulseAll(_lock);
                                throw new InvalidOperationException();
                            }
                        }
                        if (_val != null) {
                            Monitor.PulseAll(_lock);
                            _threadQueue.Remove(node);
                            return _val;
                        }
                        try {
                            Monitor.Wait(_lock);
                        }
                        catch (ThreadInterruptedException) {
                            _threadQueue.Remove(node);
                            Monitor.PulseAll(_lock);
                            throw;
                        }


                    } while (true);
                }
            }
        }


    }
}