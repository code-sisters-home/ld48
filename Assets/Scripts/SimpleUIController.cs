using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SimpleUIController : MonoBehaviour
{
    [SerializeField] private Canvas _psy;
    [SerializeField] private Canvas _menu;
    [SerializeField] private Button _start;
    [SerializeField] private Button[] _levels;
    [SerializeField] private Button _backButton;

    private bool _inMenu;

    private void Awake()
    {
        _menu.gameObject.SetActive(true);
        _psy.gameObject.SetActive(false);

        for (int i = 0; i < _levels.Length; i++)
        {
            var index = i + 1;
            _levels[i].onClick.AddListener(() => GoToLevel(index));
        }

        _backButton.onClick.AddListener(GoToMenu);
    }

    private void GoToLevel(int i)
    {
        _inMenu = false;

        _menu.gameObject.SetActive(_inMenu);
        _psy.gameObject.SetActive(!_inMenu);

        SceneLoader.Instance.LoadLevel(i);
    }

    private void GoToMenu()
    {
        _inMenu = true;

        _menu.gameObject.SetActive(_inMenu);
        _psy.gameObject.SetActive(!_inMenu);

        SceneLoader.Instance.UnloadLevel();
    }

    private void Update()
    {
        if (!_inMenu && Input.GetKeyDown(KeyCode.Escape))
            GoToMenu();
    }
}
