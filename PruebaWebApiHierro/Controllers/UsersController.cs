using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PruebaWebApiHierro.Data;
using PruebaWebApiHierro.Models;
using PruebaWebApiHierro.Services;
using System.Text.Json;
using BCrypt.Net;

namespace PruebaWebApiHierro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ReniecService _reniec;
        private readonly TwilioVerifyService _verify;

        public UsersController(AppDbContext context, ReniecService reniec, TwilioVerifyService verify)
        {
            _context = context;
            _reniec = reniec;
            _verify = verify;
        }

        // POST: api/Users/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] JsonElement body)
        {
            string dni = body.GetProperty("dni").GetString();
            string password = body.GetProperty("password").GetString();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.DocumentNumber == dni);
            if (user == null)
                return NotFound("Usuario no encontrado.");

            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return Unauthorized("Contraseña incorrecta.");

            return Ok(new
            {
                user.Id,
                user.FirstName,
                user.LastNameP,
                user.LastNameM,
                user.Role
            });
        }


        // GET: api/users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers() =>
            await _context.Users.ToListAsync();

        // GET: api/users/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            return user == null ? NotFound() : user;
        }

        // POST: api/users/dni
        [HttpPost("dni")]
        public async Task<ActionResult<object>> GetFromReniec([FromBody] JsonElement body)
        {
            var dni = body.GetProperty("dni").GetString();
            var data = await _reniec.GetUserDataByDni(dni);
            if (data == null) return NotFound("No encontrado");

            return Ok(new
            {
                firstName = data.Value.FirstName,
                lastNameP = data.Value.LastNameP,
                lastNameM = data.Value.LastNameM
            });
        }

        // POST: api/users/verify/send
        [HttpPost("verify/send")]
        public async Task<IActionResult> SendVerification([FromBody] JsonElement body)
        {
            string phone = body.GetProperty("phone").GetString();
            var result = await _verify.SendVerificationCode(phone);
            return result ? Ok("Código enviado") : StatusCode(500, "Error al enviar el código");
        }

        // POST: api/users/verify/check
        [HttpPost("verify/check")]
        public async Task<IActionResult> CheckVerification([FromBody] JsonElement body)
        {
            string phone = body.GetProperty("phone").GetString();
            string code = body.GetProperty("code").GetString();

            var result = await _verify.CheckVerificationCode(phone, code);
            if (result)
            {
                VerifiedPhones.Phones.Add(phone); // ✅ Marcar como verificado
                return Ok("Teléfono verificado correctamente");
            }

            return BadRequest("Código incorrecto o expirado");
        }


        // POST: api/users
        [HttpPost]
        public async Task<ActionResult<User>> CreateUser(User user)
        {
            // Validar si el DNI ya fue registrado
            bool dniExists = await _context.Users.AnyAsync(u => u.DocumentNumber == user.DocumentNumber);
            if (dniExists)
                return BadRequest("El número de DNI ya está registrado.");

            if (!VerifiedPhones.Phones.Contains(user.Phone))
                return BadRequest("El número no ha sido verificado.");

            // Consultar datos desde RENIEC si no se enviaron nombres
            if (!string.IsNullOrWhiteSpace(user.DocumentNumber))
            {
                var reniecData = await _reniec.GetUserDataByDni(user.DocumentNumber);
                if (reniecData != null)
                {
                    user.FirstName = reniecData.Value.FirstName;
                    user.LastNameP = reniecData.Value.LastNameP;
                    user.LastNameM = reniecData.Value.LastNameM;
                }
            }

            user.IsPhoneVerified = true;
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
            user.CreatedAt = DateTime.UtcNow;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            VerifiedPhones.Phones.Remove(user.Phone); // Limpieza
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }


        // PUT: api/users/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, User user)
        {
            if (id != user.Id) return BadRequest();
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/users/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // POST: api/users/recover-password
        [HttpPost("recover-password")]
        public async Task<IActionResult> RecoverPassword([FromBody] JsonElement body)
        {
            var dni = body.GetProperty("dni").GetString();
            var phone = body.GetProperty("phone").GetString();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.DocumentNumber == dni && u.Phone == phone);
            if (user == null)
                return NotFound("No se encontró un usuario con ese DNI y número.");

            var result = await _verify.SendVerificationCode(phone);
            return result ? Ok("Código enviado") : StatusCode(500, "Error al enviar el código");
        }


        // POST: api/users/reset-password
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] JsonElement body)
        {
            var dni = body.GetProperty("dni").GetString();
            var phone = body.GetProperty("phone").GetString();
            var code = body.GetProperty("code").GetString();
            var newPassword = body.GetProperty("newPassword").GetString();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.DocumentNumber == dni && u.Phone == phone);
            if (user == null) return NotFound("No se encontró al usuario.");

            var verified = await _verify.CheckVerificationCode(phone, code);
            if (!verified) return BadRequest("Código incorrecto o expirado.");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _context.SaveChangesAsync();

            return Ok("Contraseña actualizada correctamente.");
        }


    }




}
