using DataAccessLayer.MenuItems;
using DataAccessLayer.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RestaurantWebApi.Controllers.MenuItems
{
    [Route("api/Restaurant")]
    [ApiController]
    public class MenuItemsController : ControllerBase
    {
        [HttpGet("GetMenuItemsByMenuItemID/{MenuItemId}", Name = "GetMenuItemsByMenuItemID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Admin")]
        public ActionResult<MenuItemDTO> GetMenuItemsByMenuItemID([FromRoute] int MenuItemId)
        {


            if (MenuItemId <= 0)
                return BadRequest("Not Accepted CustomerID");

            BusinessLayer.MenuItems.MenuItems MenuItem = BusinessLayer.MenuItems.MenuItems.Find(MenuItemId);
            if (MenuItem == null)
                return NotFound($"MenuItem With ID={MenuItemId} Not Have Any MenuItem");

            return Ok(MenuItem.MDTO);
        }

        [HttpGet("GetAllMenuItems")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<MenuItemDTO>> GetAllMenuItems()
        {
            var MenuItem = BusinessLayer.MenuItems.MenuItems.GetAllMenuItems();

            if (MenuItem == null || MenuItem.Count == 0)
                return NotFound("No MenuItem found");

            List<MenuItemDTO> dtoList = new List<MenuItemDTO>();
            foreach (var r in MenuItem)
            {
                dtoList.Add(r.MDTO);
            }

            return Ok(dtoList);
        }

        [HttpGet("GetAllMenuItemsByCategoryId/{CategoryId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<MenuItemDTO>> GetAllMenuItemsByCategoryId(int CategoryId)
        {
            var MenuItem = BusinessLayer.MenuItems.MenuItems.GetAllMenuItemsByCategoryID(CategoryId);

            if (MenuItem == null || MenuItem.Count == 0)
                return NotFound("No MenuItem found");

            List<MenuItemDTO> dtoList = new List<MenuItemDTO>();
            foreach (var r in MenuItem)
            {
                dtoList.Add(r.MDTO);
            }

            return Ok(dtoList);
        }

        [HttpPost("AddMenuItem", Name = "AddMenuItem")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "Admin")]
        public ActionResult<MenuItemDTO> AddMenuItem([FromBody] AddMenuItemDTO AddMenuItemDTO)
        {
            if (AddMenuItemDTO == null || string.IsNullOrEmpty(AddMenuItemDTO.Name)||AddMenuItemDTO.CategoryId<0
                ||AddMenuItemDTO.Price<0||AddMenuItemDTO.PerparationTime<0)
            {
                return BadRequest("Invalid MenuItem Data");
            }

            MenuItemDTO NewMenuItemDTO = new MenuItemDTO(
                0,
                AddMenuItemDTO.Name,
                AddMenuItemDTO.Description,
                AddMenuItemDTO.CategoryId,
                AddMenuItemDTO.Price,
                DateTime.Now,
                AddMenuItemDTO.PerparationTime
            );

            BusinessLayer.MenuItems.MenuItems MenuItem = new BusinessLayer.MenuItems.MenuItems(NewMenuItemDTO);
            if (!MenuItem.Save())
                return StatusCode(500, new { Message = "Error adding MenuItem" });

            NewMenuItemDTO.MenuItemId = MenuItem.MenuItemId;

            return CreatedAtRoute("GetMenuItemsByMenuItemID", new { MenuItemId = NewMenuItemDTO.MenuItemId }, new
            {
                MenuItem = MenuItem.MDTO
            });
        }

        [HttpPut("UpdaterMenuItem", Name = "UpdateMenuItem")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "Admin")]
        public ActionResult<MenuItemDTO> UpdateMenuItem([FromBody]UpdateMenuItemDTO dto)
        {

            //if (!ModelState.IsValid)
            //    return BadRequest(ModelState);

            if (dto.MenuItemId < 1 ||
                string.IsNullOrWhiteSpace(dto.Name) ||
                dto.CategoryId < 1 ||
                dto.Price < 0 ||
                dto.PerparationTime < 0)
            {
                return BadRequest("Invalid MenuItem Data");
            }

            var menuItem = BusinessLayer.MenuItems.MenuItems.Find(dto.MenuItemId);
            if (menuItem == null)
                return NotFound();

            menuItem.Name = dto.Name;
            menuItem.Description = dto.Description;
            menuItem.CategoryId = dto.CategoryId;
            menuItem.Price = dto.Price;
            menuItem.IsActive = dto.IsActive;

            if (menuItem.Save())
                return Ok(menuItem.MDTO);
            else
                return StatusCode(500, "Error Update MenuItem");

        }

        [HttpDelete("DeleteMenuItem/{MenuItemID}", Name = "DeleteMenuItem")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteMenuItem([FromRoute] int MenuItemID)
        {
            if (MenuItemID < 1)
            {
                return BadRequest("Invaled MenuItem Data");
            }

            if (BusinessLayer.MenuItems.MenuItems.DeleteMenuItem(MenuItemID))
                return Ok(new { success = true, message = $"MenuItem with ID {MenuItemID} Deleted" });
            else
                return NotFound(new { success = false, message = $"MenuItem with ID {MenuItemID} Not Found" });
        }

    }
}
