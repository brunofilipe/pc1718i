import org.junit.Assert;
import org.junit.Test;
import serie2.UnsafeRefCountedHolder;

import java.io.IOException;
import java.util.LinkedList;

public class UnsafeRefCountedHolderTests {
    @Test
    public void TestMultipleAccess() throws InterruptedException, IOException {
        Person person = new Person("PT",111);
        UnsafeRefCountedHolder<Person> counter = new UnsafeRefCountedHolder<>(person);
        LinkedList<Exception> list = new LinkedList<>();
        LinkedList<Person>values = new LinkedList<>();
        Thread th1 = new Thread(()->{
            try {
                counter.AddRef();
            }catch (IllegalStateException ex){
                list.addLast(ex);
            }
        });
        Thread th2 = new Thread(()->{
            try {
                counter.ReleaseRef();
            }catch (IllegalStateException ex){
                list.addLast(ex);
            }
        });
        Thread th3 = new Thread(()->{
            try {
                Person p1 = counter.getValue();
                values.addLast(p1);
            }catch (IllegalStateException ex){
                list.addLast(ex);
            }
        });
        Thread th4 = new Thread(()->{
            try {
                counter.AddRef();
            }catch (IllegalStateException ex){
                list.addLast(ex);
            }
        });
        th1.start();
        th2.start();
        th3.start();
        th4.start();
        th1.join();
        th2.join();
        th3.join();
        th4.join();
        Assert.assertEquals(0,list.size());
        Assert.assertEquals(person,values.getFirst());
    }

    @Test
    public void testFailureCounted() throws InterruptedException {
        Person person = new Person("PT",111);
        UnsafeRefCountedHolder<Person> counter = new UnsafeRefCountedHolder<>(person);
        LinkedList<Exception> list = new LinkedList<>();
        LinkedList<Person>values = new LinkedList<>();
        Thread th1 = new Thread(()->{
            try {
                counter.ReleaseRef();
            }catch (IllegalStateException ex){
                list.addLast(ex);
            }
        });
        Thread th2 = new Thread(()->{
            try {
                counter.getValue();
            }catch (IllegalStateException ex){
                list.addLast(ex);
            }
        });
        Thread th3 = new Thread(()->{
            try {
                counter.AddRef();
            }catch (IllegalStateException ex){
                list.addLast(ex);
            }
        });
        Thread th4 = new Thread(()->{
            try {
                counter.AddRef();
            }catch (IllegalStateException ex){
                list.addLast(ex);
            }
        });
        th1.start();
        th2.start();
        th3.start();
        th4.start();
        th1.join();
        th2.join();
        th3.join();
        th4.join();
        Assert.assertEquals(2,list.size());

    }
}
