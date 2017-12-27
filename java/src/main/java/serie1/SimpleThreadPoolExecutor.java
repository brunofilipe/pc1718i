package serie1;

import java.util.HashMap;
import java.util.LinkedList;
import java.util.Map;
import java.util.concurrent.RejectedExecutionException;
import java.util.concurrent.TimeUnit;
import java.util.concurrent.locks.Condition;
import java.util.concurrent.locks.Lock;
import java.util.concurrent.locks.ReentrantLock;

public class SimpleThreadPoolExecutor {
    private int maxPoolSize;
    private int keepAliveTime;

    private int currentThreads;
    private final Lock lock;
    private final Map<Long,MyThread> pool;
    private boolean shutdown;
    private LinkedList<Job> jobs ;

    private Condition shutdownCondition;
    private Condition waitingCondition;
    private Condition awaitTerminationCondition;

    public SimpleThreadPoolExecutor(int maxPoolSize, int keepAliveTime){
        this.maxPoolSize = maxPoolSize;
        this.keepAliveTime = keepAliveTime;
        lock = new ReentrantLock();
        waitingCondition = lock.newCondition();
        shutdownCondition = lock.newCondition();
        pool = new HashMap<>();
        jobs = new LinkedList<>();
    }

    public boolean execute(Runnable command, int timeout) throws InterruptedException {
        lock.lock();
        try {
            if (shutdown) {
                throw new RejectedExecutionException();
            }
            Condition condition = lock.newCondition();
            Job job = new Job(command,condition);
            jobs.addLast(job);
            if(pool.size() < maxPoolSize){
                MyThread th = new MyThread();
                pool.put(th.getId(),th);
                th.start();
            }
            else if(currentThreads < pool.size()){
                waitingCondition.signalAll();
            }
            boolean time;
            do {
                try {
                    time = condition.await(timeout,TimeUnit.NANOSECONDS);
                }catch (InterruptedException ex){
                    jobs.remove(job);
                    throw ex;
                }
                if(!time){
                    return false;
                }
                if(job.isAccepted()){
                    return true;
                }
            }while(true);
        }finally {
            if(jobs.isEmpty() && shutdown){
                shutdownCondition.signal();
            }
            lock.unlock();
        }
    }

    public void shutdown() throws InterruptedException {
        lock.lock();
        try {
            this.shutdown = true;
            while (currentThreads > 0 || jobs.size()!= 0){
                shutdownCondition.await();
            }
            awaitTerminationCondition.signal();
        }catch (InterruptedException ex){
            throw ex;
        } finally {
            lock.unlock();
        }
    }

    public boolean awaitTermination(int timeout) throws InterruptedException {
        lock.lock();
        try {
            boolean time = false;
            do{
                try {
                    time =  awaitTerminationCondition.await(timeout,TimeUnit.NANOSECONDS);
                }catch (InterruptedException ex){
                    throw ex;
                }
                if(!time){
                    return false;
                }
                else{
                    return true;
                }
            }while (true);
        }finally {
            lock.unlock();
        }
    }

    private class MyThread extends Thread{
        @Override
        public void run() {
            lock.lock();
            long nanos = TimeUnit.MILLISECONDS.toNanos(keepAliveTime);
            try {
                do{
                   Job job;
                   while ((job = jobs.removeFirst())!=null){
                       currentThreads++;
                       try {
                           lock.unlock();
                           job.getCommand().run();
                       } finally {
                           lock.lock();
                           job.setWork();
                           job.getCondition().signal();
                           currentThreads--;
                       }
                   }
                   if(shutdown || nanos<=0){
                       return;
                   }
                   try {
                       nanos = waitingCondition.awaitNanos(nanos);
                   } catch (InterruptedException e) {
                      break;
                   }
                }while (true);
            }finally {
                pool.remove(currentThread().getId());
                lock.unlock();
            }
        }
    }
}
