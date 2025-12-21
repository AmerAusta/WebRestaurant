using BusinessLayer;
using DataAccessLayer;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RestaurantWebApi.Helper
{
    public class ReservationStatusUpdater : BackgroundService
    {
        private readonly ILogger<ReservationStatusUpdater> _logger;
        private readonly IConfiguration _configuration;

        public ReservationStatusUpdater(ILogger<ReservationStatusUpdater> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        private DateTime _lastDelete = DateTime.MinValue; 
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var connectionString = "Server=localhost;Database=RestaurantWebDB;User Id=sa;Password=sa123456;Encrypt=False;TrustServerCertificate=True;Connection Timeout=30";

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var conn = new SqlConnection(connectionString);
                    await conn.OpenAsync(stoppingToken);

                    using (var cmd1 = conn.CreateCommand())
                    {
                        cmd1.CommandText = @"
                         UPDATE Reservations
                         SET StatusId = 4
                         WHERE StatusId = 1 AND ReservationDate <= GETDATE();";
                        var affected1 = await cmd1.ExecuteNonQueryAsync(stoppingToken);
                        _logger.LogInformation("Set Active. Rows affected: {Count}", affected1);
                    }

                    using (var cmd2 = conn.CreateCommand())
                    {
                        cmd2.CommandText = @"
                        UPDATE Reservations
                        SET StatusId = 3
                        WHERE StatusId = 4 AND DATEADD(HOUR, 1, ReservationDate) <= GETDATE();";
                        var affected2 = await cmd2.ExecuteNonQueryAsync(stoppingToken);
                        _logger.LogInformation("Set Complet (expired). Rows affected: {Count}", affected2);
                    }

                    
                    using (var cmd3 = conn.CreateCommand())
                    {
                        cmd3.CommandText = @"
                          update Orders
                          set StatusId=3
                          where ExpiryDate< GETDATE() and StatusId=1;";

                        var affected = await cmd3.ExecuteNonQueryAsync(stoppingToken);
                        _logger.LogInformation("Update Order Status: {Count}", affected);
                    }

                        
                    

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while updating reservation status");
                }

                await Task.Delay(TimeSpan.FromMinutes(2), stoppingToken);


               
            }
        }

        
    }
}