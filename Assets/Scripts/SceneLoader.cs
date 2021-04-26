using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public Action OnAllLevelsUnloaded = () => { };

    public Action OnSpawned = () => { };

    private static SceneLoader _sceneLoader;
    public static SceneLoader Instance => _sceneLoader;

    private int _currentLevelPrefix = -1; //it's level's number in name, not a build index! And not a array index;

    public readonly int LARGEST_PREFIX = 4;

    private void Awake()
    {
        if(_sceneLoader != null)
        {
            Destroy(_sceneLoader);
        }
        _sceneLoader = this;
        DontDestroyOnLoad(this);

        LoadUI();
    }

    private void LoadUI()
    {
        StartCoroutine(LoadScene("UI"));
    }

    private void LoadBoss()
    {
        StartCoroutine(LoadScene("Boss"));
    }

    public void LoadLevel(int prefix)
    {
        if(_currentLevelPrefix == prefix)
        {
            return;
        }

        StartCoroutine(LoadScene($"Level0{prefix}"));

        _currentLevelPrefix = prefix;
    }

    public void LoadTutorial()
    {
        StartCoroutine(LoadScene("Tutorial"));
    }

    public void UnloadLevel()
    {
        if(SceneManager.GetActiveScene().name.Contains("Boss"))
        {
            StartCoroutine(UnloadScene($"Boss"));
        }
        else if (SceneManager.GetActiveScene().name.Contains("Tutorial"))
        {
            StartCoroutine(UnloadScene($"Tutorial"));
        }
        else
        {
            if(_currentLevelPrefix < 0)
            {
                return;
            }

            StartCoroutine(UnloadScene($"Level0{_currentLevelPrefix}"));

            _currentLevelPrefix = -1;
        }
    }

    private IEnumerator LoadScene(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync($"Scenes/{sceneName}", LoadSceneMode.Additive);

        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            if (operation.progress >= 0.9f)
            {
                operation.allowSceneActivation = true;
            }

            yield return null;
        }

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));

        if(sceneName.Contains("Level") || sceneName.Contains("Boss"))
        {
            Camera.main.GetComponent<CameraFollow>().Target = GameObject.FindGameObjectWithTag("CameraFollowTarget").transform;
            GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController2D>().OnPlayerDied += ReloadScene;
        }
        if (sceneName.Contains("Tutorial"))
        {
            Camera.main.GetComponent<CameraFollow>().Target = GameObject.FindGameObjectWithTag("CameraFollowTarget").transform;
        }
    }

    private IEnumerator UnloadScene(string sceneName, string loadAfter = "")
    {
        yield return new WaitForEndOfFrame();

        SceneManager.SetActiveScene(SceneManager.GetSceneByName("UI"));

        AsyncOperation operation = SceneManager.UnloadSceneAsync($"Scenes/{sceneName}");

        if (operation == null)
            yield break;

        while(!operation.isDone)
        {
            yield return null;
        }

        Resources.UnloadUnusedAssets();

        yield return null;

        if (!string.IsNullOrEmpty(loadAfter))
            StartCoroutine(LoadScene(loadAfter));
    }

    private void ReloadScene()
    {
        var sceneName = SceneManager.GetActiveScene().name;
        StartCoroutine(UnloadScene(sceneName, sceneName));
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            TryToLoadLevel(_currentLevelPrefix + 1);
        }

        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            TryToLoadLevel(_currentLevelPrefix - 1);
        }
    }

    public void Next()
    {
        TryToLoadLevel(_currentLevelPrefix + 1);
    }

    private void TryToLoadLevel(int prefix)
    {
        if (prefix <= 0 || prefix >= LARGEST_PREFIX)
        {
            if(prefix == LARGEST_PREFIX)
            {
                UnloadLevel();
                LoadBoss();
            }
            else
            { 
                OnAllLevelsUnloaded();
            }
        }
        else
        {
            UnloadLevel();
            LoadLevel(prefix);
        }
    }
}
