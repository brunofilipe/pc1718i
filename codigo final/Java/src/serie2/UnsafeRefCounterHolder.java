package serie2;

import java.util.concurrent.atomic.AtomicInteger;
import java.util.concurrent.atomic.AtomicReference;

public class UnsafeRefCounterHolder<T> {

    private AtomicReference<T> value;
    private AtomicInteger refCount;

    public UnsafeRefCounterHolder(T v) {
        value = new AtomicReference<>(v);
        refCount = new AtomicInteger(1);
    }

    public void AddRef() {
        while (true) {
            int auxCount = refCount.get();
            if (auxCount == 0)
                throw new IllegalStateException();
            refCount.compareAndSet(auxCount, auxCount + 1);
        }
    }

    public void ReleaseRef() {
        while (true) {
            int auxCount = refCount.get();
            if (auxCount == 0)
                throw new IllegalStateException();

            T auxValue = value.get();

            if (refCount.compareAndSet(auxCount, auxCount - 1) && auxCount == 1) {
                AutoCloseable autoCloseable = (AutoCloseable) auxValue;
                value.set(null);
                if (autoCloseable != null)
                    try {
                        autoCloseable.close();
                    } catch (Exception e) {
                        e.printStackTrace();
                    }
            }
        }
    }

    public T Value() {
        int auxCount = refCount.get();
        if (auxCount == 0)
            throw new IllegalStateException();
        return value.get();
    }
}
