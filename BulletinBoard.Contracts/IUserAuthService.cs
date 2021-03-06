using System.Threading.Tasks;
using BulletinBoard.Entities.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace BulletinBoard.Contracts
{
    public interface IUserAuthService
    {
        Task<IdentityResult> RegisterNewUserAsync(UserForRegistrationViewModel model);
        Task<bool> SignInUserAsync(UserForLogInViewModel model, string returnUrl);
        Task SignOutUserAsync();
        Task<string> GetPasswordResetTokenAsync(PasswordForgotViewModel forgotPasswordModel);
        Task<IdentityResult> ResetUserPasswordAsync(PasswordResetViewModel model);
    }
}