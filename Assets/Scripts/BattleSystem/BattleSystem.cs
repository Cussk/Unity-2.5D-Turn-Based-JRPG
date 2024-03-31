using System.Collections.Generic;
using Characters;
using Characters.Enemies;
using Characters.Party;
using UnityEngine;

namespace BattleSystem
{
    public class BattleSystem : MonoBehaviour
    {
        [Header("Dependency Injections")]
        [SerializeField] PartyManager partyManager;
        [SerializeField] EnemyManager enemyManager;
        [SerializeField] GameObject battleUICanvas;
        
        [Header("Spawn Points")]
        [SerializeField] Transform[] partySpawnPoints;
        [SerializeField] Transform[] enemySpawnPoints;
        
        [Header("Battlers Lists")]
        [SerializeField] List<BattleEntity> allBattlers = new();
        [SerializeField] List<BattleEntity> enemyBattlers = new();
        [SerializeField] List<BattleEntity> playerBattlers = new();

        BattleUI _battleUI;
        int _currentPlayer;
    
        void Start()
        {
            AddPartyMembers();
            AddEnemies();
            _battleUI = new BattleUI(enemyBattlers, SelectEnemy, battleUICanvas);
            _battleUI.ShowBattleMenu(playerBattlers, _currentPlayer);
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
        
        void SelectEnemy(int currentEnemy)
        {
            var currentPlayerEntity = playerBattlers[_currentPlayer];
            currentPlayerEntity.SetTarget(allBattlers.IndexOf(enemyBattlers[currentEnemy]));

            currentPlayerEntity.battleAction = BattleEntity.Action.Attack;
            _currentPlayer++;

            HaveAllPlayersSelected(currentPlayerEntity);
        }

        void HaveAllPlayersSelected(BattleEntity currentPlayerEntity)
        {
            if (_currentPlayer >= playerBattlers.Count)
            {
                //start turn
                Debug.Log("Start Battle");
                Debug.Log("We are attacking: " + allBattlers[currentPlayerEntity.actionTarget].name);
                _currentPlayer = 0;
            }
            else
            {
                _battleUI.ToggleEnemySelectionMenu();
                _battleUI.ShowBattleMenu(playerBattlers, _currentPlayer);
            }
        }
    }

    [System.Serializable]
    public class BattleEntity
    {
        public enum Action
        {
            Attack,
            Run
        }
        
        public BattleVisuals battleVisuals;
        public Action battleAction;
        public string name;
        public int level;
        public int currentHealth;
        public int maxHealth;
        public int strength;
        public int initiative;
        public int actionTarget;
        public bool isPlayer;

        public BattleEntity(IEntity entity) 
        {
            SetEntityValues(entity);
        }

        void SetEntityValues<T>(T entity) where T : IEntity
        {
            if (entity == null) return;

            name = entity.GetMemberName();
            level = entity.GetLevel();
            currentHealth = entity.GetCurrentHealth();
            maxHealth = entity.GetMaxHealth();
            strength = entity.GetStrength();
            initiative = entity.GetInitiative();
            isPlayer = entity is PartyMember;;
        }

        public void SetTarget(int target)
        {
            actionTarget = target;
        }
    }
}