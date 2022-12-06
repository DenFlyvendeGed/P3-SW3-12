namespace P3_Project.Models.DB;
using MySql.Data.MySqlClient;
using System.Data;
using System.Collections;
using Microsoft.Data.SqlClient;

public class MySqlDB : DataBase {
	MySqlConnection conn = new();
	MySqlCommand cmd = new();
	static void log(string s) => Console.WriteLine($"Opening {s}");
	public MySqlDB(string connectionString) {
		conn.ConnectionString = connectionString;
		cmd.CommandType = CommandType.Text;
		cmd.Connection = conn;
	}

	public string GetField(string key, string value, string table, string field){
		// Create Query
		cmd.CommandText = "SELECT "+ field + " FROM " + table + " Where " + key + " = '" + value + "'";

		// Open Connection
		log("GetField");
		conn.Open();

		// Exectute Query
		var sdr = cmd.ExecuteReader();

		// Retrieve data from table and display result
		string rtn = "";
		while(sdr.Read())
			rtn = sdr[field].ToString() ?? "";
		conn.Close();
		return rtn;
	}

	public IList<T> GetItems<T>(Func<IList<object>, T> initializer, string table, string? where = null){
		var rtn = new List<T>();
		cmd.CommandText = "SELECT * FROM " + table + (where == null ? "" : " WHERE " + where);

		log("GetItems");
		conn.Open();
		try {
			var sdr = cmd.ExecuteReader();
			while(sdr.Read()){
				var l = new List<object>();
				for(int i = 0; i < sdr.FieldCount; i++){
					l.Add(sdr.GetValue(i));
				}
				rtn.Add(initializer(l));
			}
		} finally {
			conn.Close();
		}
		return rtn;

	}

	public List<string> GetAllElementsField(string tableName, string key){
		var list = new List<string>();
		cmd.CommandText = "SELECT * FROM " + tableName;

		log($"GetAllElements {tableName}");
		conn.Open();
		var sdr = cmd.ExecuteReader();
		while(sdr.Read())
			list.Add(sdr[key].ToString() ?? "NULL");
		conn.Close();
		return list; 
	}
	public List<T>  GetAllElements<T>(string tableName, T objectClass, string? whereKey = null, string? whereValue = null) where T : notnull
	{

		cmd.CommandText = "SELECT * FROM " + tableName;
		if(whereKey != null && whereValue != null)
		{
            cmd.CommandText += " WHERE " + whereKey + " = '" + whereValue + "'";
        }

		var properties = objectClass.GetType().GetProperties();
		var fields = objectClass.GetType().GetFields();
		var list = new List<T>();

		log($"GetAllElements {tableName}");
		conn.Open();
		var sdr = cmd.ExecuteReader();
		try {
			while(sdr.Read()){
				T classInstance = (T)(Activator.CreateInstance(typeof(T)) ?? 
					throw new Exception("Couldn't Create Instance of " + objectClass.ToString()));
				foreach(var property in properties){
					try {
						property.SetValue(classInstance, sdr[property.Name]);
					} catch {
						Console.WriteLine(property.Name + " Can't be set");
					}
				}
				foreach(var field in fields){
					try {
						field.SetValue(classInstance, sdr[field.Name]);
					} catch {
						Console.WriteLine(field.Name + " Can't be set");
					}
				}
				list.Add(classInstance);
			}
		} catch {
			throw new Exception("Error in reading table");
		} finally {
			conn.Close();
		}
		return list;
	}
	
	public bool CheckRow(string table, string key, string value){
		cmd.CommandText = "SELECT "+ key +" FROM " + table + " Where " + key + " = '" + value + "'";
		// open database connection.
		log($"CheckRow {table}");
		conn.Open();

		bool Result = true;
		try
		{
			//Execute the query 
			var sdr = cmd.ExecuteReader();
			sdr.Read();
			Result = sdr.HasRows;
		}
		catch(MySqlException ex)
		{
            conn.Close();
            throw new Exception(ex.Message);
		}

		//Close the connection
		conn.Close();
		return Result;

	}
	public bool CheckTable(string table) {
		cmd.CommandText = "SELECT * FROM " + table;
		// open database connection.
		log($"CheckTable {table}");
		conn.Open();
		bool Result = true;
		try {
			cmd.ExecuteReader();
		}
		catch{
			Result = false;
		}

		conn.Close();
		return Result;
	}

