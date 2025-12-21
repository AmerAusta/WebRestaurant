using BusinessLayer.Reservations;
using BusinessLayer.User;
using DataAccessLayer.Reservations;
using DataAccessLayer.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Security.Claims;

namespace RestaurantWebApi.Controllers.Reservations
{
    [Route("api/Restaurant")]
    [ApiController]
    
    public class ReservationsController : ControllerBase
    {

        [HttpGet("GetReservationsByCustomerID/{CustomerID}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Policy = "AdminOrSameUser")]
        public ActionResult<IEnumerable<ReservationDTO>> GetReservationsByCustomerID([FromRoute] int CustomerID)
        {


            if (CustomerID <= 0)
                return BadRequest("Not Accepted CustomerID");

            var reservations = BusinessLayer.Reservations.Reservations.GetReservationsByCustomerID(CustomerID);

            if (reservations == null || reservations.Count == 0)
                return NotFound("No reservations found");


            List<ReservationDTO> dtoList = new List<ReservationDTO>();
            foreach (var r in reservations)
            {
                dtoList.Add(r.RDTO);
            }

            return Ok(dtoList);
        }

        [HttpGet("GetAllReservations")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Admin")]
        public ActionResult<IEnumerable<ReservationDTO>> GetAllReservations()
        {

            var reservations = BusinessLayer.Reservations.Reservations.GetAllReservations();

            if (reservations == null || reservations.Count == 0)
                return NotFound("No reservations found");


            List<ReservationDTO> dtoList = new List<ReservationDTO>();
            foreach (var r in reservations)
            {
                dtoList.Add(r.RDTO);
            }

            return Ok(dtoList);
        }

        [HttpGet("GetReservationByReservationID/{ReservationId}",Name = "GetReservationByReservationID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles ="Admin")]
        public ActionResult<ReservationDTO> GetReservationByReservationID([FromRoute] int ReservationId)
        {


            if (ReservationId <= 0)
                return BadRequest("Not Accepted CustomerID");

            BusinessLayer.Reservations.Reservations reservations = BusinessLayer.Reservations.Reservations.GetReservationByReservationID(ReservationId);
            if (reservations == null)
                return NotFound($"Reservation With ID={ReservationId} Not Have Any reservations");

            return Ok(reservations.RDTO);
        }

        [HttpPost("AddNewReservation", Name = "AddNew")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize]
        public ActionResult<ReservationDTO> AddNewReservation([FromBody] AddReservationDTO AddreservationDTO)
        {
            if (AddreservationDTO == null || AddreservationDTO.CustomerId<=0 ||
                AddreservationDTO.ReservationDate < DateTime.Now||string.IsNullOrEmpty(AddreservationDTO.Phone))
            {
                return BadRequest("Invalid Reservation Data");
            }

            Users user = Users.Find(AddreservationDTO.CustomerId);
            user.Phone = AddreservationDTO.Phone;
            if (!user.Save())
                return StatusCode(500, new { Mesaage = "Error Update User" });


            ReservationDTO NewReservationDTO = new ReservationDTO(
               0,
               AddreservationDTO.CustomerId,
               0,
               AddreservationDTO.ReservationDate,
               ReservationDTO.enStatusLookup.Pending,
               AddreservationDTO.Notes,
               DateTime.Now
            );
            BusinessLayer.Reservations.Reservations reservations=new BusinessLayer.Reservations.Reservations(NewReservationDTO);
            if (!reservations.AddNewReservation())
                return StatusCode(500, new { Message = "Error adding reservations" });
            
            NewReservationDTO.ReservationId = reservations.ReservationId;

            

            return CreatedAtRoute("GetReservationByReservationID", new { ReservationId = NewReservationDTO.ReservationId }, new
            {
                reservations = reservations.RDTO,   
            });
        }

        [HttpDelete("CancelReservation/{ReservationID}", Name = "CancelReservation")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize]
        public ActionResult CanceleReservation([FromRoute]int ReservationID)
        {
            if (ReservationID < 1)
            {
                return BadRequest(new { success = false, message = "Invalid Reservation ID" });
            }

            if (BusinessLayer.Reservations.Reservations.CancelReservation(ReservationID))
                return Ok(new { success = true, message = $"Reservation with ID {ReservationID} Cancele" });
            else
                return NotFound(new { success =false, message = $"Reservation with ID {ReservationID} Not Found" });
        }






    }
}
