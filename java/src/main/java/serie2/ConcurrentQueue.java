package serie2;

import java.util.concurrent.atomic.AtomicReference;

public class ConcurrentQueue<E> {
    private static class Node<T>{
        final T item;
        final AtomicReference<Node<T>> next;
        public Node(T item, Node<T> next){
            this.item = item;
            this.next = new AtomicReference<Node<T>>(next);
        }
    }
    private final AtomicReference<Node<E>> head;
    private final AtomicReference<Node<E>> tail;

    public ConcurrentQueue(){
        Node<E> dummy = new Node<E>(null,null);
        this.head = new AtomicReference<>(dummy);
        this.tail = new AtomicReference<>(dummy);
    }

    public void enqueue(E item){
        Node<E> newNode = new Node<E>(item, null);
        while (true) {
            Node<E> currTail = tail.get();
            Node<E> tailNext = currTail.next.get();
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

    public E tryDequeue() {
        while(true) {
            Node<E> headCur = head.get();
            Node<E> tailCur = tail.get();
            Node<E> headNext = headCur.next.get();

            if(headCur == head.get()) {
                if(headCur == tailCur) {
                    if(headNext == null)
                        return null;

                    tail.compareAndSet(tailCur, headNext);
                } else   {
                    E val = headNext.item;
                    if(head.compareAndSet(headCur, headNext))
                        return val;
                }
            }
        }
    }

    // dequeue a datum - spinning if necessary
    public E dequeue() throws InterruptedException {
        E v;
        while ((v = tryDequeue()) == null) {
            Thread.sleep(0);
        }
        return v;
    }

    public boolean isEmpty() {
        return tail.get() == head.get();
    }

}
