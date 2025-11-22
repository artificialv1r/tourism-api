using Microsoft.AspNetCore.Mvc;
using tourism_api.Domain;
using tourism_api.Repositories;

namespace tourism_api.Controllers;
[Route("api/tours/{tourId}/reservations")]
[ApiController]

public class TourReservationController:ControllerBase
{
    private readonly TourReservationRepository _tourReservationRepository;
    private readonly UserRepository _userRepository;
    private readonly TourRepository _tourRepository;

    public TourReservationController(IConfiguration configuration)
    {
        _tourReservationRepository = new TourReservationRepository(configuration);
        _userRepository = new UserRepository(configuration);
        _tourRepository = new TourRepository(configuration);
    }

    [HttpGet]
    public ActionResult GetByTourId(int tourId)
    {
        try
        {
            List<TourReservation> reservations = _tourReservationRepository.GetReservationsByTourId(tourId);

            return Ok(reservations);
        }
        catch (Exception e)
        {
            return Problem("An error occurred while fetching the reservations." + e.Message);
        }
    }

    [HttpPost]

    public ActionResult<TourReservation> Create(int tourId, [FromBody] TourReservation newTourReservation)
    {
        if (!newTourReservation.IsValid())
        {
            return BadRequest("Invalid reservation data");
        }
        
        List<TourReservation> reservations = _tourReservationRepository.GetReservationsByTourId(newTourReservation.TourId);
        Tour selecetedTour = _tourRepository.GetById(tourId);

        if (selecetedTour == null)
        {
            return NotFound();
        }
        
        int reservedSeats = 0;
        foreach (TourReservation reservation in reservations)
        {
            reservedSeats += reservation.Guests;
        }

        if (newTourReservation.Guests > (selecetedTour.MaxGuests - reservedSeats))
        {
            return BadRequest("Tour is full, number of free seats: " + (selecetedTour.MaxGuests - reservedSeats));
        }

        try
        {
            User user = _userRepository.GetById(newTourReservation.TouristId);
            if(user == null){
                return NotFound($"User with id {newTourReservation.TouristId} not found");
            }
            
            TourReservation createdTourReservation = _tourReservationRepository.Create(newTourReservation);
            return Ok(createdTourReservation);
        }
        catch (Exception e)
        {
            return Problem("An error occurred while creating the reservation."  + e.Message);
        }
    }
    
    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        try
        {
            bool isDeleted = _tourReservationRepository.Delete(id);
            if (isDeleted)
            {
                return NoContent();
            }
            return NotFound($"Tour reservation with ID {id} not found.");
        }
        catch (Exception ex)
        {
            return Problem("An error occurred while deleting the tour reservation." + ex.Message);
        }
    }
}