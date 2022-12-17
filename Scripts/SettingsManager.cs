using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsManager
{
    public static bool SettingsOpen { get; set; } = true;

    private static int radius;
    public static int Radius {
        get { return radius; }
        set {
            radius = value;
            OnRadiusChange?.Invoke();
        }
    }
    public delegate void RadiusChange();
    public static event RadiusChange OnRadiusChange;

    private static int height;
    public static int Height
    {
        get { return height; }
        set {
            height = value;
            OnHeightChange?.Invoke();
        }
    }
    public delegate void HeightChange();
    public static event HeightChange OnHeightChange;
}
