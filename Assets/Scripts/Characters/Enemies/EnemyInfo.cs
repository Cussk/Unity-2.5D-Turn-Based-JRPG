using UnityEngine;

namespace Characters.Enemies
{
    [CreateAssetMenu (menuName = "New Enemy")]
    public class EnemyInfo : ScriptableObject
    {
        public GameObject enemyVisualPrefab;
        public string enemyName;
        public int baseHealth;
        public int baseStr;
        public int baseInitiative;
    }
}
