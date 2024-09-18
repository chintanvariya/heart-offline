using UnityEngine;

namespace FGSOfflineHeart
{
    public class HT_ReconnectionHandle : MonoBehaviour
    {
        [Header("===== Reconnection Data =====")]
        [SerializeField] private Animator animator;

        private void OnEnable() => animator.enabled = true;

        private void OnDisable() => animator.enabled = false;
    }
}