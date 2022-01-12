using UnityEngine;

namespace HoverCar.Player
{
    public class GunController : MonoBehaviour
    {
        public GameObject bulletPrefab;
        public Transform barrelShotPoint;

        private void Update()
        {
            var camera = Camera.main;
            var screenPoint = camera.ViewportToScreenPoint(Vector3.one * .5f);
            var ray = camera.ScreenPointToRay(screenPoint);
            if (Physics.Raycast(ray, out var hitInfo, LayerMask.GetMask("Default", "Ground")))
            {
                var hitInfoPoint = hitInfo.point;
                var direction = hitInfoPoint - transform.position;
                var eulerAngles = Quaternion.LookRotation(direction, Vector3.up).eulerAngles;
                transform.rotation = Quaternion.Euler(0, eulerAngles.y, 0);
            }

            if (Input.GetButtonDown("Fire1"))
            {
                Shoot(ray);
            }
        }

        public void Shoot(Ray ray)
        {
            var instance = Instantiate(bulletPrefab, barrelShotPoint.position,
                Quaternion.LookRotation(ray.direction));
            var component = instance.GetComponent<Rigidbody>();
            component.AddForce(instance.transform.forward * 800f);
        }
    }
}