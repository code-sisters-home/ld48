using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField] private Enemy _enemyPrefab;
    [SerializeField] private int _count = 1;
    [SerializeField] private float _timer = 10.0f;

    private SimpleUIController _uiController;

    private void Awake()
    {
        _uiController = GameObject.Find("SimpleUIController").GetComponent<SimpleUIController>();
        _enemyPrefab.gameObject.SetActive(false);
    }
    private void OnEnable()
    {
        for (int i = 0; i < _count; i++)
        {
            //var random = new System.Random();
            StartCoroutine(Spawn(UnityEngine.Random.Range(0f, _timer)));
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

        yield return new WaitForSeconds(1);

        enemy.enabled = true;

        if (_uiController != null)
        {
            int dialog_id = 0;
            if (_enemyPrefab.name == "Enemy_house")
                dialog_id = 1;
            if (_enemyPrefab.name == "Enemy_clock")
                dialog_id = 2;
            _uiController.OnSpawned(dialog_id);
        }
    }
}
