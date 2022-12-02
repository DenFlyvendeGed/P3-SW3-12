namespace P3_Project.Models.Orders;

public class OrderCheckExpirationDate : BackgroundService {
	protected override async Task ExecuteAsync(CancellationToken stoppingToken){
		while(!stoppingToken.IsCancellationRequested) {
			Globals.OrderDB.CheckExpirationDate();
			await Task.Delay(new TimeSpan(0, 0, 20), stoppingToken);
		}
	}
}




