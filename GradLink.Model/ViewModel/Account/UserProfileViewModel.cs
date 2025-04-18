using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace GradLink.Model.ViewModel.Account
{
    public class UserProfileViewModel
    {
        public string UserName { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        public string Phone { get; set; }

        public string Address { get; set; }

        //public HttpPostedFileBase ProfileImage { get; set; }

        public string ExistingProfileImagePath { get; set; }
    }
}
