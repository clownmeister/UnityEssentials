using System;
using UnityEngine;

namespace ClownMeister.UnityEssentials.Camera
{
    [ExecuteInEditMode]
    public class Billboard : MonoBehaviour
    {
        [Serializable]public struct FreezeRotation
        {
            public bool x;
            public bool y;
            public bool z;
        }

        [SerializeField]public FreezeRotation freezeRotation;
        private UnityEngine.Camera _mainCamera;

        public float damping = 3;
        private void Start()
        {
            this._mainCamera = UnityEngine.Camera.main;
        }

        private void Update()
        {
            Vector3 camPosition = this._mainCamera.transform.position;
            Vector3 lookPos = camPosition - transform.position;
            Quaternion rotation = Quaternion.LookRotation(-lookPos);
            Quaternion smoothRotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * this.damping);
            transform.rotation = smoothRotation;

            //TODO: Fix freeze
            // Vector3 smoothRotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * this.damping).eulerAngles;
            // Vector3 frozenRotation = new();
            // if (!this.freezeRotation.x) frozenRotation.x = smoothRotation.x;
            // if (!this.freezeRotation.y) frozenRotation.y = smoothRotation.y;
            // if (!this.freezeRotation.z) frozenRotation.z = smoothRotation.z;
            // transform.Rotate(frozenRotation);
        }
    }
}