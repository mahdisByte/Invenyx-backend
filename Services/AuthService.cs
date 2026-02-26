using InventoryManagementAPI.Data;
using InventoryManagementAPI.DTOs;
using InventoryManagementAPI.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(AppDbContext context,
                       IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public string Register(User user)
    {
        if (_context.Users.Any(u => u.Email == user.Email))
            throw new Exception("User already exists");

        _context.Users.Add(user);
        _context.SaveChanges();

        return "Registration successful";
    }

    public object Login(LoginRequest request)
    {
        var user = _context.Users
            .FirstOrDefault(u =>
                u.Email == request.Email &&
                u.Password == request.Password);

        if (user == null)
            throw new Exception("Invalid credentials");

        var token = GenerateJwt(user);

        return new
        {
            token,
            role = user.Role,
            name = user.Name
        };
    }

    private string GenerateJwt(User user)
    {
        var jwtSettings = _configuration.GetSection("Jwt");
        var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(2),
            Issuer = jwtSettings["Issuer"],
            Audience = jwtSettings["Audience"],
            SigningCredentials =
                new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256)
        };

        var handler = new JwtSecurityTokenHandler();
        var token = handler.CreateToken(descriptor);

        return handler.WriteToken(token);
    }
}

