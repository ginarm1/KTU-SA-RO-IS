using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using KTU_SA_RO.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KTU_SA_RO.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            //[Phone]
            //[Display(Name = "Phone number")]
            //public string PhoneNumber { get; set; }

            [Display(Name = "Vardas")]
            public string Name { get; set; }
            [Display(Name = "Pavardė")]
            public string Surname { get; set; }

            [Display(Name = "Atstovybė")]
            public Representative Representative { get; set; }
        }

        private void Load(ApplicationUser user)
        {
            //var userName = await _userManager.GetUserNameAsync(user);
            //var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            //Username = userName;

            var name = user.Name;
            var surname = user.Surname;
            var representative = user.Representative;

            Input = new InputModel
            {
                //PhoneNumber = phoneNumber
                Name = name,
                Surname = surname,
                Representative = representative
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var userRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault();

            userRole = SetUserRole(userRole);
            if (userRole == null)
            {
                TempData["danger"] = "Naudotojo rolė nerasta";
                userRole = "rolė nerasta";
            }
            ViewData["userRole"] = userRole;
            Load(user);
            return Page();
        }

        public string SetUserRole(string positionName)
        {
            if (positionName.Equals("registered"))
                return "Registruotas naudotojas";
            if (positionName.Equals("eventCoord"))
                return "Renginio koordinatorius";
            if (positionName.Equals("fsaOrgCoord"))
                return "ORK koordinatorius";
            if (positionName.Equals("fsaBussinesCoord"))
                return "VIP koordinatorius";
            if (positionName.Equals("fsaPrCoord"))
                return "RSV koordinatorius";
            if (positionName.Equals("orgCoord"))
                return "CSA ORK koordinatorius";
            if (positionName.Equals("admin"))
                return "Administratorius";
            else
                return null;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var name = user.Name;
            var surname = user.Surname;
            var representative = user.Representative;


            if (Input.Name != name)
            {
                user.Name = Input.Name;
                await _userManager.UpdateAsync(user);
            }
            if (Input.Surname != surname)
            {
                user.Surname = Input.Surname;
                await _userManager.UpdateAsync(user);
            }
            if (Input.Representative != representative)
            {
                user.Representative = Input.Representative;
                await _userManager.UpdateAsync(user);
            }

            if (!ModelState.IsValid)
            {
                Load(user);
                return Page();
            }



            //var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            //if (Input.PhoneNumber != phoneNumber)
            //{
            //    var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
            //    if (!setPhoneResult.Succeeded)
            //    {
            //        StatusMessage = "Unexpected error when trying to set phone number.";
            //        return RedirectToPage();
            //    }
            //}

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Jūsų profilis buvo sėkmingai atnaujintas";
            return RedirectToPage();
        }
    }
}
