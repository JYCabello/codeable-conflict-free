namespace ConflictFree.Tests.Inventory;

public class InsertStockShould
{
  [Fact(DisplayName = "register the stock that was inserted")]
  public void Test()
  {
    var service = new InventoryService();
    var productId = new Guid("0C4702CE-C1E4-4082-A06D-EB8773DAD6E8");
    service.InsertStock(productId, 10);
    Assert.Equal(10, service.GetStock(productId));
  }

  [Fact(DisplayName = "register the stock that was inserted for two products")]
  public void Test2()
  {
    var service = new InventoryService();
    var product1Id = new Guid("E798C10E-183C-4E2E-8E7D-8FE88C893588");
    service.InsertStock(product1Id, 10);
    Assert.Equal(10, service.GetStock(product1Id));
    var product2Id = new Guid("2F4F24AE-09C7-4F0E-851D-5D0A0A00CA28");
    service.InsertStock(product2Id, 1);
    Assert.Equal(1, service.GetStock(product2Id));
  }
}
