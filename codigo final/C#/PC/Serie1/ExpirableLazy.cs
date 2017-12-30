using System;
using System.Collections.Generic;
using System.Threading;

namespace Serie1
{
    public class ExpirableLazy<T> where T : class
    {
        private readonly Func<T> _provider;
        private bool _isCompleted;
        private T _val;
        private readonly TimeSpan _timeSpan;
        private  DateTime _timeToLive;
        private readonly object _lock = new object();
        private readonly LinkedList<int> _threadQueue;

        public ExpirableLazy(Func<T> provider, TimeSpan timeToLive) {
            _provider = provider;
            _timeSpan = timeToLive;
            _threadQueue = new LinkedList<int>();
        }

        public T Value {
            get {
                lock (_lock) {
               
                    var threadId = Thread.CurrentThread.ManagedThreadId;
                    var now = DateTime.Now;
                    if (_timeToLive > now && _isCompleted) {
                        return _val;
                    }
                    
                    //fazer o calculo, e retornar o val , tudo na thread invocante
                    if ((!_isCompleted || _timeToLive <= now) && _threadQueue.Count == 0) {
                        try {
                            _val = _provider();
                            _timeToLive = DateTime.Now.Add(_timeSpan);
                            _isCompleted = true;
                            return _val;
                        }
                        catch (Exception e) {
                            throw new InvalidOperationException(e.Message);
                        }
                    }
                    
                    var node = _threadQueue.AddLast(threadId);
                    
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
                                _val = _provider();
                                _timeToLive = DateTime.Now.Add(_timeSpan);
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
                        
                    } while (true);
                }
            }
        }
    }
}