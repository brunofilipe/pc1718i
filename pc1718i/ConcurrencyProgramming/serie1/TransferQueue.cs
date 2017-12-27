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
                Put(msg);
                int lastTime = (timeout != Timeout.Infinite) ? Environment.TickCount : 0;
                do {
                    if (!_node.Value.IsTransferring) {
                        queue.Remove(_node);
                        return true;
                    }
                    try {
                        Monitor.Wait(_lock, timeout);
                    }catch (ThreadInterruptedException) {
                        queue.Remove(_node);
                        throw;
                    }
                    if (SyncUtils.AdjustTimeout(ref lastTime, ref timeout) == 0) {
                        queue.Remove(_node);
                        return false;
                    }
                } while (true);

            }

        }

        public bool Take(int timeout, out T msg) {
            lock (_lock) {
                try {
                    LinkedListNode<MessageHolder<T>> msgNode = queue.First;
                    if (msgNode == null) {
                        msg = default(T);
                        return false;
                    }
                    msgNode.Value.IsTransferring = false;
                    msg = msgNode.Value.Msg;
                    return true;
                }catch (ThreadInterruptedException) {
                    throw;
                }
            }
        }
    }
}