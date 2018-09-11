using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Proptiwise.ViewModels
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Enter Email")]
        [RegularExpression(@"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$", ErrorMessage = "Email address must contain at least @ and dot")]
        public string Email { get; set; }
        public string Password { get; set; }
        public bool rememberme { get; set; }
    }
}