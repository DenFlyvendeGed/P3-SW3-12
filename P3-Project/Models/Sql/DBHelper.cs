namespace P3_Project.Models.DB;

public class Helper {
	public static string CreateTableCreationQuery<T>(DataBase db, string name, T obj) where T : notnull {
		var cmd = !db.CheckTable(name) 
			? "CREATE TABLE " + name + "(" 
			: throw new Exception("Table Already exists");

		var properties = obj.GetType().GetProperties();
		int i = 0;

		// If empty just end
		if(properties.Count() == 0)
			goto TailOfLoop;

		// Start loop, it is not empty so first field is good and don't write a comma
		goto BeginLoop;
		HeadOfLoop:
			// If no more elements don't write comma and quit
			if(i == properties.Count()){
				goto TailOfLoop;
			}
			cmd += ", ";
		BeginLoop:
			var property = properties[i++];
			try {
				switch(property.PropertyType.Name){
					case "String":
						cmd += (string)property.Name;
						cmd += " ";
						cmd += "varchar(255)";
						break;
					case "Int32":
						cmd += (string)property.Name;
						cmd += " ";
						cmd += "int";
						break;
					case "DateTime":
						cmd += (string)property.Name;
						cmd += " ";
						cmd += "date";
						break;
					default:
						throw new NotImplementedException();
				}
			}
			catch{Console.WriteLine(property.Name + " Failed to read in storageDB");}
			goto HeadOfLoop;
		TailOfLoop:
		return cmd + ");";
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
			if(val != null && property.Name != "Id") {
				columns += property.Name + ", ";
				string value = "";
				switch(property.Name){
					case "DateTime": value = ((DateTime)val).ToString("yyyy-MM-dd"); break;
					default: value = val.ToString(); break;
				}
				values += "'" + value.Replace("'", "''") + "', ";
			}
		}

		cmd += columns.Remove(columns.Length - 2) + ") VALUES (" 
			 + values .Remove(values .Length - 2) + ")";

		return cmd;
	}
}

