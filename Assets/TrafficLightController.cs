using UnityEngine;

public class TrafficLightController : MonoBehaviour
{
    public enum LightState { Red, Yellow, Green }

    [Header("Estado actual del semáforo")]
    public LightState currentState = LightState.Red;

    [Header("Tiempos (segundos)")]
    public float greenTime = 2f;
    public float yellowTime = 4f;
    public float redTime = 8f;

    [Header("Línea de alto")]
    public Transform stopLine;

    private float timer = 0f;

    public LightState CurrentState => currentState;

    void Update()
    {
        timer += Time.deltaTime;

        switch (currentState)
        {
            case LightState.Red:
                if (timer >= redTime)
                {
                    currentState = LightState.Green;
                    timer = 0f;
                }
                break;

            case LightState.Green:
                if (timer >= greenTime)
                {
                    currentState = LightState.Yellow;
                    timer = 0f;
                }
                break;

            case LightState.Yellow:
                if (timer >= yellowTime)
                {
                    currentState = LightState.Red;
                    timer = 0f;
                }
                break;
        }
    }
}
