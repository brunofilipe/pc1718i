package serie1;

import java.util.concurrent.locks.Condition;

public class Job {
    private Runnable command;
    private Condition condition;
    private boolean isAccepted;

    public boolean isAccepted() {
        return isAccepted;
    }


    public Job(Runnable command, Condition condition) {
        this.command = command;
        this.condition = condition;
    }

    public void setWork(){
        isAccepted = true;
    }

    public Runnable getCommand() {
        return command;
    }

    public Condition getCondition() {
        return condition;
    }


}
