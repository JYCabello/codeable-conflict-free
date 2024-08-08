using ConflictFree.Tests;

public class InventoryRepositoryShould
{
  [Fact(DisplayName = "Insertar un evento de cambio de stock de 1 y recuperar stock correcto")]
  public async Task Test1()
  {
    var inventoryRepository = new InventoryRepository();
    var newId = Guid.NewGuid();

    inventoryRepository.InsertEvent(new StockRestored(newId, 1, DateTime.UtcNow));
    var productStock = inventoryRepository.GetProductStock(newId);

    Assert.Equal(1, productStock.Stock);
  }

  [Fact(DisplayName = "Insertar un evento de cambio de stock de 2 y recuperar stock correcto")]
  public void Test2()
  {
    var inventoryRepository = new InventoryRepository();
    var newId = Guid.NewGuid();

    inventoryRepository.InsertEvent(new StockRestored(newId, 2, DateTime.UtcNow));
    var productStock = inventoryRepository.GetProductStock(newId);

    Assert.Equal(2, productStock.Stock);
  }

  [Fact(DisplayName = "No reduce stock si no tiene")]
  public void Test3()
  {
    var inventoryRepository = new InventoryRepository();
    var productId = Guid.NewGuid();

    inventoryRepository.InsertEvent(new StockRestored(productId, 2, DateTime.UtcNow));
    Assert.Equal(2, inventoryRepository.GetProductStock(productId).Stock);
    inventoryRepository.InsertEvent(new StockRemovalRequested(productId, 3, Guid.NewGuid(), DateTime.UtcNow));
    Assert.Equal(2, inventoryRepository.GetProductStock(productId).Stock);
  }

  [Fact(DisplayName = "Registra las solicitudes fallidas de retiro de stock")]
  public void Test4()
  {
    var inventoryRepository = new InventoryRepository();
    var productId = Guid.NewGuid();
    var requestId = Guid.NewGuid();
    inventoryRepository.InsertEvent(new StockRemovalRequested(productId, 3, requestId, DateTime.UtcNow));
    Assert.Contains(requestId, inventoryRepository.GetProductStock(productId).FailedRequests);
  }
}
