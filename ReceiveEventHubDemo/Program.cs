using Microsoft.Azure.EventHubs.Processor;
using Microsoft.Azure.EventHubs;

namespace ReceiveEventHubDemo
{
    class Program
    {
        private const string EventHubConnectionString = "Endpoint=sb://azureeventhubdemotest.servicebus.windows.net/;SharedAccessKeyName=EventHubdemoSAS;SharedAccessKey=6y7emH3yDPqmfj8jy/GVXZQZSxjFFyItKjtTClbIqPM=;EntityPath=eventhubdemo";
        private const string EventHubName = "eventhubdemo";
        private const string StorageContainerName = "blobazurecontainer";
        private const string StorageAccountName = "myblobazure";
        private const string StorageAccountKey = "";

        private static readonly string StorageConnectionString = string.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}", StorageAccountName, StorageAccountKey);
        static void Main(string[] args)
        {
            Console.WriteLine("Hello Receiver ....");
            MainAsync(args).GetAwaiter().GetResult();
        }

        private static async Task MainAsync(string[] args)
        {
            Console.WriteLine("Registering EventProcessor...");

            var eventProcessorHost = new EventProcessorHost(
                EventHubName,
                PartitionReceiver.DefaultConsumerGroupName,
                EventHubConnectionString,
                StorageConnectionString,
                StorageContainerName);

            // Registers the Event Processor Host and starts receiving messages
            await eventProcessorHost.RegisterEventProcessorAsync<SimpleEventProcessor>();

            Console.WriteLine("Receiving. Press ENTER to stop worker.");
            Console.ReadLine();

            // Disposes of the Event Processor Host
            await eventProcessorHost.UnregisterEventProcessorAsync();
        }
    }
}