package serie1;

import org.junit.Assert;
import org.junit.Before;
import org.junit.Test;

import java.util.LinkedList;

import static org.junit.Assert.*;

public class SimpleThreadPoolExecutorTests {

    private LinkedList<Exception> exceptions;
    private LinkedList<Boolean> executionAsserts;

    @Before
    public void init(){
        exceptions = new LinkedList<>();
        executionAsserts = new LinkedList<>();
    }

    private Thread createThread(final SimpleThreadPoolExecutor simpleThreadPoolExecutor, final int aux, final int timeout) {
        return new Thread(() -> {
            try {
                boolean var2 = simpleThreadPoolExecutor.execute(() -> System.out.println("test"+aux), timeout);
                executionAsserts.add(var2);
            } catch (InterruptedException e) {
                exceptions.add(e);
            }
        });
    }

    @Test
    public void testSimpleExecution() throws Exception{
        SimpleThreadPoolExecutor simpleThreadPoolExecutor = new SimpleThreadPoolExecutor(1, 100);

        assertTrue(simpleThreadPoolExecutor.execute(() -> System.out.println("test - true"), 50));
    }

    @Test
    public void testTimeoutExecution() throws Exception {
        SimpleThreadPoolExecutor simpleThreadPoolExecutor = new SimpleThreadPoolExecutor(1, 100);

        assertFalse(simpleThreadPoolExecutor.execute(() -> System.out.println("test - false"), 0));
    }

    @Test
    public void testSimpleExecutionWithMultipleThreads() throws Exception {
        SimpleThreadPoolExecutor simpleThreadPoolExecutor = new SimpleThreadPoolExecutor(2, 1000);

        final int aux1 = 0;
        final int aux2 = 1;

        Thread thread1 = createThread(simpleThreadPoolExecutor, aux1, 500);
        Thread thread2 = createThread(simpleThreadPoolExecutor, aux2, 500);

        thread1.start();
        thread2.start();

        Thread.sleep(50);   //time to setup the worker threads before shutdown

        simpleThreadPoolExecutor.shutdown();

        executionAsserts.forEach(Assert::assertTrue);
        assertEquals(0, exceptions.size());
        assertTrue(simpleThreadPoolExecutor.awaitTermination(500));
    }

    @Test
    public void testSimpleExecutionWithInterruptedThreadsBeforeShutdown() throws Exception {
        final SimpleThreadPoolExecutor simpleThreadPoolExecutor = new SimpleThreadPoolExecutor(2, 1000);

        final int aux1 = 0;
        final int aux2 = 1;

        Thread thread1 = createThread(simpleThreadPoolExecutor, aux1, 500);
        Thread thread2 = createThread(simpleThreadPoolExecutor, aux2, 500);

        thread1.start();
        thread2.start();

        thread1.interrupt();

        Thread.sleep(50);   //time to setup the worker threads before shutdown

        simpleThreadPoolExecutor.shutdown();

        //an interrupted thread throws an exception
        assertEquals(1, exceptions.size());

        assertTrue(simpleThreadPoolExecutor.awaitTermination(500));
    }

    @Test
    public void testAwaitTerminationBeforeShutDown() throws Exception {
        final SimpleThreadPoolExecutor simpleThreadPoolExecutor = new SimpleThreadPoolExecutor(2, 1000);

        final int aux1 = 0;
        final int aux2 = 1;

        Thread thread1 = createThread(simpleThreadPoolExecutor, aux1, 500);
        Thread thread2 = createThread(simpleThreadPoolExecutor, aux2, 500);

        thread1.start();
        thread2.start();

        Thread.sleep(50);   //time to setup the worker threads before shutdown

        executionAsserts.forEach(Assert::assertTrue);

        executionAsserts.clear();

        Thread awaitTerminationThread = new Thread(() -> {
            try {
                boolean var = simpleThreadPoolExecutor.awaitTermination(500);
                executionAsserts.add(var);
            } catch (InterruptedException e) {
                exceptions.add(e);
            }
        });

        Thread shutDownThread = new Thread(simpleThreadPoolExecutor::shutdown);

        awaitTerminationThread.start();

        Thread.sleep(100);

        awaitTerminationThread.interrupt();

        shutDownThread.start();

        executionAsserts.forEach(Assert::assertFalse);
        assertEquals(0, exceptions.size());
    }
}
