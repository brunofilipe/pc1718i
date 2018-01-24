using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace ConcurrencyProgramming.serie3.TAPServer {
    class Listener {
        /// <summary>
        /// TCP port number in use.
        /// </summary>
        private readonly int portNumber;
        private Thread logger;
        private const int MAX_CONNECTIONS = 8;
        private int connectionsCounter = 0;
        private static ThreadLocal<int> counterCallbacks = new ThreadLocal<int>();
        private int conectionCounter;
        private MessageQueue logQueue = new MessageQueue(5);
        /// <summary> Initiates a tracking server instance.</summary>
        /// <param name="_portNumber"> The TCP port number to be used.</param>
        public Listener(int _portNumber) { portNumber = _portNumber; }


        /// <summary>
        ///	Server's main loop implementation.
        /// </summary>
        /// <param name="log"> The Logger instance to be used.</param>
        /// 
        public void Run(Logger log) {
            logger = new Thread(() => {
                do {
                    log.LogMessage(logQueue.Read());
                } while (true);
            }) {
                Priority = ThreadPriority.Lowest
            };
            logger.Start();
            TcpListener srv = null;
            try {
                srv = new TcpListener(IPAddress.Loopback, portNumber);
                srv.Start();
                Task.WaitAll(ListeningWorkAsync(srv));
                Console.ReadKey();
            } finally {
                logQueue.TryPut("Listener - Ending.");
                srv.Stop();
            }
        }

        private async Task ListeningWorkAsync(TcpListener srv) {
            logQueue.TryPut("Listener - Waiting for connection requests.");
            TcpClient connection = null;
            try {
                connection = await srv.AcceptTcpClientAsync();
                if(Interlocked.Increment(ref conectionCounter) < MAX_CONNECTIONS) {
                     Task.Factory.StartNew(() => ListeningWorkAsync(srv));
                }
                logQueue.TryPut(String.Format("activeConnections connections {0}. Cannot be more than MAX\n", conectionCounter));
                ProcessConnectionAsync(connection);
                if (Interlocked.Decrement(ref conectionCounter) == MAX_CONNECTIONS - 1)
                    await Task.Factory.StartNew(() => ListeningWorkAsync(srv));

            } catch (SocketException se) {
            Console.WriteLine("****error: " + se.Message);
            }
        }
        private async Task ProcessConnectionAsync(TcpClient socket) {
            Stream stream = null;
            try {
                stream = socket.GetStream();
                socket.LingerState = new LingerOption(true, 10);
                logQueue.TryPut(String.Format("Listener - Connection established with {0}.", socket.Client.RemoteEndPoint));
                // Instantiating protocol handler and associate it to the current TCP connection
                Handler protocolHandler = new Handler(socket.GetStream(), logQueue);
                // Synchronously process requests made through de current TCP connection
                await protocolHandler.Run();
            } catch (Exception ex) {
                logQueue.TryPut(string.Format("Error : {0} !!!", ex.Message));
            } finally {
                if (stream != null)
                    stream.Close();
                socket.Close();
            }
        }

    }
}
