using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;

class Program
{
    private const string connectionString = "Endpoint=sb://firstservicebus1.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=HE4ryujaS3HQhoT2oYIZHR44kZh6yBzPq+ASbMpsZas=";
    private const string queueName = "orderqueue";

    static async Task Main()
    {
        Console.WriteLine("Select an option:");
        Console.WriteLine("1. Send Message");
        Console.WriteLine("2. Receive Messages");
        Console.Write("Enter your choice: ");

        string? choice = Console.ReadLine();

        if (choice == "1")
        {
            await SendMessageAsync();
        }
        else if (choice == "2")
        {
            await ReceiveMessagesAsync();
        }
        else
        {
            Console.WriteLine("❌ Invalid choice. Please restart and enter 1 or 2.");
        }
    }

    // 📨 Method to Send a Message to Azure Service Bus
    static async Task SendMessageAsync()
    {
        await using var client = new ServiceBusClient(connectionString);
        ServiceBusSender sender = client.CreateSender(queueName);

        Console.Write("Enter message to send: ");
        string? messageText = Console.ReadLine();

        ServiceBusMessage message = new ServiceBusMessage(messageText);
        await sender.SendMessageAsync(message);

        Console.WriteLine($"✅ Message sent: {messageText}");
    }

    // 📥 Method to Receive Messages from Azure Service Bus
    static async Task ReceiveMessagesAsync()
    {
        await using var client = new ServiceBusClient(connectionString);
        ServiceBusProcessor processor = client.CreateProcessor(queueName);

        processor.ProcessMessageAsync += async args =>
        {
            string body = args.Message.Body.ToString();
            Console.WriteLine($"📩 Received Message: {body}");
            await args.CompleteMessageAsync(args.Message);
        };

        processor.ProcessErrorAsync += args =>
        {
            Console.WriteLine($"⚠️ Error: {args.Exception.Message}");
            return Task.CompletedTask;
        };

        await processor.StartProcessingAsync();
        Console.WriteLine("✅ Listening for messages... Press ENTER to stop.");
        Console.ReadLine();
        await processor.StopProcessingAsync();
    }
}
