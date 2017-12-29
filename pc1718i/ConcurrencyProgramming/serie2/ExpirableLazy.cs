using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConcurrencyProgramming.serie2 {
    class ExpirableLazy<T> where T : class {
        private class ValueHolder{
            internal readonly T value;
            internal DateTime _timeToLive;
            public ValueHolder(T value, DateTime _timeToLive) {
                this.value = value;
                this._timeToLive = _timeToLive;
            }
        }
        private readonly Func<T> provider;
        private volatile ValueHolder state = null;
        private const ValueHolder UNAVAILABLE = null;
        private static readonly ValueHolder BUSY = new ValueHolder(default(T), DateTime.Now.Add(new TimeSpan(0, 0, 0, 0, Timeout.Infinite)));
        private readonly TimeSpan timeSpan;
        private readonly object _lock = new object();
        private readonly LinkedList<int> _threadQueue;

        public ExpirableLazy(Func<T> provider, TimeSpan timeToLive) {
            this.provider = provider;
            this.timeSpan = timeToLive;
            this._threadQueue = new LinkedList<int>();
        }

        private bool IsAvailable(ValueHolder observed) {
            return observed != UNAVAILABLE && observed != BUSY && observed._timeToLive > DateTime.Now;
        }

        public T Value {
            get {
                do {
                    ValueHolder currState = state;
                    if(currState == UNAVAILABLE) {
                        do {

                        } while(Interlocked.CompareExchange(ref state, BUSY, sn))
                    }
                } while (true);
                lock (_lock) {
                    int threadId = Thread.CurrentThread.ManagedThreadId;
                    DateTime now = DateTime.Now;
                    //fazer o calculo, e retornar o val , tudo na thread invocante
                    if ((!_isCompleted || _timeToLive > now) && _threadQueue.Count == 0) {
                        try {
                            _val = provider();
                            _timeToLive = DateTime.Now.Add(timeSpan);
                            _isCompleted = true;
                            return _val;
                        } catch {
                            throw new InvalidOperationException();
                        }
                    }
                    LinkedListNode<int> node = _threadQueue.AddLast(threadId);
                    do {

                        try {
                            Monitor.Wait(_lock);
                        } catch (ThreadInterruptedException) {
                            _threadQueue.Remove(node);
                            Monitor.PulseAll(_lock);
                            throw;
                        }
                        if (_val == null && _threadQueue.First.Value == threadId) {
                            _threadQueue.Remove(node);
                            try {
                                _val = provider();
                                _timeToLive = DateTime.Now.Add(timeSpan);
                                _isCompleted = true;
                                return _val;
                            } catch (Exception) {
                                Monitor.PulseAll(_lock);
                                throw new InvalidOperationException();
                            }
                        }
                        if (_val != null) {
                            Monitor.PulseAll(_lock);
                            _threadQueue.Remove(node);
                            return _val;
                        }
                    } while (true);
                }
            }
        }

    }
}
