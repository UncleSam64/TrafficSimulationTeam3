using UnityEngine;

public class CarroData : MonoBehaviour
{
    public string carritoID = "carro1";

    void Update()
    {
        if (FirebaseInit.DBref == null) return;

        float velocidad = GetComponent<Rigidbody>().linearVelocity.magnitude;
        Vector3 pos = transform.position;

        string path = "carros/" + carritoID;

        FirebaseInit.DBref.Child(path).Child("posX").SetValueAsync(pos.x);
        FirebaseInit.DBref.Child(path).Child("posY").SetValueAsync(pos.y);
        FirebaseInit.DBref.Child(path).Child("posZ").SetValueAsync(pos.z);
        FirebaseInit.DBref.Child(path).Child("velocidad").SetValueAsync(velocidad);
    }
}
