using Microsoft.Data.Sqlite;
using tourism_api.Domain;

namespace tourism_api.Repositories;

public class TourReservationRepository
{
    private readonly string _connectionString;

    public TourReservationRepository(IConfiguration configuration)
    {
        _connectionString = configuration["ConnectionString:SQLiteConnection"];
    }

    public List<TourReservation> GetReservationsByTourId(int tourId)
    {
        List<TourReservation> reservations = new List<TourReservation>();

        try
        {
            using SqliteConnection  connection = new SqliteConnection(_connectionString);
            connection.Open();
            
            string query = "SELECT * FROM TourReservations WHERE TourId = @tourId";
            using SqliteCommand command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@tourId", tourId);
            
            using SqliteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                reservations.Add(new TourReservation
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    TourId = Convert.ToInt32(reader["TourId"]),
                    TouristId = Convert.ToInt32(reader["TouristId"]),
                    Guests = Convert.ToInt32(reader["Guests"]),
                    CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                });
            }
            return reservations;
        }
        
        catch (SqliteException ex)
        {
            Console.WriteLine($"Greška pri konekciji ili izvršavanju neispravnih SQL upita: {ex.Message}");
            throw;
        }
        catch (FormatException ex)
        {
            Console.WriteLine($"Greška u konverziji podataka iz baze: {ex.Message}");
            throw;
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Konekcija nije otvorena ili je otvorena više puta: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Neočekivana greška: {ex.Message}");
            throw;
        }
    }
    
    public TourReservation Create(TourReservation tourReservation)
    {
        try
        {
            using SqliteConnection  connection = new SqliteConnection(_connectionString);
            connection.Open();
            
            string query = @"
                   INSERT INTO TourReservations (TourId, TouristId,Guests,CreatedAt)
                   VALUES (@tourId, @touristId, @guests, @createdAt);
                   SELECT LAST_INSERT_ROWID();";
            using SqliteCommand command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@tourId", tourReservation.TourId);
            command.Parameters.AddWithValue("@touristId", tourReservation.TouristId);
            command.Parameters.AddWithValue("@guests", tourReservation.Guests);
            command.Parameters.AddWithValue("@createdAt", tourReservation.CreatedAt);
            tourReservation.Id = Convert.ToInt32(command.ExecuteScalar());
            return tourReservation;
        }
        catch (SqliteException ex)
        {
            Console.WriteLine($"Greška pri konekciji ili izvršavanju neispravnih SQL upita: {ex.Message}");
            throw;
        }
        catch (FormatException ex)
        {
            Console.WriteLine($"Greška u konverziji podataka iz baze: {ex.Message}");
            throw;
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Konekcija nije otvorena ili je otvorena više puta: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Neočekivana greška: {ex.Message}");
            throw;
        }
    }

    public bool Delete(int id)
    {
        try
        {
            using SqliteConnection connection = new SqliteConnection(_connectionString);
            connection.Open();

            string query = "DELETE FROM TourReservations WHERE Id = @Id";
            using SqliteCommand command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            int rowsAffected = command.ExecuteNonQuery();

            return rowsAffected > 0;
        }
        catch (SqliteException ex)
        {
            Console.WriteLine($"Greška pri konekciji ili izvršavanju neispravnih SQL upita: {ex.Message}");
            throw;
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Konekcija nije otvorena ili je otvorena više puta: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Neočekivana greška: {ex.Message}");
            throw;
        }
    }
}