namespace Messenger.Application.Interfaces;

public interface IBaseDirService
{
    string GetPathWwwRoot();

    string GetPathAppSettingsJson(bool isDevelopment);
}