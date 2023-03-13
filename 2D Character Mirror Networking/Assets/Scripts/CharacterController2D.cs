using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class CharacterController2D : NetworkBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    KeyCode Left = KeyCode.A;
    [SerializeField]
    KeyCode Right = KeyCode.D;
    [SerializeField]
    KeyCode Down = KeyCode.S;
    [SerializeField]
    KeyCode Up = KeyCode.W;

    private Animator _animator;
    private Rigidbody2D _rigidbody2D;

    private Vector3 _currentVelocity;
    private Vector3 _previousPosition;
    private Vector3 _localVelocity;

    private string animatorX = "VelocityX";
    private string animatorY = "VelocityY";
    private float _horizontalInput;
    private float _verticalInput;


    private enum Direction { Up , Right, Down, Left}
    [SerializeField] private Direction direction;
    private float _velocityX = 0;
    private float _velocityY = 0;
    [SerializeField] private float movementSpeed = 5f;

    private AudioSource _audioSource;
    private void Awake()
    {
        _rigidbody2D = this.GetComponent<Rigidbody2D>();
        _animator = this.GetComponent<Animator>();
        _audioSource = this.GetComponent<AudioSource>();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (isLocalPlayer)
        {
            Camera.main.transform.SetParent(this.transform);
            Camera.main.transform.localPosition = Vector3.zero;
        }
    }
    private void FixedUpdate()
    {
        // Calculate velocity from transform position
        Vector3 currentVelocity = (transform.position - _previousPosition) / Time.fixedDeltaTime;
        _previousPosition = transform.position;

        _localVelocity = transform.InverseTransformDirection(currentVelocity);
        

        _animator.SetFloat(animatorX, _velocityX);
        _animator.SetFloat(animatorY, _velocityY);

        if (isLocalPlayer)
        {
            Vector2 movementDir = new Vector2(_horizontalInput, _verticalInput);
            _rigidbody2D.AddForce(movementDir * movementSpeed);
        }
    }
    void Update()
    {
        SetStopDirectionAndVelocity();

        if (isLocalPlayer)
        {
            KeyPressCheck();
        }
    }
    void SetStopDirectionAndVelocity()
    { 
        if (Mathf.Abs(_localVelocity.x) > Mathf.Abs(_localVelocity.y))
        {
            if (_localVelocity.magnitude > 0.1f)
            {
                direction = (_localVelocity.x > 0) ? Direction.Right : Direction.Left;
                _velocityX = _localVelocity.x; 
                _velocityY = 0;
                _velocityX = Mathf.Clamp(_velocityX, -1f, 1f);
            }
        }
        else
        {
            if (_localVelocity.magnitude > 0.1f)
            {
                direction = (_localVelocity.y > 0) ? Direction.Up : Direction.Down;
                _velocityY = _localVelocity.y;
                _velocityX = 0;
                _velocityY = Mathf.Clamp(_velocityY, -1f, 1f);
            }
        }


        if (_localVelocity.magnitude <0.1f)
        {
            //we stopped set our direction based on velocity
            if (direction == Direction.Up) { _velocityY = 0.1f; _velocityX = 0; }
            else if (direction == Direction.Down) { _velocityY = -0.1f; _velocityX = 0; }
            else if (direction == Direction.Left) { _velocityX = -0.1f; _velocityY = 0f; }
            else if (direction == Direction.Right) { _velocityX = 0.1f; _velocityY = 0f; }

        }
    }
    void KeyPressCheck()
    {
        // Check if left or right arrow keys are pressed
        if (Input.GetKey(Left))
        {
            _horizontalInput = -1f;
        }
        else if (Input.GetKey(Right))
        {
            _horizontalInput = 1f;
        }
        else
        {
            _horizontalInput = 0f;
            _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x * 0f,_rigidbody2D.velocity.y);
        }
        // Check if up or down arrow keys are pressed
        if (Input.GetKey(Up))
        {
            _verticalInput = 1f;
        }
        else if (Input.GetKey(Down))
        {
            _verticalInput = -1f;
        }
        else
        {
            _verticalInput = 0f;
            _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, _rigidbody2D.velocity.y * 0f);
        }
    }
    public void AnimatorMethod_PlayFootstep() => _audioSource.Play();
}
