using UnityEngine;

namespace FaintFear
{
    public class MentalSafeZone : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                PlayerHealth health = other.GetComponent<PlayerHealth>();
                if (health != null)
                    health.IsInSafeZone =true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                PlayerHealth health = other.GetComponent<PlayerHealth>();
                if (health != null)
                    health.IsInSafeZone = false;
            }
        }
    }
}
