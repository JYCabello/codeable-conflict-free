namespace ConflictFree.Tests.Inventory;

public class IsSuccessfulShould
{
    [Fact(DisplayName = "should return true for a successful request")]
    public void Test()
    {
        var service = new InventoryService();
        var productId = Guid.NewGuid();
        service.InsertStock(productId, 32);
        var requestId = service.RetrieveStock(productId, 32);
        Assert.True(service.IsSuccessful(requestId));
    }
}