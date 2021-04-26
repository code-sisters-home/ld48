using UnityEngine;

public class DebugUIController : MonoBehaviour, IController
{
    public CameraFollow _cam;

    public void NotifyOnSpawned(int id) { }

    public void SubscribeSpawnPoint(SpawnPoint spawnPoint) { }

    public void UnsubscribeSpawnPoint(SpawnPoint spawnPoint) { }

    private void Awake()
    {
        _cam.Target = GameObject.FindGameObjectWithTag("CameraFollowTarget").transform;
    }
}
