using System;
using System.Collections.Generic;

namespace PruebaWebApiHierro.Models;

public class User
{
    public enum TipoRole
    {
        Administrador,
        Gestor,
        Gestante,
        Niño
    }

    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastNameP { get; set; }
    public string LastNameM { get; set; }
    public string DocumentNumber { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }

    // Rol fijo: Admin, Gestor, Familiar, Niño, Gestante
    public TipoRole Role { get; set; }

    public DateTime? BirthDate { get; set; }
    public string Gender { get; set; }
    public string Address { get; set; }
    public bool IsPhoneVerified { get; set; } = false;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
