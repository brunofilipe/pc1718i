package serie2;

import java.util.concurrent.atomic.AtomicReference;

public class QNode<E> {
    final E data;
    final NodeType type;
    final AtomicReference<QNode<E>> next;
    final AtomicReference<QNode<E>> request;

    public QNode(E data, NodeType type) {
        this.data = data;
        this.type = type;
        this.next = new AtomicReference<>(null);
        this.request = new AtomicReference<>(null);
    }
}
