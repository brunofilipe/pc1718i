package serie2;

import java.util.concurrent.atomic.AtomicReference;

public class ConcurrentQueue<T> {
    private static class Node<E>{
        final E item;
        final AtomicReference<Node<E>> next;
        public Node(E item, Node<E> next){
            this.item = item;
            this.next = new AtomicReference<>(next);
        }
    }
    private final AtomicReference<Node<T>> head;
    private final AtomicReference<Node<T>> tail;

    public ConcurrentQueue(){
        Node<T> dummy = new Node<T>(null,null);
        this.head = new AtomicReference<>(dummy);
        this.tail = new AtomicReference<>(dummy);
    }

    public void put(T item){
        Node<T> newNode = new Node<T>(item, null);
        while (true) {
            Node<T> currTail = tail.get();
            Node<T> tailNext = currTail.next.get();
            if (tail.get()==currTail) { // Are tail and next consistent?
                if (tailNext != null) {
                    // queue in intermediate state, advance state
                    tail.compareAndSet(currTail, tailNext);
                } else {
                    if (currTail.next.compareAndSet(null, newNode)) {
                        // insertion succeeded, try advancing tail
                        tail.compareAndSet(currTail, newNode);
                        return;
                    }
                }
            }
        }
    }

    public T tryTake() {
        while(true) {
            Node<T> headCur = head.get();
            Node<T> tailCur = tail.get();
            Node<T> headNext = headCur.next.get();

            if(headCur == head.get()) {
                if(headCur == tailCur) {
                    if(headNext == null)
                        return null;

                    tail.compareAndSet(tailCur, headNext);
                } else   {
                    T val = headNext.item;
                    if(head.compareAndSet(headCur, headNext))
                        return val;
                }
            }
        }
    }

    // take a datum - spinning if necessary
    public T take() throws InterruptedException {
        T v;
        while ((v = tryTake()) == null) {
            Thread.sleep(0);
        }
        return v;
    }

    public boolean isEmpty() {
        return tail.get() == head.get();
    }

}
