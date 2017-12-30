package serie2;

public class AuxiliarObject implements AutoCloseable {

    private String value;

    public AuxiliarObject(String value)
    {
        this.value = value;
    }

    public String getValue() {
        return value;
    }

    @Override
    public void close() throws Exception {
        System.out.println("Disposing "+value+"...");
    }
}
