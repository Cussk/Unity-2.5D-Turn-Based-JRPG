using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


public class PlayerController : MonoBehaviour
{
    const string IS_WALKING_PARAM = "IsWalking";
    const string BATTLE_SCENE = "BattleScene";
    const float TIME_PER_STEP = 0.5F;
    
    [SerializeField] Animator animator;
    [SerializeField] SpriteRenderer playerSprite;
    [SerializeField] LayerMask grassLayer;
    [SerializeField] int stepsInGrass;
    [SerializeField] int speed;
    [SerializeField] int minStepsToEncounter;
    [SerializeField] int maxStepsToEncounter;
    
    PlayerControls _playerControls;
    Rigidbody _rigidbody;
    Vector3 _movement;
    bool _movingInGrass;
    float _stepTimer;
    int _stepsToEncounter;
    
    void Awake()
    {
        _playerControls = new PlayerControls();
        SetRandomStepsToEncounter();
    }

    void OnEnable()
    {
        _playerControls.Enable();
    }

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
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

    void PlayerMove()
    {
        var x = _playerControls.Player.Move.ReadValue<Vector2>().x;
        var z = _playerControls.Player.Move.ReadValue<Vector2>().y;

        _movement = new Vector3(x, 0, z).normalized;

        var isMoving = _movement != Vector3.zero;
        animator.SetBool(IS_WALKING_PARAM, isMoving);

        SpriteFlipX(x);
    }

    void SpriteFlipX(float x)
    {
        switch (x)
        {
            case < 0:
                playerSprite.flipX = true;
                break;
            case > 0:
                playerSprite.flipX = false;
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
