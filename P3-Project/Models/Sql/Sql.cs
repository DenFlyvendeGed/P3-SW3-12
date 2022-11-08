namespace P3_Project.Models.DB;

using Microsoft.Data.SqlClient;
using System.Data;

using System.Collections.Generic;
using System.Reflection;

public class SqlDB : DataBase
{
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

	public List<T>  GetAllElements<T>(string tableName, T objectClass) where T : notnull
	{

		cmd.CommandText = "SELECT * FROM " + tableName;



		PropertyInfo[] Properties = objectClass.GetType().GetProperties();
		// open database connection.
		conn.Open();

		FieldInfo[] fields = objectClass.GetType().GetFields();

		List<T> list = new List<T>();

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
			list.Add(classInstance);
		}
		//Close the connection
		conn.Close();

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
				if (property.GetValue(classObject) != null && property.Name != "Id")
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

		if (CheckTable(name))
		{
			throw new Exception("Table already exist");
		}
		else
		{
			cmd.CommandText = "CREATE TABLE " + name + "(";

			

			PropertyInfo[] Properties = obj.GetType().GetProperties();

			foreach (PropertyInfo property in Properties)
			{
				try
				{
					cmd.CommandText += (string)property.Name;

					switch (property.PropertyType.Name)
					{
						case "String":
							cmd.CommandText += " varchar(255), ";
							break;

						case "Int32":
							cmd.CommandText += " int, ";
							break;
						default:
							throw new NotImplementedException();
					}
						
						
				}
				catch
				{
					Console.WriteLine(property.Name + " failed to read in storageDB");
				}
				//instance.Add(field.Name, sdr[field.Name].ToString());
			}

			if (sqlType == "mySQL")
				cmd.CommandText = cmd.CommandText.Replace("Id int,", "Id int NOT NULL AUTO_INCREMENT,  PRIMARY KEY(Id), ") ;
			else
				cmd.CommandText = cmd.CommandText.Replace("Id int,", "Id int IDENTITY(1,1) PRIMARY KEY, ");

			cmd.CommandText += ")";
			// open database connection.
			conn.Open();

			//Execute the query 
			SqlDataReader sdr = cmd.ExecuteReader();

			conn.Close();
		}
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
} 
