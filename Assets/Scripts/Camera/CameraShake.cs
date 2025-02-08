using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public float shakeMagnitude = 0.05f;
    public float shakeFrequency = 1.0f;
    private Vector3 originalPosition;

    private float timeOffsetX;
    private float timeOffsetY;

    private void Start()
    {
        originalPosition = transform.localPosition;
        
        timeOffsetX = Random.Range(0f, 100f);
        timeOffsetY = Random.Range(0f, 100f);
    }

    private void Update()
    {
        float x = Mathf.PerlinNoise(Time.time * shakeFrequency + timeOffsetX, 0) * 2f - 1f;
        float y = Mathf.PerlinNoise(0, Time.time * shakeFrequency + timeOffsetY) * 2f - 1f;
        
        x *= shakeMagnitude;
        y *= shakeMagnitude;
        
        transform.localPosition = originalPosition + new Vector3(x, y, 0f);
    }
}
