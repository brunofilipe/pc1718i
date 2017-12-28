public class Person implements AutoCloseable {
    private String nationality;
    private int id;

    public Person(String nationality, int id) {
        this.nationality = nationality;
        this.id = id;
    }

    @Override
    public void close() throws Exception {
        System.out.println("Closing....");
    }
}
