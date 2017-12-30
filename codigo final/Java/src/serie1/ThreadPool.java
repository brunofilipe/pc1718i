package serie1;

import java.util.LinkedList;
import java.util.concurrent.TimeUnit;
import java.util.concurrent.locks.Condition;
import java.util.concurrent.locks.ReentrantLock;

public class ThreadPool {

    private final int maxPoolSize, keepAliveTime;
    private final ReentrantLock lock;
    private final LinkedList<Long> threadsList = new LinkedList<>();
    private final LinkedList<SimpleThreadPoolExecutor.Command> commandsToExecute = new LinkedList<>();
    private final Condition threadsWithoutWorkCondition;
    private Condition shuttingDownModeCondition;

    private boolean shuttingDownMode = false;

    private int threadsWithoutWork = 0;

    public ThreadPool(int maxPoolSize, int keepAliveTime, ReentrantLock lock) {
        this.maxPoolSize = maxPoolSize;
        this.keepAliveTime = keepAliveTime;
        this.lock = lock;
        threadsWithoutWorkCondition = lock.newCondition();
    }

    public Condition getShuttingDownModeCondition() {
        return shuttingDownModeCondition;
    }

    private class ConsumerThread {

        public void runConsumerThread(){
            lock.lock();
            try {
                boolean isConsumerThreadLoopOver;

                //loop of the thread, to execute the provided work
                do {

                    consumerThreadDoingSomeWork();

                    if(notifyAboutShuttingDownMode()) break;

                    isConsumerThreadLoopOver = waitingForWork(keepAliveTime);
                } while (!isConsumerThreadLoopOver);

                //remove the current thread from the pool
                threadsList.remove(Thread.currentThread().getId());

                notifyAboutShuttingDownMode();

            } finally {
                lock.unlock();
            }
        }

        private void consumerThreadDoingSomeWork() {
            //is there work to be done?
            while (commandsToExecute.size() > 0) {

                SimpleThreadPoolExecutor.Command command = commandsToExecute.removeFirst();

                //worker thread received work to do!
                command.setReceivedWork(true);

                //the command is executed outside of the lock, so other threads can also do some work in another commands of the list
                lock.unlock();

                command.getCommand().run();

                lock.lock();

                //notify that this command has been executed
                command.getCondition().signal();
            }
        }

        private boolean waitingForWork(long keepAliveTimeMillis){

            boolean isConsumerThreadLoopOver = false;

            //converting milliseconds to nanoseconds, so awaitNanos method can be used
            long waitTimeInNanoSeconds = TimeUnit.MILLISECONDS.toNanos(keepAliveTimeMillis);

            //waiting for work or timeout
            do {
                ++threadsWithoutWork;

                try {
                    waitTimeInNanoSeconds = threadsWithoutWorkCondition.awaitNanos(waitTimeInNanoSeconds);
                } catch (InterruptedException e) {
                    //if the thread's life time has ended, the worker thread should terminate
                    Thread.currentThread().interrupt();
                } finally {
                    //if the thread was interrupted or signaled
                    --threadsWithoutWork;
                }

                //check if there is work to be done, until is time left
                if(commandsToExecute.size() > 0) break;

                //with no time left, this thread must be terminated, so breaks out of the loop
                if(waitTimeInNanoSeconds <= 0) {
                    isConsumerThreadLoopOver = true;
                    break;
                }

                if(notifyAboutShuttingDownMode()) break;
            }while (true);

            return isConsumerThreadLoopOver;
        }
    }

    public void executeCommand(SimpleThreadPoolExecutor.Command command) {
        commandsToExecute.add(command);

        //signal the next thread waiting for work
        if(threadsWithoutWork > 0)
            threadsWithoutWorkCondition.signal();

        //if maxPoolSize is achieved, then another new worker thread is created
        else if(threadsList.size() < maxPoolSize) {
            ConsumerThread consumerThread = new ConsumerThread();
            Thread thread = new Thread(consumerThread::runConsumerThread);
            threadsList.add(thread.getId());
            thread.start();
        }
    }

    public void shuttingDown(){
        shuttingDownMode = true;

        shuttingDownModeCondition = lock.newCondition();

        //notify all threads about shutting down mode
        if(threadsWithoutWork > 0) threadsWithoutWorkCondition.signalAll();
    }

    public boolean hasWork(){
        return (threadsList.size() == 0 && commandsToExecute.size() == 0);
    }

    private boolean notifyAboutShuttingDownMode() {  //notify state about being ready to shutdown
        boolean toReturn = shuttingDownMode && threadsList.size() == 0;
        if(toReturn) shuttingDownModeCondition.signal();
        return toReturn;
    }
}
