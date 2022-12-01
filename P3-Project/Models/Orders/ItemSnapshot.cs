namespace P3_Project.Models.Orders;
using P3_Project.Models.DB;
/*
## The Item-Snapshot-table

When ever a new item is bought it is checked to see if it can be found in the table, if not then it shall be added

| Field | ID                    | Name                                | Price | Sales Tax | Last Edit Date |
| :---  | :---                  | :---                                | :---  | :---      | :---           |
| Type  | Int                   | Int *Refers to The Item Name Table* | Int   | Int       | Date           |

*/

public class ItemSnapshot: SnapShot{
	public int? SnapShotId {get; private set; } = null;

	public int ModelId {get; set;} = 0;
	public int ItemId { get; set; } = 0;

	public string Name { get; set; } = "";
	public string Color { get; set; } = "";
	public string Size { get; set; } = "";
	public int Price { get; set; } = 0;
	public int SalesTax { get; set; } = 0;

	public ItemSnapshot () {}
	public ItemSnapshot (ItemSnapshotDBInfo dbInfo){
		SnapShotId = dbInfo.SnapShotId;
		ModelId = dbInfo.ModelId;
		ItemId = dbInfo.ItemId;

		Name = Globals.ItemNameTable.Fetch(dbInfo.Color);
		Color = Globals.ItemColorTable.Fetch(dbInfo.Color);
		Size = Globals.ItemSizeTable.Fetch(dbInfo.Size);

		Price = dbInfo.Price;
		SalesTax = dbInfo.SalesTax;
	}
}

public class ItemSnapshotDBInfo{
	public int SnapShotId {get; set; }

	public int ModelId { get;set; }
	public int ItemId { get; set; }
	
	public int Name {get; set; }
	public int Color {get; set; }
	public int Size {get; set; }
	public int Price {get; set; }
	public int SalesTax {get; set; }
}

public class ItemSnapshotTable {
	string table {get; set;}
	static DataBase db = DB.Globals.StorageDB.DB;

	public ItemSnapshotTable(string Table){
		this.table = Table;
		CreateTable();
	}
	
	void CreateTable() {
		if(db.CheckTable(table)) return;
		db.CreateTable(table, (IEnumerable<(string, SQLType)>) new (string, SQLType)[] {
			("SnapShotId", SQLType.IntAutoIncrement),
			("ModelId", SQLType.Int),
			("ItemId", SQLType.Int),
			("Name", SQLType.Int),
			("Color", SQLType.Int),
			("Size", SQLType.Int),
			("Price", SQLType.Int),
			("SalesTax", SQLType.Int),
		});
	}
	void Push(ItemSnapshot snapshot){
		int name, color, size;
		name = Globals.ItemNameTable.PushIfNone(snapshot.Name);
		color = Globals.ItemColorTable.PushIfNone(snapshot.Color);
		size = Globals.ItemSizeTable.PushIfNone(snapshot.Size);

		db.PushToTable(table, new (string, object)[] {
			("ModelId", snapshot.ModelId),
			("ItemId", snapshot.ItemId),
			("Name", name),
			("Color", color),
			("Size", size),
			("Price", snapshot.Price),
			("SalesTax", snapshot.SalesTax),
		});
	}
	public int PushIfNone(ItemSnapshot snapshot) {
		int name, color, size;
		int index = 0;
		if(snapshot.SnapShotId != null || (
			(name = Globals.ItemNameTable.IsInDB(snapshot.Name)) == -1 ||
			(color= Globals.ItemColorTable.IsInDB(snapshot.Color)) == -1 ||
			(size = Globals.ItemSizeTable.IsInDB(snapshot.Size)) == -1 ||
			0 == db.ReadFromTable(table,
				new string[] {"SnapShotId"},
				$"ModelId='{snapshot.ModelId}' AND " +
				$"ItemId='{snapshot.ItemId}' AND " +
				$"Name='{name}' AND " +
				$"Color='{color}' AND " +
				$"Size='{size}' AND " +
				$"Price='{snapshot.Price}' AND " +
				$"SalesTax='{snapshot.SalesTax}'",
				e => {index = (int)e[0]; return 0;}
			).Count)
		){
			Push(snapshot);
			return db.ReadFromTable(table, new[]{"SnapShotId"}, r => (int)r[0]).Last();
		} else {
			return index;
		}
	}

	public ItemSnapshot Fetch(int snapShotId){
		var dbInfo = db.ReadFromTable(table, $"SnapShotId='{snapShotId}'", r => new ItemSnapshotDBInfo() {
			SnapShotId = (int)r[0],
			ModelId = (int)r[1],
			ItemId = (int)r[2],
			Name = (int)r[3],
			Color = (int)r[4],
			Size = (int)r[5],
			Price = (int)r[6],
			SalesTax = (int)r[7],
		})[0];
		return new(dbInfo);
	}

	public List<ItemSnapshot> FetchAll(){
		var dbInfos = db.ReadFromTable(table, r => new ItemSnapshotDBInfo() {
			SnapShotId = (int)r[0],
			ModelId = (int)r[1],
			ItemId = (int)r[2],
			Name = (int)r[3],
			Color = (int)r[4],
			Size = (int)r[5],
			Price = (int)r[6],
			SalesTax = (int)r[7],
		});

		List<ItemSnapshot> l = new();
		foreach(var dbInfo in dbInfos)
			l.Add(new (dbInfo));
		return l;
	}
}

