namespace P3_Project.Models.Orders;
using P3_Project.Models.DB;

public class OrderShopUnit {
	public SnapShot ShopUnit {get; set;}
	public int Amount {get; set; } = 0;
	public int Discount {get; set;} = 0;

	public OrderShopUnit(SnapShot ShopUnit) => this.ShopUnit = ShopUnit;
}

public class Order{
	public int? Id {get; private set;} = null;
	public int Price {get; set;} = 0;
	public int SalesTax {get; set;} = 0;
	public DateTime ExpirationDate {get; set;} = DateTime.Now.AddDays(3);
	public bool IsActive{get; set;} = true;
	public bool IsPaid {get; set;} = false;
	public string Name  {get; set;} = "";
	public string Email {get; set;} = "";
	public List<OrderShopUnit> ShopUnits {get; set;} = new();
}

public class OrderDBInfo{
	public int? Id {get; private set;} = null;
	public int Price {get; set;} = 0;
	public int SalesTax {get; set;} = 0;
	public DateTime ExpirationDate {get; set;} = DateTime.Now;
	public string IsActive{get; set;} = "";
	public string IsPaid {get; set;} = "";
	public int Name  {get; set;} = 0;
	public int Email {get; set;} = 0;
}

public class OrderDB{
	static DataBase db = DB.Globals.StorageDB.DB;
	public string table{get; private set;}

	public OrderDB(string table){
		this.table = table;
		CreateTable();
	}

	public void CreateTable(){
		if(!db.CheckTable(table))
			db.CreateTable(table, (IEnumerable<(string, SQLType)>) new (string, SQLType) [] {
				("Id", SQLType.IntAutoIncrement),
				("Price", SQLType.Int),
				("SalesTax", SQLType.Int),
				("ExpirationDate", SQLType.Date),
				("IsActive", SQLType.Char),
				("IsPaid", SQLType.Char),
				("Name", SQLType.Int),
				("Email", SQLType.Int),
			});
	}

	public Order Fetch(int id){
		return new();
	}

	public void Push(Order order) {
		var name = Globals.CoustomerNameTable.PushIfNone(order.Name);
		var email = Globals.EmailTable.PushIfNone(order.Email);

		db.PushToTable(table, new (string, object)[] {
			("Price", order.Price),
			("SalesTax", order.SalesTax),
			("ExpirationDate", order.ExpirationDate.ToString("yyyy-MM-dd")),
			("IsActive", order.IsActive ? 'T' : 'F'),
			("IsPaid", order.IsPaid ? 'T' : 'F'),
			("Name", name),
			("Email", email),
		});

		var id = db.ReadFromTable(table, $"name='{name}' AND email='{email}'", e => (int)e[0]).Last();

		var unitsTable = $"{table}_{id}";

		db.CreateTable(unitsTable, (IEnumerable<(string, SQLType)>) new (string, SQLType) [] {
			("ItemId", SQLType.Int),
			("IsPack", SQLType.Char),
			("Amount", SQLType.Int),
			("Discount", SQLType.Int),
		});

		foreach(var s in order.ShopUnits){
			if(s.ShopUnit is ItemSnapshot){
				var i = (ItemSnapshot)s.ShopUnit;
				var snapshotId = Globals.SnapshotTable.PushIfNone(i);
				db.PushToTable(unitsTable, new object[] {
					snapshotId,
					'F',
					s.Amount,
					s.Discount,
				});
			} else {
				var i = (PackSnapShot)s.ShopUnit;
				var snapshotId = Globals.PackSnapshotTable.PushIfNone(i);
				db.PushToTable(unitsTable, new object[] {
					snapshotId,
					'T',
					s.Amount,
					s.Discount,
				});
			}
		}
	}
}
