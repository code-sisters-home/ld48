using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SimpleUIController : MonoBehaviour
{
    [SerializeField] private Canvas _psy;
    [SerializeField] private Canvas _intro;
    [SerializeField] private GameObject _introSound;
    [SerializeField] private Canvas _menu;
    [SerializeField] private Button _start;
    [SerializeField] private Button[] _levels;
    [SerializeField] private Button _backButton;


    private const float INTROTIMER = 13;

    private bool _inMenu;
    private bool _inIntro;

    private float _timerIntro;

    private void Awake()
    {
        _timerIntro = 0.0f;
        _menu.gameObject.SetActive(true);
        _psy.gameObject.SetActive(false);

        for (int i = 0; i < _levels.Length; i++)
        {
            var index = i + 1;
            _levels[i].onClick.AddListener(() => GoToLevel(index));
        }

        _backButton.onClick.AddListener(GoToMenu);
        _start.onClick.AddListener(GoToIntro);
    }

    private void GoToLevel(int i)
    {
        _inMenu = false;

        _menu.gameObject.SetActive(_inMenu);
        _psy.gameObject.SetActive(!_inMenu);

        SceneLoader.Instance.LoadLevel(i);
    }

    private void GoToIntro()
    {
        _inMenu = false;
        _inIntro = true;

        _menu.gameObject.SetActive(_inMenu);
        _psy.gameObject.SetActive(_inMenu);
        _intro.gameObject.SetActive(_inIntro);

        _introSound.GetComponent<AudioSource>().Play();
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
        if (!_inMenu && !_inIntro && Input.GetKeyDown(KeyCode.Escape))
            GoToMenu();

        if (_inIntro && _timerIntro <= INTROTIMER)
            _timerIntro += Time.deltaTime;
        else if (_inIntro)
        {
            _inMenu = true;
            _inIntro = false;
            _introSound.GetComponent<AudioSource>().Stop();
            _menu.gameObject.SetActive(_inMenu);
            _intro.gameObject.SetActive(!_inMenu);

            _start.gameObject.SetActive(_inIntro);

            for (int i = 0; i < _levels.Length; i++)
            {
                var index = i + 1;
                _levels[i].gameObject.SetActive(_inMenu);
            }

        }
    }
}
