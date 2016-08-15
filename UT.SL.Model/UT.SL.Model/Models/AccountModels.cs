using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace UT.SL.Model
{
    public class ChangePasswordModel
    {
        public Guid Guid { get; set; }
        
        [Required(ErrorMessageResourceType = typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName = "FieldIsRequired")]
        [DataType(DataType.Password)]
        [Display(Name = "OldPassword", ResourceType = typeof(UT.SL.Model.Resource.Account))]
        public string OldPassword { get; set; }

        [Required(ErrorMessageResourceType = typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName = "FieldIsRequired")]
        [DataType(DataType.Password)]
        [Display(Name = "NewPassword", ResourceType = typeof(UT.SL.Model.Resource.Account))]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "ConfirmPassword", ResourceType = typeof(UT.SL.Model.Resource.Account))]
        [Compare("NewPassword", ErrorMessageResourceName = "NoMathPassword", ErrorMessageResourceType = typeof(UT.SL.Model.Resource.App_Errors))]
        public string ConfirmPassword { get; set; }

    }

    public class ChangeEmailModel
    {
        public Guid Guid { get; set; }


        [Required(ErrorMessageResourceType = typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName = "FieldIsRequired")]
        [DataType(DataType.Password)]
        [Display(Name = "Password", ResourceType = typeof(UT.SL.Model.Resource.Account))]
        public string Password { get; set; }

        [Display(Name = "Email", ResourceType = typeof(UT.SL.Model.Resource.Account))]
        [StringLength(200, ErrorMessageResourceType = typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName = "MaxLength200")]
        [RegularExpression("^[a-z0-9_\\+-]+(\\.[a-z0-9_\\+-]+)*@[a-z0-9-]+(\\.[a-z0-9-]+)*\\.([a-z]{2,4})$", ErrorMessageResourceType = typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName = "EmailFormatError")]
        [Required(ErrorMessageResourceType = typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName = "FieldIsRequired")]
        public string Email { get; set; }

        [Display(Name = "ConfirmEmail", ResourceType = typeof(UT.SL.Model.Resource.Account))]
        [StringLength(200, ErrorMessageResourceType = typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName = "MaxLength200")]
        [RegularExpression("^[a-z0-9_\\+-]+(\\.[a-z0-9_\\+-]+)*@[a-z0-9-]+(\\.[a-z0-9-]+)*\\.([a-z]{2,4})$", ErrorMessageResourceType = typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName = "EmailFormatError")]
        [Compare("Email", ErrorMessageResourceName = "NoMatchEmail", ErrorMessageResourceType = typeof(UT.SL.Model.Resource.App_Errors))]
        public string ConfirmEmail { get; set; }

    }


    public class DeleteAccountModel
    {
      
        public Guid Guid { get; set; }

        [Required(ErrorMessageResourceType = typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName = "FieldIsRequired")]
        [DataType(DataType.Password)]
        [Display(Name = "Password", ResourceType = typeof(UT.SL.Model.Resource.Account))]
        public string Password { get; set; }
    }

    public class LogInModel
    {
        [Required(ErrorMessageResourceType = typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName = "FieldIsRequired")]
        [Display(Name = "UserName", ResourceType = typeof(UT.SL.Model.Resource.Account))]
        public string UserName { get; set; }

        [Required(ErrorMessageResourceType = typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName = "FieldIsRequired")]
        [DataType(DataType.Password)]
        [Display(Name = "Password", ResourceType = typeof(UT.SL.Model.Resource.Account))]
        public string Password { get; set; }

        [Display(Name = "RememberMe", ResourceType = typeof(UT.SL.Model.Resource.Account))]
        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }
    }

    public class RegisterModel
    {
        [Display(Name = "UserName", ResourceType = typeof(UT.SL.Model.Resource.Account))]
        [StringLength(200, ErrorMessageResourceType = typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName = "MaxLength200")]
        [Required(ErrorMessageResourceType = typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName = "FieldIsRequired")]
        public string UserName { get; set; }

        [Display(Name = "FirstName", ResourceType = typeof(UT.SL.Model.Resource.Account))]
        [StringLength(200, ErrorMessageResourceType = typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName = "MaxLength200")]
        public string FirstName { get; set; }

        [Display(Name = "LastName", ResourceType = typeof(UT.SL.Model.Resource.Account))]
        [StringLength(200, ErrorMessageResourceType = typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName = "MaxLength200")]
        public string LastName { get; set; }
     
        [Display(Name = "Email", ResourceType = typeof(UT.SL.Model.Resource.Account))]
        [StringLength(200, ErrorMessageResourceType = typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName = "MaxLength200")]
        [RegularExpression("^[a-z0-9_\\+-]+(\\.[a-z0-9_\\+-]+)*@[a-z0-9-]+(\\.[a-z0-9-]+)*\\.([a-z]{2,4})$", ErrorMessageResourceType = typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName = "EmailFormatError")]
        [Required(ErrorMessageResourceType = typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName = "FieldIsRequired")]
        public string Email { get; set; }

        [Display(Name = "ConfirmEmail", ResourceType = typeof(UT.SL.Model.Resource.Account))]
        [StringLength(200, ErrorMessageResourceType = typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName = "MaxLength200")]
        [RegularExpression("^[a-z0-9_\\+-]+(\\.[a-z0-9_\\+-]+)*@[a-z0-9-]+(\\.[a-z0-9-]+)*\\.([a-z]{2,4})$", ErrorMessageResourceType = typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName = "EmailFormatError")]
        [Compare("Email", ErrorMessageResourceName = "NoMatchEmail", ErrorMessageResourceType = typeof(UT.SL.Model.Resource.App_Errors))]
        public string ConfirmEmail { get; set; }

        [Display(Name = "Password", ResourceType = typeof(UT.SL.Model.Resource.Account))]
        [StringLength(200, ErrorMessageResourceType = typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName = "MaxLength200")]
        [DataType(DataType.Password)]
        [Required(ErrorMessageResourceType = typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName = "FieldIsRequired")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "ConfirmPassword", ResourceType = typeof(UT.SL.Model.Resource.Account))]
        [Compare("Password", ErrorMessageResourceName = "NoMathPassword", ErrorMessageResourceType = typeof(UT.SL.Model.Resource.App_Errors))]
        public string ConfirmPassword { get; set; }
        
    }

    
}
