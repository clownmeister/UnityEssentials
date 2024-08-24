using UnityEngine;

namespace ClownMeister.UnityEssentials.Camera
{
    [RequireComponent(typeof(UnityEngine.Camera))]
    public class TopDownCamera3D : MonoBehaviour
    {
        [Tooltip("Determines if the camera module is active.")]
        public bool active = true;

        [Header("Movement")]
        [Tooltip("Movement speed of the camera.")]
        [SerializeField]
        private float moveSpeed = 30f;

        [Tooltip("Interpolation speed when transitioning the camera position.")]
        [SerializeField]
        private float interpolateSpeed = 20f;

        [Header("Zoom")]
        [Tooltip("Sensitivity of the zoom functionality.")]
        [SerializeField]
        //TODO: Remove or add to settings later
        private float zoomSensitivity = 1f;

        [Tooltip("Minimum zoom level (closer to the ground).")]
        [SerializeField]
        private float minZoom = 5f;

        [Tooltip("Maximum zoom level (farther from the ground).")]
        [SerializeField]
        private float maxZoom = 30f;

        [Tooltip("Speed at which the camera zooms in and out.")]
        [SerializeField]
        private float zoomSpeed = 20f;

        private UnityEngine.Camera _camera;

        private Vector3 _targetPosition;

        private void Start()
        {
            _targetPosition = transform.position;
            _camera = GetComponent<UnityEngine.Camera>();
        }

        private void Update()
        {
            if (!active) return;

            HandleMovement();
            HandleZoom();

            // Smoothly transition to the new target position
            transform.position = Vector3.Lerp(transform.position, _targetPosition, Time.deltaTime * interpolateSpeed);
        }

        private void HandleMovement()
        {
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            Vector3 direction = new Vector3(x, 0, z);

            if (direction.magnitude > 1)
            {
                direction.Normalize();
            }

            _targetPosition += direction * (moveSpeed * Time.deltaTime);
        }

        private void HandleZoom()
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (!(Mathf.Abs(scroll) > 0.01f)) return;
            Vector3 zoomDirection = transform.forward * (scroll * zoomSensitivity * zoomSpeed);
            Vector3 potentialPosition = _targetPosition + zoomDirection;

            // Clamp the Y position to ensure the camera stays within desired vertical bounds
            float clampedY = Mathf.Clamp(potentialPosition.y, minZoom, maxZoom);

            // If clampedY is different from the potential Y position, don't apply further movement
            if (Mathf.Approximately(clampedY, potentialPosition.y))
            {
                _targetPosition = new Vector3(potentialPosition.x, clampedY, potentialPosition.z);
            }
        }
    }
}