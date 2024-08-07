namespace ConflictFree.Tests.Inventory;

public class RetrieveStockShould
{
    [Fact(DisplayName = "retrieve the stock of the product with the amount requested")]
    public void Test()
    {
        var service = new InventoryService();
        var productId = Guid.NewGuid();
        service.InsertStock(productId, 32);
        service.RetrieveStock(productId, 32);
        Assert.Equal(0, service.GetStock(productId));
    }
}