using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField]
    GameObject settingsPanel;

    void Start()
    {
        SetPanelActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            SetPanelActive(!settingsPanel.activeSelf);
        }
    }

    void SetPanelActive(bool shouldBeActive)
    {
        settingsPanel.SetActive(shouldBeActive);
        SettingsManager.SettingsOpen = settingsPanel.activeSelf;

    }
}
