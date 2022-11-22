namespace P3_Project.Models.ReservationPdf;
using System.IO;
using System.Diagnostics;

public static class ReservationPdf{	
	static readonly string COMPILE_FOLDER = System.Configuration.ConfigurationManager.AppSettings["latex-compile-folder"] ?? throw new Exception("AppSetting latex-compile-folder not set in App.Config");
	public static void FromOrder(Order order){
		File.WriteAllText(COMPILE_FOLDER + "/test.tex", 
			LATEX_GLOBALS.TEX_STRING("Reservation", order.Name, order.ExpirationDate.ToString("dd/MM/yyyy"), order.Id, order.Price, order.SalesTax, new List<LatexReservationItem>()));
		OrderQRCode.Gennerate("Hello World", COMPILE_FOLDER);	
		CompileLatex("test.tex");
	}
	static void CompileLatex(string tex_file){
		var cur_dir = Directory.GetCurrentDirectory();
		Directory.SetCurrentDirectory(COMPILE_FOLDER);
		var process = Process.Start(new ProcessStartInfo(){
			FileName = "lualatex",
			Arguments = $"--interaction=nonstopmode --shell-escape {tex_file}",
			UseShellExecute = true,
		});
		Directory.SetCurrentDirectory(cur_dir);
	} 
}

