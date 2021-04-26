using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SimpleUIController : MonoBehaviour, IController
{
    public Action<int> OnSpawned;

    [SerializeField] private Canvas _psy;
    [SerializeField] private Canvas _intro;
    [SerializeField] private GameObject _introSound;
    [SerializeField] private Canvas _menu;
    [SerializeField] private Button _start;
    [SerializeField] private Button[] _levels;
    [SerializeField] private Button _backButton;
    [SerializeField] private GameObject _headerPanel;
    [SerializeField] private GameObject _Bubble;
    [SerializeField] private GameObject _Psycho;
    [SerializeField] private Button _questionButton;
    [SerializeField] private Button _skipIntroButton;
    [SerializeField] private Button _TutorialButton;
    [SerializeField] private GameObject[] _dialogs;
    [SerializeField] private GameObject _tutorial;

    private const float INTROTIMER = 12;

    private bool _inMenu;
    private bool _inIntro;

    private float _timerIntro;

    private List<SpawnPoint> _spawnPoints = new List<SpawnPoint>(); //active on current level

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
        _questionButton.onClick.AddListener(ShowOrHideTutorial);

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
        _questionButton.gameObject.SetActive(!_inMenu);
        _tutorial.SetActive(_inMenu);
        _headerPanel.SetActive(!_inMenu);
        _backButton.gameObject.SetActive(!_inMenu);
        _Bubble.SetActive(!_inMenu);
        _Psycho.SetActive(!_inMenu);

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
        for (int i = 0; i < _dialogs.Length; i++)
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

        _spawnPoints.Clear();

        SceneLoader.Instance.UnloadLevel();
    }

    private void GoToTutorial()
    {
        _inMenu = false;

        _menu.gameObject.SetActive(_inMenu);
        _psy.gameObject.SetActive(!_inMenu);
        _questionButton.gameObject.SetActive(_inMenu);
        _tutorial.SetActive(_inMenu);
        _headerPanel.SetActive(_inMenu);
        _Bubble.SetActive(_inMenu);
        _Psycho.SetActive(_inMenu);

        SceneLoader.Instance.LoadTutorial();
    }

    private void ShowOrHideTutorial()
    {
        _tutorial.SetActive(!_tutorial.activeSelf);
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
            _TutorialButton.gameObject.SetActive(_inMenu);
            _skipIntroButton.gameObject.SetActive(false);
        }
    }

    public void SubscribeSpawnPoint(SpawnPoint spawnPoint)
    {
        _spawnPoints.Add(spawnPoint);
    }

    public void UnsubscribeSpawnPoint(SpawnPoint spawnPoint)
    {
        _spawnPoints.Remove(spawnPoint);

        if (_spawnPoints.Count == 0)
        {
            _spawnPoints.Clear();
            SceneLoader.Instance.Next();
        }
    }

    public void NotifyOnSpawned(int id)
    {
        OnSpawned(id);
    }
}

public interface IController 
{
    void NotifyOnSpawned(int id);
    void SubscribeSpawnPoint(SpawnPoint spawnPoint);
    void UnsubscribeSpawnPoint(SpawnPoint spawnPoint);
}