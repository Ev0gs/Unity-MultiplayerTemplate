using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool _gameStarted = false;
    public bool _gameFinished = false;
    public bool _gamePaused = false;
    public bool _gamePausedFinished = false;
    public bool _gameOver = false;

    #region Singleton
    public static GameManager Instance;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    #endregion
}
