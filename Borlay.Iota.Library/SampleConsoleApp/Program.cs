using System;
using System.Diagnostics;
using System.Threading;
using Borlay.Iota.Library;
using Borlay.Iota.Library.Models;

namespace SampleConsoleApp
{
    /// <summary>
    /// This a a basic example app
    /// You should avoid using .Result on the async methods and use await, 
    /// but I'm using it here to keep things easy to understand.
    /// </summary>
    class Program
    {
        private const string Url = "http://node.iotawallet.info:14265";

        static void Main(string[] args)
        {
            Console.WriteLine("Connecting to a Node!");

            // !!Use a secure seed!! not this one!
            var seed = "CBM9PSNQPZVDXCHYJXDKUQITXAWQPBWZGYTBGTEIFWXOZTMHESEVHYLXWASWQFEJHUAKHIKSCA9AL9KMG";

            var api = new IotaApi(Url, 14);

            var address1 = api.GetAddress(seed, 0).Result;
            Console.WriteLine("your first address:" + address1.Address);

            var address2 = api.GetAddress(seed, 1).Result;
            Console.WriteLine("your second address:" + address2.Address);

            var stopwatch = Stopwatch.StartNew();
            // Sending a message without using any funds.
            var transfer = new TransferItem()
            {
                Address = address2.Address,
                Value = 0,
                Message = "MY9FIRST9MESSAGE",
                Tag = "TAGGOESHERE"
            };

            var transactionItem = api.AttachTransfer(transfer, CancellationToken.None).Result;
            Console.WriteLine($"You transaction took: {stopwatch.Elapsed.TotalSeconds} seconds.");
            Console.WriteLine($"Your transaction hash (might be):  {transactionItem[0].Hash}");


            Console.WriteLine($"Press any key to exit");
            Console.ReadKey();
        }    
    }
}
