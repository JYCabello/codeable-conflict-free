namespace ConflictFree.Tests.Inventory;

public class InsertStockShould
{
  [Fact(DisplayName = "register the stock that was inserted")]
  public void Test()
  {
    var service = new InventoryService();
    var productId = Guid.NewGuid();
    service.InsertStock(productId, 10);
    Assert.Equal(10, service.GetStock(productId));
  }

  [Fact(DisplayName = "register the stock that was inserted for two products")]
  public void Test2()
  {
    var service = new InventoryService();
    var product1Id = Guid.NewGuid();
    service.InsertStock(product1Id, 10);
    Assert.Equal(10, service.GetStock(product1Id));
    var product2Id = Guid.NewGuid();
    service.InsertStock(product2Id, 1);
    Assert.Equal(1, service.GetStock(product2Id));
  }
}
