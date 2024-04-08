using Characters.Party;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace Characters.Player
{
    public class PlayerController : MonoBehaviour
    {
        const string BATTLE_SCENE = "BattleScene";
        const float TIME_PER_STEP = 0.5F;
    
        static readonly int IsWalking = Animator.StringToHash(GlobalVariables.IS_WALK_PARAM);
        
        [SerializeField] LayerMask grassLayer;
        [SerializeField] GameObject popUpCanvasGameObject;
        [SerializeField] int stepsInGrass;
        [SerializeField] int speed;
        [SerializeField] int minStepsToEncounter;
        [SerializeField] int maxStepsToEncounter;
    
        PartyManager _partyManager;
        PlayerControls _playerControls;
        CharacterManager _characterManager;
        OverWorldPopUpController _overWorldPopUpController;
        Rigidbody _rigidbody;
        Animator _playerAnimator;
        SpriteRenderer _playerSprite;
        Vector3 _movement;
        bool _movingInGrass;
        float _stepTimer;
        int _stepsToEncounter;

        void Awake()
        {
            _partyManager = FindFirstObjectByType<PartyManager>();
            _playerControls = new PlayerControls();
            _overWorldPopUpController = new OverWorldPopUpController(popUpCanvasGameObject);
            _rigidbody = GetComponent<Rigidbody>();
            _characterManager = GetComponent<CharacterManager>();
            _characterManager.Init(_playerControls, _partyManager, _overWorldPopUpController);
            SetRandomStepsToEncounter();
        }

        void OnEnable()
        {
            _playerControls.Enable();
        }

        void OnDisable()
        {
            _playerControls.Disable();
        }

        void Start()
        {
            if (_partyManager.PlayerPosition != Vector3.zero)
                transform.position = _partyManager.PlayerPosition;
        }

        void Update()
        {
            PlayerMove();
        }

        void FixedUpdate()
        {
            var position = transform.position;
            _rigidbody.MovePosition(position + _movement * (speed * Time.fixedDeltaTime));

            AddStepsUntilEncounter(position);
        }

        public void SetOverWorldVisuals(Animator animator, SpriteRenderer spriteRenderer)
        {
            _playerAnimator = animator;
            _playerSprite = spriteRenderer;
        }

        void PlayerMove()
        {
            var x = _playerControls.Player.Move.ReadValue<Vector2>().x;
            var z = _playerControls.Player.Move.ReadValue<Vector2>().y;

            _movement = new Vector3(x, 0, z).normalized;

            var isMoving = _movement != Vector3.zero;
            _playerAnimator.SetBool(IsWalking, isMoving);

            SpriteFlipX(x);
        }

        void SpriteFlipX(float x)
        {
            switch (x)
            {
                case < 0:
                    _playerSprite.flipX = true;
                    break;
                case > 0:
                    _playerSprite.flipX = false;
                    break;
            }
        }
    
        void AddStepsUntilEncounter(Vector3 position)
        {
            var colliders = Physics.OverlapSphere(position, 1, grassLayer);
            _movingInGrass = colliders.Length != 0 && _movement != Vector3.zero;

            if (!_movingInGrass) return;
            _stepTimer += Time.fixedDeltaTime;
        
            if (_stepTimer > TIME_PER_STEP)
            {
                stepsInGrass++;
                _stepTimer = 0;

                if (stepsInGrass >= _stepsToEncounter)
                {
                    _partyManager.SetPosition(transform.position);
                    SceneManager.LoadScene(BATTLE_SCENE);
                
                    stepsInGrass = 0;
                    SetRandomStepsToEncounter();
                }
            }
        }

        void SetRandomStepsToEncounter()
        {
            _stepsToEncounter = Random.Range(minStepsToEncounter, maxStepsToEncounter + 1);
        }
    }
}
