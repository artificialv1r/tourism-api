using Microsoft.AspNetCore.Mvc;
using tourism_api.Domain;
using tourism_api.Repositories;

namespace tourism_api.Controllers;
[Route("api/tours/{tourId}/reviews")]
[ApiController]

public class TourReviewController: ControllerBase
{
    private readonly TourReviewRepository _tourReviewRepository;
    private readonly TourReservationRepository _tourReservationRepository;
    private readonly UserRepository _userRepository;
    private readonly TourRepository _tourRepository;

    public TourReviewController(IConfiguration configuration)
    {
        _tourReviewRepository = new TourReviewRepository(configuration);
        _tourReservationRepository = new TourReservationRepository(configuration);
        _userRepository = new UserRepository(configuration);
        _tourRepository = new TourRepository(configuration);
    }

    [HttpGet]
    public ActionResult GetByTourId(int tourId)
    {
        try
        {
            List<TourReview> reviews = _tourReviewRepository.GetByTourId(tourId);
            return Ok(reviews);
        }
        catch (Exception e)
        {
            return Problem("An error occurred while fetching the reviews." + e.Message);
        }
    }
    
    [HttpPost]
    public ActionResult<TourReview> Create(int tourId, [FromBody] TourReview newTourReview)
    {
        if (!newTourReview.IsValid())
        {
            return BadRequest("Invalid reservation data");
        }

        Tour selecetedTour = _tourRepository.GetById(tourId);
        
        if (selecetedTour == null)
        {
            return NotFound();
        }

        TourReservation selectedTourReservation = _tourReservationRepository.GetById(newTourReview.ReservationId);
        if (selectedTourReservation == null)
        {
            return NotFound();
        }

        if (selectedTourReservation.TouristId != newTourReview.TouristId)
        {
            return BadRequest();
        }
        
        if (selecetedTour.DateTime.AddHours(3) > newTourReview.CreatedAt ||
            selecetedTour.DateTime.AddDays(7) < newTourReview.CreatedAt)
        {
            return BadRequest("Leaving review is not permitted outside the allowed time window (3 hours to 7 days after the tour start.");
        }

        TourReview selecetedTourReview = _tourReviewRepository.GetByReservationId(newTourReview.ReservationId);

        if (selecetedTourReview != null)
        {
            return BadRequest("This tour is already reviewed by you.");
        }

        try
        {
            User user = _userRepository.GetById(newTourReview.TouristId);
            if(user == null){
                return NotFound($"User with id {newTourReview.TouristId} not found");
            }
            
            TourReview creaatedTourReview = _tourReviewRepository.Create(newTourReview);
            return Ok(creaatedTourReview);
        }
        catch (Exception e)
        {
            return Problem("An error occurred while creating the reservation."  + e.Message);
        }
    }
}