using UnityEngine;

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

        [SerializeField] private float scrollSensitivity = 1;
        [SerializeField] private float scrollSensitivityController = .005f;

        private Rigidbody _body;
        private Collider _bodyCollider;

        private bool _canJump;

        private Vector2 _inputVector;
        private bool _jump;
        private Vector3 _mousePosition;
        private float _nextJump;

        private void Awake()
        {
            _body = GetComponent<Rigidbody>();
            _bodyCollider = GetComponent<CapsuleCollider>();
        }

        private void Start()
        {
            _nextJump = 0;
            _canJump = false;
        }

        private void Update()
        {
            HandleInput();

            if (_jump)
            {
                Jump();
            }
        }

        private void FixedUpdate()
        {
            Vector3 targetVector = new Vector3(_inputVector.x, 0, _inputVector.y);
            Vector3 movementVector = MoveTowardTarget(targetVector);

            if (rotateTowardMouse)
            {
                RotateFromMouseVector();
            }
            else
            {
                RotateTowardMovementVector(movementVector);
            }
        }

        private void HandleInput()
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            _inputVector = new Vector2(h, v);

            _mousePosition = Input.mousePosition;
            _jump = Input.GetAxis("Jump") > 0;
        }

        private void RotateFromMouseVector()
        {
            Ray ray = mainCamera.ScreenPointToRay(_mousePosition);

            if (!Physics.Raycast(ray, out RaycastHit hitInfo, 300f)) return;

            Vector3 target = hitInfo.point;
            target.y = transform.position.y;
            transform.LookAt(target);
        }

        private Vector3 MoveTowardTarget(Vector3 targetVector)
        {
            float speed = movementSpeed * Time.deltaTime;

            targetVector = Quaternion.Euler(0, mainCamera.gameObject.transform.rotation.eulerAngles.y, 0) * targetVector;
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
            if (_nextJump > Time.time) return;
            if (!_canJump) CheckGroundStatus();
            if (!_canJump) return;

            _canJump = false;
            _nextJump = Time.time + jumpCooldown;
            _body.AddForce(0, jumpHeight * _body.mass, 0, ForceMode.Impulse);
        }

        private void CheckGroundStatus()
        {
            Vector3 rayPos = new Vector3(transform.position.x, _bodyCollider.bounds.min.y + 0.05f, transform.position.z);
            if (!Physics.Raycast(new Ray(rayPos, Vector3.down), out RaycastHit hit, rayLength, jumpRayMask))
            {
                _canJump = false;
                return;
            }

            _canJump = hit.collider;
        }
    }
}