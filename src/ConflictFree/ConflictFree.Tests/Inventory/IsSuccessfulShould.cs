namespace ConflictFree.Tests.Inventory;

public class IsSuccessfulShould
{
    [Fact(DisplayName = "should return true for a successful request")]
    public async Task Test()
    {
        var service = new InventoryService();
        var productId = Guid.NewGuid();
        await service.InsertStock(productId, 32);
        var requestId = await service.RetrieveStock(productId, 32);
        Assert.True(await service.IsSuccessful(requestId));
    }
    [Fact(DisplayName = "should return false for a failed request")]
    public async Task Test2()
    {
        var service = new InventoryService();
        var productId = Guid.NewGuid();
        await service.InsertStock(productId, 32);
        var requestId = await service.RetrieveStock(productId, 33);
        Assert.False(await service.IsSuccessful(requestId));
    }
}
