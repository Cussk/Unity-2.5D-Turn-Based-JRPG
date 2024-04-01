using System.Collections.Generic;
using UnityEngine;

namespace Characters.Party
{
    public class PartyManager : MonoBehaviour
    {
        [SerializeField] PartyMemberInfo[] allPartyMembers;
        [SerializeField] List<PartyMember> currentPartyMembers;
        [SerializeField] PartyMemberInfo defaultPartyMember;

        void Awake()
        {
            AddMemberToPartyByName(defaultPartyMember.memberName);
        }

        void AddMemberToPartyByName(string memberName)
        {
            for (var i = 0; i < allPartyMembers.Length; i++)
            {
                if (allPartyMembers[i].memberName != memberName) continue;
            
                var newPartyMember = new PartyMember(allPartyMembers[i]);
                currentPartyMembers.Add(newPartyMember);
            }
        }

        public List<PartyMember> GetPartyMembers()
        {
            return currentPartyMembers;
        }
    }

    [System.Serializable]
    public class PartyMember : IEntity
    {
        public GameObject memberBattleVisualPrefab;
        public GameObject memberOverworldVisualPrefab;
        public string memberName;
        public int level;
        public int currentHealth;
        public int maxHealth;
        public int strength;
        public int initiative;
        public int currentExp;
        public int maxExp;
    
        public PartyMember(PartyMemberInfo partyMemberInfo)
        {
            memberBattleVisualPrefab = partyMemberInfo.memberBattleVisualPrefab;
            memberOverworldVisualPrefab = partyMemberInfo.memberOverworldVisualPrefab;
            memberName = partyMemberInfo.memberName;
            level = partyMemberInfo.startingLevel;
            currentHealth = partyMemberInfo.baseHealth;
            maxHealth = currentHealth;
            strength = partyMemberInfo.baseStr;
            initiative = partyMemberInfo.baseInitiative;
        }

        #region EntityInterface

        public GameObject GetBattleVisualsPrefab()
        {
            return memberBattleVisualPrefab;
        }

        public string GetMemberName()
        {
            return memberName;
        }

        public int GetLevel()
        {
            return level;
        }

        public int GetCurrentHealth()
        {
            return currentHealth;
        }

        public int GetMaxHealth()
        {
            return maxHealth;
        }

        public int GetStrength()
        {
            return strength;
        }

        public int GetInitiative()
        {
            return initiative;
        }
        #endregion
    }
}