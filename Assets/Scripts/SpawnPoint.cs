using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField] private string _tag = "";
    [SerializeField] private Enemy _enemyPrefab;
    [SerializeField] private int _count = 1;
    [SerializeField] private float _timerMin = 1.0f;
    [SerializeField] private float _timerMax = 3.0f;

    public int Count => _count;
    public string Tag => _tag;
    private int _deadEnemies = 0;

    private IController _uiController;

    private void Awake()
    {
        _uiController = GameObject.Find("SimpleUIController").GetComponent<IController>();
        _enemyPrefab.gameObject.SetActive(false);

        _uiController.SubscribeSpawnPoint(this);
    }

    private void OnEnable()
    {
        for (int i = 0; i < _count; i++)
        {
            StartCoroutine(Spawn());
        }
    }

    private IEnumerator Spawn()
    {
        yield return new WaitForSeconds(0.1f);

        StartCoroutine(DoSpawn(UnityEngine.Random.Range(_timerMin, _timerMax)));
    }

    private IEnumerator DoSpawn(float delay)
    {
        yield return new WaitForSeconds(delay);

        var enemy = Instantiate(_enemyPrefab);
        enemy.enabled = false;
        enemy.transform.position = transform.position;
        //enemy.transform.localScale = Vector3.one;
        enemy.gameObject.SetActive(true);
        enemy.OnDead += CheckCompletion;

        yield return new WaitForEndOfFrame();
        yield return new WaitForFixedUpdate();

        enemy.GetComponent<Rigidbody2D>().AddForce(transform.up * 5f, ForceMode2D.Impulse);

        yield return new WaitForSeconds(1);

        enemy.enabled = true;

        if (_uiController != null)
        {
            int dialog_id = 0;
            if (_enemyPrefab.name == "Enemy_house")
                dialog_id = 1;
            if (_enemyPrefab.name == "Enemy_clock")
                dialog_id = 2;
            if (_enemyPrefab.name == "Enemy_heart")
                dialog_id = 3;
            if (_enemyPrefab.name == "Enemy_finger")
                dialog_id = 4;
            if (_enemyPrefab.name == "Enemy_clown")
                dialog_id = 5;
            if (_enemyPrefab.name == "Enemy_broccoli")
                dialog_id = 6;
            if (_enemyPrefab.name == "Boss")
                dialog_id = 7;
            _uiController.NotifyOnSpawned(dialog_id);
        }
    }

    private void CheckCompletion()
    {
        _deadEnemies += 1;

        if (_count <= _deadEnemies)
        {
            _deadEnemies = 0;
            _uiController.UnsubscribeSpawnPoint(this);
        }
    }
}
