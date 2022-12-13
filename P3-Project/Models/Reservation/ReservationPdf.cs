namespace P3_Project.Models.ReservationPdf;

using System.IO;
using System.Diagnostics;

using P3_Project.Models.Orders;

public static class ReservationPdf{	
	public static readonly string COMPILE_FOLDER = System.Configuration.ConfigurationManager.AppSettings["latex-compile-folder"] 
		?? throw new Exception("AppSetting latex-compile-folder not set in App.Config");
	public static readonly string ADDRESS_OF_WEBSITE = System.Configuration.ConfigurationManager.AppSettings["address-of-website"] 
		?? throw new Exception("AppSetting address-of-website not set in App.Config");

	static LatexItemModel ParseItemsnapShot(ItemSnapshot unit, int amount, int discount) {
			return new LatexItemModel (){
				Id = $"{unit.ModelId}-{unit.ItemId}",
				Name = $"{unit.Name} | {unit.Color} {unit.Size}",
				Amount = amount,
				IndividualPrice = unit.Price - discount,
				TotalPrice = amount * (unit.Price - discount)
			};
	}

	static List<LatexReservationItem> ParseOrderItems(List<OrderShopUnit> items) {
		int i = 0;
		var ItemList = new List<LatexReservationItem>();
		while(i < items.Count){
			if(items[i].ShopUnit is ItemSnapshot){
				var unit = (ItemSnapshot)items[i].ShopUnit;
				ItemList.Add(ParseItemsnapShot(unit, items[i].Amount, items[i].Discount));
				i++;
			} else {
				var unit = (PackSnapShot)items[i].ShopUnit;
				var packShopUnitList = new List<LatexItemModel>();

				i++;
				while(i < items.Count && !(items[i].ShopUnit is PackSnapShot)) {
					packShopUnitList.Add(ParseItemsnapShot((ItemSnapshot)items[i].ShopUnit, items[i].Amount, items[i].Discount));
					i++;
				}
				if (i >= items.Count) { break; }
				ItemList.Add(new LatexPackModel (){
					Id = $"{unit.PackId}",
					Name = $"{unit.Name}",
					Amount = items[i].Amount,
					IndividualPrice = unit.Price,
					TotalPrice = items[i].Amount * unit.Price,
				});
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

		OrderQRCode.Gennerate(ADDRESS_OF_WEBSITE + "/admin/confirmsale/" + order.Id, COMPILE_FOLDER);	
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

