using System;
using UnityEngine;

namespace HoverCar.Player
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Camera targetCamera;
        [SerializeField] private Transform pivot;

        private float xAngle;
        private float yAngle;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void LateUpdate()
        {
            var mouseX = Input.GetAxis("Mouse X");
            var mouseY = Input.GetAxis("Mouse Y");

            var newX = xAngle + mouseX;
            var newY = yAngle - mouseY;

            xAngle = newX;
            yAngle = newY;

            yAngle = Mathf.Clamp(yAngle, -90, 90);


            targetCamera.transform.position = pivot.position - Vector3.forward * -4f;

            targetCamera.transform.LookAt(pivot);
            targetCamera.transform.RotateAround(pivot.position, Vector3.up, xAngle);
            targetCamera.transform.RotateAround(pivot.position, targetCamera.transform.right, yAngle);
        }
    }
}