using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace ConcurrencyProgramming.serie3.TAPServer {
    class Handler {
        /// <summary>
        /// Data structure that supports message processing dispatch.
        /// </summary>
        private static readonly Dictionary<string, Action<string[], StreamWriter, MessageQueue>> MESSAGE_HANDLERS;

        static Handler() {
            MESSAGE_HANDLERS = new Dictionary<string, Action<string[], StreamWriter, MessageQueue>>();
            MESSAGE_HANDLERS["SET"] = ProcessSetMessage;
            MESSAGE_HANDLERS["GET"] = ProcessGetMessage;
            MESSAGE_HANDLERS["KEYS"] = ProcessKeysMessage;
        }

        /// <summary>
        /// Handles SET messages.
        /// </summary>
        private static void ProcessSetMessage(string[] cmd, StreamWriter wr, MessageQueue queue) {
            if (cmd.Length - 1 != 2) {
                queue.TryPut("Invalid SET Message");
                wr.WriteLine("(error) wrong number of arguments (given {0}, expected 2)\n", cmd.Length - 1);
            }
            string key = cmd[1];
            string value = cmd[2];
            Store.Instance.Set(key, value);
            queue.TryPut("SET Message OK");
            wr.WriteLine("OK\n");
        }

        /// <summary>
        /// Handles GET messages.
        /// </summary>
        private static void ProcessGetMessage(string[] cmd, StreamWriter wr, MessageQueue queue) {
            if (cmd.Length - 1 != 1) {
                queue.TryPut("Invalid GET Message");
                wr.WriteLine("(error) wrong number of arguments (given {0}, expected 1)\n", cmd.Length - 1);
            }
            string value = Store.Instance.Get(cmd[1]);
            if (value != null) {
                queue.TryPut(" GET Message  WITH " + value);
                wr.WriteLine("\"{0}\"\n", value);
            } else {
                queue.TryPut(" GET Message  WITH NIL");
                wr.WriteLine("(nil)\n");
            }
        }

        /// <summary>
        /// Handles KEYS messages.
        /// </summary>
        private static void ProcessKeysMessage(string[] cmd, StreamWriter wr, MessageQueue queue) {
            if (cmd.Length - 1 != 0) {
                queue.TryPut("Invalid KEYS Message");
                wr.WriteLine("(error) wrong number of arguments (given {0}, expected 0)\n", cmd.Length - 1);
            }
            int ix = 1;
            foreach (string key in Store.Instance.Keys()) {
                wr.WriteLine("{0}) \"{1}\"", ix++, key);
                queue.TryPut(String.Format(" KEYS - {0}) \"{1}\"", ix, key));
            }
            wr.WriteLine();
        }

        /// <summary>
        /// The handler's input (from the TCP connection)
        /// </summary>
        private readonly StreamReader input;

        /// <summary>
        /// The handler's output (to the TCP connection)
        /// </summary>
        private readonly StreamWriter output;

        /// <summary>
        /// The Logger instance to be used.
        /// </summary>
        private readonly MessageQueue queue;

        /// <summary>
        ///	Initiates an instance with the given parameters.
        /// </summary>
        /// <param name="connection">The TCP connection to be used.</param>
        /// <param name="log">the Logger instance to be used.</param>
        public Handler(Stream connection, MessageQueue queue) {
            this.queue = queue;
            output = new StreamWriter(connection);
            input = new StreamReader(connection);
        }

        /// <summary>
        /// Performs request servicing.
        /// </summary>
        /// 
        public async Task Run() {
            try {
                string request;
                while ((request = await input.ReadLineAsync()) != null && request != string.Empty) {
                    string[] cmd = request.Trim().Split(' ');
                    Action<string[], StreamWriter, MessageQueue> handler = null;
                    if (cmd.Length < 1 || !MESSAGE_HANDLERS.TryGetValue(cmd[0], out handler)) {
                        queue.TryPut("(error) unnown message type");
                        return;
                    }
                    // Dispatch request processing
                    handler(cmd, output, queue);
                    output.Flush();
                }
            } catch (IOException ioe) {
                // Connection closed by the client. Log it!
                queue.TryPut(String.Format("Handler - Connection closed by client {0}", ioe));
            } finally {
                input.Close();
                output.Close();
            }
        }
    }
}
