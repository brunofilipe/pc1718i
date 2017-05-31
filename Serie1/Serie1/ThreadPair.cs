using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Serie1
{
    public class ThreadPair<T> where T : class {

        private T first;
        private T second;
        private bool exchanging;
        private bool completed;

        public bool Completed {
            get { return completed; }
            set { completed = value; }
        }

        public ThreadPair(T first) {
            this.first = first;
            exchanging = false;
            completed = false;
        }

        public T First {
            get { return first; }
            set { first = value; }
        }

        public T Second {
            get { return second; }
            set { second = value; }
        }

        public bool Exchanging {
            get { return exchanging;}
            set { }
        }


        
    }
}
