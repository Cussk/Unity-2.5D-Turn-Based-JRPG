using Characters.Enemies;
using UnityEngine;

namespace EncounterSystem
{
    public class EncounterSystem : MonoBehaviour
    {
        [SerializeField] Encounter[] enemiesInScene;
        [SerializeField] int maxNumberEnemies;
        EnemyManager _enemyManager;

        void Awake()
        {
            _enemyManager = FindFirstObjectByType<EnemyManager>();
        }

        void Start()
        {
            _enemyManager.GenerateEnemiesByEncounter(enemiesInScene, maxNumberEnemies);
        }
    }

    [System.Serializable]
    public class Encounter
    {
        public EnemyInfo enemyInfo;
        public int minLevel;
        public int maxLevel;
    }
}