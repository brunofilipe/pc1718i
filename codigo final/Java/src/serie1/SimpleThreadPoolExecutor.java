package serie1;

import java.util.concurrent.RejectedExecutionException;
import java.util.concurrent.TimeUnit;
import java.util.concurrent.locks.Condition;
import java.util.concurrent.locks.ReentrantLock;

public class SimpleThreadPoolExecutor {

    private final ReentrantLock lock = new ReentrantLock();
    private final ThreadPool threadPool;
    private final Condition awaitTerminationCondition = lock.newCondition();
    private boolean isShutdown = false, alreadyFinished = false;

    public SimpleThreadPoolExecutor(int maxPoolSize, int keepAliveTime){
        threadPool = new ThreadPool(maxPoolSize, keepAliveTime, lock);
    }

    public class Command {
        private Runnable command;
        private boolean receivedWork;
        private Condition condition;

        public Command(Runnable command, boolean receivedWork, Condition condition){
            this.command = command;
            this.receivedWork = receivedWork;
            this.condition = condition;
        }

        public Runnable getCommand() {
            return command;
        }

        public boolean isReceivedWork() {
            return receivedWork;
        }

        public Condition getCondition() {
            return condition;
        }

        public void setReceivedWork(boolean receivedWork) {
            this.receivedWork = receivedWork;
        }
    }

    public boolean execute(Runnable command, int timeout) throws InterruptedException{
        lock.lock();
        try {
            if(timeout <= 0) return false;

            if(isShutdown) throw new RejectedExecutionException();

            Condition commandCondition = lock.newCondition();

            Command cmd = new Command(command, false, commandCondition);
            threadPool.executeCommand(cmd);

            //converting milliseconds to nanoseconds, so awaitNanos method can be used
            long nanosTimeout = TimeUnit.MILLISECONDS.toNanos(timeout);

            do {
                try {
                    nanosTimeout = commandCondition.awaitNanos(nanosTimeout);
                } catch (InterruptedException e) {
                    throw e;
                }

                if(nanosTimeout <= 0 && !cmd.isReceivedWork()) return false;

                if(cmd.isReceivedWork()) return true;

            } while (true);

        } finally {
            lock.unlock();
        }
    }

    public void shutdown(){
        lock.lock();
        try {
            isShutdown = true;

            threadPool.shuttingDown();

            if(threadPool.hasWork()) {
                do {
                    try {
                        threadPool.getShuttingDownModeCondition().await();
                    } catch (InterruptedException e) {
                        //nothing to do here, can't throw exception
                    }
                    if(!threadPool.hasWork()) break;
                } while (true);
            }

            //notifies that shutdown of the executor has been finished
            alreadyFinished = true;
            awaitTerminationCondition.signal();
        } finally {
            lock.unlock();
        }
    }

    public boolean awaitTermination(int timeout) throws InterruptedException{
        lock.lock();
        try {
            if(alreadyFinished) return true;

            long nanosTimeout = TimeUnit.MILLISECONDS.toNanos(timeout);

            do {
                try {
                    nanosTimeout = awaitTerminationCondition.awaitNanos(nanosTimeout);
                } catch (InterruptedException e) {
                    return false;
                }

                if(nanosTimeout <= 0 && !alreadyFinished) return false;

                if(alreadyFinished) return true;
            } while (true);

        } finally {
            lock.unlock();
        }
    }
}
