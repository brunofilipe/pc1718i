using System;
using System.Threading;

namespace serie2 {
    
    public class ExpirableLazy<T> where T : class {
        
        private class ValueHolder{
            internal readonly T Value;
            internal readonly DateTime TimeToLive;
            public ValueHolder(T value, DateTime timeToLive) {
                Value = value;
                TimeToLive = timeToLive;
            }
        }
        
        private readonly Func<T> _provider;
        private volatile ValueHolder _state;
        private const ValueHolder Unavailable = null;
        private static readonly ValueHolder Busy = new ValueHolder(default(T), DateTime.Now.Add(new TimeSpan(0, 0, 0, 0, Timeout.Infinite)));
        private readonly TimeSpan _timeSpan;

        public ExpirableLazy(Func<T> provider, TimeSpan timeToLive) {
            _provider = provider;
            _timeSpan = timeToLive;
        }

        public T Value {
            get {
                do {
                    var currState = _state;
                    T value;
                    if(currState == Unavailable) {
                        while(Interlocked.CompareExchange(ref _state, Busy, currState) == currState) { 
                            try {
                                value = _provider();
                                var timeToLive = DateTime.Now.Add(_timeSpan);
                                _state = new ValueHolder(value, timeToLive);
                                return value;
                            } catch (Exception e) {
                                _state = currState;
                                throw new InvalidOperationException(e.Message);
                            }
                        }
                    }
                    if(currState != Busy) {
                        while (Interlocked.CompareExchange(ref _state, Busy, currState) == currState) {
                            if(currState.TimeToLive <= DateTime.Now) {
                                try {
                                    value = _provider();
                                    var timeToLive = DateTime.Now.Add(_timeSpan);
                                    _state = new ValueHolder(value, timeToLive);
                                    return value;
                                } catch (Exception) {
                                    _state = currState;
                                    throw new InvalidOperationException();
                                }
                            }
                            value = currState.Value;
                            //TimeToLive didn´t expired and has value calculated
                            Interlocked.Exchange(ref _state, currState);
                            return value;
                        }
                    }
                } while (true);
            }
        }
    }
}
