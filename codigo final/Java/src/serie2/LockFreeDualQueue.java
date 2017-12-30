package serie2;

import java.util.concurrent.atomic.AtomicReference;

public class LockFreeDualQueue<T> {
    private final AtomicReference<QNode<T>> head;
    private final AtomicReference<QNode<T>> tail;

    public LockFreeDualQueue(){
        QNode<T> dummy = new QNode<T>(null, NodeType.DATUM);
        head = new AtomicReference<>(dummy);
        tail = new AtomicReference<>(dummy);
    }

    public void enqueue(T item){
        QNode<T>h,t,req,tailNext,headNext;
        QNode<T>n = new QNode<T>(item,NodeType.DATUM);
        while (true){
            h = head.get();
            t = tail.get();
            if((t==h) || t.type != NodeType.REQUEST){
                tailNext = t.next.get();
                if(t == tail.get()){
                    if(tailNext != null){
                        tail.compareAndSet(t,tailNext);
                    }
                    else{
                        if(t.next.compareAndSet(null,n)){
                            tail.compareAndSet(t,n);
                            return;
                        }

                    }
                }
            } else {
                headNext = h.next.get();
                if(t == tail.get()){
                    req = h.request.get();
                    if(h == head.get()){
                        boolean success = (req == null && h.request.compareAndSet(null,n));
                        head.compareAndSet(h,headNext);
                        if(success){
                            return;
                        }
                    }
                }
            }
        }
    }

    public T dequeue() throws InterruptedException {
        QNode<T>h,t,tailNext,headNext;
        QNode<T>n = new QNode<>(null,NodeType.REQUEST);
        while (true){
            h = head.get();
            t = tail.get();
            if ((t==h || t.type ==NodeType.REQUEST)){
                tailNext = t.next.get();
                if (t == tail.get()){
                    if(tailNext!=null){
                        tail.compareAndSet(t,tailNext);
                    }else{
                        if(t.next.compareAndSet(null,n)){
                            tail.compareAndSet(t,n);
                            if(h == head.get() && h.request.get() != null){
                                head.compareAndSet(h,h.next.get());
                            }
                            while (t.request.get() == null){
                                Thread.sleep(0);
                            }
                            h = head.get();
                            if(h == t){
                                head.compareAndSet(h,n);
                            }
                            return t.request.get().data;
                        }
                    }
                }
                else{
                    headNext = h.next.get();
                    if(t == tail.get()){
                        T data = headNext.data;
                        if(head.compareAndSet(h,headNext)){
                            return data;
                        }
                    }
                }
            }
        }
    }

    public boolean isEmpty(){
        QNode<T> t, tailNext;
        while (true){
            t = tail.get();
            if(head.get() == t){
                tailNext = t.next.get();
                if(tailNext  == null && t == tail.get()){
                    return true;
                }
                tail.compareAndSet(t,tailNext);
            } else{
                t = tail.get();
                if(t == tail.get() && t.type == NodeType.REQUEST){
                    return true;
                }
            }
        }
    }

}
