namespace UwvLlm.App.Core.Interfaces;

public interface IUiService
{
    Task ShowAlertAsync(string title, string message, string cancel);
}
