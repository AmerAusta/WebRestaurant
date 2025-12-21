using DataAccessLayer.Globale;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DataAccessLayer.Reservations.ReservationDTO;
using static DataAccessLayer.User.UserDTO;

namespace DataAccessLayer.Category
{
    public class CategoryDTO
    {
        public int CategoryID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public CategoryDTO(int categoryID, string name, string description)
        {
            CategoryID = categoryID;
            Name = name;
            Description = description;
        }
    }

    public class AddCategoryDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public AddCategoryDTO( string name, string description)
        {
            Name = name;
            Description = description;
        }
    }

    public class CategoryData
    {
        public static CategoryDTO GetCategoryByCategoryID(int CategoryID)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString._connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_GetCategoryByID", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@CategoryID", CategoryID);

                    try
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new CategoryDTO(
                                    reader.GetInt32(reader.GetOrdinal("CategoryId")),
                                    reader.GetString(reader.GetOrdinal("Name")),
                                    reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description"))
                                    );
                            }
                            else
                                return null;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Get Category By ID failed: {ex.Message}");
                        return null;
                    }
                }
            }
        }

        public static List<CategoryDTO> GetAllCategory()
        {
            var list = new List<CategoryDTO>();
            using (SqlConnection conn = new SqlConnection(ConnectionString._connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_GetAllCategory", conn))
                {

                    cmd.CommandType = CommandType.StoredProcedure;

                    try
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var DTO = new CategoryDTO(
                                    reader.GetInt32(reader.GetOrdinal("CategoryId")),
                                    reader.GetString(reader.GetOrdinal("Name")),
                                    reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description"))
                                    );
                                list.Add(DTO);
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Get All Category failed: {ex.Message}");
                        return null;
                    }
                }

            }
            return list;
        }

        public static int AddNewCategory(CategoryDTO CategoryDTO)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString._connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_AddCategory", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Name", CategoryDTO.Name);
                    if (string.IsNullOrEmpty(CategoryDTO.Description))
                    {
                        cmd.Parameters.AddWithValue("@Description", DBNull.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@Description", CategoryDTO.Description);
                    }
                    

                    var outputIDParam = new SqlParameter("@CategoryID", SqlDbType.Int)
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
                        Console.WriteLine($"Add Category failed: {ex.Message}");
                        return -1;
                    }
                }
            }
        }

        public static bool UpdateCategory(CategoryDTO CategoryDTO)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString._connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_UpdateCategory", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@CategoryID", CategoryDTO.CategoryID);
                    cmd.Parameters.AddWithValue("@Name", CategoryDTO.Name);
                    if (string.IsNullOrEmpty(CategoryDTO.Description))
                    {
                        cmd.Parameters.AddWithValue("@Description", DBNull.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@Description", CategoryDTO.Description);
                    }

                    try
                    {
                        conn.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Update Category failed: {ex.Message}");
                        return false;
                    }
                }
            }
        }

        public static bool DeleteCategory(int CategoryID)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString._connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_DeleteCategory", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@CategoryID", CategoryID);

                    try
                    {
                        conn.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Delete Category failed: {ex.Message}");
                        return false;
                    }
                }
            }
        }

    }
}
