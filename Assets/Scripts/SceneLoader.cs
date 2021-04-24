using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private static SceneLoader _sceneLoader;
    public static SceneLoader Instance => _sceneLoader;

    private int _currentLevel = -1; //it's level's number in name, not a build index!

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

    public void LoadLevel(int index)
    {
        if(_currentLevel == index)
        {
            return;
        }

        StartCoroutine(LoadScene($"Level0{index}"));

        _currentLevel = index;
    }

    public void UnloadLevel()
    {
        if(_currentLevel < 0)
        {
            return;
        }

        StartCoroutine(UnloadScene($"Level0{_currentLevel}"));

        _currentLevel = -1;
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

        if(sceneName.Contains("Level"))
        {
            Camera.main.GetComponent<CameraFollow>().Target = GameObject.FindGameObjectWithTag("CameraFollowTarget").transform;
        }
    }

    private IEnumerator UnloadScene(string sceneName)
    {
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("UI"));

        AsyncOperation operation = SceneManager.UnloadSceneAsync($"Scenes/{sceneName}");

        if (operation == null)
            yield break;

        while(!operation.isDone)
        {
            yield return null;
        }

        Resources.UnloadUnusedAssets();
    }
}
