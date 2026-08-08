using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class PlayerMovement : MonoBehaviour
    {
        private const string GroundTag = "Floor";

        private InputAction _moveAction;
        private Rigidbody2D _rb;
        private Collider2D _collider;
        private Vector3 _normalScale;
        private float _ungroundedTime;
        private readonly Collider2D[] _contacts = new Collider2D[16];

        [Header("Input Movement")]
        [SerializeField] private float maxMoveSpeed = 6f;
        [SerializeField] private float acceleration = 40f;
        [SerializeField] private float deceleration = 55f;

        [Header("Ground Death")]
        [SerializeField] private float groundDeathDelay = 0.2f;
        [SerializeField, Range(0.01f, 1f)] private float minimumScaleMultiplier = 0.05f;
        
        [SerializeField] private PlayerInput playerInput;

        // Input movement is capped; external speed can be injected separately.
        private Vector2 _inputVelocity;
        private Vector2 _externalVelocity;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _collider = GetComponent<Collider2D>();
            _normalScale = transform.localScale;

            if (playerInput == null)
            {
                playerInput = GetComponent<PlayerInput>();
            }
        }

        private void OnEnable()
        {
            _moveAction = null;

            if (playerInput != null)
            {
                _moveAction = playerInput.actions.FindAction("Move");
            }

            if (_moveAction == null)
            {
                _moveAction = InputSystem.actions?.FindAction("Move");
            }

            if (_moveAction != null && !_moveAction.enabled)
            {
                _moveAction.Enable();
            }
            else if (_moveAction == null)
            {
                Debug.LogWarning("PlayerMovement: Could not find Input Action named 'Move'.");
            }
        }

        private void FixedUpdate()
        {
            bool isGrounded = false;
            int contactCount = _collider.GetContacts(_contacts);

            for (int i = 0; i < contactCount; i++)
            {
                if (_contacts[i] != null && _contacts[i].CompareTag(GroundTag))
                {
                    isGrounded = true;
                    break;
                }
            }

            if (isGrounded)
            {
                _ungroundedTime = 0f;
                transform.localScale = _normalScale;
            }
            else
            {
                _ungroundedTime += Time.fixedDeltaTime;

                float shrinkDuration = Mathf.Max(0.0001f, groundDeathDelay);
                float shrinkT = Mathf.Clamp01(_ungroundedTime / shrinkDuration);
                float scaleMultiplier = Mathf.Lerp(1f, minimumScaleMultiplier, shrinkT);
                transform.localScale = _normalScale * scaleMultiplier;

                if (groundDeathDelay <= 0f || _ungroundedTime >= groundDeathDelay)
                {
                    Destroy(gameObject);
                    return;
                }
            }

            Vector2 input = _moveAction != null ? _moveAction.ReadValue<Vector2>() : Vector2.zero;
            if (input.sqrMagnitude > 1f) input.Normalize();

            Vector2 targetInputVelocity = input * maxMoveSpeed;
            float rate = input.sqrMagnitude > 0.0001f ? acceleration : deceleration;
            _inputVelocity = Vector2.MoveTowards(_inputVelocity, targetInputVelocity, rate * Time.fixedDeltaTime);
            
            _externalVelocity = Vector2.Lerp(_externalVelocity, Vector2.zero, 10f * Time.fixedDeltaTime); 

            // Apply only the movement we control, so wall collision doesn't get re-fed into motion.
            _rb.linearVelocity = _inputVelocity + _externalVelocity;
        }

        public void AddExternalVelocity(Vector2 velocity)
        {
            _externalVelocity += velocity;
        }
    }
}