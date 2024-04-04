using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BattleSystem
{
    public class BattleVisuals : MonoBehaviour
    {
        const string LVL_STRING = "Lvl: ";
        
        static readonly int IsAttack = Animator.StringToHash("IsAttack");
        static readonly int IsHit = Animator.StringToHash("IsHit");
        static readonly int IsDead = Animator.StringToHash("IsDead");
        
        [SerializeField] Slider healthBar;
        [SerializeField] TextMeshProUGUI levelText;
        Animator _animator;
        int _currentHealth;
        int _maxHealth;
        int _level;

        void Awake()
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
                Destroy(gameObject, 1f);
            }
            
            UpdateHealthBar();
        }

        public void PlayAttackAnimation()
        {
            _animator.SetTrigger(IsAttack);
        }

        public void PlayHitAnimation()
        {
            _animator.SetTrigger(IsHit);
        }

        void PlayDeathAnimation()
        {
            _animator.SetTrigger(IsDead);
        }
        
        void UpdateHealthBar()
        {
            healthBar.maxValue = _maxHealth;
            healthBar.value = _currentHealth;
        }
    }
}
