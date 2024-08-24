using UnityEngine;

namespace ClownMeister.UnityEssentials.Camera
{
    [RequireComponent(typeof(UnityEngine.Camera))]
    public class TopDownCamera3D : MonoBehaviour
    {
        [Tooltip("Determines if the camera module is active.")]
        public bool active = true;

        [Tooltip("Movement speed of the camera.")]
        [SerializeField]
        private float moveSpeed = 10f;

        [Tooltip("Interpolation speed when transitioning the camera position.")]
        [SerializeField]
        private float interpolateSpeed = 1f;
        private float _interpolateStep;

        private Vector3 _targetPosition, _oldTargetPosition, _startingTransitionPosition;

        private void Start()
        {
            Vector3 position = transform.position;

            _startingTransitionPosition = position;
            _oldTargetPosition = position;
            _targetPosition = position;
        }

        private void Update()
        {
            if (!active) return;

            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            Move(new Vector3(x, 0, z));
        }

        private void Move(Vector3 direction)
        {
            if (Mathf.Abs(direction.x + direction.z) > 1)
            {
                direction = direction.normalized;
            }

            _targetPosition += direction * (moveSpeed * Time.deltaTime);

            if (_targetPosition != _oldTargetPosition)
            {
                _oldTargetPosition = _targetPosition;
                _startingTransitionPosition = transform.position;
            }

            _interpolateStep = Vector3.SqrMagnitude(transform.position - _targetPosition) * interpolateSpeed;

            if (transform.position == _targetPosition)
            {
                return;
            }

            transform.position = Vector3.Lerp(_startingTransitionPosition, _targetPosition, _interpolateStep);
        }
    }
}