using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class DrawLineToNext : GameObjectBehavior {
    
    
    //[HideInInspector]
    public bool show = false;
    
    //private bool active = true;
    private Color color = Color.cyan;
    private GameVehicleAIDriverController aiDriverController;

    public void OnDrawGizmos() {
        
        //if (active && (!Application.isPlaying || show))
        if (!Application.isPlaying) {
            //color = renderer.material.color;
            color = renderer.sharedMaterial.color;
            
            string fullName = gameObject.name;
            int delimiterPos = fullName.LastIndexOf("_");
            string preFix = fullName.Substring(0, delimiterPos);
            string strNumber = fullName.Substring(delimiterPos + 1);
            int intNumber;
            
            if (int.TryParse(strNumber, out intNumber)) {                   
                
                intNumber += 1;
                string nextWpName = preFix + "_" + intNumber.ToString();
                                
                Transform nextWP = gameObject.transform.parent.FindChild(nextWpName);
                if (nextWP != null) {
                    //if (show && active)
                    if (show) {
                        nextWP.GetComponent<Renderer>().sharedMaterial.color = renderer.sharedMaterial.color;
                        Debug.DrawLine(transform.position, nextWP.position, color);                 
                    }
                    
                    DrawLineToNext drawLineToNext = nextWP.gameObject.GetComponent<DrawLineToNext>() as DrawLineToNext;
                    if (drawLineToNext != null) {
                        //drawLineToNext.active = active;
                        drawLineToNext.show = show;
                    }
                    //DrawLineToNext drawLineToNext = nextWP.GetComponent<DrawLineToNext>() as DrawLineToNext;
                    //drawLineToNext.active = active;
                    //drawLineToNext.show = show;
                                        
                }
                
                //Catmull Rom - Kubisch Hermitescher Spline - cSpline
                
            }                   
                        
        }
                
    }
    
}
