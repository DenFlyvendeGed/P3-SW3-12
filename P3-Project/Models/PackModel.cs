using P3_Project.Models.DB;
using P3_Project.Helpers;

namespace P3_Project.Models
{

	public class PsudoPackModel {
		public int PackID {get; private set;} = 0;
		public string Name {get; private set;} = "";

        const string TABLE_NAME = "PackModel"; // If changed, it should also be changed in PackModel
		public static List<PsudoPackModel> GetAllFromDatabase(StorageDB db) {
			try
			{
				return db.DB.ReadFromTable(TABLE_NAME, new string[] { "Id", "Name" }, (r) => new PsudoPackModel()
				{
					PackID = (int)r[0],
					Name = (string)r[1]
				});
			}
			catch
			{
				return new();
			}
        }
    }
    public class PackModel
    {
        public int? PackID { get; private set; } = null;
        public string Name  { get; set; } = "";
		public int    Price { get; set; } = 0;
        public string Description {get; set; } = "";
		//                ID   ModelName
        public List<List<int>> Options { get; set; } = new();

        public List<ImageModel>? Pictures { get; set; }

        public List<Tag>? Tags { get; set; }

        public PackModel() {
			PackID = null;
		}

		public PackModel (int id, StorageDB db){
			this.PackID = id;
			var table_data = ((int, string, int, string, short))db.DB.ReadFromTable(TABLE_NAME, $"Id={this.PackID}", 
					(l) => (l[0], l[1], l[2], l[3], l[4]))[0];
			this.Name = table_data.Item2;
			this.Price = table_data.Item3;
			this.Description = table_data.Item4;

			foreach(var i in new Counter(table_data.Item5)){
				var l = new List<int>();
				var itemIds = db.DB.ReadFromTable($"{TABLE_NAME}_{this.PackID}_{i}", (e) => e[0].ToString() ?? "");
				var data = itemIds.Count != 0 
					? db.DB.ReadFromTable("ItemModels", new string[] {"Id", "ModelName"}, $"Id in ({string.Join(',', itemIds)})", (e) => (int)e[0])
					: new();
				foreach(var d in data) l.Add(d);
				this.Options.Add(l);
			}
		}


		// Push or Update /this/ in database
        const string TABLE_NAME = "PackModel"; // If changed, it should also be changed in PsudoPackModel
        public void PushToDB(StorageDB db){
			if(!db.DB.CheckTable(TABLE_NAME)) 
				db.DB.CreateTable(TABLE_NAME,(IEnumerable<(string, SQLType)>) new (string, SQLType)[] {
					("Id", SQLType.IntAutoIncrement),
					("Name", SQLType.String64),
					("Price", SQLType.Int),
					("Description", SQLType.String512),
					("NOptions", SQLType.Small)
				});
			if(PackID == null) {
				db.DB.PushToTable(TABLE_NAME,  new (string, object)[] {
					("Name", Name),
					("Price", Price),
					("Description", Description),
					("NOptions", Options.Count)
				});
				PackID = db.DB.ReadFromTable(TABLE_NAME, new string[] {"Id"}, $"Name='{Name}'", (i) => (int)i[0])[0];
			} else {
				db.DB.UpdateTable(TABLE_NAME,  new (string, object)[] {
					("Name", Name),
					("Price", Price),
					("Description", Description),
					("NOptions", Options.Count)
				}, $"Id = {PackID}");
			}


			// Create the option tables
			int i;
			for(i = 0; i < Options.Count; i++){
				if(db.DB.CheckTable($"{TABLE_NAME}_{PackID}_{i}"))
					db.DB.DeleteTable($"{TABLE_NAME}_{PackID}_{i}");
                db.DB.CreateTable($"{TABLE_NAME}_{PackID}_{i}",(IEnumerable<(string, SQLType)>) new (string, SQLType)[] {("ItemModelId", SQLType.Int)});


				foreach(var item in Options[i]) {
					db.DB.PushToTable($"{TABLE_NAME}_{PackID}_{i}", new object[] {item});
				}
			}
			// Delete tables if options were deleted in edit
			for(; db.DB.CheckTable($"{TABLE_NAME}_{this.PackID}_{i}"); i++) 
				db.DB.DeleteTable($"{TABLE_NAME}_{this.PackID}_{i}");



            if (Pictures != null)
            {
                foreach (ImageModel item in Pictures)
                {
                    if (item.Data != null)
                        item.Save((int)PackID, "PackModel");
                }
            }
            if (Tags != null)
            {
				if(!db.DB.CheckTable($"PackModel_{PackID}_Tags"))
					db.DB.CreateTable($"PackModel_{PackID}_Tags", new Tag());
                foreach (Tag tag in Tags)
                {
                    if (db.DB.CheckRow("Tags", "Id", tag.Id.ToString()))
                    {
                        Tag dbTag = db.DB.GetRow("Tags", new Tag(), tag.Id.ToString());
                        db.DB.AddRowToTable($"PackModel_{PackID}_Tags", dbTag);
                    }
                }
            }
        }
        public void LoadTags()
        {
            StorageDB db = new StorageDB();
            Tags = db.DB.GetAllElements($"PackModel_{PackID}_Tags", new Tag());
        }

        public void LoadImages()
        {
            DirectoryInfo dir = ImageModel.GetDir((int)PackID, "PackModel");

            foreach (FileInfo file in dir.GetFiles())
            {
                ImageModel img = new();
                img.Name = file.Name;
                img.Type = file.Extension;
                img.Id = PackID;
                //img.Data = Convert.ToBase64String(File.ReadAllBytes(Path.Combine(file.DirectoryName, file.Name)));
                //img.Data = $"data:image/{img.Type};base64,{img.Data}";
                img.FilePath = img.GetFilePath("PackModel");

				if (Pictures == null)
					Pictures = new List<ImageModel>();
                Pictures.Add(img);
            }
        }

        public ImageModel GetFirstImg()
        {
            DirectoryInfo dir = ImageModel.GetDir((int)PackID, "PackModel");

            foreach (FileInfo file in dir.GetFiles())
            {
                ImageModel img = new();
                img.Name = file.Name;
                img.Type = file.Extension;
                img.Id = PackID;
                //img.Data = Convert.ToBase64String(File.ReadAllBytes(Path.Combine(file.DirectoryName, file.Name)));
                //img.Data = $"data:image/{img.Type};base64,{img.Data}";
                img.FilePath = img.GetFilePath("PackModel");

                return img;
            }
            return null;
        }

        public void DeleteFromDB(StorageDB db){
			for(int i = 0; db.DB.CheckTable($"{TABLE_NAME}_{this.PackID}_{i}"); i++)
				db.DB.DeleteTable($"{TABLE_NAME}_{this.PackID}_{i}");
			db.DB.RemoveRow($"{TABLE_NAME}", "Id", (this.PackID ?? throw new Exception("Item Pack isn't saved")).ToString());

			db.DB.DeleteTable($"PackModel_{PackID}_Tags");
		}
    }
}
