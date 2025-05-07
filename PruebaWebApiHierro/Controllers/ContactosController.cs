using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PruebaWebApiHierro.Data;
using PruebaWebApiHierro.Models;
using PruebaWebApiHierro.Services;

namespace PruebaWebApiHierro.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactosController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ReniecService _reniec;

        public ContactosController(AppDbContext context, ReniecService reniec)
        {
            _context = context;
            _reniec = reniec;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetAll()
        {
            var contactos = await _context.Contactos
                .Include(c => c.Paciente)
                    .ThenInclude(p => p.User)
                .Select(c => new
                {
                    c.Id,
                    c.TipoDocumento,
                    c.Documento,
                    c.Telefono,
                    c.ApellidoPaterno,
                    c.ApellidoMaterno,
                    c.NombreCompleto,
                    c.FechaNacimiento,
                    c.Genero,
                    c.Direccion,
                    c.Parentesco,
                    c.Notificaciones,
                    c.PacienteId,
                    Paciente = new
                    {
                        c.Paciente.Id,
                        c.Paciente.TieneAnemia,
                        Usuario = c.Paciente.User != null ? new
                        {
                            c.Paciente.User.Id,
                            Nombre = $"{c.Paciente.User.FirstName} {c.Paciente.User.LastNameP}"
                        } : null
                    }
                })
                .ToListAsync();

            return Ok(contactos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetById(int id)
        {
            var contacto = await _context.Contactos
                .Include(c => c.Paciente)
                    .ThenInclude(p => p.User)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (contacto == null) return NotFound();

            return Ok(new
            {
                contacto.Id,
                contacto.TipoDocumento,
                contacto.Documento,
                contacto.Telefono,
                contacto.ApellidoPaterno,
                contacto.ApellidoMaterno,
                contacto.NombreCompleto,
                contacto.FechaNacimiento,
                contacto.Genero,
                contacto.Direccion,
                contacto.Parentesco,
                contacto.Notificaciones,
                contacto.PacienteId,
                Paciente = new
                {
                    contacto.Paciente?.Id,
                    contacto.Paciente?.TieneAnemia,
                    Usuario = contacto.Paciente?.User != null ? new
                    {
                        contacto.Paciente.User.Id,
                        Nombre = $"{contacto.Paciente.User.FirstName} {contacto.Paciente.User.LastNameP}"
                    } : null
                }
            });
        }

        [HttpPost]
        public async Task<ActionResult<Contacto>> Create(Contacto contacto)
        {
            var paciente = await _context.Pacientes.FindAsync(contacto.PacienteId);
            if (paciente == null)
            {
                return BadRequest("El contacto debe estar asociado a un paciente válido.");
            }

            var existe = await _context.Contactos
                .AnyAsync(c => c.Documento == contacto.Documento && c.PacienteId == contacto.PacienteId);
            if (existe)
            {
                return BadRequest("Ya existe un contacto con este documento para el paciente seleccionado.");
            }

            if (contacto.TipoDocumento == TipoDocumento.DNI)
            {
                var datos = await _reniec.GetUserDataByDni(contacto.Documento);
                if (datos != null)
                {
                    contacto.ApellidoPaterno = datos.Value.LastNameP;
                    contacto.ApellidoMaterno = datos.Value.LastNameM;
                    contacto.NombreCompleto = datos.Value.FirstName;
                }
            }

            _context.Contactos.Add(contacto);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = contacto.Id }, contacto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Contacto contacto)
        {
            if (id != contacto.Id) return BadRequest();

            var paciente = await _context.Pacientes.FindAsync(contacto.PacienteId);
            if (paciente == null)
            {
                return BadRequest("El paciente asociado no existe.");
            }

            _context.Entry(contacto).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var contacto = await _context.Contactos.FindAsync(id);
            if (contacto == null) return NotFound();

            _context.Contactos.Remove(contacto);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("pacientes-disponibles")]
        public async Task<IActionResult> GetPacientesPermitidos()
        {
            var pacientes = await _context.Pacientes
                .Include(p => p.User)
                .Select(p => new
                {
                    p.Id,
                    Nombre = $"{p.User.LastNameP} {p.User.LastNameM}, {p.User.FirstName}"
                })
                .ToListAsync();

            return Ok(pacientes);
        }
    }
}
