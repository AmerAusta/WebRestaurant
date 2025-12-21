using BusinessLayer.User;
using DataAccessLayer.Category;
using DataAccessLayer.Reservations;
using DataAccessLayer.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RestaurantWebApi.Controllers.Category
{
    [Route("api/Restaurant")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        [HttpGet("GetCategoryByCategoryID/{CategoryId}", Name = "GetCategoryByCategoryID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles ="Admin")]
        public ActionResult<CategoryDTO> GetCategoryByCategoryID([FromRoute] int CategoryId)
        {


            if (CategoryId <= 0)
                return BadRequest("Not Accepted CustomerID");

            BusinessLayer.Category.Category category = BusinessLayer.Category.Category.Find(CategoryId);
            if (category == null)
                return NotFound($"Category With ID={CategoryId} Not Have Any Category");

            return Ok(category.CDTO);
        }

        [HttpGet("GetAllCategory")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<CategoryDTO>> GetAllCategory()
        {
            var Category = BusinessLayer.Category.Category.GetAllCategory();

            if (Category == null || Category.Count == 0)
                return NotFound("No Category found");

            List<CategoryDTO> dtoList = new List<CategoryDTO>();
            foreach (var r in Category)
            {
                dtoList.Add(r.CDTO);
            }

            return Ok(dtoList);
        }

        [HttpPost("AddCategory", Name = "AddCategory")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles ="Admin")]
        public ActionResult<CategoryDTO> AddCategory([FromBody] AddCategoryDTO AddCategoryDTO)
        {
            if (AddCategoryDTO == null || string.IsNullOrEmpty(AddCategoryDTO.Name))
            {
                return BadRequest("Invalid Category Data");
            }

            CategoryDTO NewCategoryDTO = new CategoryDTO(
                0,
                AddCategoryDTO.Name,
                AddCategoryDTO.Description
            );

            BusinessLayer.Category.Category category = new BusinessLayer.Category.Category(NewCategoryDTO);
            if (!category.Save())
                return StatusCode(500, new { Message = "Error adding Category" });

            NewCategoryDTO.CategoryID = category.CategoryID;

            return CreatedAtRoute("GetCategoryByCategoryID", new { Categoryid = NewCategoryDTO.CategoryID }, new
            {
                category = category.CDTO
            });
        }

        [HttpPut("UpdaterCategory", Name = "UpdateCategory")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "Admin")]
        public ActionResult<CategoryDTO> UpdateCategory(CategoryDTO CategoryDTOUpdate)
        {
            if (CategoryDTOUpdate.CategoryID < 1 || CategoryDTOUpdate == null || string.IsNullOrEmpty(CategoryDTOUpdate.Name))
            {
                return BadRequest("Invalid Category Data");
            }

            BusinessLayer.Category.Category category = BusinessLayer.Category.Category.Find(CategoryDTOUpdate.CategoryID);
            if (category == null)
                return NotFound($"Category With ID={CategoryDTOUpdate.CategoryID} Not Found");

            category.Name = CategoryDTOUpdate.Name;
            category.Description = CategoryDTOUpdate.Description;


            if (category.Save())
                return Ok(category.CDTO);
            else
                return StatusCode(500, new { Mesaage = "Error Update Category" });

        }

        [HttpDelete("DeleteCategory/{CategoryID}", Name = "DeleteCategory")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteCategory([FromRoute] int CategoryID)
        {
            if (CategoryID < 1)
            {
                return BadRequest("Invaled Category Data");
            }

            if (BusinessLayer.Category.Category.DeleteCategory(CategoryID))
                return Ok(new { success = true, message = $"Category with ID {CategoryID} Deleted" });
            else
                return NotFound(new { success = false, message = $"Category with ID {CategoryID} Not Found" });
        }

    }
}
