using UnityEngine;

namespace Party
{
    [CreateAssetMenu (menuName = "New Party Member")]
    public class PartyMemberInfo : ScriptableObject
    {
        public GameObject memberBattleVisualPrefab;
        public GameObject memberOverworldVisualPrefab;
        public string memberName;
        public int startingLevel;
        public int baseHealth;
        public int baseStr;
        public int baseInitiative;
    }
}
