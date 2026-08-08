using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerMovement : MonoBehaviour
    {
        private InputAction _moveAction;
        private Rigidbody2D _rb;

        [Header("Input Movement")]
        [SerializeField] private float maxMoveSpeed = 6f;
        [SerializeField] private float acceleration = 40f;
        [SerializeField] private float deceleration = 55f;
        
        [SerializeField] private PlayerInput playerInput;

        // Input movement is capped; external speed can be injected separately.
        private Vector2 _inputVelocity;
        private Vector2 _externalVelocity;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();

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