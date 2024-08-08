namespace ConflictFree.Tests.Inventory;

public class RetrieveStockShould
{
  [Fact(DisplayName = "retrieve the stock of the product with the amount requested")]
  public async Task Test()
  {
    var service = new InventoryService();
    var productId = Guid.NewGuid();
    await service.InsertStock(productId, 32);
    await service.RetrieveStock(productId, 32);
    Assert.Equal(0, await service.GetStock(productId));
  }

  [Fact(DisplayName = "handle parallel retrieval requests correctly")]
  public async Task ParallelRetrievals()
  {
    var inventoryService = new InventoryService();
    var productId = Guid.NewGuid();
    await inventoryService.InsertStock(productId, 100);

    var retrievals = new int[] { 1, 2, 3, 4, 2, 4, 6, 8, 30, 40 };

    var tasks = new List<Task>();
    foreach (var retrieval in retrievals)
    {
      tasks.Add(inventoryService.RetrieveStock(productId, retrieval));
    }

    await Task.WhenAll(tasks);
    Assert.Equal(0, await inventoryService.GetStock(productId));
  }

  [Fact(DisplayName = "not allow zero amount retrieval requests for two products")]
  public async Task NoZeroRetrievalsForTwoProducts()
  {
    var service = new InventoryService();
    var product1Id = Guid.NewGuid();
    await service.InsertStock(product1Id, 5);
    await service.RetrieveStock(product1Id, 5);
    var exception1 = await Assert.ThrowsAsync<ArgumentException>(
      () => service.RetrieveStock(product1Id, 0)
    );
    Assert.Equal("Amount must be greater than zero. (Parameter 'amount')", exception1.Message);

    var product2Id = Guid.NewGuid();
    await service.InsertStock(product2Id, 10);
    await service.RetrieveStock(product2Id, 10);
    var exception2 = await Assert.ThrowsAsync<ArgumentException>(
      () => service.RetrieveStock(product2Id, 0)
    );
    Assert.Equal("Amount must be greater than zero. (Parameter 'amount')", exception2.Message);
  }

  [Fact(DisplayName = "retrieve an amount greater than stock")]
  public async Task AmountGreaterThanStock()
  {
    var service = new InventoryService();
    var productId = Guid.NewGuid();
    await service.InsertStock(productId, 15);

    // Intentar retirar una cantidad mayor al stock
    var retrievalId = await service.RetrieveStock(productId, 20);

    // Verificar que la operación no fue exitosa
    var isSuccess = await service.IsSuccessful(retrievalId);
    Assert.False(
      isSuccess,
      "La operación debería haber fallado porque la cantidad es mayor al stock disponible"
    );

    // Verificar que el stock no haya cambiado
    Assert.Equal(15, await service.GetStock(productId));
  }
}
