using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SkinSelector : MonoBehaviour
{
    [SerializeField] private List<Sprite> _skins;
    [SerializeField] private SpriteRenderer _selectedSkinSR;
    [SerializeField] private GameObject _playerPrefab;
    private int _index = 0;

    private void Awake()
    {
        _selectedSkinSR.sprite = _skins[0];
    }

    public void OnNext()
    {
        _index += 1;
        if (_index >= _skins.Count)
            _index = 0;
        _selectedSkinSR.sprite = _skins[_index];
    }

    public void OnPrevious()
    {
        _index -= 1;
        if (_index < 0)
            _index = _skins.Count - 1;
        _selectedSkinSR.sprite = _skins[_index];
    }

    public void OnSelected()
    {
        // Send sprite to GameManager variable
        _playerPrefab.GetComponent<SpriteRenderer>().sprite = _selectedSkinSR.sprite;
        SceneManager.LoadScene(1); //Toujours Utiliser quelque chose de parlant, comme une enum si tu ne veux pas faire un LoadScene(string) mais LoadScene(int), un exemple en dessous
        // Exemple => SceneManager.LoadScene(((int) ScenesNames.SC_Lobby));
    }

    public enum ScenesNames
    {
        SC_Lobby = 1,
    }
}
