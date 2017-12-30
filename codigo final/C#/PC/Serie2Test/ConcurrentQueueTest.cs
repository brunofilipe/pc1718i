using System;
using System.Threading;
using serie2;

namespace Serie2Test
{
    public class ConcurrentQueueTest
    {
        public static void Main() {
            Console.WriteLine("%n--> Test Michael-Scott concurrent queue: "+(TestMichaelScottQueue() ? "passed" : "failed")+"\n");
        }
        
        //
        // Test method.
        //

        public static bool TestMichaelScottQueue() {
            var CONSUMER_THREADS = 2;
            var PRODUCER_THREADS = 1;
            var MAX_PRODUCE_INTERVAL = 100;
            var MAX_CONSUME_TIME = 25;
            var FAILURE_PERCENT = 5;
            var JOIN_TIMEOUT = 100;
            var RUN_TIME = 5 * 1000;
            var POLL_INTERVAL = 20;

            var consumers = new Thread[CONSUMER_THREADS];
            var producers = new Thread[PRODUCER_THREADS];
            var msqueue = new ConcurrentQueue<string>();
            var productions = new int[PRODUCER_THREADS];
            var consumptions = new int[CONSUMER_THREADS];
            var failuresInjected = new int[PRODUCER_THREADS];
            var failuresDetected = new int[CONSUMER_THREADS];

            Console.WriteLine("\n\n--> Start test of Michael-Scott queue in producer/consumer context...\n\n");
            Console.WriteLine(default(string));

            // create and start the consumer threads.		
            for (var i = 0; i < CONSUMER_THREADS; i++) {
                var tid = i;
                consumers[i] = new Thread(() => {
                    var rnd = new Random(Thread.CurrentThread.ManagedThreadId);
                    var count = 0;

                    Console.WriteLine("-->c#" + tid + " starts...\n");
                    do {
                        try {
                            var data = msqueue.Take();
                            if (!data.Equals("hello")) {
                                failuresDetected[tid]++;
                                Console.WriteLine("[f#" + tid + "]");
                            }

                            if (++count % 10 == 0)
                                Console.WriteLine("[c#" + tid + "]");

                            // Simulate the time needed to process the data.

                            if (MAX_CONSUME_TIME > 0)
                                Thread.Sleep(rnd.Next(MAX_CONSUME_TIME));
                        } catch (ThreadInterruptedException ie) {
                            //do {} while (tid == 0);
                            break;
                        }
                    } while (true);

                    // display the consumer thread's results.				
                    Console.WriteLine("\n<--c#" + tid + " exits, consumed: " + count + ", failures: " +
                                    failuresDetected[tid]);
                    consumptions[tid] = count;
                });
                //consumers[i].SetDaemon(true);
                consumers[i].Start();
            }

            // create and start the producer threads.		
            for (var i = 0; i < PRODUCER_THREADS; i++) {
                var tid = i;
                producers[i] = new Thread(() => {
                    var rnd = new Random(Thread.CurrentThread.ManagedThreadId);
                    var count = 0;

                    Console.WriteLine("-->p#" + tid + " starts...\n");
                    do {
                        string data;

                        if (rnd.Next(100) >= FAILURE_PERCENT) {
                            data = "hello";
                        } else {
                            data = "HELLO";
                            failuresInjected[tid]++;
                        }

                        // enqueue a data item
                        msqueue.Put(data);

                        // increment request count and periodically display the "alive" menssage.
                        if (++count % 10 == 0)
                            Console.WriteLine("[p#" + tid + "]");

                        // production interval.

                        try {
                            Thread.Sleep(rnd.Next(MAX_PRODUCE_INTERVAL));
                        } catch (ThreadInterruptedException ie) {
                            //do {} while (tid == 0);
                            break;
                        }
                    } while (true);

                    // display the producer thread's results
                    Console.WriteLine("\n<--p#" + tid + " exits, produced: " + count + ", failures: " +
                                    failuresInjected[tid]);
                    productions[tid] = count;
                });
                //producers[i].setDaemon(true);			
                producers[i].Start();
            }

            // run the test RUN_TIME milliseconds.

            Thread.Sleep(RUN_TIME);

            // interrupt all producer threads and wait for for until each one finished. 
            int stillRunning = 0;
            for (int i = 0; i < PRODUCER_THREADS; i++) {
                producers[i].Interrupt();
                producers[i].Join(JOIN_TIMEOUT);
                if(producers[i].IsAlive)
                    stillRunning++;
            }

            // wait until the queue is empty 
            while (!msqueue.IsEmpty())
                Thread.Sleep(POLL_INTERVAL);

            // interrupt each consumer thread and wait for a while until each one finished.
            for (int i = 0; i < CONSUMER_THREADS; i++) {
                consumers[i].Interrupt();
                consumers[i].Join(JOIN_TIMEOUT);
                if(consumers[i].IsAlive)
                    stillRunning++;
            }

            // if any thread failed to fisnish, something is wrong.
            if (stillRunning > 0) {
                Console.WriteLine("\n*** failure: " + stillRunning + " thread(s) did answer to interrupt\n");
                return false;
            }

            // compute and display the results.

            long sumProductions = 0, sumFailuresInjected = 0;
            for (var i = 0; i < PRODUCER_THREADS; i++) {
                sumProductions += productions[i];
                sumFailuresInjected += failuresInjected[i];
            }

            long sumConsumptions = 0, sumFailuresDetected = 0;
            for (var i = 0; i < CONSUMER_THREADS; i++) {
                sumConsumptions += consumptions[i];
                sumFailuresDetected += failuresDetected[i];
            }

            Console.WriteLine("\n\n<-- successful: " + sumProductions + "/" + sumConsumptions + ", failed: " +
                            sumFailuresInjected + "/" + sumFailuresDetected + "\n");

            return sumProductions == sumConsumptions && sumFailuresInjected == sumFailuresDetected;
        }
    }
}