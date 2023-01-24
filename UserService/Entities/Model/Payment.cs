using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Model
{
    public class Payment : Commonmodel
    {
        [Required]
        [Column(name: "address_type")]
        public string Type { get; set; } = string.Empty;

        [Required]
        [Column(name: "name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Column(name: "payment_value")]
        public string PaymentValue { get; set; } = string.Empty;

        [Required]
        [Column(name: "expirydate")]
        public string? Expiry { get; set; }

        [Required]
        [ForeignKey("UserId")]
        [Column(name: "user_id")]
        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}
