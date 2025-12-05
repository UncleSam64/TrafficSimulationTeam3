using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;

public class FirebaseInit : MonoBehaviour
{
    public static DatabaseReference DBref;

    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                Debug.Log("Firebase inicializado correctamente!");
                DBref = FirebaseDatabase.DefaultInstance.RootReference;
            }
            else
            {
                Debug.LogError("Error al inicializar Firebase: " + task.Result);
            }
        });
    }
}
