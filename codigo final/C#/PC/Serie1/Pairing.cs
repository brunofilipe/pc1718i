using System;
using System.Threading;

namespace Serie1
{
    public class Pairing<T, U>
    {
        private readonly object _lock = new object();
        private T valueT;
        private U valueU;
        private bool _hasValueT, _hasValueU;
        
        public Tuple<T, U> Provide(T value, int timeout)
        {
            lock (_lock)
            {
                //in case that's a second time using the same value
                if (Equals(valueT, value)) return null;
                
                if (_hasValueU)
                {
                    _hasValueT = true;
                    valueT = value;
                    _hasValueU = false;
                    Monitor.PulseAll(_lock);
                    return new Tuple<T, U>(value, valueU);
                }
                
                if (timeout == 0)
                    throw new TimeoutException();
                
                var timeoutHolder = new TimeoutHolder(timeout);

                _hasValueT = true;
                valueT = value;
                
                do
                {
                    try
                    {
                        Monitor.Wait(_lock, timeout);

                        if (_hasValueU)
                        {
                            _hasValueU = false;
                            Monitor.PulseAll(_lock);
                            return new Tuple<T, U>(value, valueU);
                        }
                        
                        if (timeoutHolder.Value == 0)
                        {
                            _hasValueT = false;
                            throw new TimeoutException();
                        }
                    }
                    catch (ThreadInterruptedException)
                    {
                        _hasValueT = false;
                        throw;
                    }
                } while (true);
            }
        }

        public Tuple<T, U> Provide(U value, int timeout)
        {
            lock (_lock)
            {
                //in case that's a second time using the same value
                if (Equals(valueU, value)) return null;
                
                if (_hasValueT)
                {
                    _hasValueU = true;
                    valueU = value;
                    _hasValueT = false;
                    Monitor.PulseAll(_lock);
                    return new Tuple<T, U>(valueT, value);
                }
                
                if (timeout == 0)
                    throw new TimeoutException();
                
                var timeoutHolder = new TimeoutHolder(timeout);

                _hasValueU = true;
                valueU = value;
                
                do
                {
                    try
                    {
                        Monitor.Wait(_lock, timeout);

                        if (_hasValueT)
                        {
                            _hasValueT = false;
                            Monitor.PulseAll(_lock);
                            return new Tuple<T, U>(valueT, value);
                        }
                        
                        if (timeoutHolder.Value == 0)
                        {
                            _hasValueU = false;
                            throw new TimeoutException();
                        }
                    }
                    catch (ThreadInterruptedException)
                    {
                        _hasValueU = false;
                        throw;
                    }
                } while (true);
            }
        }
    }
}