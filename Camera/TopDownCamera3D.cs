using UnityEngine;
using UnityEngine.InputSystem;

namespace ClownMeister.UnityEssentials.Camera
{
    [RequireComponent(typeof(UnityEngine.Camera))]
    public class TopDownCamera3D : MonoBehaviour
    {
        [Tooltip("Determines if the camera module is active.")]
        public bool active = true;

        [Header("Movement")]
        [SerializeField] private float moveSpeed = 30f;
        [SerializeField] private float interpolateSpeed = 20f;

        [Header("Zoom")]
        [SerializeField] private float zoomSensitivity = 1f;
        [SerializeField] private float minZoom = 5f;
        [SerializeField] private float maxZoom = 30f;
        [SerializeField] private float zoomSpeed = 20f;

        private Vector3 targetPosition;

        private InputAction moveAction;
        private InputAction scrollAction;

        private void Awake()
        {
            moveAction = new InputAction("Move", InputActionType.Value);
            moveAction.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/w")
                .With("Down", "<Keyboard>/s")
                .With("Left", "<Keyboard>/a")
                .With("Right", "<Keyboard>/d");

            scrollAction = new InputAction("Scroll", InputActionType.Value, "<Mouse>/scroll");
        }

        private void OnEnable()
        {
            moveAction.Enable();
            scrollAction.Enable();
        }

        private void OnDisable()
        {
            moveAction.Disable();
            scrollAction.Disable();
        }

        private void Start()
        {
            targetPosition = transform.position;
        }

        private void Update()
        {
            if (!active) return;

            HandleMovement();
            HandleZoom();

            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * interpolateSpeed);
        }

        private void HandleMovement()
        {
            var input = moveAction.ReadValue<Vector2>();
            var direction = new Vector3(input.x, 0f, input.y);

            if (direction.magnitude > 1f)
                direction.Normalize();

            targetPosition += direction * (moveSpeed * Time.deltaTime);
        }

        private void HandleZoom()
        {
            float scroll = scrollAction.ReadValue<Vector2>().y;
            if (Mathf.Abs(scroll) < 0.01f) return;

            Vector3 zoomDirection = transform.forward * (scroll * zoomSensitivity * zoomSpeed);
            Vector3 potentialPosition = targetPosition + zoomDirection;

            float clampedY = Mathf.Clamp(potentialPosition.y, minZoom, maxZoom);
            if (Mathf.Approximately(clampedY, potentialPosition.y))
            {
                targetPosition = new Vector3(potentialPosition.x, clampedY, potentialPosition.z);
            }
        }
    }
}
