using UnityEngine;
using UnityEngine.InputSystem;

namespace ClownMeister.UnityEssentials.Camera
{
    [RequireComponent(typeof(UnityEngine.Camera))]
    public class FlyCamera : MonoBehaviour
    {
        [Header("Movement")]
        public float acceleration = 50f;
        public float accSprintMultiplier = 4f;

        [Header("Rotation")]
        public float lookSensitivity = 1f;

        [Header("Damping")]
        public float dampingCoefficient = 5f;

        [Header("Cursor")]
        public bool focusOnEnable = true;

        private Vector3 velocity;

        private InputAction moveAction;
        private InputAction lookAction;
        private InputAction sprintAction;
        private InputAction upAction;
        private InputAction downAction;
        private InputAction escapeAction;
        private InputAction clickAction;

        private static bool Focused
        {
            get => Cursor.lockState == CursorLockMode.Locked;
            set
            {
                Cursor.lockState = value ? CursorLockMode.Locked : CursorLockMode.None;
                Cursor.visible = !value;
            }
        }

        private void Awake()
        {
            moveAction = new InputAction("Move", InputActionType.Value);
            moveAction.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/w")
                .With("Down", "<Keyboard>/s")
                .With("Left", "<Keyboard>/a")
                .With("Right", "<Keyboard>/d");

            lookAction = new InputAction("Look", InputActionType.Value, "<Pointer>/delta");

            sprintAction = new InputAction("Sprint", InputActionType.Button, "<Keyboard>/leftShift");
            upAction = new InputAction("Up", InputActionType.Button, "<Keyboard>/space");
            downAction = new InputAction("Down", InputActionType.Button, "<Keyboard>/leftCtrl");
            escapeAction = new InputAction("Escape", InputActionType.Button, "<Keyboard>/escape");
            clickAction = new InputAction("Click", InputActionType.Button, "<Mouse>/leftButton");
        }

        private void OnEnable()
        {
            moveAction.Enable();
            lookAction.Enable();
            sprintAction.Enable();
            upAction.Enable();
            downAction.Enable();
            escapeAction.Enable();
            clickAction.Enable();

            if (focusOnEnable)
                Focused = true;
        }

        private void OnDisable()
        {
            moveAction.Disable();
            lookAction.Disable();
            sprintAction.Disable();
            upAction.Disable();
            downAction.Disable();
            escapeAction.Disable();
            clickAction.Disable();

            Focused = false;
        }

        private void Update()
        {
            if (Focused)
            {
                UpdateInput();
            }
            else if (clickAction.WasPressedThisFrame())
            {
                Focused = true;
            }

            velocity = Vector3.Lerp(velocity, Vector3.zero, dampingCoefficient * Time.deltaTime);
            transform.position += velocity * Time.deltaTime;
        }

        private void UpdateInput()
        {
            Vector3 accel = GetAccelerationVector();
            velocity += accel * Time.deltaTime;

            Vector2 mouseDelta = lookAction.ReadValue<Vector2>() * lookSensitivity;
            Quaternion horiz = Quaternion.AngleAxis(mouseDelta.x, Vector3.up);
            Quaternion vert = Quaternion.AngleAxis(-mouseDelta.y, Vector3.right);
            transform.rotation = horiz * transform.rotation * vert;

            if (escapeAction.WasPressedThisFrame())
                Focused = false;
        }

        private Vector3 GetAccelerationVector()
        {
            var move = moveAction.ReadValue<Vector2>();
            var dir = new Vector3(move.x, 0, move.y);

            if (upAction.IsPressed()) dir += Vector3.up;
            if (downAction.IsPressed()) dir += Vector3.down;

            Vector3 worldDir = transform.TransformDirection(dir.normalized);
            float speed = sprintAction.IsPressed() ? acceleration * accSprintMultiplier : acceleration;

            return worldDir * speed;
        }
    }
}
