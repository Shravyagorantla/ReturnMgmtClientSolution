using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ReturnMgmtClientProject.Models.View_Models
{
    public class PaymentModel
    {
        public int RequestId { get; set; }

        [Display(Name = "CreditCardNumber")]
        [Required]
        [DataType(DataType.CreditCard)]
        [MaxLength(16, ErrorMessage = " Length should be 16")]
        public long CreditCardNumber { get; set; }
        public int CreditLimit { get; set; }
    }
}
