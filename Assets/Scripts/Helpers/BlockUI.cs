using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockUI : MonoBehaviour
{
    [SerializeField] private GameObject panelBlockUI = default;
    public static BlockUI Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    public void Block()
    {
        panelBlockUI.SetActive(true);
    }
    public void UnBlock()
    {
        panelBlockUI.SetActive(false);
    }
}
