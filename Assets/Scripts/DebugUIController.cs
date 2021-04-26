using UnityEngine;

public class DebugUIController : MonoBehaviour, IController
{
    public void NotifyOnSpawned(int id) { }

    public void SubscribeSpawnPoint(SpawnPoint spawnPoint) { }

    public void UnsubscribeSpawnPoint(SpawnPoint spawnPoint) { }

    private void Awake()
    {
        Camera.main.GetComponent<CameraFollow>().Target = GameObject.FindGameObjectWithTag("CameraFollowTarget").transform;
    }
}
