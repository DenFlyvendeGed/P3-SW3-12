
using MySql.Data;
using MySql.Data.MySqlClient;

using System;


namespace P3_Project.Models.DB
{
	public enum SQLType{
		Small, Int, Large, IntAutoIncrement, Bool,
		Float,
		Char, String8, String16, String32, String64, String128, String256, String512,
		Date
	}


	public interface DataBase{
		string GetField(string key, string value, string table, string field);
	    IList<T> GetItems<T>(Func<IList<object>, T> initializer, string table, string? where = null);
		
		List<string> GetAllElementsField(string tableName, string key);
		public List<T> GetAllElements<T>(string tableName, T objectClass, string? whereKey = null, string? whereValue = null) where T : notnull;


        bool CheckRow(string table, string key, string value);
		bool CheckTable(string table);

		void UpdateField(string table, string selectorKey, string selectorValue, string field, string fieldValue);
		void AddRowToTable<T>(string table, T classObject) where T : notnull;
		void CreateTable<T>(string name, T obj) where T : notnull;
		void CreateTable(string name, IEnumerable<(string, SQLType)> columns);

		void PushToTable(string name, IEnumerable<object> values);
		void PushToTable(string name, IEnumerable<(string, object)> values);
	
		List<List<string>> GetSortedList(string tableName, List<string> columns, string sortkey, string sortValue);
		T GetRow<T>(string tableName, T objectClass, string id);

		List<T> ReadFromTable<T>(string name, Func<IList<object>, T> initializer) where T : notnull;
		List<T> ReadFromTable<T>(string name, string where, Func<IList<object>, T> initializer) where T : notnull;
		List<T> ReadFromTable<T>(string name, IEnumerable<string> columns, Func<IList<object>, T> initializer ) where T : notnull;
		List<T> ReadFromTable<T>(string name, IEnumerable<string> columns, string where, Func<IList<object>, T> initializer ) where T : notnull;

		void RemoveRow(string table, string key, string value);
		void DeleteTable(string Name);

		int GetStockAmount(string tableName);



    }

	public class StorageDB {
		public DataBase DB;
		public StorageDB() {
			var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["String"].ConnectionString;
			var connectionType = System.Configuration.ConfigurationManager.AppSettings["database"];
			switch(connectionType){
				case "SQL":
					DB = new SqlDB(connectionString);
					break;
				case "MySQL":
					DB = new MySqlDB(connectionString);
					break;
				default:
					throw new Exception("No database was specified in App.Config");
			}
		}
		
		public string GetItemTable(int ItemModelId){
			return DB.GetField("Id", ItemModelId.ToString(), "ItemModels", "ItemTable");
		}
	}
}