	public void UpdateField(string table, string selectorKey, string selectorValue, string field, string fieldValue){
		if (CheckRow(table, selectorKey, selectorValue)) {
			//open database connection.
			log($"UpdateField {table}");
			conn.Open();

			cmd.CommandText = "UPDATE " + table + " Set " + field + "  = '" + fieldValue + "' Where " + selectorKey + "= " + selectorValue;

			//Execute the query 
			cmd.ExecuteReader();

			//Close the connection
			conn.Close();
		}
		else
		{
			throw new Exception("Row with given field dosent exist");
		}
	}
	public void AddRowToTable<T>(string table, T classObject) where T : notnull {
		cmd.CommandText = Helper.AddRowToTableQuryCreator(this, table, classObject);
		log($"AddRowToTable {table}");
		conn.Open();
		cmd.ExecuteReader();
		conn.Close();
	}
	public void CreateTable<T>(string name, T obj) where T : notnull{
		cmd.CommandText = Helper.CreateTableCreationQuery(this, name, obj); 
		cmd.CommandText = cmd.CommandText.Replace("Id int,", "Id int NOT NULL AUTO_INCREMENT,  PRIMARY KEY(Id),");

		Console.WriteLine(cmd.CommandText);
		log($"CreateTable {name}");
		conn.Open();
		cmd.ExecuteReader();
		conn.Close();
	}
	public void RemoveRow(string table, string key, string value){
		cmd.CommandText = CheckTable(table)
			? "DELETE FROM " + table + " WHERE " + key + " = '" + value + "'"
			: throw new Exception("Table doesn't exist");
		log($"RemoveRow {table}");
		conn.Open();
		cmd.ExecuteReader();
		conn.Close();
	}
	public void DeleteTable(string Name){
		cmd.CommandText = "DROP TABLE " + Name;
		log($"DeleteTable {Name}");
		conn.Open();
		cmd.ExecuteReader();
		conn.Close();
	}

	List<List<object>> RunCommand(string command){
		cmd.CommandText = command;
		var rtn = new List<List<object>>();
		log($"RunCommand {command}");
		conn.Open();
		var sdr = cmd.ExecuteReader();

		while(sdr.Read()){
			var l = new List<object>();
			for(int i = 0; i < sdr.FieldCount; i++)
				l.Add(sdr.GetValue(i));
			rtn.Add(l);
		}
		conn.Close();

		return rtn;
	}
	public T GetRow<T>(string tableName, T objectClass, string id)
	{

		cmd.CommandText = "SELECT * FROM " + tableName + " WHERE Id = " + id;



		var Properties = objectClass.GetType().GetProperties();
		// open database connection.
		log($"GetRow {tableName}");
		conn.Open();

		var fields = objectClass.GetType().GetFields();



		//Execute the query 
		var sdr = cmd.ExecuteReader();

		////Retrieve data from table and Display result
		while (sdr.Read())
		{
			T classInstance = (T)Activator.CreateInstance(typeof(T));

			foreach (var property in Properties)
			{
				try
				{
					property.SetValue(classInstance, sdr[property.Name]);
				}
				catch
				{
					Console.WriteLine(property.Name + " Cant be set");
				}
				//instance.Add(field.Name, sdr[field.Name].ToString());
			}
			foreach (var field in fields)
			{
				try
				{
					field.SetValue(classInstance, sdr[field.Name]);
				}
				catch
				{
					Console.WriteLine(field.Name + " Cant be set");
				}
				//instance.Add(field.Name, sdr[field.Name].ToString());
			}
			objectClass = classInstance;
		}
		//Close the connection
		conn.Close();

		return objectClass;
	}
	public List<List<string>> GetSortedList(string tableName, List<string> columns, string sortkey, string sortValue)
	{

		cmd.CommandText = "SELECT ";
		columns.ForEach(column => cmd.CommandText += column + ",");
		cmd.CommandText = cmd.CommandText.Remove(cmd.CommandText.Length - 1);
		cmd.CommandText += " FROM " + tableName + " WHERE " + sortkey + " = '" + sortValue +"'";


		// open database connection.
		log($"GetSortedList {tableName}");
		conn.Open();

		//Execute the query 
		var sdr = cmd.ExecuteReader();

		List<List<string>> Result = new List<List<string>>();
		////Retrieve data from table and Display result
		while (sdr.Read())
		{

			List<string> list = new List<string>();
			foreach (string column in columns)
			{

				list.Add(sdr[column].ToString());
			}
			Result.Add(list);
		}
		//Close the connection
		conn.Close();

		return Result;
	}

