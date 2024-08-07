namespace ConflictFree.Tests;

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
}

public class InventoryService
{
  private readonly Dictionary<Guid, int> _stock = new();
  private readonly Dictionary<Guid, bool> _isSuccessful = new();

  public async Task InsertStock(Guid productId, int amount)
  {
    _stock[productId] = _stock.GetValueOrDefault(productId) + amount;
  }

  public async Task<Guid> RetrieveStock(Guid productId, int amount) {
    var currentStock = _stock.GetValueOrDefault(productId);
    var requestId = Guid.NewGuid();
    if (currentStock < amount) {
      _isSuccessful[requestId] = false;
      return requestId;
    }
    _stock[productId] = _stock.GetValueOrDefault(productId) - amount;
    _isSuccessful[requestId] = true;
    return requestId;
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
