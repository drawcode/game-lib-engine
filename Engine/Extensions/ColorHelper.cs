using System;
using System.Collections.Generic;
using Engine.Utility;
using UnityEngine;

public static class ColorHelper {

    public static Color FromRGB(float r, float g, float b) {
        return new Color(r/255.0f, g/255.0f, b/255.0f);
    }

    public static Color FromRGB(float r, float g, float b, float a) {
        return new Color(r/255.0f, g/255.0f, b/255.0f, a/1.0f);
    }
	
}