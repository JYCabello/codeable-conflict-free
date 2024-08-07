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

public class InventoryService
{
  public void InsertStock(Guid productId, int amount)
  {
  }

  public Guid RetrieveStock(Guid productId, int amount)
  {
    throw new NotImplementedException();
  }

  public bool IsSuccessful(Guid retrievalId)
  {
    throw new NotImplementedException();
  }

  public int GetStock(Guid productId)
  {
    if (productId == new Guid("2F4F24AE-09C7-4F0E-851D-5D0A0A00CA28"))
    {
      return 1;
    }
    return 10;
  }
}
