using ConcurrencyProgramming.serie3.APMServer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ConcurrencyProgramming.serie3.APMServer {
    class Server {
            class Program {
                /// <summary>
                ///	Application's starting point. Starts a tracking server that listens at the TCP port 
                ///	specified as a command line argument.
                /// </summary>
                public static void Main(string[] args) {
                    String execName = AppDomain.CurrentDomain.FriendlyName.Split('.')[0];
                    // Checking command line arguments
                    if (args.Length > 1) {
                        Console.WriteLine("Usage: {0} [<TCPPortNumber>]", execName);
                        Environment.Exit(1);
                    }

                    ushort port = 8080;
                    if (args.Length == 1) {
                        if (!ushort.TryParse(args[0], out port)) {
                            Console.WriteLine("Usage: {0} [<TCPPortNumber>]", execName);
                            return;
                        }
                    }
                    Console.WriteLine("--server starts listen on port {0}", port);

                    // Start servicing
                    Logger log = new Logger();
                    log.Start();
                    try {
                        new Listener(port).Run(log);
                    } finally {
                        log.Stop();
                    }
                }
            }
        }
    }
