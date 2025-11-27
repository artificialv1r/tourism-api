namespace tourism_api.Domain;

public class TourReview
{
    public int Id { get; set; }
    public int TouristId { get; set; }
    public int ReservationId { get; set; }
    public int Grade { get; set; }
    
    public string? Comment { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public bool IsValid()
    {
        return Grade >= 1 && Grade <=5;
    }
}