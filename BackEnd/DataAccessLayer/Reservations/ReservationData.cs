using DataAccessLayer.Globale;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DataAccessLayer.Reservations.ReservationDTO;

namespace DataAccessLayer.Reservations
{
    public class ReservationDTO
    {
        public enum enStatusLookup { Pending = 1, Cancelled = 2, Completed = 3, Active = 4 }

        public int ReservationId { get; set; }
        public int CustomerId { get; set; }
        public int TableId { get; set; }
        public DateTime ReservationDate { get; set; }
        public enStatusLookup StatusId { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedAt { get; set; }

        public ReservationDTO(int reservationId, int customerId, int tableId, DateTime reservationDate, enStatusLookup statusId, string notes, DateTime createdAt)
        {
            ReservationId = reservationId;
            CustomerId = customerId;
            TableId = tableId;
            ReservationDate = reservationDate;
            StatusId = statusId;
            Notes = notes;
            CreatedAt = createdAt;
        }
    }

    public class AddReservationDTO
    {

        public int CustomerId { get; set; }
        public DateTime ReservationDate { get; set; }
        public string Notes { get; set; }
        public string Phone {  get; set; }

        public AddReservationDTO(int customerId, DateTime reservationDate, string notes, string phone)
        {
            CustomerId = customerId;
            ReservationDate = reservationDate;
            Notes = notes;
            Phone = phone;
        }
    }

    public class ReservationData
    {
        public static int GetNextTableIDReservation(DateTime reservationDate)
        {
            using(SqlConnection conn=new SqlConnection(ConnectionString._connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_GetNextTableIDReservation", conn))
                {
                    cmd.CommandType=CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ReservationDate", reservationDate);

                    try
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return reader.GetInt32(reader.GetOrdinal("NextTableID"));
                            }
                            else
                            {
                                return 0;
                            }
                        }
                    }
                    catch(Exception ex) 
                    {

                        Console.WriteLine($"Get Next Table failed: {ex.Message}");
                        return 0;
                    }
                }
            }
        }

        public static List<ReservationDTO> GetReservationsByCustomerID(int customerId)
        {
            var list = new List<ReservationDTO>();
            using (SqlConnection conn = new SqlConnection(ConnectionString._connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_GetReservationsByCustomerID", conn))
                {
                    
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CustomerId", customerId);

                    try
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var DTO= new ReservationDTO(
                                    reader.GetInt32(reader.GetOrdinal("ReservationId")),
                                    reader.GetInt32(reader.GetOrdinal("CustomerId")),
                                    reader.GetInt32(reader.GetOrdinal("TableId")),
                                    reader.GetDateTime(reader.GetOrdinal("ReservationDate")),
                                    (enStatusLookup)(int)reader.GetByte(reader.GetOrdinal("StatusId")),
                                    reader.IsDBNull(reader.GetOrdinal("Notes"))? null: reader.GetString(reader.GetOrdinal("Notes")),
                                    reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
                                    );
                                list.Add( DTO );
                            }
                        }

                    }
                    catch (Exception ex) {
                        Console.WriteLine($"Get Reservation By Customer ID failed: {ex.Message}");
                        return null;
                    }
                }

            }
            return list;
        }

        public static List<ReservationDTO> GetAllReservations()
        {
            var list = new List<ReservationDTO>();
            using (SqlConnection conn = new SqlConnection(ConnectionString._connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_GetAllReservations", conn))
                {

                    cmd.CommandType = CommandType.StoredProcedure;

                    try
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var DTO = new ReservationDTO(
                                    reader.GetInt32(reader.GetOrdinal("ReservationId")),
                                    reader.GetInt32(reader.GetOrdinal("CustomerId")),
                                    reader.GetInt32(reader.GetOrdinal("TableId")),
                                    reader.GetDateTime(reader.GetOrdinal("ReservationDate")),
                                    (enStatusLookup)(int)reader.GetByte(reader.GetOrdinal("StatusId")),
                                    reader.IsDBNull(reader.GetOrdinal("Notes")) ? null : reader.GetString(reader.GetOrdinal("Notes")),
                                    reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
                                    );
                                list.Add(DTO);
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Get Reservation By Customer ID failed: {ex.Message}");
                        return null;
                    }
                }

            }
            return list;
        }

        public static ReservationDTO GetReservationByReservationID(int ReservationId)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString._connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_GetReservationsByReservationID", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ReservationID", ReservationId);

                    try
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new ReservationDTO(
                                    reader.GetInt32(reader.GetOrdinal("ReservationId")),
                                    reader.GetInt32(reader.GetOrdinal("CustomerId")),
                                    reader.GetInt32(reader.GetOrdinal("TableId")),
                                    reader.GetDateTime(reader.GetOrdinal("ReservationDate")),
                                    (enStatusLookup)(int)reader.GetByte(reader.GetOrdinal("StatusId")),
                                    reader.IsDBNull(reader.GetOrdinal("Notes")) ? null : reader.GetString(reader.GetOrdinal("Notes")),
                                    reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
                                    );
                            }
                            else
                            {
                                return null;
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Get Reservation By Customer ID failed: {ex.Message}");
                        return null;
                    }
                }

            }

        }

        public static int AddNewReservation(ReservationDTO reservationDTO)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString._connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_AddNewReservation", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CustomerId", reservationDTO.CustomerId);
                    cmd.Parameters.AddWithValue("@TableId", reservationDTO.TableId);
                    cmd.Parameters.AddWithValue("@ReservationDate", reservationDTO.ReservationDate);
                    cmd.Parameters.AddWithValue("@StatusId", reservationDTO.StatusId);
                    if (string.IsNullOrEmpty(reservationDTO.Notes))
                    {
                        cmd.Parameters.AddWithValue("@Notes", DBNull.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@Notes",reservationDTO.Notes);
                    }
                    cmd.Parameters.AddWithValue("@CreatedAt",reservationDTO.CreatedAt);

                    var outputIDParam=new SqlParameter("@ReservationId",SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(outputIDParam);

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        return outputIDParam.Value == DBNull.Value ? -1 : (int)outputIDParam.Value;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Add Reservation failed: {ex.Message}");
                        return -1;
                    }
                }
            }
        }

        public static bool CanceleReservation(int ReservationID,Byte StatusId)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString._connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_CanceledReservation", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@ReservationId", ReservationID);
                    cmd.Parameters.AddWithValue("@StatusId", StatusId);
                    try
                    {
                        conn.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Cancele Reservation failed: {ex.Message}");
                        return false;
                    }
                }
            }
        }
    }
}
