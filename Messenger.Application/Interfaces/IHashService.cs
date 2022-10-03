
namespace Messenger.Application.Interfaces;

public interface IHashService
{
	public string HMACSHA512CryptoHash(string value, out string salt);

	public string HMACSHA512CryptoHashWithSalt(string value, string salt);
}