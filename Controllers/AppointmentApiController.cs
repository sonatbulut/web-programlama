using HospitaAppointmentSystem.Data;
using HospitaAppointmentSystem.ViewModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HospitaAppointmentSystem
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Roles = "admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AppointmentApiController : ControllerBase
    {
        private readonly IConfiguration _configuration;
                private readonly DataContext _context;

        private readonly UserManager<AppUser> _userManager; // Assuming you are using Identity

        public AppointmentApiController(IConfiguration configuration, UserManager<AppUser> userManager,DataContext context)
        {
            _configuration = configuration;
            _context = context;
            _userManager = userManager; // Inject UserManager if you are using ASP.NET Core Identity
        }

        [AllowAnonymous]
[HttpPost("GenerateToken")]
public async Task<IActionResult> GenerateToken([FromBody] LoginViewModel model)
{
    if (!ModelState.IsValid)
    {
        return BadRequest(ModelState);
    }

    var user = await _userManager.FindByEmailAsync(model.Email);
    if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
    {
        var roles = await _userManager.GetRolesAsync(user);
        var isAdmin = roles.Contains("admin");

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, isAdmin ? "admin" : "user")
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiry = DateTime.Now.AddDays(Convert.ToDouble(_configuration["Jwt:ExpiryInDays"]));

        var token = new JwtSecurityToken(
            _configuration["Jwt:Issuer"],
            _configuration["Jwt:Audience"],
            claims,
            expires: expiry,
            signingCredentials: creds
        );

        return Ok(new
        {
            token = new JwtSecurityTokenHandler().WriteToken(token),
            expiration = token.ValidTo,
            role = isAdmin ? "admin" : "user"
        });
    }

    return Unauthorized("Credentials are invalid.");
}
        [HttpDelete("appointment/{id}")]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }

            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();

            return NoContent(); // Standard response for a successful delete action
        }
        [HttpDelete("patient/{id}")]
        public async Task<IActionResult> DeletePatient(int id)
        {
            var appointment = await _context.Patients.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }

            _context.Patients.Remove(appointment);
            await _context.SaveChangesAsync();

            return NoContent(); // Standard response for a successful delete action
        }
        [HttpDelete("doctor/{id}")]
        public async Task<IActionResult> DeleteDoctor(int id)
        {
            var appointment = await _context.Doctors.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }

            _context.Doctors.Remove(appointment);
            await _context.SaveChangesAsync();

            return NoContent(); // Standard response for a successful delete action
        }

        // ... Other API methods ...
    }
}
