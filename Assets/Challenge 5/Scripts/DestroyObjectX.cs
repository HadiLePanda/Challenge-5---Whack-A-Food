using UnityEngine;

public class DestroyObjectX : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float timer = 2f;

    private void Start()
    {
        // auto destroy after time
        Destroy(gameObject, timer);
    }
}
