using System;
using System.Collections;
using System.Collections.Generic;
using Characters;
using Characters.Enemies;
using Characters.Party;
using UnityEngine;

namespace BattleSystem
{
    public class BattleSystem : MonoBehaviour
    {
        const int TURN_DURATION = 1;
        
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
            _battleUI = new BattleUI(playerBattlers, enemyBattlers, SelectEnemy, battleUICanvas);
            _battleUI.ShowBattleMenu(_currentPlayer);
        }

        IEnumerator BattleRoutine()
        {
            _battleUI.ToggleEnemySelectionMenu();
            battleState = BattleState.Battle;
            _battleUI.ToggleBottomPoPUp();

            for (var i = 0; i < allBattlers.Count; i++)
            {
                var battler = allBattlers[i];

                switch (battler.battleAction)
                {
                    case BattleEntity.Action.Attack:
                        yield return StartCoroutine(AttackRoutine(battler, allBattlers[battler.actionTarget]));
                        break;
                    case BattleEntity.Action.Run:
                        break;
                    default:
                        Debug.Log("Not a valid Action");
                        break;
                }
            }

            CheckStillBattling();

            yield return null;
        }
        
        
        void CheckStillBattling()
        {
            if (battleState != BattleState.Battle) return;
            
            _battleUI.ToggleBottomPoPUp();
            _currentPlayer = 0;
            _battleUI.ShowBattleMenu(_currentPlayer);
        }

        IEnumerator AttackRoutine(BattleEntity currentAttacker, BattleEntity currentTarget)
        {
            if (currentAttacker.isPlayer)
            {
                AttackAction(currentAttacker, currentTarget);
                yield return new WaitForSeconds(TURN_DURATION);

                if (currentTarget.currentHealth <= 0)
                {
                    _battleUI.ShowDefeatedText(currentAttacker.name, currentTarget.name);
                    yield return new WaitForSeconds(TURN_DURATION);
                    enemyBattlers.Remove(currentTarget);
                    allBattlers.Remove(currentTarget);
                    
                    if (enemyBattlers.Count <= 0)
                    {
                        battleState = BattleState.Won;
                        _battleUI.ShowWinText();
                    }
                }
            }
            yield return null;
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
                _battleUI.ToggleEnemySelectionMenu();
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