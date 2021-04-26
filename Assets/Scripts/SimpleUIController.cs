using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class SimpleUIController : MonoBehaviour, IController
{
    public Action<int> OnSpawned;

    [SerializeField] private Canvas _psy;
    [SerializeField] private Canvas _intro;
    [SerializeField] private GameObject _introSound;
    [SerializeField] private Canvas _menu;
    [SerializeField] private Canvas _loseWin;
    [SerializeField] private Animator _loseWinAnimator;
    [SerializeField] private Image _loseImg;
    [SerializeField] private Image _winImg;
    [SerializeField] private TextMeshProUGUI _loseWinTxt;
    [SerializeField] private Button _start;
    [SerializeField] private Button[] _levels;
    [SerializeField] private Button _backButton;
    [SerializeField] private GameObject _headerPanel;
   //[SerializeField] private GameObject GameObject _Bubble;;
    [SerializeField] private GameObject _Psycho;
    [SerializeField] private Button _questionButton;
    [SerializeField] private Button _skipIntroButton;
    [SerializeField] private Button _TutorialButton;
    [SerializeField] private GameObject[] _dialogs;
    [SerializeField] private GameObject _tutorial;

    private static SimpleUIController _ui;
    public static SimpleUIController Instance => _ui;

    private const float INTROTIMER = 12;

    private bool _inMenu;
    private bool _inIntro;

    private float _timerIntro;

    public List<SpawnPoint> _spawnPoints = new List<SpawnPoint>(); //active on current level

    private void Awake()
    {
        if (_ui != null)
        {
            Destroy(_ui);
        }
        _ui = this;
        DontDestroyOnLoad(this);

        _timerIntro = 0.0f;
        _menu.gameObject.SetActive(true);
        _psy.gameObject.SetActive(false);
        _loseWin.gameObject.SetActive(false);

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
        _Psycho.SetActive(!_inMenu);

        _spawnPoints.Clear();

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
        _loseWin.gameObject.SetActive(false);

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
        //  _Bubble.SetActive(_inMenu);
        for (int i = 0; i < _dialogs.Length; i++)
        {
            _dialogs[i].SetActive(false);
        }
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
        var tag = spawnPoint.Tag;
        _spawnPoints.Remove(spawnPoint);

        if (_spawnPoints.Count == 0)
        {
            _spawnPoints.Clear();

            if (tag.Contains("Boss"))
            {
                ShowLoseWin(true);
            }
            else
            {
                SceneLoader.Instance.Next();
            }
        }
    }

    public void NotifyOnSpawned(int id)
    {
        OnSpawned(id);
    }

    public void ShowLoseWin(bool isWin)
    {
        GoToMenu();

        StartCoroutine(DoShow(isWin));
    }

    private IEnumerator DoShow(bool isWin)
    {
        _spawnPoints.Clear();

        _loseWinAnimator.enabled = false;
        _loseWin.gameObject.SetActive(true);
        _winImg.enabled = isWin;
        _loseImg.enabled = !isWin;

        if (isWin)
        {
            _loseWinTxt.SetText("Oh darn, I was counting on 5 more paid sessions.I have a mortgage too!");
        }
        else
        {
            _loseWinTxt.SetText("Take it easy. We’re making progress! At least you can get 10% off next session");
        }

        yield return new WaitForSeconds(3f);

        _winImg.enabled = false;
        _loseImg.enabled = false;

        _loseWinAnimator.enabled = true;
        _loseWinAnimator.Play("Intro");

        yield return new WaitForSeconds(4f);

        _loseWinAnimator.enabled = false;
        _loseWin.gameObject.SetActive(false);
    }

    public void ClearSpawnPoints()
    {
        _spawnPoints.Clear();
    }
}

public interface IController 
{
    void NotifyOnSpawned(int id);
    void SubscribeSpawnPoint(SpawnPoint spawnPoint);
    void UnsubscribeSpawnPoint(SpawnPoint spawnPoint);
}