	public void CreateTable(string name, IEnumerable<(string, SQLType)> columns){
		if(this.CheckTable(name)) throw new Exception("Table Already exists");

		this.cmd.CommandText = $"CREATE TABLE {name}(";

		foreach(var (field, type) in columns) {
			cmd.CommandText += $"{field} ";
			switch(type){
				case SQLType.Small            : cmd.CommandText += "SMALLINT"; break;
				case SQLType.Int              : cmd.CommandText += "INT"; break;
				case SQLType.Large            : cmd.CommandText += "BIGINT"; break;
				case SQLType.IntAutoIncrement : cmd.CommandText += $"INT NOT NULL AUTO_INCREMENT, PRIMARY KEY({field})"; break;
				case SQLType.Bool             : cmd.CommandText += "char(1)"; break;
				case SQLType.Char             : cmd.CommandText += "char(1)"; break;
				case SQLType.String8          : cmd.CommandText += "varchar(8)"; break;
				case SQLType.String16         : cmd.CommandText += "varchar(16)"; break;
				case SQLType.String32         : cmd.CommandText += "varchar(32)"; break;
				case SQLType.String64         : cmd.CommandText += "varchar(64)"; break;
				case SQLType.String128        : cmd.CommandText += "varchar(128)"; break;
				case SQLType.String256        : cmd.CommandText += "varchar(256)"; break;
				case SQLType.String512        : cmd.CommandText += "varchar(512)"; break;
				case SQLType.Float            : cmd.CommandText += "float"; break;
				case SQLType.Date             : cmd.CommandText += "date"; break;
			}
			cmd.CommandText += ", ";
		}

		cmd.CommandText = cmd.CommandText.Remove(cmd.CommandText.Length - 2) + ")";
		log($"CreateTable {name}");
		conn.Open();
		cmd.ExecuteReader();
		conn.Close();
	}

	public void PushToTable(string name, IEnumerable<object> values){
		var l = new List<string>();
		foreach(var s in values) l.Add($"'{s.ToString()}'");
		cmd.CommandText= $"INSERT INTO {name} VALUES ({string.Join(',', l)})";
		
		Console.WriteLine(cmd.CommandText);
		log($"PushToTable {name}");
		conn.Open();
		cmd.ExecuteReader();
		conn.Close();
	}
	public void PushToTable(string name, IEnumerable<(string, object)> values){
		var field  = new List<string>();
		var vals    = new List<string>();
		foreach(var (f, v) in values){
			field.Add(f);
			vals.Add($"'{v.ToString()}'");
		};
		cmd.CommandText= $"INSERT INTO {name} ({string.Join(',', field)}) VALUES ({string.Join(',', vals)})";
		Console.WriteLine(cmd.CommandText);

		log($"PushToTable {name}");
		conn.Open();
		cmd.ExecuteReader();
		conn.Close();

	}
	public void UpdateTable(string name, IEnumerable<(string, object)> values, string where){
		var fields  = new List<string>();
		foreach(var (f, v) in values){
			fields.Add($"{f} = '{v.ToString()}'");
		};

		cmd.CommandText = $"UPDATE {name} SET {string.Join(',', fields)} WHERE {where}";

		log($"UpdateTable {name}");
		conn.Open();
		cmd.ExecuteReader();
		conn.Close();
	}


	void ReadToArrayFunc<T>(MySqlDataReader sdr, List<T> rtn, Func<IList<object>, T>initializer) where T : notnull{
		while(sdr.Read()){
			var l = new List<object>();
			for(int i = 0; i < sdr.FieldCount; i++){
				l.Add(sdr.GetValue(i));
			}
			rtn.Add(initializer(l));
		}
	}

