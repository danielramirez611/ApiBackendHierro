using Microsoft.AspNetCore.Mvc;
using PruebaWebApiHierro.Data;
using PruebaWebApiHierro.Models;
using Microsoft.EntityFrameworkCore;

namespace PruebaWebApiHierro.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AsignacionesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AsignacionesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("gestores")]
        public async Task<IActionResult> GetAllGestores()
        {
            var gestores = await _context.Users
                .Where(u => u.Role == PruebaWebApiHierro.Models.User.TipoRole.Gestor)
                .Select(u => new {
                    u.Id,
                    u.FirstName,
                    u.LastNameP,
                    u.LastNameM
                })
                .ToListAsync();

            return Ok(gestores);
        }

        [HttpGet("/api/Tambos/all")]
        public async Task<IActionResult> GetAllTambos()
        {
            var tambos = await _context.Tambos
                .Select(t => new {
                    t.Id,
                    t.Name
                })
                .ToListAsync();

            return Ok(tambos);
        }


        // GET: api/Asignaciones
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Asignacion>>> GetAll()
        {
            return await _context.Asignaciones.ToListAsync();
        }

        // GET: api/Asignaciones/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Asignacion>> GetById(int id)
        {
            var asignacion = await _context.Asignaciones.FindAsync(id);
            if (asignacion == null) return NotFound();
            return asignacion;
        }

        // POST: api/Asignaciones
        [HttpPost]
        public async Task<IActionResult> Asignar([FromBody] Asignacion asignacion)
        {
            var tambo = await _context.Tambos.FindAsync(asignacion.TamboId);

            if (tambo == null)
                return NotFound("Tambo no encontrado.");

            // Verifica si el tambo ya tiene asignación activa
            var existe = await _context.Asignaciones
                .AnyAsync(a => a.TamboId == asignacion.TamboId && a.Estado);

            if (existe)
                return BadRequest("Este tambo ya tiene un gestor asignado.");

            // Copia automáticamente los datos del tambo
            asignacion.Departamento = tambo.Departamento;
            asignacion.Provincia = tambo.Provincia;
            asignacion.Distrito = tambo.Distrito;
            asignacion.FechaAsignacion = DateTime.Now;

            _context.Asignaciones.Add(asignacion);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetGestoresDisponibles), new { id = asignacion.Id }, asignacion);
        }


        // PUT: api/Asignaciones/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Asignacion asignacion)
        {
            if (id != asignacion.Id) return BadRequest();

            _context.Entry(asignacion).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Asignaciones.Any(a => a.Id == id))
                    return NotFound();

                throw;
            }

            return NoContent();
        }

        // DELETE: api/Asignaciones/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var asignacion = await _context.Asignaciones.FindAsync(id);
            if (asignacion == null) return NotFound();

            // Soft delete
            asignacion.Estado = false;
            await _context.SaveChangesAsync();

            return NoContent();
        }
        [HttpGet("extendidas")]
        public async Task<IActionResult> GetAsignacionesExtendidas()
        {
            var asignaciones = await _context.Asignaciones
                .Include(a => a.Tambo)
                .Include(a => a.Gestor)
                .Select(a => new {
                    a.Id,
                    a.GestorId,
                    a.TamboId,
                    a.FechaAsignacion,
                    a.Departamento,
                    a.Provincia,
                    a.Distrito,
                    a.CentroPoblado,
                    a.Estado,
                    gestorNombre = a.Gestor.FirstName + " " + a.Gestor.LastNameP + " " + a.Gestor.LastNameM,
                    tamboNombre = a.Tambo.Name
                })
                .ToListAsync();

            return Ok(asignaciones);
        }

        // Tambos filtrados
        [HttpGet("tambos-disponibles")]
        public async Task<IActionResult> GetTambosDisponibles([FromQuery] string? departamento, [FromQuery] string? provincia, [FromQuery] string? distrito)
        {
            var query = _context.Tambos
                .Where(t => t.Estado) // solo activos
                .Where(t => !_context.Asignaciones
                    .Any(a => a.TamboId == t.Id && a.Estado)); // sin asignación activa

            if (!string.IsNullOrWhiteSpace(departamento))
                query = query.Where(t => t.Departamento == departamento);
            if (!string.IsNullOrWhiteSpace(provincia))
                query = query.Where(t => t.Provincia == provincia);
            if (!string.IsNullOrWhiteSpace(distrito))
                query = query.Where(t => t.Distrito == distrito);

            var tambos = await query
                .Select(t => new {
                    t.Id,
                    t.Name,
                    t.Departamento,
                    t.Provincia,
                    t.Distrito
                })
                .ToListAsync();

            return Ok(tambos);
        }


        // Gestores filtrados
        [HttpGet("gestores-disponibles")]
        public async Task<IActionResult> GetGestoresDisponibles()
        {
            var gestores = await _context.GestoresDisponibles.ToListAsync();
            return Ok(gestores);
        }
    }
}
