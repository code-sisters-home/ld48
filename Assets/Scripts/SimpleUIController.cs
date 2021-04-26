using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SimpleUIController : MonoBehaviour
{
    public Action<int> OnSpawned;

    [SerializeField] private Canvas _psy;
    [SerializeField] private Canvas _intro;
    [SerializeField] private GameObject _introSound;
    [SerializeField] private Canvas _menu;
    [SerializeField] private Button _start;
    [SerializeField] private Button[] _levels;
    [SerializeField] private Button _backButton;
    [SerializeField] private Button _skipIntroButton;
    [SerializeField] private Button _TutorialButton;
    [SerializeField] private GameObject[] _dialogs;

    private const float INTROTIMER = 12;

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

        _TutorialButton.onClick.AddListener(GoToTutorial);

        _skipIntroButton.onClick.AddListener(()=> { _inIntro = true; _timerIntro = INTROTIMER; });

        SceneLoader.Instance.OnAllLevelsUnloaded += GoToMenu;

        OnSpawned += (index) => {
            ShowDialog(index);
        };
    }

    private void GoToLevel(int i)
    {
        _inMenu = false;
        _introSound.GetComponent<AudioSource>().Stop();

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
    private void ShowDialog(int index)
    {
        //hide other dialogs
        for (int i = 0; i < _levels.Length; i++)
        {
            _dialogs[i].SetActive(false);
        }
        //show dialog by index
        _dialogs[index].SetActive(true);
    }
    
    private void GoToMenu()
    {
        _inMenu = true;

        _menu.gameObject.SetActive(_inMenu);
        _psy.gameObject.SetActive(!_inMenu);

        SceneLoader.Instance.UnloadLevel();
    }

    private void GoToTutorial()
    {
        _inMenu = false;

        _menu.gameObject.SetActive(_inMenu);
        _psy.gameObject.SetActive(!_inMenu);

        SceneLoader.Instance.LoadTutorial();
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
            
            _menu.gameObject.SetActive(_inMenu);
            _intro.gameObject.SetActive(!_inMenu);

            _start.gameObject.SetActive(_inIntro);

            for (int i = 0; i < _levels.Length; i++)
            {
                _levels[i].gameObject.SetActive(_inMenu);
            }
            _skipIntroButton.gameObject.SetActive(false);
        }
    }
}
