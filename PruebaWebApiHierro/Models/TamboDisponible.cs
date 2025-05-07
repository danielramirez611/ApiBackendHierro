// Models/TamboDisponible.cs
namespace PruebaWebApiHierro.Models
{
    public class TamboDisponible
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Departamento { get; set; }
        public string Provincia { get; set; }
        public string Distrito { get; set; }
        public bool Estado { get; set; }
    }
}
