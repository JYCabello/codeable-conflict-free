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
}
