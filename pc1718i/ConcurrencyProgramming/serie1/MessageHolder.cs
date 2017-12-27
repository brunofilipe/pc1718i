namespace ConcurrencyProgramming.serie1 {
    public class MessageHolder<T> {
        public readonly T Msg;
        public bool IsTransferring { get; set; }

        public MessageHolder(T msg) {
            this.Msg = msg;
            this.IsTransferring = true;
        }
    }
}