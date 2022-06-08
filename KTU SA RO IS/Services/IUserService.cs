﻿using KTU_SA_RO.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KTU_SA_RO.Services
{
    public interface IUserService
    {
        /// <summary>
        /// Get users role
        /// </summary>
        /// <returns></returns>
        public Task<Dictionary<string, IdentityRole>> GetUsersRole();
        /// <summary>
        /// Get one user role by his email
        /// </summary>
        public Task<string> GetUserRole(string email);
        /// <summary>
        /// Add new user role
        /// </summary>
        public void AddNewUserRole(ApplicationUser tempUser, string roleId);
    }
}
