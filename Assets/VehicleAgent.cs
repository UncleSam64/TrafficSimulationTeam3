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

    [Header("Vehicle Detection")]
    public float frontSensorLength = 20f;
    public float safeDistance = 16f;
    public LayerMask vehicleLayer;

    private float speed = 0f;
    private float distanceToLight = Mathf.Infinity;

    private bool carAhead = false;
    private float distAhead = Mathf.Infinity;

    void Update()
    {
        DetectLight();
        DetectVehicles();
        Decide();
        MoveForward();
    }

    // ==========================
    // DETECTAR SEMÁFORO
    // ==========================
    void DetectLight()
    {
        if (trafficLight == null || trafficLight.stopLine == null)
        {
            distanceToLight = Mathf.Infinity;
            return;
        }

        distanceToLight = Vector3.Distance(transform.position, trafficLight.stopLine.position);
    }

    // ==========================
    // DETECTAR OTROS VEHÍCULOS
    // ==========================
    void DetectVehicles()
    {
        carAhead = false;

        // Origen del raycast
        Vector3 origin = transform.position + Vector3.up * 0.5f;

        // --- Sensor frontal ---
        if (Physics.Raycast(origin, transform.forward, out RaycastHit hitFront, frontSensorLength, vehicleLayer))
        {
            if (hitFront.collider.CompareTag("Car"))
            {
                carAhead = true;
                distAhead = hitFront.distance;

                Debug.DrawLine(origin, hitFront.point, Color.red);
                if(carAhead && distAhead <safeDistance)
                {
                    Brake(brakingForce);
                    return;
                }
            }
        }
        else distAhead = Mathf.Infinity;

    }

    // ==========================
    // TOMAR DECISIONES
    // ==========================
    void Decide()
    {
        bool mustStopForLight =
            distanceToLight < lightReactionDistance &&
            (trafficLight.CurrentState == TrafficLightController.LightState.Red ||
             trafficLight.CurrentState == TrafficLightController.LightState.Yellow);


        // --- Frenar por semáforo ---
        if (mustStopForLight)
        {
            float required = (speed * speed) / (2f * brakingForce);

            if (distanceToLight <= required + stopBuffer)
            {
                Brake(brakingForce);
                return;
            }
        }

        // --- Frenar por coche enfrente ---
        if (carAhead && distAhead < safeDistance)
        {
            if(distAhead < safeDistance/3)
            {
                speed = 0f;
            }
            Brake(brakingForce);
            return;
        }

        // --- Acelerar si todo está libre ---
        Accelerate();
    }

    // ==========================
    // MOVIMIENTO
    // ==========================
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

    // ==========================
    // VISUAL DEBUG
    // ==========================
    void OnDrawGizmosSelected()
    {
        Vector3 origin = transform.position + Vector3.up * 0.5f;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(origin, origin + transform.forward * frontSensorLength);
    }
}
