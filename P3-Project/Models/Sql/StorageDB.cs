
using MySql.Data;
using MySql.Data.MySqlClient;

using System;


namespace P3_Project.Models.DB
{
	public interface DataBase{
		string GetField(string key, string value, string table, string field);
		
		List<string> GetAllElementsField(string tableName, string key);
		List<T>      GetAllElements<T>(string tableName, T objectClass) where T: notnull;
		
		bool CheckRow(string table, string key, string value);
		bool CheckTable(string table);

		void UpdateField(string table, string selectorKey, string selectorValue, string field, string fieldValue);
		void AddRowToTable<T>(string table, T classObject) where T : notnull;
		void CreateTable<T>(string name, T obj) where T : notnull;
		void RemoveRow(string table, string key, string value);
		void DeleteTable(string Name);
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

		public void CreatePromoCode() {

		}
	}
}
