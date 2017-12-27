using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConcurrencyProgramming.serie1
{

    public struct TimeoutHolder{
        private int _timeout;
        private int _refTime;

        public TimeoutHolder(int timeout){
            this._timeout = timeout;
            this._refTime = timeout != Timeout.Infinite ? Environment.TickCount : 0;
        }

        // returns the remaining timeout
        public int Value{
            get
            {
                if (_timeout != Timeout.Infinite && _timeout != 0)
                {
                    int now = Environment.TickCount;
                    if (now != _refTime)
                    {
                        int elapsed = now - _refTime;
                        _refTime = now;
                        _timeout = elapsed < _timeout ? _timeout - elapsed : 0;
                    }
                }
                return _timeout;
            }
        }
    }

}