	public List<T> ReadFromTable<T>(string name, Func<IList<object>, T> initializer) where T : notnull{
		cmd.CommandText = $"SELECT * FROM {name}";
		log($"ReadFromTable {name}");
		conn.Open();

		var rtn = new List<T>();
		
		Console.WriteLine(cmd.CommandText);
		try {
			var sdr = cmd.ExecuteReader();
			ReadToArrayFunc(sdr, rtn, initializer);
		} finally {
			conn.Close();
		}
		return rtn;
	}
	public List<T> ReadFromTable<T>(string name, string where, Func<IList<object>, T> initializer) where T : notnull{
		cmd.CommandText = $"SELECT * FROM {name} WHERE {where}";
		log($"ReadFromTable {name}");
		conn.Open();

		var rtn = new List<T>();
		
		Console.WriteLine(cmd.CommandText);
		try {
			var sdr = cmd.ExecuteReader();
			ReadToArrayFunc(sdr, rtn, initializer);
		} finally {
			conn.Close();
		}
		return rtn;
	}
	public List<T> ReadFromTable<T>(string name, IEnumerable<string> columns, Func<IList<object>, T> initializer ) where T : notnull{
		cmd.CommandText = $"SELECT {string.Join(',', columns )} FROM {name}";

		log($"ReadFromTable {name}");

		Console.WriteLine(cmd.CommandText);

		conn.Open();

		var rtn = new List<T>();
		
		try {
			var sdr = cmd.ExecuteReader();
			ReadToArrayFunc(sdr, rtn, initializer);
		} finally {
			conn.Close();
		}
		return rtn;

	}
	public List<T> ReadFromTable<T>(string name, IEnumerable<string> columns, string where, Func<IList<object>, T> initializer ) where T : notnull{
		cmd.CommandText = $"SELECT {string.Join(',', columns )} FROM {name} WHERE {where}";
		log($"ReadFromTable {name}");

		Console.WriteLine(cmd.CommandText);
		conn.Open();

		var rtn = new List<T>();
		try {
			var sdr = cmd.ExecuteReader();
			ReadToArrayFunc(sdr, rtn, initializer);
		} finally {	
			conn.Close();
		}
		return rtn;
	}
	public List<T> ReadLastFromTable<T>(string name, Func<IList<object>, T> initializer) where T : notnull{
		cmd.CommandText = $"SELECT LAST * FROM {name}";
		Console.WriteLine(cmd.CommandText);
		conn.Open();

		var rtn = new List<T>();
		
		Console.WriteLine(cmd.CommandText);
		try {
			var sdr = cmd.ExecuteReader();
			ReadToArrayFunc(sdr, rtn, initializer);
		} finally {
			conn.Close();
		}
		return rtn;
	}
	public List<T> ReadLastFromTable<T>(string name, string where, Func<IList<object>, T> initializer) where T : notnull{
		cmd.CommandText = $"SELECT LAST * FROM {name} WHERE {where}";
		Console.WriteLine(cmd.CommandText);
		conn.Open();

		var rtn = new List<T>();
		
		var sdr = cmd.ExecuteReader();
		ReadToArrayFunc(sdr, rtn, initializer);
		conn.Close();
		return rtn;
	}
	public List<T> ReadLastFromTable<T>(string name, IEnumerable<string> columns, Func<IList<object>, T> initializer ) where T : notnull{
		cmd.CommandText = $"SELECT LAST {string.Join(',', columns )} FROM {name}";
		Console.WriteLine(cmd.CommandText);
		conn.Open();

		var rtn = new List<T>();
		
		var sdr = cmd.ExecuteReader();
		ReadToArrayFunc(sdr, rtn, initializer);
		conn.Close();
		return rtn;

	}
	public List<T> ReadLastFromTable<T>(string name, IEnumerable<string> columns, string where, Func<IList<object>, T> initializer ) where T : notnull{
		cmd.CommandText = $"SELECT LAST {string.Join(',', columns )} FROM {name} WHERE {where}";
		Console.WriteLine(cmd.CommandText);
		conn.Open();

		var rtn = new List<T>();
		try {
			var sdr = cmd.ExecuteReader();
			ReadToArrayFunc(sdr, rtn, initializer);
		} finally {	
			conn.Close();
		}
		return rtn;
	}
	
	
	
    public int GetStockAmount(string tableName)
    {
        cmd.CommandText = "SELECT SUM(Stock) FROM " + tableName;

        // open database connection.
		log($"GetStockAmount {tableName}");
        conn.Open();

        //Execute the query 
        MySqlDataReader sdr = cmd.ExecuteReader();

        ////Retrieve data from table and Display result
        int sum = 0;
        while (sdr.Read())
        {
            sum = (int)(decimal)sdr["SUM(Stock)"];
        }
        //Close the connection
        conn.Close();

        return sum;
    }
}

