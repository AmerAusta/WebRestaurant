using DataAccessLayer.Order;
using DataAccessLayer.Reservations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DataAccessLayer.Reservations.ReservationDTO;

namespace BusinessLayer.Reservations
{
    public class Reservations
    {
        public int ReservationId { get; set; }
        public int CustomerId { get; set; }
        public int TableId { get; set; }
        public DateTime ReservationDate { get; set; }
        public enStatusLookup StatusId { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedAt { get; set; }

        public ReservationDTO RDTO
        { get { return new ReservationDTO(ReservationId, CustomerId, TableId, ReservationDate, StatusId, Notes, CreatedAt); } }

        public Reservations(ReservationDTO reservationDTO) 
        {
            ReservationId= reservationDTO.ReservationId;
            CustomerId= reservationDTO.CustomerId;
            TableId= reservationDTO.TableId;
            ReservationDate = reservationDTO.ReservationDate;
            StatusId= reservationDTO.StatusId;
            Notes = reservationDTO.Notes;
            CreatedAt = reservationDTO.CreatedAt;
        }

        public static List<Reservations> GetReservationsByCustomerID(int customerId)
        {
            List<ReservationDTO> listDTO = ReservationData.GetReservationsByCustomerID(customerId);

            if (listDTO == null || listDTO.Count == 0)
                return null;

            List<Reservations> list = new List<Reservations>();

            foreach (var dto in listDTO)
            {
                list.Add(new Reservations(dto));   
            }

            return list;
        }

        public static List<Reservations> GetAllReservations()
        {
            List<ReservationDTO> listDTO = ReservationData.GetAllReservations();

            if (listDTO == null || listDTO.Count == 0)
                return null;

            List<Reservations> list = new List<Reservations>();

            foreach (var dto in listDTO)
            {
                list.Add(new Reservations(dto));
            }

            return list;
        }

        public static Reservations GetReservationByReservationID(int ReservationId)
        {
            ReservationDTO RDTO = ReservationData.GetReservationByReservationID(ReservationId);
            if (RDTO != null)
            {
                return new Reservations(RDTO);
            }
            else
            {
                return null;
            }
        }

        public static int GetNextTableIDReservation(DateTime reservationDate)
        {
            return ReservationData.GetNextTableIDReservation(reservationDate);
        }

        public  bool AddNewReservation()
        {
            int NextTableID = GetNextTableIDReservation(ReservationDate);
            if (NextTableID != 0)
            {
                TableId=NextTableID;
                ReservationId = ReservationData.AddNewReservation(RDTO);
                return ReservationId != -1;
            }
            else 
            {
                return false;
            }
        }

        public static bool CancelReservation(int reservationID)
        {
            var reservation = ReservationData.GetReservationByReservationID(reservationID);
            if (reservation == null)
                return false;

            if (reservation.ReservationDate <= DateTime.Now.AddHours(1))
                return false;

            bool reservationCancelled =
                ReservationData.CanceleReservation(reservationID,(Byte)reservation.StatusId);

            if (!reservationCancelled)
                return false;

            var order = OrderData.GetOrderByReservationID(reservationID);
            if (order != null)
            {
                OrderData.CancelledOrderByReservationID(reservationID);
            }

            return true;
        }
    }
}
