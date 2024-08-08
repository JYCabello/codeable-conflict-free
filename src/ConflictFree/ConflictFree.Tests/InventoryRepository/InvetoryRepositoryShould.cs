using ConflictFree.Tests;

public class Integration
{
  [Fact(DisplayName = "Insertar un evento de cambio de stock de 1 y recuperar stock correcto")]
  public async Task Test1()
  {
    var inventoryRepository = new InventoryRepository();
    var newId = Guid.NewGuid();

    inventoryRepository.InsertEvent(new StockRestored(newId, 1));
    var productStock = inventoryRepository.GetProductStock(newId);

    Assert.Equal(1, productStock.Stock);
  }

  [Fact(DisplayName = "Insertar un evento de cambio de stock de 2 y recuperar stock correcto")]
  public async Task Test2()
  {
    var inventoryRepository = new InventoryRepository();
    var newId = Guid.NewGuid();

    inventoryRepository.InsertEvent(new StockRestored(newId, 2));
    var productStock = inventoryRepository.GetProductStock(newId);

    Assert.Equal(2, productStock.Stock);
  }
}