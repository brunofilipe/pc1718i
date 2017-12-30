package serie2;

import java.util.concurrent.atomic.AtomicReference;

public class ConcurrentQueue<E> {
    private static class Node<T> {
        final T item;
        final AtomicReference<Node<T>> next;
        public Node(T item, Node<T> next) {
            this.item = item;
            this.next = new AtomicReference<>(next);
        }
    }

    private final AtomicReference<Node<E>> head;
    private final AtomicReference<Node<E>> tail;

    public ConcurrentQueue() {
        Node<E> dummy = new Node<>(null, null);
        head = new AtomicReference<>(dummy);
        tail = new AtomicReference<>(dummy);
    }

    public void put(E item) {
        Node<E> newNode = new Node<>(item, null);
        while (true) {
            Node<E> currentTail = tail.get();
            Node<E> nextTail = currentTail.next.get();
            if(tail.get() == currentTail) {
                if(nextTail != null)
                    tail.compareAndSet(currentTail, nextTail);
                else {
                    if(currentTail.next.compareAndSet(null, newNode)) {
                        tail.compareAndSet(currentTail, newNode);
                        return;
                    }
                }
            }
        }
    }

    public E tryTake() {
        while (true) {
            Node<E> currentHead = head.get();
            Node<E> currentTail = tail.get();
            Node<E> nextHead = currentHead.next.get();

            if(currentHead == head.get()) {
                if(currentHead == currentTail) {
                    if(nextHead == null)
                        return null;

                    tail.compareAndSet(currentTail, nextHead);
                } else   {
                    E value = nextHead.item;
                    if(head.compareAndSet(currentHead, nextHead))
                        return value;
                }
            }
        }
    }

    public E take() throws InterruptedException {
        E value;
        while ((value = tryTake()) == null)
            Thread.sleep(0);
        return value;
    }

    public boolean isEmpty() {
        return tail.get() == head.get();
    }
}
