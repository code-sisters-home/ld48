using UnityEngine;

public class DebugUIController : MonoBehaviour, IController
{
    public void NotifyOnSpawned(int id) { }

    public void SubscribeSpawnPoint(SpawnPoint spawnPoint) { }

    public void UnsubscribeSpawnPoint(SpawnPoint spawnPoint) { }
}
