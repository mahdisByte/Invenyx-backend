using InventoryManagementAPI.DTOs;
using InventoryManagementAPI.Data;
using InventoryManagementAPI.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


public interface IAuthService
{
    string Register(User user);
    object Login(LoginRequest request);
}