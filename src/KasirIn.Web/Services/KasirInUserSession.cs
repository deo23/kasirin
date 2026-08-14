namespace KasirIn.Web.Services;

public class KasirInUserSession
{
    public string CurrentRole { get; private set; } = "OWNER";

    public bool IsOwner => CurrentRole == "OWNER";
    public bool IsCashier => CurrentRole == "CASHIER";

    public event Action? OnRoleChanged;

    public void SwitchRole(string newRole)
    {
        if (CurrentRole != newRole)
        {
            CurrentRole = newRole;
            OnRoleChanged?.Invoke();
        }
    }
}
