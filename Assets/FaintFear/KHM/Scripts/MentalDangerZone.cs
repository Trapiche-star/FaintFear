using UnityEngine;
namespace FaintFear
{
    public class MentalDangerZone : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                PlayerHealth health = other.GetComponent<PlayerHealth>();
                if (health != null)
                    health.IsInCorpseRoom = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                PlayerHealth health = other.GetComponent<PlayerHealth>();
                if (health != null)
                    health.IsInCorpseRoom = false;
            }
        }
    }
}