using System;
using UnityEngine;

namespace HoverCar.Player
{
    public class GunController : MonoBehaviour
    {
        public GameObject bulletPrefab;
        public Transform barrelShotPoint;

        [SerializeField] private Transform turretRotator;
        [SerializeField] private Transform barrelRotator;
        [SerializeField] private RectTransform gunPositionGizmo;

        private Camera _targetCamera;
        private LayerMask _mask;

        private const float MaxDistance = 50f;
        private const float BulletForce = 800f;
        private const float GunMovementSpeed = 4f;

        private void Start()
        {
            _targetCamera = Camera.main;
            if (_targetCamera is null)
                throw new ApplicationException("MainCamera was not found");
            _mask = LayerMask.GetMask("Default", "Ground");
        }

        private void Update()
        {
            var crosshairTargetPoint = GetCrosshairTargetPoint();
            var direction = crosshairTargetPoint - turretRotator.position;

            var lookRotationEulerAngles = Quaternion.LookRotation(direction, transform.up).eulerAngles;
            turretRotator.rotation = Quaternion.Lerp(turretRotator.rotation,
                Quaternion.Euler(0, lookRotationEulerAngles.y, 0), GunMovementSpeed * Time.deltaTime);
            barrelRotator.localRotation = Quaternion.Lerp(barrelRotator.localRotation,
                Quaternion.Euler(lookRotationEulerAngles.x, 0, 0), GunMovementSpeed * Time.deltaTime);

            var isValid = GetGunTargetPoint(out var gunPoint);
            if (isValid)
            {
                var gunDirection = gunPoint - barrelShotPoint.position;
                var dot = Vector3.Dot(_targetCamera.transform.forward, gunDirection);
                if (dot > 0)
                {
                    gunPositionGizmo.gameObject.SetActive(true);
                    var screenPoint = _targetCamera.WorldToScreenPoint(gunPoint);
                    gunPositionGizmo.anchoredPosition = screenPoint;
                }
                else
                    gunPositionGizmo.gameObject.SetActive(false);

                if (Input.GetButtonDown("Fire1"))
                    Shoot(gunDirection);
            }
            else
            {
                gunPositionGizmo.gameObject.SetActive(false);
            }
        }

        private Vector3 GetCrosshairTargetPoint()
        {
            var screenPoint = new Vector3((float) Screen.width / 2, (float) Screen.height / 2);
            var ray = _targetCamera.ScreenPointToRay(screenPoint);
            return Physics.Raycast(ray, out var hitInfo, MaxDistance, _mask)
                ? hitInfo.point
                : ray.GetPoint(MaxDistance);
        }

        private bool GetGunTargetPoint(out Vector3 point)
        {
            var ray = new Ray(barrelShotPoint.position, barrelShotPoint.forward);
            var isCast = Physics.Raycast(ray, out var hitInfo);
            point = hitInfo.point;
            return isCast;
        }

        public void Shoot(Vector3 direction)
        {
            var instance = Instantiate(
                bulletPrefab,
                barrelShotPoint.position,
                Quaternion.LookRotation(direction));
            var component = instance.GetComponent<Rigidbody>();
            component.AddForce(instance.transform.forward * BulletForce);
        }
    }
}