namespace P3_Project.Models.DB;

public class Helper {
	public static string CreateTableCreationQuery<T>(DataBase db, string name, T obj) where T : notnull {
		var cmd = !db.CheckTable(name) 
			? "CREATE TABLE " + name + "(" 
			: throw new Exception("Table Already exists");

		var properties = obj.GetType().GetProperties();

		for (int i = 0; i < properties.Count(); i++){
			var property = properties[i];
			string type;
			try {
				switch(property.PropertyType.Name){
					case "String":
						type = "varchar(255)";
						break;
					case "Int32":
						type = "int";
						break;
					case "DateTime":
						type = "date";
						break;
					default:
						continue;
				}
				cmd += (string)property.Name + " " + type + ", ";
			}
			catch{Console.WriteLine(property.Name + " Failed to read in storageDB");}
		}
		return cmd.Remove(cmd.Length - 2) + ");";
	}

	public static string AddRowToTableQuryCreator<T>(DataBase db, string table, T classObjet) where T: notnull{
		var cmd = db.CheckTable(table) 
			? "INSERT INTO " + table + " ("
			: throw new Exception("Table doesn't exists");
		
		var properties = classObjet.GetType().GetProperties();
		var columns = "";
		var values = "";

		foreach(var property in properties) {
			var val = property.GetValue(classObjet);
			if(val != null && property.Name != "Id" && property.CustomAttributes.Count() == 0) {
				columns += property.Name + ", ";
				string value = "";
				switch(property.Name){
					case "DateTime": value = ((DateTime)val).ToString("yyyy-MM-dd"); break;
					default: value = val.ToString() ?? ""; break;
				}
				values += "'" + value.Replace("'", "''") + "', ";
			}
		}

		cmd += columns.Remove(columns.Length - 2) + ") VALUES (" 
			 + values .Remove(values .Length - 2) + ")";

		return cmd;
	}
}

