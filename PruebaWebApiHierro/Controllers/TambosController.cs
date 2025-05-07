using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PruebaWebApiHierro.Data;
using PruebaWebApiHierro.Models;
using PruebaWebApiHierro.Services;
using System.Text.Json;

namespace PruebaWebApiHierro.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TambosController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly HttpClient _http;
        private readonly ReniecService _reniec;

        public TambosController(AppDbContext context, IHttpClientFactory factory, ReniecService reniec)
        {
            _context = context;
            _http = factory.CreateClient();
            _reniec = reniec;
        }

        [HttpPost]
        public async Task<ActionResult<Tambo>> Create(Tambo tambo)
        {
            // Consultar API de RENIEC si se proporciona DNI
            if (!string.IsNullOrWhiteSpace(tambo.DocumentoRepresentante))
            {
                var info = await _reniec.GetUserDataByDni(tambo.DocumentoRepresentante);
                if (info is { } datos)
                {
                    var (firstName, lastNameP, lastNameM) = datos;
                    tambo.Representante = $"{lastNameP} {lastNameM} {firstName}";
                }

            }

            _context.Tambos.Add(tambo);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = tambo.Id }, tambo);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tambo>>> GetAll()
        {
            return await _context.Tambos.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Tambo>> GetById(int id)
        {
            var tambo = await _context.Tambos.FindAsync(id);
            return tambo == null ? NotFound() : tambo;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Tambo tambo)
        {
            if (id != tambo.Id) return BadRequest();

            _context.Entry(tambo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Tambos.Any(e => e.Id == id)) return NotFound();
                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var tambo = await _context.Tambos.FindAsync(id);
            if (tambo == null) return NotFound();

            _context.Tambos.Remove(tambo);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // --- UBIGEO ---
        private async Task<List<Ubigeo>> ObtenerUbigeos()
        {
            var json = await _http.GetStringAsync("https://free.e-api.net.pe/ubigeos.json");
            using var doc = JsonDocument.Parse(json);

            var ubigeos = new List<Ubigeo>();
            foreach (var dep in doc.RootElement.EnumerateObject())
            {
                string departamento = dep.Name;
                foreach (var prov in dep.Value.EnumerateObject())
                {
                    string provincia = prov.Name;
                    foreach (var dist in prov.Value.EnumerateObject())
                    {
                        string distrito = dist.Name;
                        ubigeos.Add(new Ubigeo
                        {
                            departamento = departamento,
                            provincia = provincia,
                            distrito = distrito
                        });
                    }
                }
            }
            return ubigeos;
        }

        [HttpGet("departamentos")]
        public async Task<IActionResult> GetDepartamentos()
        {
            var ubigeos = await ObtenerUbigeos();
            var departamentos = ubigeos.Select(u => u.departamento).Distinct().OrderBy(x => x).ToList();
            return Ok(departamentos);
        }

        [HttpGet("provincias/{departamento}")]
        public async Task<IActionResult> GetProvincias(string departamento)
        {
            var ubigeos = await ObtenerUbigeos();
            var provincias = ubigeos.Where(u => u.departamento.Equals(departamento, StringComparison.OrdinalIgnoreCase))
                                    .Select(u => u.provincia).Distinct().OrderBy(x => x).ToList();
            return Ok(provincias);
        }

        [HttpGet("distritos/{departamento}/{provincia}")]
        public async Task<IActionResult> GetDistritos(string departamento, string provincia)
        {
            var ubigeos = await ObtenerUbigeos();
            var distritos = ubigeos.Where(u =>
                u.departamento.Equals(departamento, StringComparison.OrdinalIgnoreCase) &&
                u.provincia.Equals(provincia, StringComparison.OrdinalIgnoreCase))
                .Select(u => u.distrito).Distinct().OrderBy(x => x).ToList();
            return Ok(distritos);
        }
    }

    public class Ubigeo
    {
        public string departamento { get; set; }
        public string provincia { get; set; }
        public string distrito { get; set; }
    }
}
