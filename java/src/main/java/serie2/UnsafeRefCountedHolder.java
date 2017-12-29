package serie2;

import java.util.concurrent.atomic.AtomicInteger;
import java.util.concurrent.atomic.AtomicReference;

public class UnsafeRefCountedHolder <T> {
    private AtomicReference<T> value;
    private AtomicInteger refCount;

    public UnsafeRefCountedHolder(T v) {
        value = new AtomicReference<>(v);
        refCount = new AtomicInteger(1);
    }

    public void AddRef() {
            int currRefCount = refCount.get();
            if (currRefCount == 0)
                throw new IllegalStateException();
            refCount.compareAndSet(currRefCount, currRefCount + 1);
    }

    public void ReleaseRef() {
        T currValue = value.get();
        int currRefCount = refCount.get();
        if (currRefCount == 0)
            throw new IllegalStateException();

        if (refCount.compareAndSet(currRefCount, currRefCount - 1)) {
            if (currRefCount == 1) {
                AutoCloseable disposable = (AutoCloseable) currValue;
                value.set(null);
                if (disposable != null)
                    try {
                        disposable.close();
                    } catch (Exception e) {
                        e.printStackTrace();
                    }
            }
        }
    }

    public T getValue() {
        int currRefCount = refCount.get();
        if (currRefCount == 0)
            throw new IllegalStateException();
        return value.get();
    }
}
