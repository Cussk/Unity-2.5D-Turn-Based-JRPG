using UnityEngine;

namespace Interfaces
{
    public interface IEntity
    {
        GameObject GetBattleVisualsPrefab();
        string GetMemberName();
        int GetLevel();
        int GetCurrentHealth();
        int GetMaxHealth();
        int GetStrength();
        int GetInitiative();
    }
}
