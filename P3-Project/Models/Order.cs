using P3_Project.Models.DB;

public class Order{
	public int? Id {get; private set;} = null;
	public int Price {get; set;} = 0;
	public int SalesTax {get; set;} = 0;
	public DateTime ExpirationDate {get; set;} = DateTime.Now.AddDays(3);
	public bool IsPaied{ get; set;} = false;
	public string Name { get; set;} = "";
	public string Email {get; set;} = "";
	//public List<(int, int)> ShopUnits {get; set;} = new();

	const string TABLE_NAME = "Orders";

	public Order(){
	}
	public Order(int id, StorageDB db){}

	public void PushToDB(StorageDB db){}
	public void DeleteFromDB(StorageDB db){} 
}

