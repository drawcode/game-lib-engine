using System;
using System.Collections;
using System.Collections.Generic;
using Engine.Utility;
using UnityEngine;

public class ColorItem {
    public float r = 0f;
    public float g = 0f;
    public float b = 0f;
    public float a = 0f;

    public ColorItem() {
        
    }
    
    public ColorItem(float r, float g, float b) {
        Set(r, g, b, 255);
    }

    public ColorItem(float r, float g, float b, float a) {
        Set(r, g, b, a);
    }
        
    public void Set(float rTo, float gTo, float bTo, float aTo) {
        r = rTo;
        g = gTo;
        b = bTo;
        a = aTo;
    }

    public void Set(List<double> color) {
        if (color == null) {
            return;
        }

        if (color.Count > 0) 
            r = (float)color[0];
        
        if (color.Count > 1) 
            g = (float)color[1];
        
        if (color.Count > 2) 
            b = (float)color[2];
        
        if (color.Count > 3) 
            a = (float)color[3];
    }

    public Color GetColor(List<double> color) {
        if (color == null) {
            return Color.white;
        } 

        Set(color);

        return GetColor();
    }

    public Color GetColor() {
        return new Color(r / 255.0f, g / 255.0f, b / 255.0f, a / 255.0f);
    }
}

public static class ColorHelper {

    public static Color FromRGB(List<double> color) {
        return new ColorItem().GetColor(color);
    }

    public static Color FromRGB(float r, float g, float b) {
        return new Color(r / 255.0f, g / 255.0f, b / 255.0f);
    }

    public static Color FromRGB(float r, float g, float b, float a) {
        return new Color(r / 255.0f, g / 255.0f, b / 255.0f, a / 1.0f);
    }
    
    // Note that Color32 and Color implictly convert to each other. You may pass a Color object to this method without first casting it.
    public static string ToHex(Color32 color) {
        string hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
        return hex;
    }
    
    public static Color FromHex(string hex) {
        byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        return new Color32(r, g, b, 255);
    }
}