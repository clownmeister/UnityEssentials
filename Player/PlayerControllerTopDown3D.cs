// using UnityEngine;
// using UnityEngine.InputSystem;
// using Input = UnityEngine.Windows.Input;
//
// namespace ClownMeister.Player
// {
//     [RequireComponent(typeof(Rigidbody))]
//     [RequireComponent(typeof(CapsuleCollider))]
//     public class PlayerControllerTopDown3D : MonoBehaviour
//     {
//         [SerializeField] private UnityEngine.Camera mainCamera;
//
//         [Header("Movement")]
//         [SerializeField] private bool rotateTowardMouse;
//         [SerializeField] private float movementSpeed;
//         [SerializeField] private float rotationSpeed;
//         [Header("Jump")]
//         [Tooltip("In seconds")]
//         [SerializeField] private float jumpCooldown;
//         [SerializeField] private float jumpHeight;
//         [SerializeField] private LayerMask jumpRayMask;
//         [SerializeField] private float rayLenght;
//
//         private Rigidbody body;
//         private Collider bodyCollider;
//
//         private bool canJump;
//         private float nextJump;
//
//         private void Start()
//         {
//             this.nextJump = 0;
//             this.canJump = false;
//         }
//
//         private void Awake()
//         {
//             this.body = GetComponent<Rigidbody>();
//             this.bodyCollider = GetComponent<CapsuleCollider>();
//         }
//
//         private void OnJump()
//         {
//             Jump();
//         }
//
//         private void FixedUpdate()
//         {
//             Vector3 targetVector = new(this.input.InputVector.x, 0, this.input.InputVector.y);
//             Vector3 movementVector = MoveTowardTarget(targetVector);
//
//             if (!this.rotateTowardMouse) RotateTowardMovementVector(movementVector);
//             if (this.rotateTowardMouse) RotateFromMouseVector();
//
//
//         }
//
//         private void RotateFromMouseVector()
//         {
//             Ray ray = this.mainCamera.ScreenPointToRay(this.input.MousePosition);
//
//             if (!Physics.Raycast(ray, out RaycastHit hitInfo, 300f)) return;
//
//             Vector3 target = hitInfo.point;
//             target.y = transform.position.y;
//             transform.LookAt(target);
//         }
//
//         private Vector3 MoveTowardTarget(Vector3 targetVector)
//         {
//             float speed = this.movementSpeed * Time.deltaTime;
//
//             targetVector = Quaternion.Euler(0, this.mainCamera.gameObject.transform.rotation.eulerAngles.y, 0) * targetVector;
//             Vector3 targetPosition = transform.position + targetVector * speed;
//             transform.position = targetPosition;
//             return targetVector;
//         }
//
//         private void RotateTowardMovementVector(Vector3 movementDirection)
//         {
//             if (movementDirection.magnitude == 0) return;
//             Quaternion rotation = Quaternion.LookRotation(movementDirection);
//             transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, this.rotationSpeed);
//         }
//
//         private void Jump()
//         {
//             if (this.nextJump > Time.time) return;
//             if (!this.canJump) CheckGroundStatus();
//             if (!this.canJump) return;
//
//             this.canJump = false;
//             this.nextJump = Time.time + this.jumpCooldown;
//             this.body.AddForce(0, this.jumpHeight * this.body.mass, 0, ForceMode.Impulse);
//         }
//
//         private void CheckGroundStatus()
//         {
//             Vector3 rayPos = new(transform.position.x, this.bodyCollider.bounds.min.y + 0.05f, transform.position.z);
//             if (!Physics.Raycast(new Ray(rayPos, Vector3.down), out RaycastHit hit, this.rayLenght, this.jumpRayMask)) {
//                 this.canJump = false;
//                 return;
//             }
//
//             this.canJump = hit.collider != null;
//         }
//     }
// }

