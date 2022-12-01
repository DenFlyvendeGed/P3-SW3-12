namespace P3_Project.Models.Orders;

public class InputShopUnit {
	public bool IsPack{get; set;} = false;
	public int ModelId{get; set;} = 0;
	public int PackId{get => ModelId; set => ModelId = value;}
	public int ItemId{get; set;}
	public string Name{get; set;} = "";
	public string Color{get; set;} = "";
	public string Size{get; set;} = "";
	public int Price {get; set;} = 0;
}

public class InputItem {
	public int Amount{get; set;} = 0;
	public InputShopUnit ShopUnit {get; set;} = new();
}

public class InputOrder{
	public int Price {get; set;} = 0;
	public string Name  {get; set;} = "";
	public string Email {get; set;} = "";
	public List<InputItem> ShopUnits {get; set;} = new();
	public List<string> PromoCodes {get; set; } = new();

	public Order ToOrder(){
		var order = new Order();
		order.Name = this.Name;
		order.Price = this.Price;
		order.Email = this.Email;

		foreach(var unit in this.ShopUnits){
			OrderShopUnit shop_unit;
			if(unit.ShopUnit.IsPack){
				PackSnapShot p = new();
				p.Name = unit.ShopUnit.Name;
				p.Price = unit.ShopUnit.Price;
				p.PackId = unit.ShopUnit.PackId;
				p.Price = unit.ShopUnit.Price;
				p.SalesTax = unit.ShopUnit.Price;
				shop_unit = new(p);
			} else {
				ItemSnapshot i = new();
				i.Name= unit.ShopUnit.Name;
				i.Price = unit.ShopUnit.Price;
				i.ModelId = unit.ShopUnit.ModelId;
				i.ItemId = unit.ShopUnit.ItemId;
				i.Color = unit.ShopUnit.Color;
				i.Size = unit.ShopUnit.Size;
				shop_unit = new(i);
			}
			shop_unit.Amount = unit.Amount;
			shop_unit.Discount = 0;
			order.ShopUnits.Add(shop_unit);
		}

		return order;
	}
}

