using ConflictFree.Tests;

public class Integration
{
  [Fact(DisplayName = "Insertar un evento de cambio de stock y recuperar stock correcto")]
  public async Task Test1()
  {
    var inventoryRepository = new InventoryRepository();
    var newId = Guid.NewGuid();

    inventoryRepository.InsertEvent(new StockRestored(newId, 1));
    var productStock = inventoryRepository.GetProductStock(newId);

    Assert.Equal(1, productStock.Stock);
  }
}