using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Proptiwise.ViewModels.Stripe
{
    public class BuyerViewModel
    {
        public int? id { get; set; }
        public string Email { get; set; }

        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Card Number is required")]
        [RegularExpression(@"^((4\d{3})|(5[1-5]\d{2}))(-?|\040?)(\d{4}(-?|\040?)){3}|^(3[4,7]\d{2})(-?|\040?)\d{6}(-?|\040?)\d{5}", ErrorMessage = "Card Number is invalid")]
        public string CardNumber { get; set; }

        [Required(ErrorMessage = "CVC is required")]
        [RegularExpression(@"^(?!000)\d{3,4}$", ErrorMessage = "CVC is invalid")]
        public string CVC { get; set; }

        [Required(ErrorMessage = "Expiration month is required")]
        [MinLength(2, ErrorMessage = "Invalid month length")]
        public string ExpirationMonth { get; set; }

        [Required(ErrorMessage = "Expiration year is required")]
        [MinLength(4, ErrorMessage = "Invalid year length")]
        public string ExpirationYear { get; set; }
    }
}