using Microsoft.Azure.Amqp.Framing;
using Microsoft.Azure.EventHubs;
using System;
using System.Text;
using System.Text.Json;

namespace EventHubDemo
{
    public class Program
    {
        private static EventHubClient eventHubClient;
        private const string EventHubConnectionString = "Endpoint=sb://azureeventhubdemotest.servicebus.windows.net/;SharedAccessKeyName=EventHubdemoSAS;SharedAccessKey=6y7emH3yDPqmfj8jy/GVXZQZSxjFFyItKjtTClbIqPM=;EntityPath=eventhubdemo";
        private const string EventHubName = "eventhubdemo";
        public static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }

        private static async Task MainAsync(string[] args)
        {
            // Creates an EventHubsConnectionStringBuilder object from the connection string, and sets the EntityPath.
            // Typically, the connection string should have the entity path in it, but for the sake of this simple scenario
            // we are using the connection string from the namespace.
            var connectionStringBuilder = new EventHubsConnectionStringBuilder(EventHubConnectionString)
            {
                EntityPath = EventHubName
            };

            eventHubClient = EventHubClient.CreateFromConnectionString(connectionStringBuilder.ToString());

            Console.WriteLine("Please enter desire number of message to send on EventHub...");
            var sendMessagesCount = Console.ReadLine();
            int.TryParse(sendMessagesCount, out int numMessagesToSend);

            await SendMessagesToEventHub(numMessagesToSend);

            await eventHubClient.CloseAsync();

            Console.WriteLine("Press ENTER to exit.");
            Console.ReadLine();
        }

        public class WeatherForecast
        {
            
            public int messageId { get; set; }
            public string? deviceId { get; set; }
            public string? temperature { get; set; }
            public string? humidity { get; set; }
        }

        // Uses the event hub client to send 100 messages to the event hub.
        private static async Task SendMessagesToEventHub(int numMessagesToSend)
        {
            var random = new Random();
            for (var i = 0; i < numMessagesToSend; i++)
            {
                try
                {
                    var weatherForecast = new WeatherForecast
                    {
                        messageId= i,
                        deviceId= Guid.NewGuid().ToString(),
                        temperature= random.Next(0, 50).ToString(),
                        humidity = random.Next(0, 30).ToString()
                    };
                    string jsonString = JsonSerializer.Serialize(weatherForecast);
                    //var message = $"Message {i}";
               
                    Console.WriteLine($"Sending message: {jsonString}");
                    await eventHubClient.SendAsync(new EventData(Encoding.UTF8.GetBytes(jsonString)));
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"{DateTime.Now} > Exception: {exception.Message}");
                }

                await Task.Delay(10);
            }

            Console.WriteLine($"{numMessagesToSend} messages sent.");
        }
    }
}