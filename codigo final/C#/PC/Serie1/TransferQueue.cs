using System.Collections.Generic;
using System.Threading;

namespace Serie1
{
    public class TransferQueue<T>
    {
        private readonly LinkedList<T> _trList = new LinkedList<T>();
        private readonly object _lock = new object();
        
        public void Put(T msg)
        {
            lock (_lock)
            {
                _trList.AddLast(msg);
                Monitor.PulseAll(_lock);
            }
        }

        public bool Transfer(T msg, int timeout)
        {
            lock (_lock)
            {
                if (timeout == 0) return false;

                var timeoutHolder = new TimeoutHolder(timeout);
                
                _trList.AddLast(msg);

                do
                {
                    try
                    {
                        Monitor.Wait(_lock, timeout);
                    }
                    catch (ThreadInterruptedException)
                    {
                        _trList.Remove(msg);
                        Monitor.PulseAll(_lock);
                        throw;
                    }
                    
                    if ((timeout = timeoutHolder.Value) == 0)
                    {
                        _trList.Remove(msg);
                        Monitor.PulseAll(_lock);
                        return false;
                    }
                    
                    if (!_trList.Contains(msg))
                    {
                        Monitor.PulseAll(_lock);
                        return true;
                    }
                    
                } while (true);
            }
        }

        public bool Take(int timeout, out T rmsg)
        {
            lock (_lock)
            {
                rmsg = default(T);
                
                if (timeout == 0) return false;
                
                if (_trList.Count > 0)
                {
                    rmsg = _trList.First.Value;    //using FIFO politics
                    _trList.RemoveFirst();
                    
                    Monitor.PulseAll(_lock);
                    return true;
                }

                var timeoutHolder = new TimeoutHolder(timeout);

                do
                {
                    try
                    {
                        Monitor.Wait(_lock, timeout);
                    }
                    catch (ThreadInterruptedException)
                    {
                        Monitor.PulseAll(_lock);
                        throw;
                    }
                    
                    if ((timeout = timeoutHolder.Value) == 0)
                    {
                        Monitor.PulseAll(_lock);
                        return false;
                    }
                    
                    if (_trList.Count > 0)
                    {
                        rmsg = _trList.First.Value;    //using FIFO politics
                        _trList.RemoveFirst();
                    
                        Monitor.PulseAll(_lock);
                        return true;
                    }
                    
                } while (true);
            }
        }
    }
}