using System;
using UnityEngine;

namespace ClownMeister.UnityEssentials.Camera
{
    [RequireComponent(typeof(UnityEngine.Camera))]
    public class TopDownFollowCamera3D : MonoBehaviour
    {
        public Transform target;
        public float smoothSpeed = .13f;
        public float rotationSpeed = 5f;
        public float zoomSensitivity = 1;
        [SerializeField] private float offsetMultiplier = .5f;

        public Vector3 offset;
        public Vector3 offsetMin = new Vector3(0, 5, -5);
        public Vector3 offsetMax = new Vector3(0, 30, -15);

        private void Update()
        {
            HandleScrollInput();
        }

        private void LateUpdate()
        {
            offset = GetOffset();
            Vector3 desiredPosition = target.position + offset;
            Vector3 lerpedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = lerpedPosition;

            transform.LookAt(target);
            Quaternion targetRotation = Quaternion.LookRotation(target.transform.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        private void HandleScrollInput()
        {
            float scrollY = Input.GetAxis("Mouse ScrollWheel");

            if (!(Math.Abs(scrollY) > 0.01f))
                return;

            float scrollOffset = offsetMultiplier - scrollY * zoomSensitivity;
            offsetMultiplier = Mathf.Clamp(scrollOffset, 0, 1);
        }

        private Vector3 GetOffset()
        {
            return (offsetMax - offsetMin) * offsetMultiplier + offsetMin;
        }
    }
}