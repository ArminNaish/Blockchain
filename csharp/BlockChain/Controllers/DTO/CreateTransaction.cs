using System;
using System.ComponentModel.DataAnnotations;

namespace BlockChain.Controllers.DTO
{
    public class CreateTransaction
    {
        [Required]
        public Guid Sender { get; set; }
        [Required]
        public Guid Recepient { get; set; }
        [Required]
        public long Amount { get; set; }
    }
}