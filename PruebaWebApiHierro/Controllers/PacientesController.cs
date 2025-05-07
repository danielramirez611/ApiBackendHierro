using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PruebaWebApiHierro.Data;
using PruebaWebApiHierro.Models;
using static PruebaWebApiHierro.Models.User;

namespace PruebaWebApiHierro.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PacientesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PacientesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetAll()
        {
            var pacientes = await _context.Pacientes
                .Include(p => p.User)
                .Include(p => p.Contactos)
                .Select(p => new
                {
                    p.Id,
                    p.TieneAnemia,
                    Usuario = p.User != null ? new
                    {
                        p.User.Id,
                        p.User.FirstName,
                        p.User.LastNameP,
                        p.User.LastNameM,
                        p.User.DocumentNumber,
                        p.User.Email,
                        p.User.Phone,
                        p.User.Role
                    } : null,
                    Contactos = p.Contactos.Select(c => new
                    {
                        c.Id,
                        c.NombreCompleto,
                        c.Documento,
                        c.Parentesco,
                        c.Telefono,
                        c.Notificaciones
                    })
                })
                .ToListAsync();

            return Ok(pacientes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetById(int id)
        {
            var paciente = await _context.Pacientes
                .Include(p => p.User)
                .Include(p => p.Contactos)
                .Select(p => new
                {
                    p.Id,
                    p.TieneAnemia,
                    Usuario = p.User != null ? new
                    {
                        p.User.Id,
                        p.User.FirstName,
                        p.User.LastNameP,
                        p.User.LastNameM,
                        p.User.DocumentNumber,
                        p.User.Email,
                        p.User.Phone,
                        p.User.Role
                    } : null,
                    Contactos = p.Contactos.Select(c => new
                    {
                        c.Id,
                        c.NombreCompleto,
                        c.Documento,
                        c.Parentesco,
                        c.Telefono,
                        c.Notificaciones
                    })
                })
                .FirstOrDefaultAsync(p => p.Id == id);

            return paciente == null ? NotFound() : Ok(paciente);
        }

        [HttpPost]
        public async Task<ActionResult<Paciente>> Create(Paciente paciente)
        {
            var user = await _context.Users.FindAsync(paciente.UserId);
            if (user == null || (user.Role != TipoRole.Niño && user.Role != TipoRole.Gestante))
            {
                return BadRequest("El usuario debe tener rol Niño o Gestante.");
            }

            _context.Pacientes.Add(paciente);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = paciente.Id }, paciente);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Paciente paciente)
        {
            if (id != paciente.Id) return BadRequest();

            var user = await _context.Users.FindAsync(paciente.UserId);
            if (user == null || (user.Role != TipoRole.Niño && user.Role != TipoRole.Gestante))
            {
                return BadRequest("El usuario debe tener rol Niño o Gestante.");
            }

            _context.Entry(paciente).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var paciente = await _context.Pacientes.FindAsync(id);
            if (paciente == null) return NotFound();

            _context.Pacientes.Remove(paciente);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("usuarios-pacientes")]
        public async Task<IActionResult> GetUsuariosPacientes()
        {
            var usuarios = await _context.Users
                .Where(u => u.Role == TipoRole.Niño || u.Role == TipoRole.Gestante)
                .Select(u => new
                {
                    u.Id,
                    Nombre = $"{u.LastNameP} {u.LastNameM}, {u.FirstName}"
                }).ToListAsync();

            return Ok(usuarios);
        }
    }
}
