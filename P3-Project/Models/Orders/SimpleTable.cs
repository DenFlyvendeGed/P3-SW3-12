namespace P3_Project.Models.Orders;
using P3_Project.Models.DB;

public class SimpleTable<T> where T : notnull {
	private string table {get; set;} 

	public SimpleTable(string Table, SQLType dbType) {
		this.table = Table;
		CreateTable(dbType);
	}

	static DataBase db = DB.Globals.StorageDB.DB;

	void CreateTable(SQLType dbType) {
		if(db.CheckTable(table)) return;
		db.CreateTable(table, (IEnumerable<(string, SQLType)>) new (string, SQLType)[] {
			("Id", SQLType.IntAutoIncrement),
			("Value", dbType),
		});
	}

	public List<(int, T)> FetchAll () =>
		db.ReadFromTable(table, (r) => (
			(int)r[0],
			(T)r[1]
		));

	public T Fetch(int id){
		var l = db.ReadFromTable(table, new string[] {"Value"}, $"Id='{id}'", r => (T)r[0]);
		return l[0];
	}

	public void Push(T name) {
		db.PushToTable(table, new (string, object)[] { ("Value" , name.ToString() ?? throw new Exception("name may not be null")) });
	}

	public int IsInDB(T name) {
		var data = db.ReadFromTable(table, new string[] {"Id"}, $"Value='{name.ToString()}'", (r) => (int)r[0]);
		return data.Count == 0 ? -1 : data.Last(); 
	}

	public int PushIfNone(T name) {
		var i = this.IsInDB(name);	
		if(i != -1)
			return i;	
		else
			this.Push(name);
		return this.IsInDB(name);
	}
}

