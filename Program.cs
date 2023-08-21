using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Newtonsoft.Json;

var numEvent = 100;

Console.WriteLine("Initial Event Producer...");

var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()) // Use UseBasePath here
                .AddJsonFile("appsettings.json")
                .Build();

var eventHubSettings = configuration.GetSection("EventHubSettings");


string connectionString = eventHubSettings["ConnectionString"] ?? "";
string eventHubName = eventHubSettings["Name"] ?? "";

await using (var producerClient = new EventHubProducerClient(connectionString, eventHubName))
{
    Random random = new Random();

    var batchOptions = new CreateBatchOptions();
    using EventDataBatch eventBatch = await producerClient.CreateBatchAsync(batchOptions);

    Console.WriteLine("Start sending data...");

    for (int i = 0; i < numEvent; i++)
    {
        string device = random.Next(1, 3).ToString();
        string status = random.Next(0, 2) == 0 ? "success" : "failed";

        EventData eventData = new EventData(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new
        {
            device,
            status
        })));

        if (!eventBatch.TryAdd(eventData))
        {
            // If the batch is full, send it and create a new one
            await producerClient.SendAsync(eventBatch);
        }
        Console.WriteLine($"Sent: Device {device}, Status: {status}");
    }

    await producerClient.CloseAsync();

    Console.WriteLine("Close event producer...");
}