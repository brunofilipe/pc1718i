using ConcurrencyProgramming.serie3.APMServer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Threading.Tasks;

namespace ConcurrencyProgramming.serie3.APMServer {
    class Listener {
        /// <summary>
        /// TCP port number in use.
        /// </summary>
        private readonly int portNumber;
        private Thread logger;
        private const int MAX_CONNECTIONS = 8;
        private int connectionsCounter = 0;
        //each thread should have a max callback
        private const int MAX_CALLBACKS = 2;
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
                ListeningWork(srv);
            } finally {
                logQueue.TryPut("Listener - Ending.");
                srv.Stop();
            }
        }

        private void ListeningWork(TcpListener srv) {
            logQueue.TryPut("Listener - Waiting for connection requests.");
            AsyncCallback onProccessConnection = null;
            AsyncCallback onAcceptConnection = (ar) => {
                if (!ar.CompletedSynchronously) 
                    onProccessConnection(ar);
                else {
                    if (counterCallbacks.Value >= MAX_CALLBACKS)
                        ThreadPool.QueueUserWorkItem((_ar) => onProccessConnection((IAsyncResult)_ar), ar);
                    else {
                        counterCallbacks.Value++;
                        onProccessConnection(ar);
                        counterCallbacks.Value--; // Caso nao tenha executado recursivamente o callback (lançar a operação assincrono e de seguida executa-la)
                                                              // decerementa.
                    }
                }
            };

            // Recebe uma conexao e realiza e executa o processamento
            onProccessConnection = (ar) => {
                TcpClient connection = null;
                try {
                    connection = srv.EndAcceptTcpClient(ar);

                    int currentActiveConnection = Interlocked.Increment(ref conectionCounter);
                    if (currentActiveConnection < MAX_CONNECTIONS) {
                        //still can do work!!!
                        srv.BeginAcceptTcpClient(onAcceptConnection, null);
                    }
                    logQueue.TryPut(String.Format("activeConnections connections {0}. Cannot be more than MAX\n", conectionCounter));
                    ProcessConnection(connection);
                    currentActiveConnection = Interlocked.Decrement(ref conectionCounter);
                    if (currentActiveConnection == MAX_CONNECTIONS - 1)
                        srv.BeginAcceptTcpClient(onAcceptConnection, null);

                } catch (SocketException se) {
                    Console.WriteLine("****error: " + se.Message);
                } 

            };
            srv.BeginAcceptTcpClient(onAcceptConnection, null);
        }
        private void ProcessConnection(TcpClient socket) {
            Stream stream = null;
            try {
                stream = socket.GetStream();
                socket.LingerState = new LingerOption(true, 10);
                logQueue.TryPut(String.Format("Listener - Connection established with {0}.", socket.Client.RemoteEndPoint));
                // Instantiating protocol handler and associate it to the current TCP connection
                Handler protocolHandler = new Handler(socket.GetStream(), logQueue);
                // Synchronously process requests made through de current TCP connection
                protocolHandler.Run();
            } catch(Exception ex) {
               logQueue.TryPut(string.Format("Error : {0} !!!", ex.Message));
            } finally {
                if (stream != null)
                    stream.Close();
                socket.Close();
            }
        }

    }
}
