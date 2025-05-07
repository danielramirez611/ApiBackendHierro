namespace PruebaWebApiHierro.Models
{
    public class Paciente
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }  // ✅ ahora es opcional

        public bool TieneAnemia { get; set; } = false;

        public ICollection<Contacto> Contactos { get; set; } = new List<Contacto>();

    }
}
