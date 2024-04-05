using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Characters;
using Characters.Enemies;
using Characters.Party;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace BattleSystem
{
    public class BattleSystem : MonoBehaviour
    {
        const int TURN_DURATION = 1;
        const int RUN_THRESHOLD = 50;
        const string OVERWORLD_SCENE = "OverworldScene";
        
        enum BattleState
        {
            Start,
            Selection,
            Battle,
            Won,
            Lost,
            Run
        }

        [SerializeField] BattleState battleState;
        
        [Header("Dependency Injections")]
        [SerializeField] GameObject battleUICanvas;
        
        [Header("Spawn Points")]
        [SerializeField] Transform[] partySpawnPoints;
        [SerializeField] Transform[] enemySpawnPoints;
        
        [Header("Battlers Lists")]
        [SerializeField] List<BattleEntity> allBattlers = new();
        [SerializeField] List<BattleEntity> enemyBattlers = new();
        [SerializeField] List<BattleEntity> playerBattlers = new();
        
        PartyManager _partyManager;
        EnemyManager _enemyManager;
        BattleUI _battleUI;
        int _currentPlayer;

        void Awake()
        {
            _partyManager = FindFirstObjectByType<PartyManager>();
            _enemyManager = FindFirstObjectByType<EnemyManager>();
        }

        void Start()
        {
            AddPartyMembers();
            AddEnemies();
            _battleUI = new BattleUI(playerBattlers, enemyBattlers, SelectEnemy, SelectRunAction, battleUICanvas);
            _battleUI.ShowBattleMenu(_currentPlayer);
            DetermineBattleOrder();
        }
        
        void AddPartyMembers()
        {
            var currentParty = _partyManager.GetAlivePartyMembers();
            AddEntities(true, currentParty);
        }
        
        void AddEnemies()
        {
            var currentEnemies = _enemyManager.GetEnemies();
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

        IEnumerator BattleRoutine()
        {
            _battleUI.ToggleEnemySelectionMenu(false);
            battleState = BattleState.Battle;
            _battleUI.ToggleBottomPoPUp(true);

            for (var i = 0; i < allBattlers.Count; i++)
            {
                if (battleState == BattleState.Battle && allBattlers[i].currentHealth > 0)
                {

                    var battler = allBattlers[i];
                    switch (battler.battleAction)
                    {
                        case BattleEntity.Action.Attack:
                            yield return StartCoroutine(AttackRoutine(battler));
                            break;
                        case BattleEntity.Action.Run:
                            yield return StartCoroutine(RunRoutine());
                            break;
                        default:
                            Debug.Log("Not a valid Action");
                            break;
                    }
                }
            }
            
            RemoveDeadBattlers();
            CheckStillBattling();
        }
        
        
        void CheckStillBattling()
        {
            if (battleState != BattleState.Battle) return;
            
            _battleUI.ToggleBottomPoPUp(false);
            _currentPlayer = 0;
            _battleUI.ShowBattleMenu(_currentPlayer);
        }

        IEnumerator AttackRoutine(BattleEntity currentAttacker)
        {
            SetRandomTarget(currentAttacker);

            var currentTarget = allBattlers[currentAttacker.actionTarget];

            AttackAction(currentAttacker, currentTarget);
            yield return new WaitForSeconds(TURN_DURATION);

            if (currentTarget.currentHealth <= 0)
            {
                _battleUI.ShowDefeatedText(currentAttacker.name, currentTarget.name);
                yield return new WaitForSeconds(TURN_DURATION);
        
                HandleTargetDefeated(currentTarget);
            }
        }

        IEnumerator RunRoutine()
        {
            var runChance = Random.Range(0, 100);
            if (runChance >= RUN_THRESHOLD)
            {
                battleState = BattleState.Run;
                _battleUI.ShowGenericText(GlobalVariables.RUN_MSG);
                allBattlers.Clear();
                yield return new WaitForSeconds(TURN_DURATION);
                SceneManager.LoadScene(OVERWORLD_SCENE);
            }
            else
            {
                _battleUI.ShowGenericText(GlobalVariables.RUN_FAIL_MSG);
                yield return new WaitForSeconds(TURN_DURATION);
            }
        }

        void SetRandomTarget(BattleEntity currentAttacker)
        {
            switch (currentAttacker.isPlayer)
            {
                case true when (allBattlers[currentAttacker.actionTarget].isPlayer || allBattlers[currentAttacker.actionTarget].currentHealth <= 0):
                    currentAttacker.SetTarget(GetRandomBattler(false));
                    break;
                case false:
                    currentAttacker.SetTarget(GetRandomBattler(true));
                    break;
            }
        }

        void HandleTargetDefeated(BattleEntity currentTarget)
        {
            if (currentTarget.isPlayer)
            {
                playerBattlers.Remove(currentTarget);
                if (playerBattlers.Count <= 0)
                {
                    battleState = BattleState.Lost;
                    _battleUI.ShowGenericText(GlobalVariables.LOSS_MSG);
                }
            }
            else
            {
                enemyBattlers.Remove(currentTarget);
                if (enemyBattlers.Count <= 0)
                {
                    battleState = BattleState.Won;
                    _battleUI.ShowGenericText(GlobalVariables.WIN_MSG);
                    SceneManager.LoadScene(OVERWORLD_SCENE);
                }
            }
        }

        void RemoveDeadBattlers()
        {
            for (var i = 0; i < allBattlers.Count; i++)
            {
                if (allBattlers[i].currentHealth <= 0)
                    allBattlers.RemoveAt(i);
            }
        }
        
        
        void SelectEnemy(int currentEnemy)
        {
            var currentPlayerEntity = playerBattlers[_currentPlayer];
            currentPlayerEntity.SetTarget(allBattlers.IndexOf(enemyBattlers[currentEnemy]));

            currentPlayerEntity.battleAction = BattleEntity.Action.Attack;
            _currentPlayer++;

            HaveAllPlayersSelected();
        }

        void SelectRunAction()
        {
            battleState = BattleState.Selection;
            var currentPlayerEntity = playerBattlers[_currentPlayer];

            currentPlayerEntity.battleAction = BattleEntity.Action.Run;
            _currentPlayer++;

            HaveAllPlayersSelected();
        }

        void HaveAllPlayersSelected()
        {
            if (_currentPlayer >= playerBattlers.Count)
            {
                StartCoroutine(BattleRoutine());
            }
            else
            {
                _battleUI.ToggleEnemySelectionMenu(false);
                _battleUI.ShowBattleMenu(_currentPlayer);
            }
        }

        void AttackAction(BattleEntity currentAttacker, BattleEntity currentTarget)
        {
            var damage = currentAttacker.strength;
            currentAttacker.battleVisuals.PlayAttackAnimation();
            currentTarget.currentHealth -= damage;
            currentTarget.battleVisuals.PlayHitAnimation();
            currentTarget.UpdateHealthBar();
            _battleUI.ShowDamageText(currentAttacker.name, currentTarget.name, damage);
            SavePartyHealth();
        }

        int GetRandomBattler(bool isPlayer)
        {
            var tempBattlerList = Enumerable.Range(0, allBattlers.Count)
                .Where(index => allBattlers[index].isPlayer == isPlayer && allBattlers[index].currentHealth > 0)
                .ToList();
            
            return tempBattlerList[Random.Range(0, tempBattlerList.Count)];
        }

        void SavePartyHealth()
        {
            for (var i = 0; i < playerBattlers.Count; i++)
            {
                _partyManager.SaveHealth(i, playerBattlers[i].currentHealth);
            }
        }

        void DetermineBattleOrder()
        {
            allBattlers = allBattlers.OrderByDescending(battler => battler.initiative).ToList();
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
        
        public void SetTarget(int target)
        {
            actionTarget = target;
        }

        public void UpdateHealthBar()
        {
            battleVisuals.ChangeHealth(currentHealth);
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