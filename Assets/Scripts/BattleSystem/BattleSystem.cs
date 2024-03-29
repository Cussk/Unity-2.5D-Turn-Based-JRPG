using System.Collections.Generic;
using Enemies;
using Interfaces;
using Party;
using UnityEngine;

namespace BattleSystem
{
    public class BattleSystem : MonoBehaviour
    {
        [SerializeField] PartyManager _partyManager;
        [SerializeField] EnemyManager _enemyManager;
        [SerializeField] List<BattleEntities> allBattlers = new List<BattleEntities>();
        [SerializeField] List<BattleEntities> enemyBattlers = new List<BattleEntities>();
        [SerializeField] List<BattleEntities> playerBattlers = new List<BattleEntities>();
    
        void Start()
        {
            AddEntitiesToBattle();
        }

        void AddEntitiesToBattle()
        {
            AddPartyMembers();
            AddEnemies();
        }

        void AddPartyMembers()
        {
            var currentParty = _partyManager.GetPartyMembers();
            foreach (var member in currentParty)
            {
                var battleEntity = new BattleEntities(true, member);
                playerBattlers.Add(battleEntity);
                allBattlers.Add(battleEntity);
            }
        }
        
        void AddEnemies()
        {
            var currentEnemies = _enemyManager.GetEnemies();
            foreach (var enemy in currentEnemies)
            {
                var battleEntity = new BattleEntities(false, null, enemy);
                enemyBattlers.Add(battleEntity);
                allBattlers.Add(battleEntity);
            }
        }
    }

    [System.Serializable]
    public class BattleEntities
    {
        public string memberName;
        public int level;
        public int currentHealth;
        public int maxHealth;
        public int strength;
        public int initiative;
        public bool isPlayer;

        public BattleEntities(bool isPlayer, PartyMember partyMember = null, Enemy enemy = null) 
        {
            SetEntityValues(partyMember, isPlayer);
            SetEntityValues(enemy, isPlayer);
        }

        void SetEntityValues<T>(T entity, bool isEntityPlayer) where T : IEntity
        {
            if (entity == null) return;

            memberName = entity.GetMemberName();
            level = entity.GetLevel();
            currentHealth = entity.GetCurrentHealth();
            maxHealth = entity.GetMaxHealth();
            strength = entity.GetStrength();
            initiative = entity.GetInitiative();
            isPlayer = isEntityPlayer;
        }
    }
}