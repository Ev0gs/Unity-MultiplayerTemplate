using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class Countdown : NetworkBehaviour
{
    private NetworkVariable<float> _timeLeft = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone);
    private bool _timerOn;
    [SerializeField] private TMP_Text _timerText;

    private void Start()
    {
        _timeLeft.OnValueChanged += UpdateTimerText;
        _timeLeft.Value = 90;
    }

    /// <summary>
    /// Update Countdown UI
    /// </summary>
    /// <param name="previousValue"></param>
    /// <param name="newValue"></param>
    private void UpdateTimerText(float previousValue, float newValue)
    {
        float minutes = Mathf.FloorToInt(_timeLeft.Value / 60);
        float seconds = Mathf.FloorToInt(_timeLeft.Value % 60);

        _timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    /// <summary>
    /// Countdown decreasing
    /// </summary>
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            _timerOn = true;
        }
        if (_timerOn)
        {
            if (_timeLeft.Value > 0)
            {
                _timeLeft.Value -= Time.deltaTime;
            }
            else
            {
                Debug.Log("TimeIsUp!");
                _timeLeft.Value = 0;
                _timerOn = false;
            }
        }
    }
}
