using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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

        [Header("Animations")]
        [SerializeField] private Animator animator;
        [SerializeField] private SpriteRenderer spriteRenderer;

        // Type the EXACT names of your Animation Clips in the inspector here
        [SerializeField] private string walkDownAnim = "WalkDown";
        [SerializeField] private string walkUpAnim = "WalkUp";
        [SerializeField] private string walkRightAnim = "WalkRight"; 
        [SerializeField] private string idleAnim = "Idle"; 

        private string _currentState;
        private Vector2 _inputVelocity;
        private Vector2 _externalVelocity;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();

            if (playerInput == null) playerInput = GetComponent<PlayerInput>();
            if (animator == null) animator = GetComponent<Animator>();
            if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
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

        private void Update()
        {
            HandleAnimations();
        }

        private void FixedUpdate()
        {
            Vector2 input = _moveAction != null ? _moveAction.ReadValue<Vector2>() : Vector2.zero;
            if (input.sqrMagnitude > 1f) input.Normalize();

            Vector2 targetInputVelocity = input * maxMoveSpeed;
            float rate = input.sqrMagnitude > 0.0001f ? acceleration : deceleration;
            _inputVelocity = Vector2.MoveTowards(_inputVelocity, targetInputVelocity, rate * Time.fixedDeltaTime);
            
            _externalVelocity = Vector2.Lerp(_externalVelocity, Vector2.zero, 10f * Time.fixedDeltaTime); 

            _rb.linearVelocity = _inputVelocity + _externalVelocity;
        }

        private void HandleAnimations()
        {
            if (_moveAction == null || animator == null) return;

            Vector2 input = _moveAction.ReadValue<Vector2>();

            // If the player is barely pressing anything, play Idle
            if (input.sqrMagnitude < 0.01f)
            {
                ChangeAnimationState(idleAnim);
                return;
            }

            // Determine if horizontal movement is stronger than vertical movement
            if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
            {
                // We are moving horizontally
                ChangeAnimationState(walkRightAnim);

                // Flip sprite based on left/right
                if (input.x < 0) 
                    spriteRenderer.flipX = true; // Moving Left
                else 
                    spriteRenderer.flipX = false; // Moving Right
            }
            else
            {
                // We are moving vertically
                if (input.y > 0)
                    ChangeAnimationState(walkUpAnim); // Moving Up
                else
                    ChangeAnimationState(walkDownAnim); // Moving Down
            }
        }

        // Helper method to ensure we don't restart an animation that is already playing
        private void ChangeAnimationState(string newState)
        {
            if (_currentState == newState) return;

            animator.Play(newState);
            _currentState = newState;
        }

        public void AddExternalVelocity(Vector2 velocity)
        {
            _externalVelocity += velocity;
        }
        
        private void OnDestroy()
        {
            if (gameObject.scene.isLoaded)
            {
                Time.timeScale = 1;
                SceneManager.LoadScene("Item Selector");
            }
        }
    }
}