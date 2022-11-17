namespace P3_Project.Models.Mail;
using P3_Project.Models.DB;
using System.Collections;

public class MailList : IEnumerable<string> {
	public List<string> List {get; set;} = new();

	const string TABLE_NAME = "MailList";
	public MailList(){}
	public MailList(StorageDB db){
		this.List = db.DB.ReadFromTable(TABLE_NAME, (r) => (string)r[0]);
	}
	public void PushToDB(StorageDB db){
		if( db.DB.CheckTable(TABLE_NAME) ) db.DB.DeleteTable(TABLE_NAME);
		db.DB.CreateTable(TABLE_NAME, (IEnumerable<(string, SQLType)>) new (string, SQLType)[] { ( "Mail", SQLType.String64 ) });
		foreach(var l in this) {
			db.DB.PushToTable(TABLE_NAME, new List<object>(){l});	
		}
	}

	public IEnumerator<string> GetEnumerator() => List.GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
}
