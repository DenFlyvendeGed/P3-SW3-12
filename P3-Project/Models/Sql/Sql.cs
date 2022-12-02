namespace P3_Project.Models.DB;

using Microsoft.Data.SqlClient;
using System.Data;

using System.Collections.Generic;
using System.Reflection;
using static System.Formats.Asn1.AsnWriter;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

public class SqlDB : DataBase
{
	static void log(string s) => Console.WriteLine($"Opening {s}");
	private SqlConnection conn = new SqlConnection();
	private SqlCommand cmd = new SqlCommand();
	private string sqlType = "SQLServer";
	public SqlDB(string connectionString)
	{
		conn.ConnectionString = connectionString;
		
		cmd.CommandType = CommandType.Text;
		cmd.Connection = conn;
	}
	public string GetField(string key, string value, string table, string field)
	{


		cmd.CommandText = "SELECT "+ field + " FROM " + table + " Where " + key + " = '" + value + "'";
		

		// open database connection.
		conn.Open();

		//Execute the query 
		SqlDataReader sdr = cmd.ExecuteReader();

		////Retrieve data from table and Display result
		string Result = "";
		while (sdr.Read())
		{
			Result = sdr[field].ToString();
		   
			
		}
		//Close the connection
		conn.Close();
		return Result;
	}
	public IList<T> GetItems<T>(Func<IList<object>, T> initializer, string table, string? where = null){
		var rtn = new List<T>();
		cmd.CommandText = "SELECT * FROM " + table + (where == null ? "" : " WHERE " + where);

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
	public bool CheckRow(string table, string key, string value)
	{

		cmd.CommandText = "SELECT "+ key +" FROM " + table + " Where " + key + " = '" + value + "'";


		// open database connection.
		conn.Open();


		bool Result = true;
		try
		{
			//Execute the query 
			SqlDataReader sdr = cmd.ExecuteReader();
			sdr.Read();
			Result = sdr.HasRows;
			//sdr[key].ToString();
		}
		catch(SqlException ex)
		{
            conn.Close();
            throw new Exception(ex.Message);
		}

		//Close the connection
		conn.Close();
		return Result;
	}
	public bool CheckTable(string table)
	{
		cmd.CommandText = "SELECT * FROM " + table;


		// open database connection.
		conn.Open();

		
		bool Result = true;
		try
		{
			//Execute the query 
			cmd.ExecuteReader();
		}
		catch
		{
			Result = false;
		}

		Console.WriteLine(Result);
		//Close the connection
		conn.Close();
		return Result;
	}

	///int currentAmount = int.Parse(GetField(selectorKey, selectorValue, table, field));



	public string GetFieldType(string table, string column)
	{
		cmd.CommandText = "SELECT DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '" + table + "' AND  COLUMN_NAME ='" + column +"'";

		// open database connection.
		conn.Open();

		//Execute the query 
		SqlDataReader sdr = cmd.ExecuteReader();

		////Retrieve data from table and Display result
		sdr.Read();
		string dataType = (string)sdr[0];

		//Close the connection
		conn.Close();
		return dataType;


	}
	public void UpdateField(string table, string selectorKey, string selectorValue, string field, string fieldValue)
	{

		if (CheckRow(table, selectorKey, selectorValue))
		{

			//switch ()
			//{

			//}

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
            conn.Close();
            throw new Exception("Row with given field dosent exist");
		}
	}

	//public void CreateItemTable(Item item)
	//{
	//    //Check if table exist
	//    if (!CheckTable(item.modelName))
	//    {
	//        if (System.Configuration.ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString.Contains("password")) 
	//        { 
	//            cmd.CommandText = "CREATE TABLE " + Name + "(" +
	//            "Id int NOT NULL AUTO_INCREMENT," +
	//            "Color varchar(255)," +
	//            "Size varchar(255)," +
	//            "ModelId Int," +
	//            "Reserved Int," +
	//            "Sold Int," +
	//            "Visible BOOL," +
	//            "RestockDate DATE," + //format YYYY-MM-DD
	//            "Stock Int)";
	//        }
	//        else
	//        {
	//            cmd.CommandText = "CREATE TABLE " + Name + "(" +
	//            "Id int NOT NULL IDENTITY(1,1) PRIMARY KEY," +
	//            "Color varchar(255)," +
	//            "Size varchar(255)," +
	//            "ModelId Int," +
	//            "Reserved Int," +
	//            "Sold Int," +
	//            "Visible bit," +
	//            "RestockDate DATE," + //format YYYY-MM-DD
	//            "Stock Int)";
	//        }

	//        // open database connection.
	//        conn.Open();

	//        //Execute the query 
	//        SqlDataReader sdr = cmd.ExecuteReader();

	//        conn.Close();
	//    }
	//    return;

	//}
	
	public int GetStockAmount(string tableName)
	{
		cmd.CommandText = "SELECT SUM(Stock) FROM " + tableName;

        // open database connection.
        conn.Open();

        //Execute the query 
        SqlDataReader sdr = cmd.ExecuteReader();

		////Retrieve data from table and Display result
		int sum = 0;
		while (sdr.Read())
        {
			sum = (int)sdr[""];


        }
        //Close the connection
        conn.Close();

		return sum;
    }

	public List<string> GetAllElementsField(string tableName, string key)
	{
		List<string> list = new List<string>();

		cmd.CommandText = "SELECT * FROM " + tableName;


		// open database connection.
		conn.Open();

		//Execute the query 
		SqlDataReader sdr = cmd.ExecuteReader();

		////Retrieve data from table and Display result
		
		while (sdr.Read())
		{
			list.Add(sdr[key].ToString());


		}
		//Close the connection
		conn.Close();

		return list;
	}

	public List<IDictionary<string, object>> getAllElements(string tableName, object objectClass)
	{

		cmd.CommandText = "SELECT * FROM " + tableName;


		FieldInfo[] myField = objectClass.GetType().GetFields();

		// open database connection.
		conn.Open();



		List<IDictionary<string, object>> objects = new List<IDictionary<string, object>>();

		//Execute the query 
		SqlDataReader sdr = cmd.ExecuteReader();

		////Retrieve data from table and Display result
		while (sdr.Read())
		{
			Dictionary<string, object> instance = new Dictionary<string, object>();
			foreach (FieldInfo field in myField)
			{
				try
				{
					instance.Add(field.Name, sdr[field.Name].ToString());
				}
				catch
				{
				}
			}
			objects.Add(instance);
		}
		//Close the connection
		conn.Close();

		return objects;
	}

	public List<T>  GetAllElements<T>(string tableName, T objectClass, string? whereKey = null, string? whereValue = null) where T : notnull
	{

		cmd.CommandText = "SELECT * FROM " + tableName;
		if(whereKey != null && whereValue != null)
		{
			cmd.CommandText += " WHERE " + whereKey + " = '" + whereValue + "'";
		}


		PropertyInfo[] Properties = objectClass.GetType().GetProperties();
		// open database connection.
		conn.Open();

		FieldInfo[] fields = objectClass.GetType().GetFields();

		List<T> list = new List<T>();

		//Execute the query 
		SqlDataReader sdr = cmd.ExecuteReader();

		////Retrieve data from table and Display result
		///
		try { 
		while (sdr.Read())
		{
			T classInstance = (T)Activator.CreateInstance(typeof(T));

			foreach (PropertyInfo property in Properties)
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
			foreach (FieldInfo field in fields)
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
			list.Add(classInstance);
		}
        }
		catch
		{
			throw new Exception("Error in reading table");
		}
		finally
		{
            //Close the connection
            conn.Close();
        }
        
		return list;
	}
	public void AddRowToTable(string table, List<(string, string)> keyValueSet)
	{

		//Id genation might need to be updated, whic affect values!!


		if (CheckTable(table))
		{
			cmd.CommandText = "INSERT INTO " + table + " (";
			string columns = "";
			string values = "";
			foreach ((string, string) set in keyValueSet)
			{
				columns += set.Item1 + ", ";
				values += "'" + set.Item2 + "', ";
			}
			columns += "itemTable";
			values += "'item" + keyValueSet[0].Item2 + "'";

			//INSERT INTO ItemModels(id, modelName, description, itemTable) VALUES(2435, 'Test shirt', 'Its a nice shirt', 'item2435')
			cmd.CommandText += columns + ") VALUES (" + values + ")";

			// open database connection.
			conn.Open();

			//Execute the query 
			cmd.ExecuteReader();

			//Close the connection
			conn.Close();
		}
		else
		{
			throw new Exception("Table dosent exist");
		}
	}

	public void AddRowToTable<T>(string table, T classObject) where T: notnull
	{

		//Id genation might need to be updated, whic affect values!!


		if (CheckTable(table))
		{
			cmd.CommandText = "INSERT INTO " + table + " (";
			string columns = "";
			string values = "";
			//FieldInfo[] fields = classObject.GetType().GetFields();
			//foreach (FieldInfo field in fields)
			//{
			//    if (!string.IsNullOrEmpty((string)field.GetValue(classObject)))
			//    {
			//        columns += field.Name + ", ";
			//        values += "'" + (string)field.GetValue(classObject) + "', ";
			//    }
				
			//}
			PropertyInfo[] Properties = classObject.GetType().GetProperties();

			foreach (PropertyInfo property in Properties)
			{
				//if (!string.IsNullOrEmpty(property.GetValue(classObject).ToString()))
				if (property.GetValue(classObject) != null && property.Name != "Id" && property.CustomAttributes.Count() == 0)
				{
					columns += property.Name + ", ";
					values += "'" + property.GetValue(classObject).ToString().Replace("'","''") + "', ";
				}
			}

					if (columns.EndsWith(", "))
				columns = columns.Remove(columns.Length - 2);

			if (values.EndsWith(", "))
				values = values.Remove(values.Length - 2);

			//string modelId = (string)Array.Find(Properties, (field) => field.Name == "modelId").GetValue(classObject);

			//INSERT INTO ItemModels(id, modelName, description, itemTable) VALUES(2435, 'Test shirt', 'Its a nice shirt', 'item2435')
			cmd.CommandText += columns + ") VALUES (" + values + ")";

			// open database connection.
			conn.Open();

			//Execute the query 
			cmd.ExecuteReader();

			//Close the connection
			conn.Close();
		}
		else
		{
			throw new Exception("Table dosent exist");
		}
	}

	public string GetItemTable(int ItemModelId)
	{
		return GetField("Id", ItemModelId.ToString(), "ItemModels", "ItemTable");
	}
	public void RemoveRow(string table, string key, string value)
	{



		if (CheckTable(table))
		{
			cmd.CommandText = "DELETE FROM " + table + " WHERE " + key + " = '" + value + "'";

			// open database connection.
			conn.Open();

			//Execute the query 
			cmd.ExecuteReader();

			//Close the connection
			conn.Close();
		}
		else
		{
			throw new Exception("Table dosent exist");
		}
	}

	public void CreateTable(string name, List<string> columns)
	{
		if (CheckTable(name))
		{
			//throw new Exception("Table already exist");
		}
		else { 
			cmd.CommandText = "CREATE TABLE " + name + "(";

			foreach(string column in columns)
			{
				cmd.CommandText += column + ", ";
			}
			cmd.CommandText += ")";
			// open database connection.
			conn.Open();

			//Execute the query 
			SqlDataReader sdr = cmd.ExecuteReader();

			conn.Close();
		}
	}

	public void CreateTable<T>(string name, T obj) where T : notnull
	{

		cmd.CommandText = Helper.CreateTableCreationQuery(this, name, obj); 
		cmd.CommandText = cmd.CommandText.Replace("Id int,", "Id int IDENTITY(1,1) PRIMARY KEY, ");

		// open database connection.
		conn.Open();

		//Execute the query 
		SqlDataReader sdr = cmd.ExecuteReader();

		conn.Close();
	}


	public void CreatePromoCode()
	{
		if (CheckTable("PromoCode"))
		{
			//add code here
		}
		else
		{
			List<string> columns = new List<string>();
			columns.Add(("test varchar(10)"));
			CreateTable("PromoCode", columns);
		}
	}

	public void DeleteTable(string Name)
	{
		cmd.CommandText = "DROP TABLE " + Name ;


		// open database connection.
		conn.Open();

		//Execute the query 
		SqlDataReader sdr = cmd.ExecuteReader();

		conn.Close();
		return;

	}
	public T GetRow<T>(string tableName, T objectClass, string id)
	{

		cmd.CommandText = "SELECT * FROM " + tableName + " WHERE Id = " + id;



		PropertyInfo[] Properties = objectClass.GetType().GetProperties();
		// open database connection.
		conn.Open();

		FieldInfo[] fields = objectClass.GetType().GetFields();



		//Execute the query 
		SqlDataReader sdr = cmd.ExecuteReader();

		////Retrieve data from table and Display result
		while (sdr.Read())
		{
			T classInstance = (T)Activator.CreateInstance(typeof(T));

			foreach (PropertyInfo property in Properties)
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
			foreach (FieldInfo field in fields)
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
		conn.Open();

		//Execute the query 
		SqlDataReader sdr = cmd.ExecuteReader();

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
	List<List<object>> RunCommand(string command){
		cmd.CommandText = command;
		var rtn = new List<List<object>>();
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
	
	public void CreateTable(string name, IEnumerable<(string, SQLType)> columns){
		if(this.CheckTable(name)) throw new Exception("Table Already exists");

		this.cmd.CommandText = $"CREATE TABLE {name}(";

		foreach(var (field, type) in columns) {
			cmd.CommandText += $"{field} ";
			switch(type){
				case SQLType.Small            : cmd.CommandText += "SMALLINT"; break;
				case SQLType.Int              : cmd.CommandText += "INT"; break;
				case SQLType.Large            : cmd.CommandText += "BIGINT"; break;
				case SQLType.IntAutoIncrement : cmd.CommandText += "INT IDENTITY(1,1) PRIMARY KEY"; break;
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
		conn.Open();
		cmd.ExecuteReader();
		conn.Close();
	}

	public void PushToTable(string name, IEnumerable<object> values){
		var l = new List<string>();
		foreach(var s in values) l.Add($"'{s.ToString()}'");
		cmd.CommandText= $"INSERT INTO {name} VALUES ({string.Join(',', l)})";

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

		conn.Open();
		cmd.ExecuteReader();
		conn.Close();
	}

	void ReadToArrayFunc<T>(SqlDataReader sdr, List<T> rtn, Func<IList<object>, T>initializer) where T : notnull{
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
		conn.Open();

		var rtn = new List<T>();
		
		var sdr = cmd.ExecuteReader();
		ReadToArrayFunc(sdr, rtn, initializer);
		conn.Close();
		return rtn;
	}
	public List<T> ReadLastFromTable<T>(string name, IEnumerable<string> columns, Func<IList<object>, T> initializer ) where T : notnull{
		cmd.CommandText = $"SELECT LAST {string.Join(',', columns )} FROM {name}";
		conn.Open();

		var rtn = new List<T>();
		
		var sdr = cmd.ExecuteReader();
		ReadToArrayFunc(sdr, rtn, initializer);
		conn.Close();
		return rtn;

	}
	public List<T> ReadLastFromTable<T>(string name, IEnumerable<string> columns, string where, Func<IList<object>, T> initializer ) where T : notnull{
		cmd.CommandText = $"SELECT LAST {string.Join(',', columns )} FROM {name} WHERE {where}";
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
} 
