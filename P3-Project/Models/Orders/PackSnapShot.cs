namespace P3_Project.Models.Orders;
using P3_Project.Models.DB;
public interface SnapShot {}
public class PackSnapShot: SnapShot{
	public int? SnapShotId{get; set;} = null;
	public string Name{get; set;} = "";
	public int PackId{get; set;} = 0;
	public int Price{ get; set; } = 0;
	public int SalesTax { get; set; } = 0;

	public PackSnapShot(){}
	public PackSnapShot(PackSnapShotDBInfo dbInfo){
		this.SnapShotId = dbInfo.SnapShotId;
		this.Name = Globals.ItemNameTable.Fetch(dbInfo.Name);
		this.Price = dbInfo.Price;
		this.SalesTax = dbInfo.SalesTax;
	}
}

public class PackSnapShotDBInfo {
	public int? SnapShotId{get; set;} = null;
	public int Name{get; set;}
	public int PackId{get; set;}
	public int Price{get; set;}
	public int SalesTax{get; set;}
}

public class PackSnapShotTable {
	string table {get; set;}
	static DataBase db = DB.Globals.StorageDB.DB;

	public PackSnapShotTable(string Table){
		this.table = Table;
		CreateTable();
	}
	
	void CreateTable() {
		if(db.CheckTable(table)) return;
		db.CreateTable(table, (IEnumerable<(string, SQLType)>) new (string, SQLType)[] {
			("SnapShotId", SQLType.IntAutoIncrement),
			("PackId", SQLType.Int),
			("Name", SQLType.Int),
			("Price", SQLType.Int),
			("SalesTax", SQLType.Int),
		});
	}
	void Push(PackSnapShot snapshot){
		int name;
		name = Globals.ItemNameTable.PushIfNone(snapshot.Name);

		db.PushToTable(table, new (string, object)[] {
			("PackId", snapshot.PackId),
			("Name", name),
			("Price", snapshot.Price),
			("SalesTax", snapshot.SalesTax),
		});
	}
	public int PushIfNone(PackSnapShot snapshot) {
		int name;
		int index = 0;
		if(snapshot.SnapShotId != null || (
			(name = Globals.ItemNameTable.IsInDB(snapshot.Name)) == -1 ||
			0 == db.ReadFromTable(table,
				new string[] {"SnapShotId"},
				$"PackId='{snapshot.PackId}' AND " +
				$"Name='{name}' AND " +
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

	public PackSnapShot Fetch(int snapShotId){
		var dbInfo = db.ReadFromTable(table, $"SnapShotId='{snapShotId}'", r => new PackSnapShotDBInfo() {
			SnapShotId = (int)r[0],
			PackId = (int)r[1],
			Name = (int)r[2],
			Price = (int)r[3],
			SalesTax = (int)r[4],
		})[0];
		return new(dbInfo);
	}

	public List<PackSnapShot> FetchAll(){
		var dbInfos = db.ReadFromTable(table, r => new PackSnapShotDBInfo() {
			SnapShotId = (int)r[0],
			PackId = (int)r[1],
			Name = (int)r[2],
			Price = (int)r[3],
			SalesTax = (int)r[4],
		});

		List<PackSnapShot> l = new();
		foreach(var dbInfo in dbInfos)
			l.Add(new (dbInfo));
		return l;
	}
}

