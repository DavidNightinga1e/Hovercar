using UnityEngine;

namespace Source.Car
{
    public class CarAnimation : MonoBehaviour
    {
        [SerializeField] private Animator animator;

        [ContextMenu("Fly")]
        public void Fly()
        {
            animator.SetTrigger("GearUp");
        }

        [ContextMenu("Idle")]
        public void Idle()
        {
            animator.SetTrigger("GearDown");
        }
    }
}