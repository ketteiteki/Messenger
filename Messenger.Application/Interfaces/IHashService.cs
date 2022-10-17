
namespace Messenger.Application.Interfaces;

public interface IHashService
{
	public string Hmacsha512CryptoHash(string value, out string salt);

	public string Hmacsha512CryptoHashWithSalt(string value, string salt);
}