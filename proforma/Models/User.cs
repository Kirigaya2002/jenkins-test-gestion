using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace proforma.Models
{
    public partial class User
    {
        public ulong Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public bool? Active { get; set; }

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [Column("deleted_at")]
        public DateTime? DeletedAt { get; set; }

        public virtual ICollection<Organization> Organizations { get; set; } = new List<Organization>(); //(nuevo)
        public virtual ICollection<Usersession> Usersessions { get; set; } = new List<Usersession>();
        public virtual ICollection<SystemConfiguration> SystemConfigurations { get; set; } = new List<SystemConfiguration>();
        public virtual ICollection<PrintingTemplate> PrintingTemplates { get; set; } = new List<PrintingTemplate>();
    }
}
