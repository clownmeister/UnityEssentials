using UnityEngine;
using UnityEngine.InputSystem;

namespace ClownMeister.UnityEssentials.Camera
{
    /// <summary>
    /// A flying camera controller with smooth movement, zoom, and pivot-based orbiting (like Cinema4D).
    /// Uses the new Unity Input System (Unity 6.1).
    /// </summary>
    [RequireComponent(typeof(UnityEngine.Camera))]
    public class EditorLikeCamera3D : MonoBehaviour
    {
        [Header("Speeds")]
        [SerializeField] private float moveSpeed = 50f;
        [SerializeField] private float fastModifier = 4f;
        [SerializeField] private float rotationSpeed = 5f;
        [SerializeField] private float zoomSpeed = 80f;
        [SerializeField] private float smoothTime = 0.1f;
        [SerializeField] private float panSpeed = 0.7f;
        [SerializeField] private float pitchClamp = 90f;
        [Tooltip("If true, enables helicopter-style 'collective' controls. Spacebar will ascend and Left Ctrl will descend, allowing for free vertical movement. If false, vertical movement is disabled.")]
        [SerializeField] private bool collectiveControl = true;
        [Header("Initialization")]
        [Tooltip("How long to wait after the scene loads before accepting input. Prevents camera jumps on startup.")]
        [SerializeField] private float inputActivationDelay = 0.2f;

        private InputAction ascendAction;
        private InputAction descendAction;
        private InputAction moveAction;
        private InputAction lookAction;
        private InputAction scrollAction;
        private InputAction setPivotAction;
        private InputAction panDeltaAction;

        private Vector2 moveInput;
        private Vector2 lookInput;
        private float scrollInput;
        private Vector2 panMouseDelta;

        private Vector3 moveVelocity;

        private Vector3 rotatePivot = Vector3.zero;
        private Vector3 zoomPivot = Vector3.zero;

        [Header("Debug")]
        public bool debug = false;

        private void Awake()
        {
            // Movement (WASD)
            moveAction = new InputAction("Move", InputActionType.Value);
            moveAction.AddCompositeBinding("2DVector").With("Up", "<Keyboard>/w").With("Down", "<Keyboard>/s").With("Left", "<Keyboard>/a").With("Right", "<Keyboard>/d");
            moveAction.performed += OnMove;
            moveAction.canceled += OnMove;

            // Mouse delta (for rotation)
            lookAction = new InputAction("Look", InputActionType.Value, "<Pointer>/delta");
            lookAction.performed += OnLook;
            lookAction.canceled += OnLook;

            // Scroll wheel (for zoom)
            scrollAction = new InputAction("Scroll", InputActionType.Value, "<Mouse>/scroll");
            scrollAction.started += OnSetZoomPivot;
            scrollAction.performed += OnScroll;
            scrollAction.canceled += OnScroll;

            // Left click for pivot (with Alt)
            setPivotAction = new InputAction("SetPivot", InputActionType.Button, "<Mouse>/leftButton");
            setPivotAction.performed += OnSetRotatePivot;

            // Mouse delta for panning (active when middle mouse is held)
            panDeltaAction = new InputAction("PanDelta", InputActionType.Value, "<Mouse>/delta");
            panDeltaAction.performed += OnPanDelta;
            panDeltaAction.canceled += OnPanDelta;
            
            ascendAction = new InputAction("Ascend", InputActionType.Button, "<Keyboard>/space");
            descendAction = new InputAction("Descend", InputActionType.Button, "<Keyboard>/leftCtrl");
        }

        private void OnEnable()
        {
            // We now enable input after a delay in Start() to prevent input accumulation during scene load.
            // StartCoroutine(EnableInputAfterDelay(inputActivationDelay));
            /*
            moveAction.Enable();
            lookAction.Enable();
            scrollAction.Enable();
            setPivotAction.Enable();
            panDeltaAction.Enable();
            ascendAction.Enable();
            descendAction.Enable();
            */
        }

        private void Start()
        {
            // Start a coroutine to enable input after a short delay.
            StartCoroutine(EnableInputAfterDelay(inputActivationDelay));
        }

        private void OnDisable()
        {
            moveAction.Disable();
            lookAction.Disable();
            scrollAction.Disable();
            setPivotAction.Disable();
            panDeltaAction.Disable();
            ascendAction.Disable();
            descendAction.Disable();
        }

