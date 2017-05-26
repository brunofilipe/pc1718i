using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serie1
{
    public class RegionObj{
        public int permits;

        private int maxWaiting;

        private readonly LinkedList<int> queue;

        public RegionObj(int permits, int maxWaiting)
        {
            queue = new LinkedList<int>();
            this.permits = permits;
            this.maxWaiting = maxWaiting;
        }
    }
}
