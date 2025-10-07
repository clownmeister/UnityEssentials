using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ClownMeister.UnityEssentials.Camera
{
    [RequireComponent(typeof(UnityEngine.Camera))]
    public class TopDownFollowCamera3D : MonoBehaviour
    {
        [Tooltip("The target the camera will follow.")]
        public Transform target;
        [Tooltip("How quickly the camera catches up to the target's position. Smaller values are faster.")]
        public float positionSmoothTime = 0.1f;
        [Tooltip("How quickly the camera rotates to look at the target.")]
        public float rotationSpeed = 5f;
        [Tooltip("How fast the camera zooms in and out with the mouse wheel.")]
        public float zoomSensitivity = 0.1f;

        [Tooltip("The current zoom level, from 0 (min) to 1 (max).")]
        [SerializeField, Range(0f, 1f)] private float zoomLevel = 0.5f;

        [Tooltip("The calculated offset from the target. Read-only.")]
        public Vector3 offset;
        [Tooltip("The minimum offset from the target (fully zoomed in).")]
        public Vector3 offsetMin = new Vector3(0, 5, -5);
        [Tooltip("The maximum offset from the target (fully zoomed out).")]
        public Vector3 offsetMax = new Vector3(0, 30, -15);

        private InputAction scrollAction;
        private Vector3 cameraVelocity = Vector3.zero;

        private void Awake()
        {
            scrollAction = new InputAction("Scroll", InputActionType.Value, "<Mouse>/scroll");
        }

        private void OnEnable()
        {
            scrollAction.Enable();
        }

        private void OnDisable()
        {
            scrollAction.Disable();
        }

        private void Update()
        {
            HandleScrollInput();
        }

        private void LateUpdate()
        {
            if (target == null) return;
            
            offset = GetOffset();
            Vector3 desiredPosition = target.position + offset;
            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref cameraVelocity, positionSmoothTime);

            Quaternion targetRotation = Quaternion.LookRotation(target.position - transform.position);
            // Use a frame-rate independent formula for Slerp for more consistent rotation speed.
            float step = 1.0f - Mathf.Exp(-rotationSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, step);
        }

        private void HandleScrollInput()
        {
            float scrollY = scrollAction.ReadValue<Vector2>().y;
            if (Math.Abs(scrollY) < 0.01f)
                return;

            // Normalize scroll value and apply sensitivity
            float zoomAmount = scrollY > 0 ? -1 : 1;
            zoomLevel += zoomAmount * zoomSensitivity;
            zoomLevel = Mathf.Clamp01(zoomLevel);
        }

        private Vector3 GetOffset()
        {
            return Vector3.Lerp(offsetMin, offsetMax, zoomLevel);
        }
    }
}
