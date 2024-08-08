using System.Collections.Concurrent;

namespace ConflictFree.Tests;

public class Integration
{
  [Fact(DisplayName = "control de inventario para un producto lleva el registro de retiradas")]
  public async Task Test1()
  {
    var inventoryService = new InventoryService(new());
    var productId = Guid.NewGuid();
    await inventoryService.InsertStock(productId, 100);

    var retrieval1Id = await inventoryService.RetrieveStock(productId, 75);
    Assert.True(await inventoryService.IsSuccessful(productId, retrieval1Id));
    Assert.Equal(25, await inventoryService.GetStock(productId));

    var failedRetrievalId = await inventoryService.RetrieveStock(productId, 30);
    Assert.False(await inventoryService.IsSuccessful(productId, failedRetrievalId));
    Assert.Equal(25, await inventoryService.GetStock(productId));

    var retrieval2Id = await inventoryService.RetrieveStock(productId, 25);
    Assert.True(await inventoryService.IsSuccessful(productId, retrieval2Id));
    Assert.Equal(0, await inventoryService.GetStock(productId));
  }

  [Fact(
    DisplayName = "control de inventario para un producto lleva el registro de retiradas en paralelo"
  )]
  public async Task Test2()
  {
    var inventoryService = new InventoryService(new());
    var productId = Guid.NewGuid();
    await inventoryService.InsertStock(productId, 100);

    var retrievals = new[] { 20, 20, 20, 20, 20 };
    var tasks = new List<Task<Guid>>();
    foreach (var retrieval in retrievals) tasks.Add(inventoryService.RetrieveStock(productId, retrieval));

    var retrievalIds = await Task.WhenAll(tasks);

    foreach (var retrievalId in retrievalIds)
    {
      var isSuccess = await inventoryService.IsSuccessful(productId, retrievalId);
      Assert.True(isSuccess, "Operation should have been successful");
      var remainingStock = await inventoryService.GetStock(productId);
      Assert.True(remainingStock >= 0, "Stock should not be negative");
    }

    var finalStock = await inventoryService.GetStock(productId);
    Assert.True(finalStock == 0, "Stock should be zero");
  }
}

public class InventoryService
{
  private readonly InventoryRepository _repository;

  public InventoryService(InventoryRepository repository)
  {
    _repository = repository;
  }

  public async Task InsertStock(Guid productId, int amount)
  {
    if (amount < 0) throw new ArgumentException("Amount can't be less than zero.", nameof(amount));
    if (amount == 0) throw new ArgumentException("Amount must be greater than zero.", nameof(amount));
    _repository.InsertEvent(new StockRestored(productId, amount, DateTime.UtcNow));
  }

  public async Task<Guid> RetrieveStock(Guid productId, int amount)
  {
    var requestId = Guid.NewGuid();

    await Task.Delay(100);
    if (amount == 0) throw new ArgumentException("Amount must be greater than zero.", nameof(amount));
    _repository.InsertEvent(new StockRemovalRequested(productId, amount, requestId, DateTime.UtcNow));

    return await Task.FromResult(requestId);
  }

  public async Task<bool> IsSuccessful(Guid productId, Guid retrievalId)
  {
    return !_repository.GetProductStock(productId).FailedRequests.Contains(retrievalId);
  }

  public async Task<int> GetStock(Guid productId)
  {
    return _repository.GetProductStock(productId).Stock;
  }

  public record RestockRequest(Guid productId, int quantity);

  public record RetrievalRequest(Guid productId, int quantity);
}

public class InventoryRepository
{
  private readonly ConcurrentBag<IEvent> _events = new();

  public ProductStock GetProductStock(Guid productId)
  {
    return _events
      .Where(e => e.ProductId == productId)
      .OrderBy(e => e.CreatedAt)
      .Aggregate(new ProductStock(productId, 0, []), (stock, @event) => stock.Apply(@event));
  }

  public void InsertEvent(IEvent @event)
  {
    _events.Add(@event);
  }
}

public record ProductStock(Guid ProductId, int Stock, Guid[] FailedRequests)
{
  public ProductStock Apply(IEvent @event)
  {
    return @event switch
    {
      StockRestored e => this with { Stock = Stock + e.Amount },
      StockRemovalRequested e =>
        Stock < e.Amount
          ? this with { FailedRequests = FailedRequests.Append(e.RequestId).ToArray() }
          : this with { Stock = Stock - e.Amount },
      _ => this
    };
  }
}

public interface IEvent
{
  Guid ProductId { get; }
  DateTime CreatedAt { get; }
}

public record StockRestored(Guid ProductId, int Amount, DateTime CreatedAt) : IEvent;

public record StockRemovalRequested(Guid ProductId, int Amount, Guid RequestId, DateTime CreatedAt) : IEvent;
