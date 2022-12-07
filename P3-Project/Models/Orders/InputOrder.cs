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
	public int Discount {get; set;} = 0;
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
			shop_unit.Discount = unit.ShopUnit.Discount;
			order.ShopUnits.Add(shop_unit);
		}

		return order;
	}

	int getDiscout(int price, PromoCode code){
		switch(code.DiscountType){
			case PromoCodeDiscountType.Fixed:
				return price - code.Value >= 0 ? code.Value : 0;
			case PromoCodeDiscountType.Percentage:
				return (price * code.Value) / 100; 
			default:
				throw new Exception("Not gonna happen");
		}	
	}

	int getPriceItem(int modelId) {
		return DB.Globals.StorageDB.DB.GetRow("ItemModels", new ItemModel(), modelId.ToString()).ModelPrice;
	}
	
	int getPricePack(int packId) {
		return new PackModel(packId, DB.Globals.StorageDB).Price;
	}

	public void Validate(){
		if     (this.Name == "") throw new Exception("Der mangler at blive intasted et navn");
		else if(this.Email == "") throw new Exception("Der mangler en email");

		bool saw_pack = false;
		var codes = new List<PromoCode>();
		foreach(var code in this.PromoCodes) {
			var (item, promo_code) = PromoCode.Validate(code, DB.Globals.StorageDB);
			if(!item) throw new Exception("Promo Code " + code + " was not validated");
			codes.Add(promo_code ?? throw new Exception("Shoundn't happen"));
		}

		foreach(var unit in this.ShopUnits) {
			if(unit.ShopUnit.IsPack) continue;
			var item = DB.Globals.StorageDB.DB.GetRow("Item" + unit.ShopUnit.ModelId, new Item(), unit.ShopUnit.ItemId.ToString());
			if(item.Stock - item.Reserved - unit.Amount < 0) 
				throw new Exception($"Der er ikke nok af {unit.ShopUnit.Name} i størrelse {unit.ShopUnit.Size} og farve {unit.ShopUnit.Color} på lager");
		}

		for(int i = 0; i < this.ShopUnits.Count; i++){
			if(!saw_pack && !this.ShopUnits[i].ShopUnit.IsPack){
				var price = getPriceItem(this.ShopUnits[i].ShopUnit.ModelId);
				foreach(var code in codes){
					if(code.ItemType == PromoCodeItemType.All || code.ItemType == PromoCodeItemType.AllItems){
						this.ShopUnits[i].ShopUnit.Discount += getDiscout(price, code);
					} else if(code.ItemType == PromoCodeItemType.Some) {
						foreach(var item in code.Items){
							if(!item.IsPack && item.Id == this.ShopUnits[i].ShopUnit.ModelId){
								this.ShopUnits[i].ShopUnit.Discount += getDiscout(price, code);
								break;
							}
						}
					}
				}
			} else { 
				saw_pack = true;
				if(!this.ShopUnits[i].ShopUnit.IsPack)
					continue;
				var price = getPricePack(this.ShopUnits[i].ShopUnit.PackId);
				foreach(var code in codes){
					if(code.ItemType == PromoCodeItemType.All || code.ItemType == PromoCodeItemType.AllItems){
						this.ShopUnits[i].ShopUnit.Discount += getDiscout(price, code);
					} else if(code.ItemType == PromoCodeItemType.Some) {
						foreach(var item in code.Items){
							if(item.IsPack && item.Id == this.ShopUnits[i].ShopUnit.PackId){
								this.ShopUnits[i].ShopUnit.Discount += getDiscout(price, code);
								break;
							}
						}
					}
				}
			}
		}
	}
}

