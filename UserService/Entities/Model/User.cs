using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Model
{
    public class User : Commonmodel
    {
        [Required]
        [Column(name: "first_name")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [Column(name: "last_name")]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [Column(name: "email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Column(name: "pwd")]
        public string Password { get; set; }

        [Required]
        [Column(name: "phone")]
        public long Phone { get; set; }

        [Required]
        [Column(name: "user_role")]
        public string Role { get; set; } = string.Empty;

    }
}
