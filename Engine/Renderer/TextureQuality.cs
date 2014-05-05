using System.Collections;
using UnityEngine;

public class TextureQuality : GameObjectBehavior {

    // Used to set the texture mipmap level based on device
    private void Awake() {
        /*

        // Quality setting for in the editor
        if( Application.isEditor )
        {
            QualitySettings.SetQualityLevel(QualitySettings.);
            LogUtil.Log( "Texture quality in editor set to: " + QualitySettings.GetQualityLevel() );
            return;
        }

        if( iPhone.generation == iPhoneGeneration.iPhone4 || iPhone.generation == iPhoneGeneration.iPhone3GS
            || iPhone.generation == iPhoneGeneration.iPad1Gen )
            QualitySettings.SetQualityLevel(QualityLevel.Beautiful);
        else
            QualitySettings.SetQualityLevel(QualityLevel.Good);

        LogUtil.Log( "Texture quality set to: " + QualitySettings.GetQualityLevel() );
        */

        // disable ourself
        this.enabled = false;
    }
}