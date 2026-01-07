using System.Collections;
using UnityEngine;

namespace FaintFear
{
    public abstract class TutorialEventBase : MonoBehaviour
    {
        protected GameObject player;
        protected Transform cameraPosition;
        protected PlayerMove playerMove;
        protected Flashlight flashlight;

        protected bool hasPlayed;
        protected Coroutine runningCoroutine;

        #region LifeCycle

        protected virtual void Awake()
        {
            if (IsTutorialCompleted())
            {
                Destroy(gameObject);
            }
        }

        protected virtual void OnDisable()
        {
            StopAllCoroutines();
            runningCoroutine = null;
        }

        #endregion

        #region Bind

        public virtual void BindPlayer(GameObject playerObj)
        {
            if (playerObj == null) return;

            player = playerObj;

            playerMove = player.GetComponent<PlayerMove>();
            flashlight = player.GetComponentInChildren<Flashlight>(true);

            cameraPosition = player.transform.Find("CameraPosition");

            if (cameraPosition == null)
                Debug.LogError($"[{GetType().Name}] CameraPosition not found");
        }

        #endregion

        #region Guard

        protected bool CanPlay()
        {
            if (hasPlayed) return false;
            if (IsTutorialCompleted()) return false;
            if (player == null) return false;
            return true;
        }

        protected void Play(IEnumerator sequence)
        {
            if (!CanPlay()) return;

            hasPlayed = true;
            runningCoroutine = StartCoroutine(sequence);
        }

        protected void End()
        {
            if (runningCoroutine != null)
            {
                StopCoroutine(runningCoroutine);
                runningCoroutine = null;
            }
        }

        #endregion

        #region Abstract

        protected abstract bool IsTutorialCompleted();

        #endregion
    }
}
