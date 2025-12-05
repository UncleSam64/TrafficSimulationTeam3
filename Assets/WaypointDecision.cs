using UnityEngine;

public class WaypointDecision : MonoBehaviour
{
    public enum TurnType { Straight, Right, Left }
    public TurnType turnType = TurnType.Straight;

    [Header("Opcional: siguiente puntos")]
    public Transform nextStraight;
    public Transform nextRight;
    public Transform nextLeft;
}
