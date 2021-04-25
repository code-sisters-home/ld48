using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField] private Enemy _enemyPrefab;
    [SerializeField] private int _count = 1;

    private void Awake()
    {
        _enemyPrefab.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        for (int i = 0; i < _count; i++)
        {
            //var random = new System.Random();
            StartCoroutine(Spawn(Random.Range(0f, 3f)));
        }
    }

    private IEnumerator Spawn(float delay)
    {
        Debug.Log(delay);
        yield return new WaitForSeconds(delay);

        var enemy = Instantiate(_enemyPrefab, transform);
        enemy.enabled = false;
        enemy.transform.localPosition = Vector3.zero;
        enemy.transform.localScale = Vector3.one;
        enemy.gameObject.SetActive(true);

        yield return new WaitForFixedUpdate();

        enemy.GetComponent<Rigidbody2D>().AddForce(transform.forward * 5f, ForceMode2D.Impulse);

        yield return new WaitForSeconds(1f);

        enemy.enabled = true;
    }
}
