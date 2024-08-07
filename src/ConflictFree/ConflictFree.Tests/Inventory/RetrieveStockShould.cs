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
}