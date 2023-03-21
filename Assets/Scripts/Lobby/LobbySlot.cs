using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbySlot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerName = default;
    [SerializeField] private TextMeshProUGUI waitMessageText = default;
    [SerializeField] private List<GameObject> charactersPreview = default;
    private string waitMessage = "Waiting for player";

    public void Start()
    {
        waitMessageText.gameObject.SetActive(false);
        this.playerName.gameObject.SetActive(false);
    }

    public void SetEmpty()
    {
        waitMessageText.text = waitMessage;
        waitMessageText.gameObject.SetActive(true);
        this.playerName.gameObject.SetActive(false);
        charactersPreview.ForEach(go => go.SetActive(false));
    }

    public void ShowPlayer(string playerName, int playerOrder)
    {
        waitMessageText.gameObject.SetActive(false);
        this.playerName.gameObject.SetActive(true);
        this.playerName.text = playerName;
        this.charactersPreview[playerOrder].SetActive(true);
    }
}
