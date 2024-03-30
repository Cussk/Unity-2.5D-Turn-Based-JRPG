using System.Collections.Generic;
using System.Linq;
using Enemies;
using Interfaces;
using Party;
using UI;
using UnityEngine;

namespace BattleSystem
{
    public class BattleSystem : MonoBehaviour
    {
        [SerializeField] PartyManager partyManager;
        [SerializeField] EnemyManager enemyManager;
        
        [Header("Spawn Points")]
        [SerializeField] Transform[] partySpawnPoints;
        [SerializeField] Transform[] enemySpawnPoints;
        
        [Header("Battlers Lists")]
        [SerializeField] List<BattleEntity> allBattlers = new();
        [SerializeField] List<BattleEntity> enemyBattlers = new();
        [SerializeField] List<BattleEntity> playerBattlers = new();
    
        void Start()
        {
            AddPartyMembers();
            AddEnemies();
        }

        void AddPartyMembers()
        {
            var currentParty = partyManager.GetPartyMembers();
            AddEntities(true, currentParty);
        }
        
        void AddEnemies()
        {
            var currentEnemies = enemyManager.GetEnemies();
            AddEntities(false, currentEnemies);
        }
        
        void AddEntities(bool isPlayer, IEnumerable<IEntity> entities)
        {
            foreach (var entity in entities)
            {
                var battleEntity = new BattleEntity(entity);
                var spawnPoint = isPlayer ? partySpawnPoints[playerBattlers.Count] : enemySpawnPoints[enemyBattlers.Count];
                InitializeBattleEntityVisuals(entity, spawnPoint, battleEntity);

                if (isPlayer)
                    playerBattlers.Add(battleEntity);
                else
                    enemyBattlers.Add(battleEntity);
        
                allBattlers.Add(battleEntity);
            }
        }

        static void InitializeBattleEntityVisuals(IEntity entity, Transform spawnPoint, BattleEntity battleEntity)
        {
            var entityBattleVisuals =
                Instantiate(entity.GetBattleVisualsPrefab(), spawnPoint.position, Quaternion.identity)
                    .GetComponent<BattleVisuals>();

            entityBattleVisuals.SetStartingValues(entity.GetCurrentHealth(), entity.GetMaxHealth(), entity.GetLevel());
            battleEntity.battleVisuals = entityBattleVisuals;
        }
    }

    [System.Serializable]
    public class BattleEntity
    {
        public BattleVisuals battleVisuals;
        public string memberName;
        public int level;
        public int currentHealth;
        public int maxHealth;
        public int strength;
        public int initiative;
        public bool isPlayer;

        public BattleEntity(IEntity entity) 
        {
            SetEntityValues(entity);
        }

        void SetEntityValues<T>(T entity) where T : IEntity
        {
            if (entity == null) return;

            memberName = entity.GetMemberName();
            level = entity.GetLevel();
            currentHealth = entity.GetCurrentHealth();
            maxHealth = entity.GetMaxHealth();
            strength = entity.GetStrength();
            initiative = entity.GetInitiative();
            isPlayer = entity is PartyMember;;
        }
    }
}