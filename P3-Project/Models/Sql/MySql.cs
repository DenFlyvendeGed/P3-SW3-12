namespace P3_Project.Models.DB;
using MySql.Data.MySqlClient;
using System.Data;

public class MySqlDB : DataBase {
	MySqlConnection conn = new();
	MySqlCommand cmd = new();
	public MySqlDB(string connectionString) {
		conn.ConnectionString = connectionString;
		cmd.CommandType = CommandType.Text;
		cmd.Connection = conn;
	}

	public string GetField(string key, string value, string table, string field){
		// Create Query
		cmd.CommandText = "SELECT "+ field + " FROM " + table + " Where " + key + " = '" + value + "'";

		// Open Connection
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

	public List<string> GetAllElementsField(string tableName, string key){
		var list = new List<string>();
		cmd.CommandText = "SELECT * FROM " + tableName;

		conn.Open();
		var sdr = cmd.ExecuteReader();
		while(sdr.Read())
			list.Add(sdr[key].ToString() ?? "NULL");
		conn.Close();
		return list; 
	}
	public List<T> GetAllElements<T>(string tableName, T objectClass) where T: notnull{
		cmd.CommandText = "SELECT * FROM " + tableName;

		var properties = objectClass.GetType().GetProperties();
		var fields = objectClass.GetType().GetFields();
		var list = new List<T>();

		conn.Open();
		var sdr = cmd.ExecuteReader();
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
		conn.Close();
		return list;
	}
	
	public bool CheckRow(string table, string key, string value){
		cmd.CommandText = "SELECT "+ key +" FROM " + table + " Where " + key + " = '" + value + "'";
		// open database connection.
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
			throw new Exception(ex.Message);
		}

		//Close the connection
		conn.Close();
		return Result;

	}
	public bool CheckTable(string table) {
		cmd.CommandText = "SELECT * FROM " + table;
		// open database connection.
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
		conn.Open();
		cmd.ExecuteReader();
		conn.Close();
	}
	public void CreateTable<T>(string name, T obj) where T : notnull{
		cmd.CommandText = Helper.CreateTableCreationQuery(this, name, obj); 
		cmd.CommandText = cmd.CommandText.Replace("Id int,", "Id int NOT NULL AUTO_INCREMENT,  PRIMARY KEY(Id),");

		conn.Open();
		cmd.ExecuteReader();
		conn.Close();
	}
	public void RemoveRow(string table, string key, string value){
		cmd.CommandText = CheckTable(table)
			? "DELETE FROM " + table + " WHERE " + key + " = '" + value + "'"
			: throw new Exception("Table doesn't exist");
		conn.Open();
		cmd.ExecuteReader();
		conn.Close();
	}
	public void DeleteTable(string Name){
		cmd.CommandText = "DROP TABLE " + Name;
		conn.Open();
		cmd.ExecuteReader();
		conn.Close();
	}
}

