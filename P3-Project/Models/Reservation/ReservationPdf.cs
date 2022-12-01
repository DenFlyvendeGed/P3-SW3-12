namespace P3_Project.Models.ReservationPdf;

using System.IO;
using System.Diagnostics;

using P3_Project.Models.Orders;

public static class ReservationPdf{	
	public static readonly string COMPILE_FOLDER = System.Configuration.ConfigurationManager.AppSettings["latex-compile-folder"] 
		?? throw new Exception("AppSetting latex-compile-folder not set in App.Config");

	static LatexItemModel ParseItemsnapShot(ItemSnapshot unit, int amount) {
			return new LatexItemModel (){
				Id = $"{unit.ModelId}-{unit.ItemId}",
				Name = $"{unit.Name} | {unit.Color} {unit.Size}",
				Amount = amount,
				IndividualPrice = unit.Price,
				TotalPrice = amount * unit.Price
			};
	}

	static List<LatexReservationItem> ParseOrderItems(List<OrderShopUnit> items) {
		int i = 0;
		var ItemList = new List<LatexReservationItem>();
		while(i < items.Count){
			if(items[i].ShopUnit is ItemSnapshot){
				var unit = (ItemSnapshot)items[i].ShopUnit;
				ItemList.Add(ParseItemsnapShot(unit, items[i].Amount));
				i++;
			} else {
				var unit = (PackSnapShot)items[i].ShopUnit;
				
				var packShopUnitList = new List<LatexItemModel>();

				while(i < items.Count && !(items[i].ShopUnit is PackSnapShot)) {
					packShopUnitList.Add(ParseItemsnapShot((ItemSnapshot)items[i].ShopUnit, items[i].Amount));
					i++;
				}

				ItemList.Add(new LatexPackModel (){
					Id = $"{unit.PackId}",
					Name = $"{unit.Name}",
					Amount = items[i].Amount,
					IndividualPrice = unit.Price,
					TotalPrice = items[i].Amount * unit.Price,
				});
				i++;
			}
		}
		return ItemList;
	}
	public static async Task FromOrder(Order order){
		var ItemList = new List<LatexReservationItem>();
		var tex_file = "order.tex";
		File.WriteAllText(COMPILE_FOLDER + "/" + tex_file, 
			LATEX_GLOBALS.TEX_STRING(
				"Reservation", 
				order.Name, 
				order.ExpirationDate.ToString("dd/MM/yyyy"), 
				order.Id == null 
				? throw new Exception("Order.Id may not be null")
				: $"{order.Id}", 
				order.Price.ToString(), 
				order.SalesTax.ToString(), 
				ParseOrderItems(order.ShopUnits)
			));
		OrderQRCode.Gennerate("Hello World", COMPILE_FOLDER);	
		await CompileLatex(tex_file);
	}

	static Task<int> CompileLatex(string tex_file){
		var cur_dir = Directory.GetCurrentDirectory();
		Directory.SetCurrentDirectory(COMPILE_FOLDER);
		var process = new Process {
			StartInfo = new ProcessStartInfo(){
				FileName = "lualatex",
				Arguments = $"--interaction=nonstopmode --shell-escape {tex_file}",
				UseShellExecute = true,
			},
			EnableRaisingEvents = true
		};

		var tcs = new TaskCompletionSource<int>();
		process.Exited += (_, _) => {
			tcs.SetResult(process.ExitCode);
			process.Dispose();
			Directory.SetCurrentDirectory(cur_dir);
		};

		process.Start();

		return tcs.Task;
	} 
}

