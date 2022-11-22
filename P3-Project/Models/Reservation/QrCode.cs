namespace P3_Project.Models.ReservationPdf;
using QRCoder;
public static class OrderQRCode{
	public static void Gennerate(string content, string save_dir) {
		using( QRCodeGenerator qrGennerator = new())
		using( QRCodeData qrData = qrGennerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q)){
			var ByteCode = new PngByteQRCode(qrData);
			var img = ByteCode.GetGraphic(20, new byte[] {59, 146, 178}, new byte[] {255, 255, 255}, false);
			File.WriteAllBytes(save_dir + "/qr-code.png", img);
		}
	}
}

