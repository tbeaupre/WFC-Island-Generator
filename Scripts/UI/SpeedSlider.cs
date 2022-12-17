using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedSlider : MonoBehaviour
{
    [SerializeField]
    Slider slider;

    void Awake()
    {
        slider.onValueChanged.AddListener(v => SettingsManager.Speed = v);
        SettingsManager.Speed = slider.value;
    }
}
