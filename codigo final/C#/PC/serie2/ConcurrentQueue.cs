using System.Threading;

namespace serie2
{
    public class ConcurrentQueue<T>
    { 
        private class Node<T> {
            public readonly T item;
            public volatile Node<T> next;

            public Node(T item, Node<T> next) {
                this.item = item;
                this.next = next;
            }
        }

        private readonly Node<T> dummy;
        private volatile Node<T> head;
        private volatile Node<T> tail;

        public ConcurrentQueue() {
            dummy = new Node<T>(default(T), null);
            head = dummy;
            tail = dummy;
        }
        
        public void Put(T item) {
            var newNode = new Node<T>(item, null);

            while (true) {
                var currentTail = tail;
                var nextTail = currentTail.next;

                if (currentTail == tail)
                    if (nextTail != null)
                        Interlocked.CompareExchange(ref tail, nextTail, currentTail);
                    else
                        if (Interlocked.CompareExchange(ref currentTail.next, newNode, null) == null) {
                            Interlocked.CompareExchange(ref tail, newNode, currentTail);
                            return;
                        }
            }
        }

        private T TryTake() {
            while (true) {
                var currentHead = head;
                var currentTail = tail;
                var nextHead = currentHead.next;

                if (currentHead == head) {
                    if (currentHead == currentTail) {
                        if (nextHead == null)
                            return default(T);

                        Interlocked.CompareExchange(ref tail, nextHead, currentTail);
                    } else {
                        var value = nextHead.item;

                        if (Interlocked.CompareExchange(ref head, nextHead, currentHead) == currentHead)
                            return value;
                    }
                }
            }
        }

        public T Take() {
            T value;
            while ((value = TryTake()) == null)
                Thread.Sleep(0);
            return value;
        }

        public bool IsEmpty() {
            return head == tail;
        }
    }
}