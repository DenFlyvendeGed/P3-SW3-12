namespace P3_Project.Models; 
using P3_Project.Models.DB;
public enum PromoCodeDiscountType {
	Percentage, Fixed
}

public enum PromoCodeItemType {
	All, AllPacks, AllItems, Some
}

public class PromoCodeSomeItemType {
	public int Id{get; set;} = 0;
	public bool IsPack{get; set;} = false;
}

public class PromoCode
{
	public int? Id { get; private set;} = null;
	public string Code {get; set;} = "";
	public int Value {get; set;}= 0;
	public PromoCodeDiscountType DiscountType{ get; set; }
	public PromoCodeItemType     ItemType { get; set; }
	public List<PromoCodeSomeItemType>  Items{get; set;} = new();
	public DateTime ExpirationDate{get; set;} = DateTime.Now;

	const string TABLE_NAME = "PromoCode";

	public PromoCode() {}
	public PromoCode(int id, StorageDB db) {
		 db.DB.ReadFromTable(TABLE_NAME, $"Id={id}", (r) => {
			Id = (int)r[0];
			Code = (string)r[1];
			Value = (int)r[2];
			DiscountType = (PromoCodeDiscountType)(short)r[3];
			ItemType = (PromoCodeItemType)(short)r[4];
			ExpirationDate = (DateTime)r[5];
			return 0;
		});

		if(this.ItemType == PromoCodeItemType.Some){
			Items = db.DB.ReadFromTable($"{TABLE_NAME}_{Id}", 
				(r) => new PromoCodeSomeItemType(){IsPack = r[0].ToString() == "T" ? true : false, Id = (int)r[1]});
		}
	}

	public static (bool, PromoCode?) Validate(string code, StorageDB db){
		var l = db.DB.ReadFromTable(TABLE_NAME, new string[]{"Id"}, $"Code='{code}'", (r) => (int)r[0]);
		if(l.Count == 0)
			return (false, null);
		var p = new PromoCode(l[0], db);
		if(p.ExpirationDate < DateTime.Now) 
			return (false, p);
		return (true, p); 
	}

	public void PushToDB(StorageDB db){
		if(!db.DB.CheckTable(TABLE_NAME))
			db.DB.CreateTable(TABLE_NAME, (IEnumerable<(string, SQLType)>) new(string, SQLType)[]{
				("Id",             SQLType.IntAutoIncrement),
				("Code",           SQLType.String32),
				("Value",          SQLType.Int),
				("DiscountType",   SQLType.Small),
				("ItemType",       SQLType.Small),
				("ExpirationDate", SQLType.Date)
			});

		if(Id != null){
			db.DB.UpdateTable(TABLE_NAME, new (string, object)[] {
				("Id", Id),
				("Code", Code),
				("Value", this.Value),
				("DiscountType", (int)DiscountType),
				("ItemType", (int)ItemType),
				("ExpirationDate", ExpirationDate.ToString("yyyy-MM-dd"))
			}, $"Id = {Id}");
		} else {
			db.DB.PushToTable(TABLE_NAME, new (string, object)[] {
				("Code", Code),
				("Value", this.Value),
				("DiscountType", (int)DiscountType),
				("ItemType", (int)ItemType),
				("ExpirationDate", ExpirationDate.ToString("yyyy-MM-dd"))
			});
			this.Id = db.DB.ReadFromTable($"{TABLE_NAME}", $"Code='{Code}'", (r) => (int)r[0])[0];
		}

		if(this.ItemType == PromoCodeItemType.Some){
			db.DB.CreateTable($"{TABLE_NAME}_{Id}", (IEnumerable<(string, SQLType)>)new(string,SQLType)[]{
				("IsPack", SQLType.Bool),
				("ShopModelID", SQLType.Int)
			});

			foreach(var item in Items) {
				db.DB.PushToTable($"{TABLE_NAME}_{Id}", new object[] {
					item.IsPack ? 'T' : 'F',
					item.Id
				});
			}
		}
	}

	public void DeleteFromDB(StorageDB db) {
		if(Id == null) throw new Exception("Promo Code not stored in Database");
		db.DB.RemoveRow(TABLE_NAME, "Id", $"{Id}");
		if(db.DB.CheckTable($"{TABLE_NAME}_{Id}"))
			db.DB.DeleteTable($"{TABLE_NAME}_{Id}");
	}
}

