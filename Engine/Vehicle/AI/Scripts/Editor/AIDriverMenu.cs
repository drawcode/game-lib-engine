using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using System.Diagnostics;

public class GameVehicleAIDriverMenu : GameObjectBehavior
{
    
    private GameObject m_container;
    public GameObject waypointFolder;

    //[MenuItem("GameObject/AI Driver Toolkit/AI Driver")]
    //static void CreateAIDPrototype()
    //{
    //    GameObject prefab = Resources.LoadAssetAtPath("Assets/GameVehicleAIDriverToolkit/Prefabs/GameVehicleAIDriverPrototype.prefab", typeof(GameObject)) as GameObject;
    //    GameObject newObject = Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
    //    newObject.name = "AI Driver";

    //    // positioned new object
    //    Ray ray = SceneView.lastActiveSceneView.camera.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
    //    RaycastHit hit;
    //    if (Physics.Raycast(ray, out hit))
    //    {
    //        newObject.transform.position = hit.point;
    //    }

    //    //select new object
    //    UnityEngine.Object[] selectedObjects = new UnityEngine.Object[1];
    //    selectedObjects[0] = newObject;
    //    Selection.objects = selectedObjects;
    //}

    //[MenuItem("GameObject/AI Driver Toolkit/Buggy")]
    //static void CreateAIDBuggy()
    //{
    //    GameObject prefab = Resources.LoadAssetAtPath("Assets/GameVehicleAIDriverToolkit/Prefabs/AIBuggy.prefab", typeof(GameObject)) as GameObject;
    //    GameObject newObject = Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
    //    newObject.name = "Buggy";

    //    // positioned new object
    //    Ray ray = SceneView.lastActiveSceneView.camera.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
    //    RaycastHit hit;
    //    if (Physics.Raycast(ray, out hit))
    //    {
    //        newObject.transform.position = hit.point;
    //    }

    //    //select new object
    //    UnityEngine.Object[] selectedObjects = new UnityEngine.Object[1];
    //    selectedObjects[0] = newObject;
    //    Selection.objects = selectedObjects;
    //}

    [MenuItem("Component/AI/GameObject/AI Driver")]
    static void CreateAIDPrototypeNew()
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath("Assets/GameVehicleAIDriverToolkit/Prefabs/GameVehicleAIDriverPrototypeNew.prefab", typeof(GameObject)) as GameObject;
        GameObject newObject = Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
        newObject.name = "AI Driver";

