using UnityEngine;

public class Push : MonoBehaviour
{
    public float moveSpeed = 2f;
    public bool isCleared = false;

    public void Move(float direction)
    {
        if (isCleared) return;

        transform.Translate(Vector3.right * direction * moveSpeed * Time.deltaTime);
    }

    public void Clear()
    {
        isCleared = true;
    }
    
}
