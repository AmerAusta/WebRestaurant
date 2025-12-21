using DataAccessLayer.Globale;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using static DataAccessLayer.User.UserDTO;

namespace DataAccessLayer.User
{
    public class UserDTORigester
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public UserDTORigester( string fullName, string email, string password)
        {
            FullName = fullName;
            Email = email;
            Password = password;
        }
    }

    public class UserDTOLogin
    {
        public string Email { get; set; }
        public string Password { get; set; }

        public UserDTOLogin(string email, string password)
        {
            Email = email;
            Password = password;
        }
    }

    public class UserDTO
    {
        public enum enRoles { Customer=1,Admin=2 }

        public int UserID {  get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public enRoles RoleId {  get; set; }
        public string Phone { get; set; }
        public bool IsActive {  get; set; }
        public DateTime CreatedAt { get; set; }

        public UserDTO(int userID, string fullName, string email, string passwordHash, enRoles roleId, string phone , bool isActive, DateTime createdAt)
        {
            UserID = userID;
            FullName = fullName;
            Email = email;
            PasswordHash = passwordHash;
            RoleId = roleId;
            Phone = phone;
            IsActive = isActive;
            CreatedAt = createdAt;
        }
    }

    public class UserDTOUpdate 
    {
        public int UserID { get; set; }
        public string FullName { get; set; }
        public string? Password { get; set; }
        public string Phone { get; set; }
        public bool IsActive { get; set; }
        public UserDTO.enRoles RoleId { get; set; }

        public UserDTOUpdate(int userID,string fullName, string password, string phone, bool isActive,UserDTO.enRoles roleId)
        {
             UserID = userID;
             FullName = fullName;
             Password = password;
             Phone = phone;
             IsActive = isActive;
             RoleId= roleId;
        }
    }

    public class UserDTOresponse
    {
        public int UserID { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public enRoles RoleId { get; set; }
        public string Phone { get; set; }
        public bool IsActive { get; set; }

        public UserDTOresponse(int userID, string fullName, string email, enRoles roleId, string phone, bool isActive)
        {
            UserID = userID;
            FullName = fullName;
            Email = email;
            RoleId = roleId;
            Phone = phone;
            IsActive = isActive;
        }
    }

    public class UsersData
    {
        public static UserDTO GetUserByEmail(string Email)
            {
            using(SqlConnection conn=new SqlConnection(ConnectionString._connectionString))
            {
                using(SqlCommand cmd=new SqlCommand("SP_GetUserByEmail",conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Email", Email);

                    try
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new UserDTO(
                                    reader.GetInt32(reader.GetOrdinal("UserId")),
                                    reader.GetString(reader.GetOrdinal("FullName")),
                                    reader.GetString(reader.GetOrdinal("Email")),
                                    reader.GetString(reader.GetOrdinal("PasswordHash")),
                                    (enRoles)reader.GetByte(reader.GetOrdinal("RoleId")),
                                    reader.IsDBNull(reader.GetOrdinal("Phone")) ? null : reader.GetString(reader.GetOrdinal("Phone")),
                                    reader.GetBoolean(reader.GetOrdinal("IsActive")),
                                    reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
                                    );
                            }
                            else
                                return null;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Get User By Email failed: {ex.Message}");
                        return null;
                    }
                }
            }
        }

        public static UserDTO GetUserByID(int ID)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString._connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_GetUserByID", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@UserID", ID);

                    try
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new UserDTO(
                                    reader.GetInt32(reader.GetOrdinal("UserID")),
                                    reader.GetString(reader.GetOrdinal("FullName")),
                                    reader.GetString(reader.GetOrdinal("Email")),
                                    reader.GetString(reader.GetOrdinal("PasswordHash")),
                                    (enRoles)reader.GetByte(reader.GetOrdinal("RoleId")),
                                    reader.IsDBNull(reader.GetOrdinal("Phone")) ? null : reader.GetString(reader.GetOrdinal("Phone")),
                                    reader.GetBoolean(reader.GetOrdinal("IsActive")),
                                    reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
                                    );
                            }
                            else
                                return null;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Get User By ID failed: {ex.Message}");
                        return null;
                    }
                }
            }
        }

        public static UserDTO GetUserByFullName(string FullName)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString._connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_GetUserByFullName", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@FullName", FullName);

                    try
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new UserDTO(
                                    reader.GetInt32(reader.GetOrdinal("UserID")),
                                    reader.GetString(reader.GetOrdinal("FullName")),
                                    reader.GetString(reader.GetOrdinal("Email")),
                                    reader.GetString(reader.GetOrdinal("PasswordHash")),
                                    (enRoles)reader.GetByte(reader.GetOrdinal("RoleId")),
                                    reader.IsDBNull(reader.GetOrdinal("Phone")) ? null : reader.GetString(reader.GetOrdinal("Phone")),
                                    reader.GetBoolean(reader.GetOrdinal("IsActive")),
                                    reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
                                    );
                            }
                            else
                                return null;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Get User By ID failed: {ex.Message}");
                        return null;
                    }
                }
            }
        }

        public static int AddNewUser(UserDTO userDTO)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString._connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_AddNewUser", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@FullName", userDTO.FullName);
                    cmd.Parameters.AddWithValue("@Email", userDTO.Email);
                    cmd.Parameters.AddWithValue("@PasswordHash", userDTO.PasswordHash);
                    if (userDTO.RoleId == enRoles.Customer)
                    {
                        cmd.Parameters.AddWithValue("@RoleId", 1);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@RoleId", 2);
                    }
                    if (string.IsNullOrEmpty(userDTO.Phone))
                    {
                        cmd.Parameters.AddWithValue("@Phone", DBNull.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@Phone", userDTO.Phone);
                    }
                    cmd.Parameters.AddWithValue("@IsActive", userDTO.IsActive);
                    cmd.Parameters.AddWithValue("@CreatedAt", userDTO.CreatedAt);


                    var outputIDParam = new SqlParameter("@UserID", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(outputIDParam);

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        return outputIDParam.Value==DBNull.Value?-1:(int)outputIDParam.Value;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Add User failed: {ex.Message}");
                        return -1;
                    }
                }
            }
        }

        public static bool UpdateUser(UserDTO userDTO)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString._connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_UpdateUser", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@UserID", userDTO.UserID);
                    cmd.Parameters.AddWithValue("@FullName",userDTO.FullName);
                    cmd.Parameters.AddWithValue("@PasswordHash", userDTO.PasswordHash);
                    if (string.IsNullOrEmpty(userDTO.Phone))
                    {
                        cmd.Parameters.AddWithValue("@Phone", DBNull.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@Phone", userDTO.Phone);
                    }
                    cmd.Parameters.AddWithValue("@IsActive", userDTO.IsActive);
                    cmd.Parameters.AddWithValue("@RoleId", (Byte)userDTO.RoleId);
                    try
                    {
                        conn.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"UpdateUser failed: {ex.Message}");
                        return false;
                    }
                }
            }
        }

        public static bool DeleteUser(int UserID)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString._connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_DeleteUser", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@UserID", UserID);

                    try
                    {
                        conn.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Delete User failed: {ex.Message}");
                        return false;
                    }
                }
            }
        }

    }
}
