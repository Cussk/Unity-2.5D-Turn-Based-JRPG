using UnityEngine;


public class PlayerController : MonoBehaviour
{
    const string IS_WALKING_PARAM = "IsWalking";
    
    [SerializeField] Animator animator;
    [SerializeField] SpriteRenderer playerSprite;
    [SerializeField] int speed;
    PlayerControls _playerControls;
    Rigidbody _rigidbody;
    Vector3 _movement;
    
    void Awake()
    {
        _playerControls = new PlayerControls();
        
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
       _rigidbody.MovePosition(transform.position + _movement * (speed * Time.fixedDeltaTime)); 
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
}
