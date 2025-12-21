using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BusinessLayer.User;
using DataAccessLayer.User;

namespace RestaurantWebApi.Controllers.User
{
    [Route("api/Restaurant")]
    [ApiController]
    public class UsersController : ControllerBase  
    {
        [HttpPost("auth/login", Name = "LoginUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<UserDTOresponse> Login([FromBody] UserDTOLogin userDTOLogin)
        {
            if (string.IsNullOrEmpty(userDTOLogin.Email) || string.IsNullOrEmpty(userDTOLogin.Password))
                return BadRequest("Email or Password missing");

            Users user = Users.GetUserByEmailAndPassword(userDTOLogin.Email, userDTOLogin.Password);
            if (user == null)
                return NotFound("Invalid email or password");

            string token = Jwt.GenerateToken(user.UDTO);
            return Ok(new
            {
                user = user.UDTOresponse,
                token
            });
        }

        [HttpPost("auth/register", Name = "RegisterUser")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<UserDTOresponse> Register([FromBody] UserDTORigester userDTORigester)
        {
            if (userDTORigester == null || string.IsNullOrEmpty(userDTORigester.FullName) ||
                string.IsNullOrEmpty(userDTORigester.Email) || string.IsNullOrEmpty(userDTORigester.Password))
            {
                return BadRequest("Invalid User Data");
            }

            UserDTO NewUserDTO = new UserDTO(
                0,
                userDTORigester.FullName,
                userDTORigester.Email,
                userDTORigester.Password,
                UserDTO.enRoles.Customer,
                null,
                true,
                DateTime.UtcNow
            );

            Users user = new Users(NewUserDTO);
            if (!user.Save())
                return StatusCode(500, new { Message = "Error adding user" });

            NewUserDTO.UserID = user.UserID;

            string token = Jwt.GenerateToken(user.UDTO);
            return CreatedAtRoute("GetUserByID", new { id = NewUserDTO.UserID }, new
            {
                user = user.UDTOresponse,
                token
            });
        }

        [HttpGet("ID/{ID}", Name = "GetUserByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Admin")]
        public ActionResult<UserDTO> GetUserByID([FromRoute]int ID)
        {
            if (ID <= 0)
                return BadRequest("Not Accepted ID");

            Users User = Users.Find(ID);
            if (User == null)
                return NotFound($"User With ID={ID} Not Found");

            return Ok(User.UDTOresponse);
        }

        [HttpGet("GetUserByFullName/{FullName}", Name = "GetUserByFullName")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Admin")]
        public ActionResult<UserDTO> GetUserByFullName([FromRoute] string FullName)
        {
            if ( string.IsNullOrWhiteSpace(FullName))
                return BadRequest("Not Accepted ID");

            Users User = Users.GetUserByFullName(FullName);
            if (User == null)
                return NotFound($"User With FullName={FullName} Not Found");

            return Ok(User.UDTOresponse);
        }

        [HttpPut("UpdateUser", Name ="UpdateUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles ="Admin")]
        public ActionResult<UserDTO> UpdateUser(UserDTOUpdate userDTOUpdate)
        {
            if(userDTOUpdate==null|| userDTOUpdate.UserID < 1 || string.IsNullOrWhiteSpace(userDTOUpdate.FullName))
            {
                return BadRequest("Invalid User Data");
            }

            Users user =Users.Find(userDTOUpdate.UserID);
            if(user == null)
                return NotFound($"User With ID={userDTOUpdate.UserID} Not Found");
            
            user.FullName = userDTOUpdate.FullName;
            if (!string.IsNullOrEmpty(userDTOUpdate.Password))
            {
                user.PasswordHash = userDTOUpdate.Password;
            }
            user.Phone = userDTOUpdate.Phone;
            user.IsActive = userDTOUpdate.IsActive;
            user.RoleId = userDTOUpdate.RoleId;

            if(user.Save())
                return Ok(user.UDTOresponse);
            else
                return StatusCode(500, new { Mesaage = "Error Update User" });

        }

        [HttpDelete("DeleteUser/{UserID}", Name = "DeleteUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = "AdminOrSameUser")]
        public ActionResult DeleteUser([FromRoute] int UserID)
        {
            if (UserID < 1)
            {
                return BadRequest("Invaled User Data");
            }

            if (Users.DeleteUser(UserID))
                return Ok(new { success = true, message = $"User with ID {UserID} Deleted" });
            else
                return NotFound(new { success = false, message = $"User with ID {UserID} Not Found" });
        }
    }  


}
