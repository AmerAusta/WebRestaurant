using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using DataAccessLayer.User;

public static class Jwt
{
    private static string SecretKey = "AmerAusta2004@RandomChars1234567";

    public static string GenerateToken(UserDTO user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(SecretKey);

        var claims = new[]
        {
            new Claim("UserID", user.UserID.ToString()),
            new Claim(ClaimTypes.Email, user.Email),

            // 🔑 مهم جدًا: نحط اسم الرول مو الرقم
            new Claim(ClaimTypes.Role, user.RoleId.ToString()), // "Admin" أو "Customer"

            // (اختياري) الرقم إذا احتجته
            new Claim("RoleId", ((int)user.RoleId).ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
