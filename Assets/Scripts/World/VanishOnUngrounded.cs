using UnityEngine;

namespace World
{
    [RequireComponent(typeof(Collider2D))]
    public class VanishOnUngrounded : MonoBehaviour
    {
        // Allow multiple tags so the component works across different projects.
        [SerializeField] private string[] groundTags = new[] { "Floor", "Ground", "Solid" };
        [SerializeField] private float groundDeathDelay = 0.2f;
        [SerializeField, Range(0.01f, 1f)] private float minimumScaleMultiplier = 0.05f;
        [SerializeField] private bool destroyOnDeath = true;

        private Collider2D _collider;
        private Vector3 _normalScale;
        private float _ungroundedTime;
        private readonly Collider2D[] _contacts = new Collider2D[16];

        private void Awake()
        {
            _collider = GetComponent<Collider2D>();
            _normalScale = transform.localScale;
        }

        private void FixedUpdate()
        {
            bool isGrounded = false;
            int contactCount = _collider.GetContacts(_contacts);

            for (int i = 0; i < contactCount; i++)
            {
                var c = _contacts[i];
                if (c == null) continue;

                // If any contact matches any of the configured ground tags, we're grounded.
                for (int tagIdx = 0; tagIdx < groundTags.Length; tagIdx++)
                {
                    if (!string.IsNullOrEmpty(groundTags[tagIdx]) && c.CompareTag(groundTags[tagIdx]))
                    {
                        isGrounded = true;
                        break;
                    }
                }

                if (isGrounded) break;
            }

            if (isGrounded)
            {
                _ungroundedTime = 0f;
                transform.localScale = _normalScale;
                return;
            }

            _ungroundedTime += Time.fixedDeltaTime;

            float shrinkDuration = Mathf.Max(0.0001f, groundDeathDelay);
            float shrinkT = Mathf.Clamp01(_ungroundedTime / shrinkDuration);
            float scaleMultiplier = Mathf.Lerp(1f, minimumScaleMultiplier, shrinkT);
            transform.localScale = _normalScale * scaleMultiplier;

            if (groundDeathDelay <= 0f || _ungroundedTime >= groundDeathDelay)
            {
                if (destroyOnDeath)
                    Destroy(gameObject);
                else
                    gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Call to reset the vanish timer and restore the original scale.
        /// </summary>
        public void CancelVanish()
        {
            _ungroundedTime = 0f;
            transform.localScale = _normalScale;
        }
    }
}

