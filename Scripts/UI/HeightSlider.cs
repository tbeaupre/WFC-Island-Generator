using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeightSlider : MonoBehaviour
{
    [SerializeField]
    Slider slider;

    void Awake()
    {
        slider.onValueChanged.AddListener(v => SettingsManager.Height = (int)v);
        SettingsManager.Height = (int)slider.value;
    }
}
