using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConcurrencyProgramming.serie1 {
    public class Pairing<T, U> {
        private readonly LinkedList<T> listOfTValues;
        private readonly LinkedList<U> listOfUValues;
        private readonly object _lock = new object();

        public Pairing() {
            listOfTValues = new LinkedList<T>();
            listOfUValues = new LinkedList<U>();
        }


        public Tuple<T, U> Provide(T value, int timeout) {
            lock (_lock) {
                if (timeout == 0) throw new TimeoutException();
                TimeoutHolder th = new TimeoutHolder(timeout);
                do {
                    if ((timeout = th.Value) == 0) throw new TimeoutException();
                    LinkedListNode<T> node = listOfTValues.AddLast(value);
                    LinkedListNode<U> nodeU = listOfUValues.First;
                    if (nodeU != null) {
                        listOfTValues.Remove(node);
                        listOfUValues.Remove(nodeU);
                        return new Tuple<T, U>(value, nodeU.Value);
                    }
                    try {
                        Monitor.Wait(_lock, timeout);
                    }
                    catch (ThreadInterruptedException) {
                        Monitor.Pulse(_lock);
                        throw;
                    }
                } while (true);
            }
        }

        public Tuple<T, U> Provide(U value, int timeout) {
            lock (_lock)
            {
                if (timeout == 0) throw new TimeoutException();
                TimeoutHolder th = new TimeoutHolder(timeout);
                do
                {
                    if ((timeout = th.Value) == 0) throw new TimeoutException();
                    LinkedListNode<U> node = listOfUValues.AddLast(value);
                    LinkedListNode<T> nodeT = listOfTValues.First;
                    if (nodeT != null)
                    {
                        listOfUValues.Remove(node);
                        listOfTValues.Remove(nodeT);
                        return new Tuple<T, U>(nodeT.Value,value);
                    }
                    try
                    {
                        Monitor.Wait(_lock, timeout);
                    }
                    catch (ThreadInterruptedException)
                    {
                        Monitor.Pulse(_lock);
                        throw;
                    }
                } while (true);
            }
        }
    }
}
