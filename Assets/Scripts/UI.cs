using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    [SerializeField] private Button changeScene;
    [SerializeField] private string sceneName;

    private void Awake() => changeScene.onClick.AddListener(ChangeScene);

    private void ChangeScene() => SceneManager.LoadScene(sceneName, LoadSceneMode.Single);

    private void OnDestroy() => changeScene.onClick.RemoveAllListeners();
}