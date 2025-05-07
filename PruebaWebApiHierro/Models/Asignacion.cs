namespace PruebaWebApiHierro.Models
{
    public class Asignacion
    {
        public int Id { get; set; }
        public int GestorId { get; set; }
        public int TamboId { get; set; }
        public DateTime FechaAsignacion { get; set; } = DateTime.Now;
        public string Departamento { get; set; }
        public string Provincia { get; set; }
        public string Distrito { get; set; }
        public string? CentroPoblado { get; set; }

        public bool Estado { get; set; } = true;


        // ✅ Propiedades de navegación
        public User? Gestor { get; set; }
        public Tambo? Tambo { get; set; }
    }


}
