using System.Collections.Generic;

namespace Serie1 {
    public class ThrottledRegion {

        private readonly int maxInside;
        private readonly int maxWaiting;
        private readonly int waitTimeout;
        private LinkedList<int> queue;

        public ThrottledRegion(int maxInside, int maxWaiting, int waitTimeout) {
            this.maxInside = maxInside;
            this.maxWaiting = maxWaiting;
            this.waitTimeout = waitTimeout;
            queue = new LinkedList<int>();
        }

        public bool TryEnter(int key) {// throws ThreadInterruptedException
            lock (this) {
                if (waitTimeout == 0) {
                    return false;
                }
            }

            return false;
        }

        public void Leave(int key) {
            
        }
    }
}