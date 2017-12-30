using System;
using System.Threading;

namespace Serie1
{
    public struct TimeoutHolder {
        private int _timeout;
        private int _refTime;
	
        public TimeoutHolder(int timeout) {
            _timeout = timeout;
            _refTime = timeout != Timeout.Infinite ? Environment.TickCount : 0;
        }
	
        public int Value {
            get {
                if (_timeout != Timeout.Infinite && _timeout != 0) {
                    var now = Environment.TickCount;
                    if (now != _refTime) {
                        var elapsed = now - _refTime;
                        _refTime = now;
                        _timeout = elapsed < _timeout ? _timeout - elapsed : 0;
                    }
                }
                return _timeout;
            }
        }
    }

}