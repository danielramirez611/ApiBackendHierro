namespace PruebaWebApiHierro.Models
{
    public enum TipoDocumento
    {
        DNI
    }

    public enum Parentesco
    {
        Madre,
        Padre,
        Tutor,
        Hermano
    }

    public enum Notificacion
    {
        SMS,
        Correo,
        WhatsApp,
        NotificacionApp
    }

    public enum Genero
    {
        Masculino,
        Femenino
    }

    public class Contacto
    {
        public int Id { get; set; }

        public TipoDocumento TipoDocumento { get; set; }
        public string Documento { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;

        public string ApellidoPaterno { get; set; } = string.Empty;
        public string ApellidoMaterno { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;

        public DateTime? FechaNacimiento { get; set; }
        public Genero Genero { get; set; }
        public string? Direccion { get; set; }

        public Parentesco Parentesco { get; set; }
        public Notificacion Notificaciones { get; set; }

        // Asociación con un paciente (solo roles Niño o Familiar)
        public int PacienteId { get; set; } // ahora se asocia a un paciente
        public Paciente? Paciente { get; set; }

    }
}
