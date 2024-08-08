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

        var retrievals = new int[]{
        1, 2, 3, 4,
        2, 4, 6, 8,
        30, 40};

        var tasks = new List<Task>();
        foreach(var retrieval in retrievals)
        {
          tasks.Add(inventoryService.RetrieveStock(productId, retrieval));
        }

        await Task.WhenAll(tasks);
        Assert.Equal(0, await inventoryService.GetStock(productId));
    }

    [Fact(DisplayName = "retrieve an amount greater than stock")]
    public async Task AmountGreaterThanStock()
    {
        var service = new InventoryService();
        var productId = Guid.NewGuid();
        await service.InsertStock(productId, 15);
        await service.RetrieveStock(productId, 20);
        Assert.Equal(0, await service.GetStock(productId));
    }
}
