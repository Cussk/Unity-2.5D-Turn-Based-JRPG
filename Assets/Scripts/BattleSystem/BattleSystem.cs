using System.Collections.Generic;
using System.Linq;
using Enemies;
using Interfaces;
using Party;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.Serialization;

namespace BattleSystem
{
    public class BattleSystem : MonoBehaviour
    {
        const string ACTION_MSG = "'s Action:";
        
        [SerializeField] PartyManager partyManager;
        [SerializeField] EnemyManager enemyManager;
        
        [Header("Spawn Points")]
        [SerializeField] Transform[] partySpawnPoints;
        [SerializeField] Transform[] enemySpawnPoints;
        
        [Header("Battlers Lists")]
        [SerializeField] List<BattleEntity> allBattlers = new();
        [SerializeField] List<BattleEntity> enemyBattlers = new();
        [SerializeField] List<BattleEntity> playerBattlers = new();

        [SerializeField] GameObject[] enemySelectionButtons;
        [SerializeField] GameObject battleMenu;
        [SerializeField] GameObject enemySelectionMenu;
        [SerializeField] TextMeshProUGUI actionTitleText;

        int currentPlayer;
    
        void Start()
        {
            AddPartyMembers();
            AddEnemies();
            //ShowBattleMenu();
            ShowEnemySelectionMenu();
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

        void ShowBattleMenu()
        {
            actionTitleText.text = playerBattlers[currentPlayer].name + ACTION_MSG;
            battleMenu.SetActive(true);
        }

        void ShowEnemySelectionMenu()
        {
            battleMenu.SetActive(false);
            SetEnemySelectionButtons();
            enemySelectionMenu.SetActive(true);
        }

        void SetEnemySelectionButtons()
        {
            foreach (var button in enemySelectionButtons)
            {
                button.SetActive(false);
            }

            for (var i = 0; i < enemyBattlers.Count(); i++)
            {
                enemySelectionButtons[i].SetActive(true);
                enemySelectionButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = enemyBattlers[i].name;
            }
        }
    }

    [System.Serializable]
    public class BattleEntity
    {
        public BattleVisuals battleVisuals;
        public string name;
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

            name = entity.GetMemberName();
            level = entity.GetLevel();
            currentHealth = entity.GetCurrentHealth();
            maxHealth = entity.GetMaxHealth();
            strength = entity.GetStrength();
            initiative = entity.GetInitiative();
            isPlayer = entity is PartyMember;;
        }
    }
}