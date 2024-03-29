namespace Interfaces
{
    public interface IEntity
    {
        string GetMemberName();
        int GetLevel();
        int GetCurrentHealth();
        int GetMaxHealth();
        int GetStrength();
        int GetInitiative();
    }
}