        private void Update()
        {
            // Apply rotation directly. This is simpler and more performant.
            // The subsequent position calculations will use this new orientation.
            HandleRotation();

            // Calculate positional displacements for movement, panning, and zooming based on the new orientation.
            Vector3 moveDisplacement = CalculateMovementDisplacement();
            Vector3 panDisplacement = CalculatePanDisplacement();
            Vector3 zoomDisplacement = CalculateZoomDisplacement();

            // Aggregate all positional changes to determine the final target position.
            Vector3 targetPosition = transform.position + moveDisplacement + panDisplacement + zoomDisplacement;
            
            // Smoothly interpolate only the position towards its target.
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref moveVelocity, smoothTime);
            
            Dd($"MoveInput: {moveInput}, LookInput: {lookInput}, ScrollInput: {scrollInput}");
        }

        private Vector3 CalculateMovementDisplacement()
        {
            // Convert input to a world-space direction and smooth
            var vertical = 0f;
            if (collectiveControl)
            {
                if (ascendAction.IsPressed()) vertical += 1f;
                if (descendAction.IsPressed()) vertical -= 1f;
            }
            var inputDir = new Vector3(moveInput.x, vertical, moveInput.y);
            if (Keyboard.current != null && Keyboard.current.shiftKey.isPressed) inputDir *= fastModifier;
            
            // Return the displacement vector for this frame.
            return transform.TransformDirection(inputDir) * (moveSpeed * Time.deltaTime);
        }

        private Vector3 CalculatePanDisplacement()
        {
            // Check if middle mouse button is pressed and there's mouse movement
            if (Mouse.current == null || !Mouse.current.middleButton.isPressed || panMouseDelta == Vector2.zero)
            {
                return Vector3.zero;
            }

            Vector3 rightMovement = transform.right * (-panMouseDelta.x * panSpeed);
            Vector3 upMovement = transform.up * (-panMouseDelta.y * panSpeed);

            // Return the panning displacement
            return (rightMovement + upMovement) * Time.deltaTime;
        }

        private void HandleRotation()
        {
            if (lookInput == Vector2.zero) return;

            float yaw = lookInput.x * rotationSpeed * Time.deltaTime;
            float pitch = -lookInput.y * rotationSpeed * Time.deltaTime;

            if (Mouse.current.rightButton.isPressed)
            {
                // Free-hand rotation (local rotation when right mouse button is held)
                // Use Euler angles to prevent roll (Z-axis rotation) and allow pitch clamping

                Vector3 currentEuler = transform.rotation.eulerAngles;

                // Convert current pitch (X) from 0-360 to -180-180 for easier clamping
                float currentPitch = currentEuler.x;
                if (currentPitch > 180) currentPitch -= 360;

                // Calculate new yaw and pitch
                float newYaw = currentEuler.y + yaw;
                float newPitch = currentPitch + pitch;

                // Clamp the new pitch to prevent looking straight up/down and avoid flipping
                newPitch = Mathf.Clamp(newPitch, -pitchClamp, pitchClamp); // Use serialized pitchClamp

                // Apply the new rotation, keeping roll (Z) at 0
                // Quaternion.Euler applies rotations in Z, X, Y order.
                // Setting Z to 0 here explicitly removes roll.
                transform.rotation = Quaternion.Euler(newPitch, newYaw, 0f);
            }
            else if (Mouse.current.leftButton.isPressed && Keyboard.current.altKey.isPressed)
            {
                // Pivot-based orbiting (existing logic: Alt + Left Mouse Button)
                transform.RotateAround(rotatePivot, Vector3.up, yaw);
                transform.RotateAround(rotatePivot, transform.right, pitch);
            }
        }

