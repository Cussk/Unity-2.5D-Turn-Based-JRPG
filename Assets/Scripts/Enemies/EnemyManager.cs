using System;
using System.Collections.Generic;
using UnityEngine;

namespace Enemies
{
    public class EnemyManager : MonoBehaviour
    {
        [SerializeField] EnemyInfo[] allEnemyInfos;
        [SerializeField] List<Enemy> currentEnemies;

        void Awake()
        {
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
    }

    [System.Serializable]
    public class Enemy
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
    }
}
