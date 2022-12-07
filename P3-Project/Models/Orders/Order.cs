namespace P3_Project.Models.Orders;
using P3_Project.Models.DB;

public class OrderShopUnit
{
    public SnapShot ShopUnit { get; set; }
    public int Amount { get; set; } = 0;
    public int Discount { get; set; } = 0;

    public OrderShopUnit(SnapShot ShopUnit) => this.ShopUnit = ShopUnit;
}

public class Order
{
    public int? Id { get; set; } = null;
    public int Price { get; set; } = 0;
    public int SalesTax { get; set; } = 0;
    public DateTime ExpirationDate { get; set; } = DateTime.Now.AddDays(3);
    public bool IsActive { get; set; } = true;
    public bool IsPaid { get; set; } = false;
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public List<OrderShopUnit> ShopUnits { get; set; } = new();
}

public class OrderDBInfo
{
    public int? Id { get; private set; } = null;
    public int Price { get; set; } = 0;
    public int SalesTax { get; set; } = 0;
    public DateTime ExpirationDate { get; set; } = DateTime.Now;
    public string IsActive { get; set; } = "";
    public string IsPaid { get; set; } = "";
    public int Name { get; set; } = 0;
    public int Email { get; set; } = 0;
}

public class OrderDB
{
    static DataBase db = DB.Globals.StorageDB.DB;
    public string table { get; private set; }

    public OrderDB(string table)
    {
        this.table = table;
        CreateTable();
    }

    public void CreateTable()
    {
        if (!db.CheckTable(table))
            db.CreateTable(table, (IEnumerable<(string, SQLType)>)new (string, SQLType)[] {
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
		int name = 0, email = 0;
		var order = db.ReadFromTable(table, $"Id='{id}'", (r) => {
			name = (int)r[6];
			email = (int)r[7];
			return new Order() {
				Id = (int)r[0],
				Price = (int)r[1],
				SalesTax = (int)r[2],
				ExpirationDate = (DateTime)r[3],
				IsActive = (string)r[4] == "T" ? true : false,
				IsPaid = (string)r[5] == "T" ? true : false,
			};
		}).Last();

		order.Name = Globals.CoustomerNameTable.Fetch(name);
		order.Email = Globals.EmailTable.Fetch(email);
        var unitTable = $"{table}_{id}";

        foreach (var (unit_id, isPack, amount, discount) in
                db.ReadFromTable(unitTable, r => ((int)r[0], (string)r[1] == "T", (int)r[2], (int)r[3])))
        {
            order.ShopUnits.Add(new(isPack ? Globals.PackSnapshotTable.Fetch(unit_id) : Globals.SnapshotTable.Fetch(unit_id))
            {
                Discount = discount,
				        Amount = amount
            });
        }
        return order;
    }
  
    public void PushReserve(Order order)
    {
        if (order.Id != null) return;
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

        order.Id = id;
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

				var item = db.GetRow("Item" + i.ModelId, new Item(), i.ItemId.ToString());
				item.ChangeReservated(s.Amount);
				item.NotifyStockAlarm(i.ModelId);

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

	public void UpdateDBStock(int id){
		var order = this.Fetch(id);
		foreach(var shopUnit in order.ShopUnits){
			if(shopUnit.ShopUnit is ItemSnapshot){
				Console.WriteLine(shopUnit.Amount);
				var unit = (ItemSnapshot)shopUnit.ShopUnit;
				var item = db.GetRow("Item" + unit.ModelId, new Item(), unit.ItemId.ToString());
				item.ChangeStock(-shopUnit.Amount);
				item.ChangeReservated(-shopUnit.Amount);
				item.ChangeSold(shopUnit.Amount);
			}
		}
	}

	public void MarkAsPaid(int id) {
		db.UpdateTable(table, new(string, object)[]{("IsPaid", 'T')}, $"Id='{id}'");
		db.UpdateTable(table, new(string, object)[]{("IsActive", 'F')}, $"Id='{id}'");
		UpdateDBStock(id);
	}
	
	public void MarkAsUnpaid(int id) {
		db.UpdateTable(table, new(string, object)[]{("IsPaid", 'F')}, $"Id='{id}'");
	}

	public void Cancel(int id) {
		var order = Fetch(id);
		db.UpdateTable(table, new(string, object)[]{("IsActive", 'F')}, $"Id={id}");
		foreach(var unit in order.ShopUnits){
			if(unit.ShopUnit is ItemSnapshot){
				var snapshot = (ItemSnapshot)unit.ShopUnit;
				var item = db.GetRow("Item" + snapshot.ModelId, new Item(), snapshot.ItemId.ToString());
				item.ChangeReservated(-unit.Amount);
			}
		} 
	}

	public void CheckExpirationDate() {
		foreach(var (id, expiration_date) in 
			db.ReadFromTable(table, new string[]{"Id", "ExpirationDate"}, "IsActive='T'", r => ((int)r[0], (DateTime)r[1]))){
			if(expiration_date < DateTime.Now){
				Cancel(id);
				Console.WriteLine($"{id} Is no longer active");
			}
			else{
				Console.WriteLine($"{id} Is sill active");
			}
		}
	}

}

