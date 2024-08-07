namespace ConflictFree.Tests;

public class Integration
{
  [Fact(DisplayName = "control de inventario para un producto lleva el registro de retiradas")]
  public void Test1()
  {
    var inventoryService = new InventoryService();
    var productId = Guid.NewGuid();
    inventoryService.InsertStock(productId, 100);
    var retrieval1Id = inventoryService.RetrieveStock(productId, 75);
    Assert.True(inventoryService.IsSuccessful(retrieval1Id));
    Assert.Equal(25, inventoryService.GetStock(productId));
    var failedRetrievalId = inventoryService.RetrieveStock(productId, 30);
    Assert.False(inventoryService.IsSuccessful(failedRetrievalId));
    Assert.Equal(25, inventoryService.GetStock(productId));
    var retrieval2Id = inventoryService.RetrieveStock(productId, 25);
    Assert.True(inventoryService.IsSuccessful(retrieval2Id));
    Assert.Equal(0, inventoryService.GetStock(productId));
  }
}
