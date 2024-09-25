using System.Collections;
using UnityEngine;

public class TargetX : MonoBehaviour
{
    [Header("References")]
    public Rigidbody rb;

    [Header("Settings")]
    public int pointValue = 1;
    public float timeOnScreen = 1.0f;
    public bool isBad = false;

    [Header("Particles")]
    public GameObject explosionFx;

    [Header("Sounds")]
    public AudioClip explosionSound;

    private void Start()
    {
        // make the target disappear after some time
        StartCoroutine(RemoveObjectRoutine());
    }

    // after a delay, moves the target behind background so it collides with the Sensor object
    private IEnumerator RemoveObjectRoutine()
    {
        yield return new WaitForSeconds(GameManagerX.singleton.RespawnRate / GameManagerX.singleton.GetDifficultyMultiplier());
        if (GameManagerX.singleton.IsGameActive)
        {
            transform.Translate(Vector3.forward * 5, Space.World);
        }
    }

    // when the mouse is clicked on this target
    private void OnMouseDown()
    {
        // only while in gameplay
        if (GameManagerX.singleton.IsGameActive)
        {
            // destroy target
            Destroy(gameObject);

            // add score
            GameManagerX.singleton.AddScore(pointValue);

            // play explosion effects
            Explode();
        }
    }

    private void Explode()
    {
        // play explosion particle
        Instantiate(explosionFx, transform.position, explosionFx.transform.rotation);

        // play explosion sound
        AudioManager.singleton.PlaySound2DOneShot(explosionSound);
    }

    // detect entering the sensor
    private void OnTriggerEnter(Collider other)
    {
        // destroy target
        Destroy(gameObject);

        // if this is a good target and it collides with sensor (it wasn't clicked on time), trigger game over
        if (other.gameObject.CompareTag("Sensor")
            && !isBad)
        {
            GameManagerX.singleton.GameOver();
        }
    }
}
