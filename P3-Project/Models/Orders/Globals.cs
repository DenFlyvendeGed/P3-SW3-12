namespace P3_Project.Models.Orders;

public static class Globals {
	public static SimpleTable<string> CoustomerNameTable{get; private set;} = new("CustomerNameTable", DB.SQLType.String64);
	public static SimpleTable<string> EmailTable{get; private set;} = new("EmailTable", DB.SQLType.String128);
	public static SimpleTable<string> ItemNameTable{get; private set;} = new("ItemNameTable", DB.SQLType.String64);
	public static SimpleTable<string> ItemColorTable{get; private set;} = new("ItemColorTable", DB.SQLType.String16);
	public static SimpleTable<string> ItemSizeTable{get; private set;} = new("ItemSizeTable", DB.SQLType.String16);

	public static ItemSnapshotTable SnapshotTable{get; private set;} = new("ItemSnapshotTable");
	public static PackSnapShotTable PackSnapshotTable{get; private set;} = new("PackSnapshotTable");

	public static OrderDB OrderDB {get; private set;} = new("OrderDB");
}

