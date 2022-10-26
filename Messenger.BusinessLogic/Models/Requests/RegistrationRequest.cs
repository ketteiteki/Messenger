using System.ComponentModel.DataAnnotations;

namespace Messenger.BusinessLogic.Models.Requests;

public class RegistrationRequest
{
	[StringLength(20, MinimumLength = 4, ErrorMessage = "Acceptable range {0} from {2} to {1}.")]
	[Required]
	public string NickName { get; set; } = null!;
	
	[StringLength(20, MinimumLength = 4, ErrorMessage = "Acceptable range {0} from {2} to {1}.")]
	[Required]
	public string DisplayName { get; set; } = null!;

	[StringLength(20, MinimumLength = 7, ErrorMessage = "Acceptable range {0} from {2} to {1}.")]
	[Required(AllowEmptyStrings = false)]
	public string Password { get; set; } = null!;

	[Compare("Password")]
	[Required]
	public string PasswordConfirm { get; set; } = null!;
}