﻿namespace ConflictFree.Tests.Inventory;

public class InsertStockShould
{
  [Fact(DisplayName = "register the stock that was inserted")]
  public async Task Test()
  {
    var service = new InventoryService();
    var productId = new Guid("0C4702CE-C1E4-4082-A06D-EB8773DAD6E8");
    await service.InsertStock(productId, 10);
    Assert.Equal(10, await service.GetStock(productId));
  }

  [Fact(DisplayName = "register the stock that was inserted for two products")]
  public async Task Test2()
  {
    var service = new InventoryService();
    
    var product1Id = new Guid("E798C10E-183C-4E2E-8E7D-8FE88C893588");
    await service.InsertStock(product1Id, 10);
    Assert.Equal(10, await service.GetStock(product1Id));
    
    var product2Id = new Guid("2F4F24AE-09C7-4F0E-851D-5D0A0A00CA28");
    await service.InsertStock(product2Id, 1);
    Assert.Equal(1, await service.GetStock(product2Id));
  }

  [Fact(DisplayName = "register the stock that was inserted for two products with different amounts")]
  public async Task Test3()
  {
    var service = new InventoryService();
    var product1Id = new Guid("E798C10E-183C-4E2E-8E7D-8FE88C893588");
    await service.InsertStock(product1Id, 11);
    Assert.Equal(11, await service.GetStock(product1Id));
    
    var product2Id = new Guid("2F4F24AE-09C7-4F0E-851D-5D0A0A00CA28");
    await service.InsertStock(product2Id, 14);
    Assert.Equal(14, await service.GetStock(product2Id));
  }

  [Fact(DisplayName = "register the stock that was inserted for two products with unknown amounts and ids")]
  public async Task Test4()
  {
    var service = new InventoryService();
    var product1Id = Guid.NewGuid();
    var amount1 = new Random().Next(100000);
    await service.InsertStock(product1Id, amount1);
    Assert.Equal(amount1, await service.GetStock(product1Id));
    var product2Id = Guid.NewGuid();
    var amount2 = new Random().Next(100000);
    await service.InsertStock(product2Id, amount2);
    Assert.Equal(amount2, await service.GetStock(product2Id));
  }
}
