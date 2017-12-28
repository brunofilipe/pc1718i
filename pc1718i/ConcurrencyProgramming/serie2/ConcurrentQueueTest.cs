using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConcurrencyProgramming.serie2
{
    class ConcurrentQueueTest{
     private static bool TestMichaelScottQueue(){

        const int CONSUMER_THREADS = 2;
        const int PRODUCER_THREADS = 1;
        const int MAX_PRODUCE_INTERVAL = 100;
        const int MAX_CONSUME_TIME = 25;
        const int FAILURE_PERCENT = 5;
        const int JOIN_TIMEOUT = 100;
        const int RUN_TIME = 5 * 1000;
        const int POLL_INTERVAL = 20;


        Thread[] consumers = new Thread[CONSUMER_THREADS];
        Thread[] producers = new Thread[PRODUCER_THREADS];
       ConcurrentQueue<String> msqueue = new ConcurrentQueue<String>();
        int[] productions = new int[PRODUCER_THREADS];
        int[] consumptions = new int[CONSUMER_THREADS];
        int[] failuresInjected = new int[PRODUCER_THREADS];
        int[] failuresDetected = new int[CONSUMER_THREADS];

        Console.WriteLine("\n\n--> Start test of Michael-Scott queue in producer/consumer context...\n\n");

        // create and start the consumer threads.
        for (int i = 0; i < CONSUMER_THREADS; i++) {
            int tid = i;
            consumers[i] = new Thread(() => {
                Random rnd = new Random(Thread.CurrentThread.ManagedThreadId);
                int count = 0;

                Console.WriteLine("-->c#" + tid + " starts...\n");
                do {
                    try {
                        String data = msqueue.Take();
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
        for (int i = 0; i < PRODUCER_THREADS; i++) {
            int tid = i;
            producers[i] = new Thread( () => {
                Random rnd = new Random(Thread.CurrentThread.ManagedThreadId);
                int count = 0;

               Console.WriteLine("-->p#" + tid + " starts...\n");
                do {
                    String data;

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
            producers[i].IsBackground = true;
            producers[i].Start();
        }

        // run the test RUN_TIME milliseconds.

        Thread.Sleep(RUN_TIME);

        // interrupt all producer threads and wait for for until each one finished.
        int stillRunning = 0;
        for (int i = 0; i < PRODUCER_THREADS; i++) {
            producers[i].Interrupt();
            producers[i].Join(JOIN_TIMEOUT);
            if (producers[i].IsAlive)
                stillRunning++;

        }

        // wait until the queue is empty
        while (!msqueue.IsEmpty())
            Thread.Sleep(POLL_INTERVAL);

        // interrupt each consumer thread and wait for a while until each one finished.
        for (int i = 0; i < CONSUMER_THREADS; i++) {
            consumers[i].Interrupt();
            consumers[i].Join(JOIN_TIMEOUT);
            if (consumers[i].IsAlive)
                stillRunning++;
        }

        // if any thread failed to fisnish, something is wrong.
        if (stillRunning > 0) {
            Console.WriteLine("\n*** failure: " + stillRunning + " thread(s) did answer to interrupt\n");
            return false;
        }

        // compute and display the results.

        long sumProductions = 0, sumFailuresInjected = 0;
        for (int i = 0; i < PRODUCER_THREADS; i++) {
            sumProductions += productions[i];
            sumFailuresInjected += failuresInjected[i];
        }
        long sumConsumptions = 0, sumFailuresDetected = 0;
        for (int i = 0; i < CONSUMER_THREADS; i++) {
            sumConsumptions += consumptions[i];
            sumFailuresDetected += failuresDetected[i];
        }
        Console.WriteLine("\n\n<-- successful: " + sumProductions + "/" + sumConsumptions + ", failed: " +
                            sumFailuresInjected + "/" + sumFailuresDetected + "\n");

        return sumProductions == sumConsumptions && sumFailuresInjected == sumFailuresDetected;
    }

     static void Main(String[] args)  {
        Console.WriteLine("\n\n--> Test Michael-Scott concurrent queue "+ 
                 (TestMichaelScottQueue() ? "passed" : "failed") + "\n");
        Console.ReadKey();
        }
    }
}
