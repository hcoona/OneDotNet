// <copyright file="Program.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace TplLab
{
    internal class Program
    {
        private static async Task Main()
        {
            // Create a BufferBlock<byte[]> object. This object serves as the
            // target block for the producer and the source block for the consumer.
            var buffer = new BufferBlock<byte[]>();

            // Start the consumer. The Consume method runs asynchronously.
            Task<int> consumer = ConsumeAsync(buffer);

            // Post source data to the dataflow block.
            Produce(buffer);

            // Print the count of bytes processed to the console.
            Console.WriteLine("Processed {0} bytes.", await consumer.ConfigureAwait(false));
        }

        // Demonstrates the production end of the producer and consumer pattern.
        private static void Produce(ITargetBlock<byte[]> target)
        {
            // Create a Random object to generate random data.
            var rand = new Random();

            // In a loop, fill a buffer with random data and
            // post the buffer to the target block.
            for (int i = 0; i < 100; i++)
            {
                // Create an array to hold random byte data.
                byte[] buffer = new byte[1024];

                // Fill the buffer with random bytes.
                rand.NextBytes(buffer);

                // Post the result to the message block.
                target.Post(buffer);
            }

            // Set the target to the completed state to signal to the consumer
            // that no more data will be available.
            target.Complete();
        }

        // Demonstrates the consumption end of the producer and consumer pattern.
        private static async Task<int> ConsumeAsync(ISourceBlock<byte[]> source)
        {
            // Initialize a counter to track the number of bytes that are processed.
            int bytesProcessed = 0;

            // Read from the source buffer until the source buffer has no
            // available output data.
            while (await source.OutputAvailableAsync().ConfigureAwait(false))
            {
                byte[] data = source.Receive();

                // Increment the count of bytes received.
                Interlocked.Add(ref bytesProcessed, data.Length);
            }

            return bytesProcessed;
        }
    }
}
