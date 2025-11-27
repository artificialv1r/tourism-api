using Microsoft.Data.Sqlite;
using tourism_api.Domain;

namespace tourism_api.Repositories;

public class TourReviewRepository
{
    private readonly string _connectionString;

    public TourReviewRepository(IConfiguration configuration)
    {
        _connectionString = configuration["ConnectionString:SQLiteConnection"];
    }

    public List<TourReview> GetByTourId(int tourId)
    {
        List<TourReview> reviews = new List<TourReview>();

        try
        {
            using SqliteConnection  connection = new SqliteConnection(_connectionString);
            connection.Open();
            
            string query = @"SELECT * FROM TourReviews 
                            INNER JOIN TourReservations ON TourReviews.ReservationId = TourReservations.Id 
                            WHERE TourId = @tourId";
            using SqliteCommand command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@tourId", tourId);
            
            using SqliteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                reviews.Add(new TourReview
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    TouristId = Convert.ToInt32(reader["TouristId"]),
                    ReservationId = Convert.ToInt32(reader["ReservationId"]),
                    Grade = Convert.ToInt32(reader["Grade"]),
                    Comment = reader["Comment"].ToString(),
                    CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                });
            }
            return reviews;
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
    
    public TourReview GetByReservationId(int reservationId)
    {
        TourReview? review = null;
        try
        {
            using SqliteConnection  connection = new SqliteConnection(_connectionString);
            connection.Open();
            
            string query = "SELECT * FROM TourReviews WHERE ReservationId = @reservationId";
            using SqliteCommand command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@reservationId", reservationId);
            
            using SqliteDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                review = new TourReview();
                review.Id = Convert.ToInt32(reader["Id"]);
                review.TouristId = Convert.ToInt32(reader["TouristId"]);
                review.ReservationId = Convert.ToInt32(reader["ReservationId"]);
                review.Grade = Convert.ToInt32(reader["Grade"]);
                review.Comment = reader["Comment"].ToString();
                review.CreatedAt = Convert.ToDateTime(reader["CreatedAt"]);
            }

            return review;
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
    
    public TourReview Create(TourReview tourReview)
    {
        try
        {
            using SqliteConnection  connection = new SqliteConnection(_connectionString);
            connection.Open();
            
            string query = @"
                   INSERT INTO TourReviews (ReservationId,TouristId,Grade,Comment,CreatedAt)
                   VALUES (@reservationId, @touristId, @grade, @comment, @createdAt);
                   SELECT LAST_INSERT_ROWID();";
            using SqliteCommand command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@reservationId", tourReview.ReservationId);
            command.Parameters.AddWithValue("@touristId", tourReview.TouristId);
            command.Parameters.AddWithValue("@grade", tourReview.Grade);
            command.Parameters.AddWithValue("@comment", tourReview.Comment);
            command.Parameters.AddWithValue("@createdAt", tourReview.CreatedAt);
            tourReview.Id = Convert.ToInt32(command.ExecuteScalar());
            return tourReview;
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
}