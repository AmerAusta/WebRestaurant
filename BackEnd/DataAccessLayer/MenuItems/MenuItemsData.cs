using DataAccessLayer.Globale;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.MenuItems
{
    public class MenuItemDTO
    {
        public int MenuItemId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; }
        public int PerparationTime { get; set; }
        public bool IActive {  get; set; }

        public MenuItemDTO(int menuItemId, string name, string description, int categoryId, decimal price, DateTime createdAt, int perparationTime, bool iActive =true )
        {
            MenuItemId = menuItemId;
            Name = name;
            Description = description;
            CategoryId = categoryId;
            Price = price;
            CreatedAt = createdAt;
            PerparationTime = perparationTime;
            IActive = iActive;
        }
    }

    public class AddMenuItemDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public decimal Price { get; set; }
        public int PerparationTime { get; set; }

        public AddMenuItemDTO( string name, string description, int categoryId, decimal price, int perparationTime)
        {
            Name = name;
            Description = description;
            CategoryId = categoryId;
            Price = price;
            PerparationTime = perparationTime;
        }
    }

    public class UpdateMenuItemDTO
    {
        public int MenuItemId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public int CategoryId { get; set; }
        public decimal Price { get; set; }
        public int PerparationTime{ get; set; }
        public bool IsActive {  get; set; }

        public UpdateMenuItemDTO(int menuItemId, string name, string? description, int categoryId, decimal price, int perparationTime, bool isActive)
        {
            MenuItemId = menuItemId;
            Name = name;
            Description = description;
            CategoryId = categoryId;
            Price = price;
            PerparationTime = perparationTime;
            IsActive = isActive;
        }
    }

    public class MenuItemsData
    {
        public static MenuItemDTO GetMenuItemByMenuItemId(int MenuItemId)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString._connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_GetMenuItemByMenuItemId", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@MenuItemId", MenuItemId);

                    try
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new MenuItemDTO(
                                    reader.GetInt32(reader.GetOrdinal("MenuItemId")),
                                    reader.GetString(reader.GetOrdinal("Name")),
                                    reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                                    reader.GetInt32(reader.GetOrdinal("CategoryId")),
                                    reader.GetDecimal(reader.GetOrdinal("Price")),
                                    reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                                    reader.GetInt32(reader.GetOrdinal("PerparationTime"))
                                    );
                            }
                            else
                                return null;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Get MenuItems By MenuItemsID failed: {ex.Message}");
                        return null;
                    }
                }
            }
        }

        public static List<MenuItemDTO> GetAllMenuItems()
        {
            var list = new List<MenuItemDTO>();
            using (SqlConnection conn = new SqlConnection(ConnectionString._connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_GetAllMenuItem", conn))
                {

                    cmd.CommandType = CommandType.StoredProcedure;

                    try
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var DTO = new MenuItemDTO(
                                    reader.GetInt32(reader.GetOrdinal("MenuItemId")),
                                    reader.GetString(reader.GetOrdinal("Name")),
                                    reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                                    reader.GetInt32(reader.GetOrdinal("CategoryId")),
                                    reader.GetDecimal(reader.GetOrdinal("Price")),                              
                                    reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                                    reader.GetInt32(reader.GetOrdinal("PerparationTime"))
                                    );
                                list.Add(DTO);
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Get All MenuItem failed: {ex.Message}");
                        return null;
                    }
                }

            }
            return list;
        }

        public static List<MenuItemDTO> GetAllMenuItemsByCategoryID(int CategoryId)
        {
            var list = new List<MenuItemDTO>();
            using (SqlConnection conn = new SqlConnection(ConnectionString._connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_GetMenuItemByCategoryID", conn))
                {

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CategoryId", CategoryId);

                    try
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var DTO = new MenuItemDTO(
                                    reader.GetInt32(reader.GetOrdinal("MenuItemId")),
                                    reader.GetString(reader.GetOrdinal("Name")),
                                    reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                                    reader.GetInt32(reader.GetOrdinal("CategoryId")),
                                    reader.GetDecimal(reader.GetOrdinal("Price")),                                    
                                    reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                                    reader.GetInt32(reader.GetOrdinal("PerparationTime"))
                                    );
                                list.Add(DTO);
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Get All MenuItem failed: {ex.Message}");
                        return null;
                    }
                }

            }
            return list;
        }

        public static int AddNewMenuItem(MenuItemDTO MenuItemDTO)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString._connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_AddMenuItem", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Name", MenuItemDTO.Name);
                    if (string.IsNullOrEmpty(MenuItemDTO.Description))
                    {
                        cmd.Parameters.AddWithValue("@Description", DBNull.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@Description", MenuItemDTO.Description);
                    }
                    cmd.Parameters.AddWithValue("@CategoryId", MenuItemDTO.CategoryId);
                    cmd.Parameters.AddWithValue("@Price", MenuItemDTO.Price);                    
                    cmd.Parameters.AddWithValue("@CreatedAt", MenuItemDTO.CreatedAt);
                    cmd.Parameters.AddWithValue("@PerparationTime", MenuItemDTO.PerparationTime);

                    var outputIDParam = new SqlParameter("@MenuItemId", SqlDbType.Int)
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
                        Console.WriteLine($"Add MenuItem failed: {ex.Message}");
                        return -1;
                    }
                }
            }
        }

        public static bool UpdateMenuItem(MenuItemDTO MenuItemDTO)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString._connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_UpdateMenuItem", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@MenuItemId", MenuItemDTO.MenuItemId);
                    cmd.Parameters.AddWithValue("@Name", MenuItemDTO.Name);
                    if (string.IsNullOrEmpty(MenuItemDTO.Description))
                    {
                        cmd.Parameters.AddWithValue("@Description", DBNull.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@Description", MenuItemDTO.Description);
                    }
                    cmd.Parameters.AddWithValue("@CategoryId", MenuItemDTO.CategoryId);
                    cmd.Parameters.AddWithValue("@Price", MenuItemDTO.Price);
                    cmd.Parameters.AddWithValue("@PerparationTime", MenuItemDTO.PerparationTime);
                    cmd.Parameters.AddWithValue("@IsActive", MenuItemDTO.IActive);
                    try
                    {
                        conn.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Update MenuItem failed: {ex.Message}");
                        return false;
                    }
                }
            }
        }

        public static bool DeleteMenuItem(int MenuItemId)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString._connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_DeleteMenuItem", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@MenuItemId", MenuItemId);

                    try
                    {
                        conn.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Delete MenuItem failed: {ex.Message}");
                        return false;
                    }
                }
            }
        }

    }
}
