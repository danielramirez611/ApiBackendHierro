namespace PruebaWebApiHierro.Models
{
    public enum TipoTambo
    {
        Temporal,
        Movil,
        Permanente
    }

    public class Tambo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }

        public string Departamento { get; set; }
        public string Provincia { get; set; }
        public string Distrito { get; set; }

        public string Direccion { get; set; }
        public string? Referencia { get; set; }
        public string? HorarioAtencion { get; set; }

        public TipoTambo Tipo { get; set; } // Usamos el enum aquí 👈

        public string Representante { get; set; }
        public string DocumentoRepresentante { get; set; }
        public string Telefono { get; set; }

        public bool Estado { get; set; } = true;
    }

}
