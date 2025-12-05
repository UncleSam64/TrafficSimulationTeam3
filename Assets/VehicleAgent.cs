using UnityEngine;

public class VehicleAgent : MonoBehaviour
{
    [Header("Movement")]
    public float maxSpeed = 8f;
    public float acceleration = 4f;
    public float brakingForce = 5f;

    [Header("Traffic Light Reaction")]
    public float lightReactionDistance = 15f;
    public float stopBuffer = 5f;
    public TrafficLightController trafficLight;

    [Header("Vehicle & Obstacle Detection")]
    public float frontSensorLength = 20f;
    public float safeDistance = 16f;
    public LayerMask vehicleLayer;   // Debe incluir autos + topes

    private float speed = 0f;
    private float distanceToLight = Mathf.Infinity;

    private bool carAhead = false;
    private float distAhead = Mathf.Infinity;

    private bool detectedSpeedBump = false;  // TOPE

    void Update()
    {
        DetectLight();
        DetectObstacles();
        Decide();
        MoveForward();
    }

    // ==============================
    // DETECTAR SEMÁFORO
    // ==============================
    void DetectLight()
    {
        if (trafficLight == null || trafficLight.stopLine == null)
        {
            distanceToLight = Mathf.Infinity;
            return;
        }

        distanceToLight = Vector3.Distance(transform.position, trafficLight.stopLine.position);
    }

    // ==============================
    // DETECTAR OBSTÁCULOS (CARROS + TOPE)
    // ==============================
    void DetectObstacles()
    {
        carAhead = false;
        detectedSpeedBump = false;    // reset en cada frame
        distAhead = Mathf.Infinity;

        Vector3 origin = transform.position + Vector3.up * 0.5f;

        if (distAhead < safeDistance / 3)
            {
                speed = 0f;
            }

        // --- SENSOR FRONTAL ---
        if (Physics.Raycast(origin, transform.forward, out RaycastHit hitFront, frontSensorLength, vehicleLayer))
        {
            Debug.DrawLine(origin, hitFront.point, Color.red);

            if (hitFront.collider.CompareTag("Car"))
            {
                carAhead = true;
                distAhead = hitFront.distance;
            }

            if (hitFront.collider.CompareTag("Tope"))
            {
                detectedSpeedBump = true;
                distAhead = hitFront.distance;
            }
        }

        // --- SENSOR BAJO ESPECIAL PARA TOPE ---
        Vector3 lowOrigin = transform.position + Vector3.up * 0.2f;
        if (Physics.Raycast(lowOrigin, transform.forward, out RaycastHit hitBump, 6f, vehicleLayer))
        {
            Debug.DrawLine(lowOrigin, hitBump.point, Color.blue);

            if (hitBump.collider.CompareTag("Tope"))
            {
                detectedSpeedBump = true;
                distAhead = hitBump.distance;
            }
        }
    }

    // ==============================
    // DECISIONES DE FRENADO Y ACELERACIÓN
    // ==============================
    void Decide()
    {
        // --- FRENADO POR SEMÁFORO ---
        bool mustStopForLight =
            distanceToLight < lightReactionDistance &&
            (trafficLight.CurrentState == TrafficLightController.LightState.Red ||
             trafficLight.CurrentState == TrafficLightController.LightState.Yellow);

        if (mustStopForLight)
        {
            float requiredStoppingDistance = (speed * speed) / (2f * brakingForce);

            if (distanceToLight <= requiredStoppingDistance + stopBuffer)
            {
                Brake(brakingForce);
                return;
            }
        }

        // --- TOPE DETECTADO ---
        if (detectedSpeedBump)
        {
            float bumpSpeed = maxSpeed * 0.25f;   // velocidad lenta
            float bumpRange = 3f;

            if (distAhead < bumpRange)
            {
                speed = Mathf.Lerp(speed, bumpSpeed, Time.deltaTime * 4f);
            }

            return;  // evitar que entre a otras lógicas
        }


        // --- CARRO ADELANTE ---
        if (carAhead && distAhead < safeDistance)
        {
            Brake(brakingForce);
            return;
        }

        // --- ACELERAR ---
        Accelerate();
    }

    // ==============================
    // MOVIMIENTO
    // ==============================
    void Accelerate()
    {
        speed += acceleration * Time.deltaTime;
        speed = Mathf.Clamp(speed, 0f, maxSpeed);
    }

    void Brake(float force)
    {
        speed -= force * Time.deltaTime;
        if (speed < 0f) speed = 0f;
    }

    void MoveForward()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    // ==============================
    // DEBUG
    // ==============================
    void OnDrawGizmosSelected()
    {
        Vector3 origin = transform.position + Vector3.up * 0.5f;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(origin, origin + transform.forward * frontSensorLength);

        Vector3 lowOrigin = transform.position + Vector3.up * 0.2f;
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(lowOrigin, lowOrigin + transform.forward * 6f);
    }
}
