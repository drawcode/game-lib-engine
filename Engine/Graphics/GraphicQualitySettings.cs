using System;
using UnityEngine;

namespace Engine.Graphics {

    public class GraphicQualitySettings {
        /*
        private static volatile GraphicQualitySettings instance;
        private static System.Object syncRoot = new System.Object();

        public static GraphicQualitySettings Instance
        {
            get
            {
             if (instance == null)
             {
                lock (syncRoot)
                {
                   if (instance == null)
                      instance = new GraphicQualitySettings();
                }
             }

             return instance;
            }
            set {
                instance = value;
            }
        }

        public GraphicQualitySettings()
        {
        }

        public QualitySettings currentQualityLevel
        {
            get {
                return currentQualityLevel;
            }
            set {
                ChangeQualitySettings(value);
            }
        }

        public void ChangeQualitySettings(QualitySettings qualityLevel)
        {
            currentQualityLevel = qualityLevel;
            QualitySettings.SetQualityLevel(qualityLevel);
        }

        public void ChangeByPlatform()
        {
#if UNITY_EDITOR
            ChangeQualitySettings(QualityLevel.Fantastic);
#elif UNITY_IPHONE
            if(iPhone.generation == iPhoneGeneration.iPhone
               || iPhone.generation == iPhoneGeneration.iPhone3G
               || iPhone.generation == iPhoneGeneration.iPhone3GS
               || iPhone.generation == iPhoneGeneration.iPodTouch1Gen
               || iPhone.generation == iPhoneGeneration.iPodTouch2Gen
               || iPhone.generation == iPhoneGeneration.iPodTouch3Gen
               ) {
                ChangeQualitySettings(QualityLevel.Fastest);
            }
            else if(iPhone.generation == iPhoneGeneration.iPad1Gen
                    || iPhone.generation == iPhoneGeneration.iPhone4
                    || iPhone.generation == iPhoneGeneration.iPodTouch4Gen
               ) {
                ChangeQualitySettings(QualityLevel.Simple);
            }
            else {
                ChangeQualitySettings(QualityLevel.Simple);
            }

#elif UNITY_ANDROID

            //ChangeQualitySettings(QualityLevel.Fastest);
#elif UNITY_WEB_PLAYER

            //ChangeQualitySettings(QualityLevel.Simple);
#else

            //ChangeQualitySettings(QualityLevel.Fantastic);
#endif
        }
*/
    }
}