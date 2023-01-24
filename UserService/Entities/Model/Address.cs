using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Model
{
    public class Address : Commonmodel
    {
       
        [Required]
        [Column(name: "name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Column(name: "line1")]
        public string Line1 { get; set; } = string.Empty;

        [Required]
        [Column(name: "line2")]
        public string Line2 { get; set; } = string.Empty;

        [Required]
        [Column(name: "city")]
        public string City { get; set; } = string.Empty;

        [Required]
        [Column(name: "pincode")]
        public int Pincode { get; set; }

        [Required]
        [Column(name: "state")]
        public string State { get; set; } = string.Empty;

        [Required]
        [Column(name: "country")]
        public string Country { get; set; } = string.Empty;

        [Required]
        [Column(name: "address_type")]
        public string Type { get; set; } = string.Empty;

        [Required]
        [Column(name: "phone")]
        public long Phone { get; set; }

        [Required]
        [ForeignKey("UserId")]
        [Column(name: "user_id")]
        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}
