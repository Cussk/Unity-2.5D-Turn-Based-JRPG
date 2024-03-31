using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BattleSystem
{
    public class BattleVisuals : MonoBehaviour
    {
        const string LVL_STRING = "Lvl: ";
        const string IS_ATTACK_PARAM = "IsAttack";
        const string IS_HIT_PARAM = "IsHit";
        const string IS_DEAD_PARAM = "IsDead";
        
        [SerializeField] Slider healthBar;
        [SerializeField] TextMeshProUGUI levelText;
        Animator _animator;
        int _currentHealth;
        int _maxHealth;
        int _level;

        void Start()
        {
            _animator = gameObject.GetComponent<Animator>();
        }

        public void SetStartingValues(int currentHealth, int maxHealth, int level)
        {
            _currentHealth = currentHealth;
            _maxHealth = maxHealth;
            _level = level;
            levelText.text = LVL_STRING + _level;
            UpdateHealthBar();
        }

        public void ChangeHealth(int currentHealth)
        {
            _currentHealth = currentHealth;
            
            if (_currentHealth <= 0)
            {
                PlayDeathAnimation();
                Destroy(gameObject);
            }
            
            UpdateHealthBar();
        }

        void UpdateHealthBar()
        {
            healthBar.maxValue = _maxHealth;
            healthBar.value = _currentHealth;
        }

        void PlayAttackAnimation()
        {
            _animator.SetTrigger(IS_ATTACK_PARAM);
        }

        void PlayHitAnimation()
        {
            _animator.SetTrigger(IS_HIT_PARAM);
        }

        void PlayDeathAnimation()
        {
            _animator.SetTrigger(IS_DEAD_PARAM);
        }
    }
}
