namespace ConflictFree.Tests;

using System.Runtime.CompilerServices;

public class Integration
{
  [Fact(DisplayName = "control de inventario para un producto lleva el registro de retiradas")]
  public async Task Test1()
  {
    var inventoryService = new InventoryService();
    var productId = Guid.NewGuid();
    await inventoryService.InsertStock(productId, 100);

    var retrieval1Id = await inventoryService.RetrieveStock(productId, 75);
    Assert.True(await inventoryService.IsSuccessful(retrieval1Id));
    Assert.Equal(25, await inventoryService.GetStock(productId));

    var failedRetrievalId = await inventoryService.RetrieveStock(productId, 30);
    Assert.False(await inventoryService.IsSuccessful(failedRetrievalId));
    Assert.Equal(25, await inventoryService.GetStock(productId));

    var retrieval2Id = await inventoryService.RetrieveStock(productId, 25);
    Assert.True(await inventoryService.IsSuccessful(retrieval2Id));
    Assert.Equal(0, await inventoryService.GetStock(productId));
  }

  [Fact(
    DisplayName = "control de inventario para un producto lleva el registro de retiradas en paralelo"
  )]
  public async Task Test2()
  {
    var inventoryService = new InventoryService();
    var productId = Guid.NewGuid();
    await inventoryService.InsertStock(productId, 100);

    var retrievals = new int[] { 20, 20, 20, 20, 20 };
    var tasks = new List<Task<Guid>>();
    foreach (var retrieval in retrievals)
    {
      tasks.Add(inventoryService.RetrieveStock(productId, retrieval));
    }

    var retrievalIds = await Task.WhenAll(tasks);

    foreach (var retrievalId in retrievalIds)
    {
      var isSuccess = await inventoryService.IsSuccessful(retrievalId);
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
  private readonly Dictionary<Guid, int> _stock = new();
  private readonly Dictionary<Guid, bool> _isSuccessful = new();
  private readonly object _lock = new();

  public async Task InsertStock(Guid productId, int amount)
  {
      if (amount < 0)
      {
        throw new ArgumentException("Amount can't be less than zero.", nameof(amount));
      }
      if (amount == 0)
      {
        throw new ArgumentException("Amount must be greater than zero.", nameof(amount));
      }

      var oldStock = _stock.GetValueOrDefault(productId);

      await Task.Delay(100);

      _stock[productId] = oldStock + amount;
  }

  public async Task<Guid> RetrieveStock(Guid productId, int amount)
  {
    var requestId = Guid.NewGuid();
    var currentStock = _stock.GetValueOrDefault(productId);

    await Task.Delay(100);
    if (amount == 0)
    {
      throw new ArgumentException("Amount must be greater than zero.", nameof(amount));
    }
    if (currentStock < amount)
    {
      _isSuccessful[requestId] = false;
    }
    else
    {
      _stock[productId] = _stock.GetValueOrDefault(productId) - amount;
      _isSuccessful[requestId] = true;
    }

    return await Task.FromResult(requestId);
  }

  public async Task<bool> IsSuccessful(Guid retrievalId)
  {
    return _isSuccessful.GetValueOrDefault(retrievalId, false);
  }

  public async Task<int> GetStock(Guid productId)
  {
    return _stock.GetValueOrDefault(productId);
  }
}
