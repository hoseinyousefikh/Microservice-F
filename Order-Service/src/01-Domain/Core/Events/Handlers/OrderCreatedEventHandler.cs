namespace Order_Service.src._01_Domain.Core.Events.Handlers
{
    public class OrderCreatedEventHandler : IDomainEventHandler<OrderCreatedEvent>
    {
        // In a real application, you would inject clients for other services here
        // private readonly IInventoryServiceClient _inventoryServiceClient;
        // private readonly INotificationServiceClient _notificationServiceClient;

        public OrderCreatedEventHandler()
        {
            // _inventoryServiceClient = inventoryServiceClient;
            // _notificationServiceClient = notificationServiceClient;
        }

        public async Task HandleAsync(OrderCreatedEvent domainEvent, CancellationToken cancellationToken = default)
        {
            Console.WriteLine($"--- Order Created Event Handled ---");
            Console.WriteLine($"Order ID: {domainEvent.OrderId}");
            Console.WriteLine($"Order Number: {domainEvent.OrderNumber}");
            Console.WriteLine($"User ID: {domainEvent.UserId}");
            Console.WriteLine("Items:");

            foreach (var item in domainEvent.Items)
            {
                Console.WriteLine($"  - Product: {item.ProductName} (ID: {item.ProductId}), Qty: {item.Quantity}, Price: {item.UnitPrice}");

                // Example Logic:
                // await _inventoryServiceClient.ReserveStockAsync(item.ProductId, item.Quantity, cancellationToken);
            }

            Console.WriteLine($"Total Price: {domainEvent.TotalPrice}");
            Console.WriteLine($"Shipping to: {domainEvent.ShippingAddress}");

            // Example Logic:
            // await _notificationServiceClient.SendOrderConfirmationEmailAsync(domainEvent.UserId, domainEvent.OrderId, cancellationToken);

            // Simulate async work
            await Task.Delay(100, cancellationToken);

            Console.WriteLine("--- Order Created Event Processing Finished ---");
        }
    }
}
