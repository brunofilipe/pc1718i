package serie2;

import org.junit.Assert;
import org.junit.Before;
import org.junit.Test;

import java.util.LinkedList;
import java.util.Objects;

public class UnsafeRefCounterHolderTests {

    private LinkedList<Exception> exceptions;
    private LinkedList<String> values;

    private void AddExceptionToList(Runnable runnable) {
        try {
            runnable.run();
        } catch (Exception e) {
            exceptions.add(e);
        }
    }

    @Before
    public void init(){
        exceptions = new LinkedList<>();
        values = new LinkedList<>();
    }

    @Test
    public void testSimpleRun() throws Exception{
        AuxiliarObject auxiliarObject = new AuxiliarObject("test");
        UnsafeRefCounterHolder<AuxiliarObject> unsafeRefCounterHolder = new UnsafeRefCounterHolder<>(auxiliarObject);

        Thread thread1 = new Thread(() -> AddExceptionToList(unsafeRefCounterHolder::AddRef));
        Thread thread2 = new Thread(() -> AddExceptionToList(unsafeRefCounterHolder::AddRef));
        Thread thread3 = new Thread(() -> AddExceptionToList(unsafeRefCounterHolder::ReleaseRef));
        Thread thread4 = new Thread(() -> AddExceptionToList(unsafeRefCounterHolder::ReleaseRef));
        Thread thread5 = new Thread(() -> {
            try {
                AuxiliarObject aux = unsafeRefCounterHolder.Value();
                values.add(aux.getValue());
            } catch (Exception e) {
                exceptions.add(e);
            }
        });

        thread1.start();
        thread2.start();
        thread3.start();
        thread4.start();
        thread5.start();

        thread1.join();
        thread2.join();
        thread3.join();
        thread4.join();
        thread5.join();

        Assert.assertTrue(values.stream().allMatch(Objects::nonNull));
        Assert.assertEquals(0, exceptions.size());
    }
}
