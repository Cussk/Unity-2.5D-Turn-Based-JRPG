using System.Collections.Generic;
using UnityEngine;

namespace Characters.Enemies
{
    public class EnemyManager : MonoBehaviour
    {
        [SerializeField] EnemyInfo[] allEnemyInfos;
        [SerializeField] List<Enemy> currentEnemies;

        void Awake()
        {
            GenerateEnemyByName("Slime", 1);
            GenerateEnemyByName("Slime", 1);
            GenerateEnemyByName("Slime", 1);
        }

        void GenerateEnemyByName(string enemyName, int level)
        {
            for (var i = 0; i < allEnemyInfos.Length; i++)
            {
                if (allEnemyInfos[i].enemyName != enemyName) continue;
                
                var newEnemy = new Enemy(allEnemyInfos[i], level);
                currentEnemies.Add(newEnemy);
            }
        }

        public List<Enemy> GetEnemies()
        {
            return currentEnemies;
        }
    }

    [System.Serializable]
    public class Enemy : IEntity
    {
        const float LEVEL_MODIFIER = 0.5f;
        
        public GameObject enemyVisualPrefab;
        public string enemyName;
        public int level;
        public int currentHealth;
        public int maxHealth;
        public int strength;
        public int initiative;
        
        public Enemy(EnemyInfo enemyInfo, int currentLevel)
        {
            enemyVisualPrefab = enemyInfo.enemyVisualPrefab;
            enemyName = enemyInfo.enemyName;
            level = currentLevel;
            maxHealth = enemyInfo.baseHealth + StatModifierByLevel();
            currentHealth = maxHealth;
            strength = enemyInfo.baseStr + StatModifierByLevel();
            initiative = enemyInfo.baseInitiative + StatModifierByLevel();
        }

        int StatModifierByLevel()
        {
            var statModifier = Mathf.RoundToInt(level * LEVEL_MODIFIER);
            return statModifier;
        }
        
        #region EntityInterface

        public GameObject GetBattleVisualsPrefab()
        {
            return enemyVisualPrefab;
        }

        public string GetMemberName()
        {
            return enemyName;
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
