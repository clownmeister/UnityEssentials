using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ClownMeister.UnityEssentials.Camera
{
    public class CameraFollowTopDown3D : MonoBehaviour
    {
        public Transform target;
        public float smoothSpeed = .13f;
        public float rotationSpeed = 5f;
        public float zoomSensitivity = 1;
        [SerializeField] private float offsetMultiplier = .5f;

        public Vector3 offset;
        public Vector3 offsetMin = new(0, 5, -5);
        public Vector3 offsetMax = new(0, 30, -15);

        public void OnScroll(InputValue inputValue)
        {
            float scrollY = inputValue.Get<Vector2>().y;

            if (scrollY == 0) return;
            float scrollOffset = this.offsetMultiplier + -scrollY * this.zoomSensitivity;
            this.offsetMultiplier = scrollOffset switch
            {
                < 0 => 0,
                > 1 => 1,
                _ => scrollOffset
            };
        }

        private void FixedUpdate()
        {
            this.offset = GetOffset();
            Vector3 desiredPosition = this.target.position + this.offset;
            Vector3 lerpedPosition = Vector3.Lerp(transform.position, desiredPosition, this.smoothSpeed);
            transform.position = lerpedPosition;

            // transform.LookAt(this.target); //TODO: stuttering fix
            Quaternion targetRotation = Quaternion.LookRotation(this.target.transform.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, this.rotationSpeed * Time.deltaTime);
        }

        private Vector3 GetOffset()
        {
            return (this.offsetMax - this.offsetMin) * this.offsetMultiplier + this.offsetMin;
        }
    }
}