        // positioned new object
        Ray ray = SceneView.lastActiveSceneView.camera.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            newObject.transform.position = hit.point;
        }

        //select new object
        UnityEngine.Object[] selectedObjects = new UnityEngine.Object[1];
        selectedObjects[0] = newObject;
        Selection.objects = selectedObjects;
    }

    [MenuItem("Component/AI/GameObject/Buggy Example")]
    static void CreateAIDBuggyNew()
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath("Assets/GameVehicleAIDriverToolkit/Prefabs/AIBuggyNew.prefab", typeof(GameObject)) as GameObject;
        GameObject newObject = Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
        newObject.name = "Buggy Example";

        // positioned new object
        Ray ray = SceneView.lastActiveSceneView.camera.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            newObject.transform.position = hit.point;
        }

        //select new object
        UnityEngine.Object[] selectedObjects = new UnityEngine.Object[1];
        selectedObjects[0] = newObject;
        Selection.objects = selectedObjects;
    }
	
	[MenuItem("Component/AI/GameObject/Barrier")]
    static void AddAddBarrier()
    {
       
        GameObject newObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
		newObject.transform.localScale = new Vector3(1, 2, 10);
        newObject.name = "Barrier";
		
		newObject.AddComponent<BarrierBehaviour>();
		
        // positioned new object
        Ray ray = SceneView.lastActiveSceneView.camera.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));		
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {			
            newObject.transform.position = hit.point;				
        }

        //select new object
        UnityEngine.Object[] selectedObjects = new UnityEngine.Object[1];
        selectedObjects[0] = newObject;
        Selection.objects = selectedObjects;
		Vector3 newPos = newObject.transform.position;
		newPos.y = newPos.y + 1;
		newObject.transform.position = newPos;
		
    }
	
	/// <summary>
	/// Adds an oa mode switcher.
	/// </summary>
	[MenuItem("Component/AI/GameObject/OA Mode Switcher")]
    static void AddOaModeSwitcher()
    {
       
        GameObject newObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
		newObject.transform.localScale = new Vector3(10, 1, 1);
        newObject.name = "OA Mode Switcher";
		
		newObject.AddComponent<SwitchOAMode>();
		
        // positioned new object
        Ray ray = SceneView.lastActiveSceneView.camera.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));		
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {			
            newObject.transform.position = hit.point;				
        }

        //select new object
        UnityEngine.Object[] selectedObjects = new UnityEngine.Object[1];
        selectedObjects[0] = newObject;
        Selection.objects = selectedObjects;
		Vector3 newPos = newObject.transform.position;
		newPos.y = newPos.y + 1;
		newObject.transform.position = newPos;
		
    }
	
	/// <summary>
	/// Adds a waypoint set changer.
	/// </summary>
	[MenuItem("Component/AI/GameObject/Waypoint Set Changer")]
    static void AddWaypointSetChanger()
    {
       
        GameObject newObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
		newObject.transform.localScale = new Vector3(10, 1, 1);
        newObject.name = "Waypoint Set Changer";
		
		newObject.AddComponent<GameAIWaypointChangeSet>();
		
        // positioned new object
        Ray ray = SceneView.lastActiveSceneView.camera.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));		
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {			
            newObject.transform.position = hit.point;				
        }

        //select new object
        UnityEngine.Object[] selectedObjects = new UnityEngine.Object[1];
        selectedObjects[0] = newObject;
        Selection.objects = selectedObjects;
		Vector3 newPos = newObject.transform.position;
		newPos.y = newPos.y + 1;
		newObject.transform.position = newPos;
		
    }
	
    [MenuItem("Component/AI/Component/AI Controller")]
    static void AddAIController()
    {
        AddAIControllerItems();
    }
    
    static void AddAIControllerItems()
    {
        Selection.activeGameObject.AddComponent<GameVehicleAIWaypointEditor>();
        Selection.activeGameObject.AddComponent<GameVehicleRespawnController>();
        Selection.activeGameObject.AddComponent<GameVehicleAIDriverController>();
        Selection.activeGameObject.AddComponent<GameVehicleMotorMapping>();
        Selection.activeGameObject.AddComponent<ShowControllerRaycasts>();            
		
		GameVehicleMotorMapping aIMotorMapping;
		aIMotorMapping = Selection.activeGameObject.GetComponent("GameVehicleMotorMapping")as GameVehicleMotorMapping;
		aIMotorMapping.usingGameVehicleAIDriverMotor = false;
		
        GameObject viewPoint = new GameObject();		
        viewPoint.name = "ViewPoint";
        viewPoint.transform.parent = Selection.activeGameObject.transform;
        viewPoint.transform.localPosition = Vector3.zero;
        viewPoint.transform.localScale = new Vector3(1, 1, 1);
        viewPoint.transform.localRotation = Quaternion.identity;
        viewPoint.AddComponent<ViewPointBehaviour>();
		Vector3 newViewPointPos;
		newViewPointPos = viewPoint.transform.position;
		newViewPointPos.y += 1;
		viewPoint.transform.position = newViewPointPos;
		
        GameObject viewPointCollider = GameObject.CreatePrimitive(PrimitiveType.Cube);        
        viewPointCollider.name="ViewPointCollider";		
        viewPointCollider.transform.parent = Selection.activeGameObject.transform;
		viewPointCollider.transform.localPosition = Vector3.zero;
		viewPointCollider.transform.localScale = new Vector3(1, 1, 1);
        viewPointCollider.transform.localRotation = Quaternion.identity;
        viewPointCollider.transform.GetComponent<Renderer>().enabled = false;

        GameVehicleAIDriverController aiDriverController;
        aiDriverController = Selection.activeGameObject.GetComponent("GameVehicleAIDriverController") as GameVehicleAIDriverController;
        aiDriverController.viewPoint = viewPoint.transform;
        
    }	
	
	[MenuItem("Component/AI/Component/Set Startposition")]
    static void AddSetStartPosition()
    {
        Selection.activeGameObject.AddComponent<SetStartPosition>();
    }
	
	//2012-06-23 -B
	/*
	[MenuItem("Custom/AI Driver Toolkit/Component/Switch OA Mode")]
    static void AddSwitchOAMode()
    {
        Selection.activeGameObject.AddComponent<SwitchOAMode>();
    }	
		
	[MenuItem("Custom/AI Driver Toolkit/Component/Spline Path Waypoints")]
    static void AddSplinePathWaypoints()
    {
        Selection.activeGameObject.AddComponent<SplinePathWaypoints>();
    }
    */
	//2012-06-23 -E
	
	[MenuItem("Component/AI/Online Documentation")]
    static void OpenWebDocumentation()
    {
		Process.Start("http://www.seifert-engineering.de/downloads/ai-driver-toolkit-documentation.pdf");        
    }
	
	[MenuItem("Component/AI/Tutorials and more")]	
    static void OpenWebTutorials()
    {		
        Process.Start("http://www.seifert-engineering.de/ai-driver-toolkit/ai-driver-toolkit-tutorials/");
    }
	
}
