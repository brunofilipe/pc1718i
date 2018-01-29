using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConcurrencyProgramming.serie3.APMServer {
    class Store {
        /// <summary>
        /// The singleton instance.
        /// </summary>
        private static readonly Store _instance = new Store();
        private readonly object _lock = new object();

        /// <summary>
        /// Gets the singleton instance.
        /// </summary>
        public static Store Instance {
            get { return _instance; }
        }

        /// <summary>
        /// The dictionary instance.
        /// </summary>
        private readonly Dictionary<string, string> _store;


        /// <summary>
        /// Initiates the store instance.
        /// </summary>
        private Store() {
            _store = new Dictionary<string, string>();
        }

        /// <summary>
        /// Sets the key value. 
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void Set(string key, string value) {
            lock (_lock) {
                _store[key] = value;
            }
        }

        /// <summary>
        /// Gets the key value. 
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public string Get(string key) {
            lock (_lock) {
                string value = null;
                _store.TryGetValue(key, out value);
                return value;
            }
           
        }

        /// <summary>
        /// Gets all keys. 
        /// </summary>        
        public IEnumerable<string> Keys() {
            lock (_lock) {
                return _store.Keys;
            }          
        }
    }
}
