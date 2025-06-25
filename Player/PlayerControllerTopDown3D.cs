using UnityEngine;
using UnityEngine.InputSystem;

namespace ClownMeister.UnityEssentials.Player
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class PlayerControllerTopDown3D : MonoBehaviour
    {
        [SerializeField] private UnityEngine.Camera mainCamera;

        [Header("Movement")]
        [SerializeField] private bool rotateTowardMouse;
        [SerializeField] private float movementSpeed;
        [SerializeField] private float rotationSpeed;

        [Header("Jump")]
        [Tooltip("In seconds")]
        [SerializeField] private float jumpCooldown;
        [SerializeField] private float jumpHeight;
        [SerializeField] private LayerMask jumpRayMask;
        [SerializeField] private float rayLength;

        private Rigidbody body;
        private Collider bodyCollider;

        private Vector2 inputVector;
        private bool jump;
        private Vector3 mousePosition;
        private float nextJump;
        private bool canJump;

        private InputAction moveAction;
        private InputAction jumpAction;
        private InputAction mousePosAction;

        private void Awake()
        {
            body = GetComponent<Rigidbody>();
            bodyCollider = GetComponent<CapsuleCollider>();

            moveAction = new InputAction("Move", InputActionType.Value);
            moveAction.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/w")
                .With("Down", "<Keyboard>/s")
                .With("Left", "<Keyboard>/a")
                .With("Right", "<Keyboard>/d");

            jumpAction = new InputAction("Jump", InputActionType.Button, "<Keyboard>/space");
            mousePosAction = new InputAction("MousePosition", InputActionType.Value, "<Pointer>/position");
        }

        private void OnEnable()
        {
            moveAction.Enable();
            jumpAction.Enable();
            mousePosAction.Enable();
        }

        private void OnDisable()
        {
            moveAction.Disable();
            jumpAction.Disable();
            mousePosAction.Disable();
        }

        private void Start()
        {
            nextJump = 0;
            canJump = false;
        }

        private void Update()
        {
            HandleInput();

            if (jump)
                Jump();
        }

        private void FixedUpdate()
        {
            var targetVector = new Vector3(inputVector.x, 0f, inputVector.y);
            Vector3 movementVector = MoveTowardTarget(targetVector);

            if (rotateTowardMouse)
                RotateFromMouseVector();
            else
                RotateTowardMovementVector(movementVector);
        }

        private void HandleInput()
        {
            inputVector = moveAction.ReadValue<Vector2>();
            jump = jumpAction.IsPressed();
            mousePosition = mousePosAction.ReadValue<Vector2>();
        }

        private void RotateFromMouseVector()
        {
            Ray ray = mainCamera.ScreenPointToRay(mousePosition);

            if (!Physics.Raycast(ray, out RaycastHit hitInfo, 300f)) return;

            Vector3 target = hitInfo.point;
            target.y = transform.position.y;
            transform.LookAt(target);
        }

        private Vector3 MoveTowardTarget(Vector3 targetVector)
        {
            float speed = movementSpeed * Time.deltaTime;

            targetVector = Quaternion.Euler(0, mainCamera.transform.eulerAngles.y, 0) * targetVector;
            Vector3 targetPosition = transform.position + targetVector * speed;
            transform.position = targetPosition;

            return targetVector;
        }

        private void RotateTowardMovementVector(Vector3 movementDirection)
        {
            if (movementDirection.magnitude == 0) return;

            Quaternion rotation = Quaternion.LookRotation(movementDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, rotationSpeed);
        }

        private void Jump()
        {
            if (nextJump > Time.time) return;
            if (!canJump) CheckGroundStatus();
            if (!canJump) return;

            canJump = false;
            nextJump = Time.time + jumpCooldown;
            body.AddForce(Vector3.up * (jumpHeight * body.mass), ForceMode.Impulse);
        }

        private void CheckGroundStatus()
        {
            var rayPos = new Vector3(transform.position.x, bodyCollider.bounds.min.y + 0.05f, transform.position.z);

            if (!Physics.Raycast(rayPos, Vector3.down, out RaycastHit hit, rayLength, jumpRayMask))
            {
                canJump = false;
                return;
            }

            canJump = hit.collider;
        }
    }
}
