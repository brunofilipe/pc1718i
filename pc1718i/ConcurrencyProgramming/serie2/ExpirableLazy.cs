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
                T value;
                do {
                    ValueHolder currState = state;
                    if(currState == UNAVAILABLE) {
                        while(Interlocked.CompareExchange(ref state, BUSY, currState) == currState) { 
                            try {
                                value = provider();
                                DateTime _timeToLive = DateTime.Now.Add(timeSpan);
                                state = new ValueHolder(value, _timeToLive);
                                return value;
                            } catch (Exception) {
                                state = currState;
                                throw new InvalidOperationException();
                            }
                        }
                    }
                    if(currState != BUSY) {
                        while (Interlocked.CompareExchange(ref state, BUSY, currState) == currState) {
                            if(currState._timeToLive <= DateTime.Now) {
                                try {
                                    value = provider();
                                    DateTime _timeToLive = DateTime.Now.Add(timeSpan);
                                    state = new ValueHolder(value, _timeToLive);
                                    return value;
                                } catch (Exception) {
                                    state = currState;
                                    throw new InvalidOperationException();
                                }
                            }
                            value = currState.value;
                            //TimeToLive didn´t expired and has value calculated
                            Interlocked.Exchange(ref state, currState);
                            return value;
                        }
                    }
                } while (true);
            }
        }

    }
}
