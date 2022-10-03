using System.ComponentModel.DataAnnotations;

namespace Messenger.BusinessLogic.Models.RequestResponse;

public class LoginRequest
{
	[StringLength(20, MinimumLength = 4, ErrorMessage = "Acceptable range {0} from {2} to {1}.")]
	[Required]
	public string NickName { get; set; } = null!;
	
	[StringLength(20, MinimumLength = 7, ErrorMessage = "Acceptable range {0} from {2} to {1}.")]
	[Required(AllowEmptyStrings = false)]
	public string Password { get; set; } = null!;
}