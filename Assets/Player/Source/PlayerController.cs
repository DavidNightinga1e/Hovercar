using Source.Car;
using UnityEngine;

namespace HoverCar.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private Rigidbody playerRigidbody;
        [SerializeField] private CarAnimation carAnimation;
        [SerializeField] private Transform[] hoverPoints;
        [SerializeField] private Transform thrustPoint;
        [SerializeField] private Collider idleCollider;
        [SerializeField] private Collider flyCollider;

        public float acceleration;
        public float rotation;
        public float hoverForce;
        public float hoverHeight;

        private bool _isGearDown = true;

        private void Start()
        {
            idleCollider.gameObject.SetActive(_isGearDown);
            flyCollider.gameObject.SetActive(!_isGearDown);
        }

        private void Update()
        {
            var gearButton = Input.GetKeyDown(KeyCode.G);
            if (gearButton)
            {
                _isGearDown = !_isGearDown;
                idleCollider.gameObject.SetActive(_isGearDown);
                flyCollider.gameObject.SetActive(!_isGearDown);
                SyncGearStateWithAnimation();
            }

            if (!_isGearDown)
            {
                Drive();
                Hover();
            }

            var isResetButton = Input.GetKeyDown(KeyCode.R);
            if (isResetButton)
            {
                playerRigidbody.angularVelocity = playerRigidbody.velocity = Vector3.zero;
                playerRigidbody.rotation = Quaternion.identity;
            }
        }

        private void Drive()
        {
            var accelerationInput = Input.GetAxis("Vertical");
            var steerInput = Input.GetAxis("Horizontal");

            playerRigidbody.AddForceAtPosition(transform.forward * (accelerationInput * acceleration * Time.deltaTime),
                thrustPoint.position);
            playerRigidbody.AddTorque(Vector3.up * (rotation * steerInput * Time.deltaTime));
        }

        public void Hover()
        {
            foreach (var hoverPoint in hoverPoints)
            {
                var isRaycast = Physics.Raycast(new Ray(hoverPoint.position, Vector3.down), out var hitInfo,
                    hoverHeight, LayerMask.GetMask("Ground"));
                if (isRaycast)
                {
                    var distance = hitInfo.distance;
                    var force = (hoverHeight - distance) / hoverHeight;
                    playerRigidbody.AddForceAtPosition(Vector3.up * (force * hoverForce * Time.deltaTime),
                        hoverPoint.position);
                }
            }
        }

        public void SyncGearStateWithAnimation()
        {
            if (_isGearDown)
                carAnimation.Idle();
            else
                carAnimation.Fly();
        }
    }
}