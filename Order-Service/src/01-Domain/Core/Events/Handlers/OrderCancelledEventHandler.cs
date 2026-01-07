namespace Order_Service.src._01_Domain.Core.Events.Handlers
{
    public class OrderCancelledEventHandler : IDomainEventHandler<OrderCancelledEvent>
    {
        // In a real application, you would inject clients for other services here
        // private readonly IInventoryServiceClient _inventoryServiceClient;
        // private readonly IPaymentServiceClient _paymentServiceClient;

        public OrderCancelledEventHandler()
        {
            // _inventoryServiceClient = inventoryServiceClient;
            // _paymentServiceClient = paymentServiceClient;
        }

        public async Task HandleAsync(OrderCancelledEvent domainEvent, CancellationToken cancellationToken = default)
        {
            Console.WriteLine($"--- Order Cancelled Event Handled ---");
            Console.WriteLine($"Order ID: {domainEvent.OrderId}");
            Console.WriteLine($"Order Number: {domainEvent.OrderNumber}");
            Console.WriteLine($"User ID: {domainEvent.UserId}");

            // Example Logic:
            // 1. Get the original order details to release stock and process refund.
            // var originalOrder = await _orderRepository.GetByIdAsync(domainEvent.OrderId, cancellationToken);
            // if (originalOrder != null)
            // {
            //     foreach (var item in originalOrder.Items)
            //     {
            //         await _inventoryServiceClient.ReleaseStockAsync(item.ProductId, item.Quantity, cancellationToken);
            //     }
            //     
            //     if (originalOrder.PaymentStatus == PaymentStatus.Succeeded)
            //     {
            //         await _paymentServiceClient.InitiateRefundAsync(originalOrder.Id, originalOrder.TotalPrice, cancellationToken);
            //     }
            // }

            // Simulate async work
            await Task.Delay(100, cancellationToken);

            Console.WriteLine("--- Order Cancelled Event Processing Finished ---");
        }
    }
}