        private Vector3 CalculateZoomDisplacement()
        {
            if (Mathf.Approximately(scrollInput, 0f)) return Vector3.zero;
            float scrollAmount = -scrollInput * zoomSpeed * Time.deltaTime;
            if (Keyboard.current != null && Keyboard.current.shiftKey.isPressed) scrollAmount *= fastModifier;

            Vector3 zoomDisplacement;

            // Check if Alt key is pressed for pivot zoom
            bool altPressed = Keyboard.current != null && Keyboard.current.altKey.isPressed;

            if (!altPressed)
            {
                // ALT IS NOT PRESSED: Zoom along camera's forward/backward axis (middle of screen)
                zoomDisplacement = transform.forward * (-scrollAmount);
            }
            else
            {
                // Vector from the pivot point to the camera
                Vector3 directionToCamera = transform.position - zoomPivot;
                float distanceToPivot = directionToCamera.magnitude;

                // Normalize the direction.
                if (distanceToPivot > Mathf.Epsilon)
                {
                    directionToCamera /= distanceToPivot; // Manual normalization
                }
                else if (scrollAmount < 0)
                {
                    // Already at the pivot or very close and trying to zoom in further
                    directionToCamera = Vector3.zero;
                    Dd("Already at the pivot or very close while trying to zoom in 🧑‍🤝‍🧑");
                }

                if (scrollAmount < 0) // Zooming IN (scrollAmount is negative, so -scrollAmount is positive)
                {
                    // Prevent zooming past the pivot when zooming in.
                    if (distanceToPivot < -scrollAmount)
                    {
                        scrollAmount = -distanceToPivot; // Clamp scrollAmount to move exactly to the pivot
                        Dd("Reset to point⚠️");
                    }
                }

                if (directionToCamera == Vector3.zero && scrollAmount > 0)
                {
                    //We are too close to set another point so just zoom out normally
                    zoomDisplacement = transform.forward * (-scrollAmount);
                }
                else
                {
                    zoomDisplacement = directionToCamera * scrollAmount;
                }

                Dd($"Zoom direction: {directionToCamera} | Scroll amount: {scrollAmount}");
            }

            return zoomDisplacement;
        }

        private System.Collections.IEnumerator EnableInputAfterDelay(float delay)
        {
            // Wait for the specified delay.
            yield return new WaitForSeconds(delay);

            // Now, enable all the input actions.
            moveAction.Enable();
            lookAction.Enable();
            scrollAction.Enable();
            setPivotAction.Enable();
            panDeltaAction.Enable();
            ascendAction.Enable();
            descendAction.Enable();
        }
        private void SetPivot(ref Vector3 pivot)
        {
            if (!UnityEngine.Camera.main)
            {
                pivot = Vector3.zero;
                return;
            }

            Ray ray = UnityEngine.Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                pivot = hit.point;
            }
            else
            {
                Plane ground = new Plane(Vector3.up, Vector3.zero);
                if (ground.Raycast(ray, out float enter))
                {
                    pivot = ray.GetPoint(enter);
                }
            }
        }

        private void Dd(object debugMessage)
        {
            if (!debug) return;
            Debug.Log($"[EditorLikeCamera] {debugMessage}");
        }

        #region Input Callbacks
        private void OnMove(InputAction.CallbackContext ctx)
        {
            moveInput = ctx.ReadValue<Vector2>();
        }

        private void OnLook(InputAction.CallbackContext ctx)
        {
            lookInput = ctx.ReadValue<Vector2>();
        }

        private void OnScroll(InputAction.CallbackContext ctx)
        {
            scrollInput = ctx.ReadValue<Vector2>().y;
        }

        // New callback for pan delta
        private void OnPanDelta(InputAction.CallbackContext ctx)
        {
            panMouseDelta = ctx.ReadValue<Vector2>();
        }

        private void OnSetRotatePivot(InputAction.CallbackContext ctx)
        {
            // ctx.performed is suitable for button actions
            if (!ctx.performed) return;

            // Ensure Alt is pressed when setting rotate pivot with left click
            if (Keyboard.current == null || !Keyboard.current.altKey.isPressed)
            {
                Dd("Attempted to set rotate pivot without Alt key.");
                return;
            }

            Dd("Setting rotate pivot...");
            SetPivot(ref rotatePivot);
        }
        private void OnSetZoomPivot(InputAction.CallbackContext ctx)
        {
            float zoom = ctx.ReadValue<Vector2>().y;
            Dd($"Trying to update zoom pivot y:{zoom}");
            if (Mathf.Approximately(zoom, 0f)) return; // Use Mathf.Approximately for float comparison
            bool altPressed = Keyboard.current != null && Keyboard.current.altKey.isPressed;
            if (!altPressed)
            {
                Dd("Alt is not pressed, zoom pivot not updated via scroll.");
                return;
            }

            ;
            Dd("Updating zoom pivot via scroll + Alt...");
            SetPivot(ref zoomPivot);
        }
        #endregion
    }
}