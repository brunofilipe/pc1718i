using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using SerieUm;

namespace ConcurrencyProgramming.serie1 {
    public class TransferQueue<T> {

        private LinkedList<MessageHolder<T>> queue = new LinkedList<MessageHolder<T>>();

        private LinkedListNode<MessageHolder<T>> _node;

        private readonly object _lock = new object();

        public void Put(T msg) {
            lock (_lock) {
                MessageHolder<T> message = new MessageHolder<T>(msg);
                _node = queue.AddLast(message);
            }
        }

        public bool Transfer(T msg, int timeout) {
            lock (_lock) {
				if (timeout == 0) return false;
                Put(msg);
                int lastTime = (timeout != Timeout.Infinite) ? Environment.TickCount : 0;
                do {
                    if (!_node.Value.IsTransferring) {
                        queue.Remove(_node);
                        return true;
                    }
					if (SyncUtils.AdjustTimeout(ref lastTime, ref timeout) == 0) {
						queue.Remove(_node);
						return false;
					}
                    try {
                        Monitor.Wait(_lock, timeout);
                    }
                    catch (ThreadInterruptedException) {
                        queue.Remove(_node);
                        Monitor.PulseAll(_lock);
                        throw;
                    }

                } while (true);

            }

        }

        public bool Take(int timeout, out T msg) {
            lock (_lock) {
                if(timeout == 0 || queue.Count == 0){
                    msg = default(T);
                    Monitor.PulseAll(_lock);
                    return false;
                }
                TimeoutHolder th = new TimeoutHolder(timeout);
                do {
                        LinkedListNode<MessageHolder<T>> msgNode = queue.First;
                        if (msgNode == null || (timeout = th.Value) == 0){
                            msg = default(T);
                            Monitor.PulseAll(_lock);
                            return false;
                        }
                        msgNode.Value.IsTransferring = false;
                        msg = msgNode.Value.Msg;
                        if (msg != null){
                            Monitor.PulseAll(_lock);
                            return true;
                        }
                        try {
                            Monitor.Wait(_lock, timeout);
                        }
                        catch (ThreadInterruptedException){
                            Monitor.PulseAll(_lock);
                            throw;
                        }
                } while (true);
             
            }
        }
    }
}