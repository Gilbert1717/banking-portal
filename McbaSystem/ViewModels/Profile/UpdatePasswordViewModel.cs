using McbaSystem.Models;

namespace McbaSystem.ViewModels.Profile;

public class UpdatePasswordViewModel
{
    public Login Login { get; set; }
    public string OldPassword { get; set; }
    public string NewPassword { get; set; }
    public string PasswordConfirm { get; set; }
}