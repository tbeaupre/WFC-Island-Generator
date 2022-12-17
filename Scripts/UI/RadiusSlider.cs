using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadiusSlider : MonoBehaviour
{
    [SerializeField]
    Slider slider;

    void Awake()
    {
        slider.onValueChanged.AddListener(v => SettingsManager.Radius = (int)v);
        SettingsManager.Radius = (int)slider.value;
    }
}
