using UnityEngine;
using System.Collections;

public class GameVehicleDrivingSpeed : GameObjectBehavior {
    public WheelCollider myWheelCollider;
    
    //public int textBoxMarginSide;
    //public int textBoxMarginBottom;
    //public int shadowOffset= 2;
    //public int textBoxWidth;
    //public int textBoxHeight;
    //public Texture2D backgroundImage;
    //private GUIStyle style = new GUIStyle();
    //private GUIStyle styleShadow = new GUIStyle();
    //public Font myFont;
    //public int fontSize = 16;
    private float currentSpeed;
    //private int leftRect, topRect, widthRect, heightRect;
	// Use this for initialization
	void Start () {

        //style.font = myFont;
        //style.normal.textColor = Color.cyan;
        //style.fontSize = fontSize;

        //styleShadow.font = myFont;
        //styleShadow.normal.textColor = Color.black;
        //styleShadow.fontSize = fontSize;
	}
	
	// Update is called once per frame
	void Update () {       
        	
	}

    /*
    void OnGUI() {
        leftRect = Screen.width - textBoxWidth - textBoxMarginSide;
        topRect = Screen.height - textBoxHeight - textBoxMarginBottom;      
        widthRect = textBoxWidth;
        heightRect = textBoxHeight;	

        currentSpeed = (Mathf.PI * 2 * myWheelCollider.radius) * myWheelCollider.rpm * 60 / 1000;
        currentSpeed = Mathf.Abs(Mathf.Round(currentSpeed));
        string output = string.Format("{0:000}",currentSpeed);

        Rect rectPosShadowBG = new Rect(Screen.width - 120, Screen.height - 60, 100, 50);
        GUI.DrawTexture(rectPosShadowBG, backgroundImage, ScaleMode.StretchToFill);

        Rect rectPosShadow = new Rect(leftRect + shadowOffset, topRect + shadowOffset, widthRect, heightRect);
        GUILayout.BeginArea(rectPosShadow);    
        
        GUILayout.EndArea();

        Rect rectPos = new Rect(leftRect, topRect, widthRect, heightRect);
        GUILayout.BeginArea(rectPos);
        GUILayout.Label(output, style);
        GUILayout.EndArea();
    }
    */
    
}
