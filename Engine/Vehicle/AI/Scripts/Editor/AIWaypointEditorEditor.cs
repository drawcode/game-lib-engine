using UnityEngine;
using System.Collections;
using System;
using UnityEditor;

[CustomEditor(typeof(GameVehicleAIWaypointEditor))]
public class GameVehicleAIWaypointEditorEditor : Editor
{
    private static bool m_editMode = false;
    private static string m_preName = "wp";
    private static string m_folderName = "wps";
    private static int m_speed = 100;
    private GameObject m_container;
    public GameObject waypointFolder;
	public bool m_batchCreating = false;
    private bool m_lastFrameBatchCreating = false;
	
    //[MenuItem("GameObject/AI Driver Toolkit/AI Driver")]
    //static void CreateAIDPrototype()
    //{
    //    GameObject prefab = Resources.LoadAssetAtPath("Assets/GameVehicleAIDriverToolkit/Prefabs/GameVehicleAIDriverPrototype.prefab", typeof(GameObject)) as GameObject;                    
    //    GameObject newObject = Instantiate(prefab,Vector3.zero,Quaternion.identity) as GameObject;
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

    //[MenuItem("GameObject/AI Driver Toolkit/Components/AI Controller")]
    //static void AddAIController()
    //{
    //    //GameObject prefab = Resources.LoadAssetAtPath("Assets/GameVehicleAIDriverToolkit/Prefabs/GameVehicleAIDriverPrototype.prefab", typeof(GameObject)) as GameObject;
    //    //GameObject newObject = Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
    //    //newObject.name = "AI Driver";

    //    //// positioned new object
    //    //Ray ray = SceneView.lastActiveSceneView.camera.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
    //    //RaycastHit hit;
    //    //if (Physics.Raycast(ray, out hit))
    //    //{
    //    //    newObject.transform.position = hit.point;
    //    //}

    //    ////select new object
    //    //UnityEngine.Object[] selectedObjects = new UnityEngine.Object[1];
    //    //selectedObjects[0] = newObject;
    //    //Selection.objects = selectedObjects;
    //}

    void OnSceneGUI()
    {
        
        if (m_editMode)
        {
            if (Event.current.type == EventType.MouseDown)
            {                           
				if (Event.current.button == 0) //2013-08-02
				{								//2013-08-02
	                Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
	                RaycastHit hit;               
	
	                //2011-04-11 cse -B
	                if (m_container == null)
	                {
	                    LogUtil.LogError("No container found. Place waypoints in scenes directly after pressing the Waypoint Editor button.");
	                    m_editMode = false;
	                    Repaint();
	                }
	                //2011-04-11 cse -E
	
	                if (m_editMode) //2011-04-11 cse
	                {               //2011-04-11 cse
	                    
	                    if (Physics.Raycast(ray, out hit))
	                    {
	                        int counter = 1;
	                        string fullPreName;
	                        fullPreName = "/" + m_folderName + "/" + m_preName + "_";
	                        while (GameObject.Find(fullPreName + counter.ToString()) != null)
	                        {
	                            counter++;
	                        }
	
	                        //Undo.RegisterUndo("Create new Waypoint");
	                        GameObject prefab = AssetDatabase.LoadAssetAtPath("Assets/GameVehicleAIDriverToolkit/Prefabs/Waypoint.prefab", typeof(GameObject)) as GameObject;
	                        GameObject waypoint = Instantiate(prefab) as GameObject;
	                        Vector3 myPosition;
	                        myPosition = hit.point;
	                        myPosition.y = (float)myPosition.y + (float)(waypoint.transform.localScale.y / 2);
	
	                        waypoint.transform.position = myPosition;
	                        waypoint.name = m_preName + "_" + counter.ToString();
	                        waypoint.transform.parent = m_container.transform;
	                        GameVehicleAIWaypoint aiwaypointScript = waypoint.GetComponent("GameVehicleAIWaypoint") as GameVehicleAIWaypoint;
	                        aiwaypointScript.speed = m_speed;
	                        EditorUtility.SetDirty(waypoint);
	
	                        //rotate last WP 
	                        GameObject lastWP = GameObject.Find(fullPreName + (counter - 1).ToString());
	                        if (lastWP != null)
	                        {
	                            lastWP.transform.LookAt(waypoint.transform);
	                            EditorUtility.SetDirty(lastWP);
	                        }
	                    }
						
	                    m_editMode = false;
						
	                }//2011-04-11 cse 
				}	//2013-08-02
            }
        }
    }

    public override void OnInspectorGUI()
    {
        
        GameVehicleAIWaypointEditor script = (GameVehicleAIWaypointEditor)target;

        script.folderName = EditorGUILayout.TextField("WP Parent", script.folderName);
        script.preName = EditorGUILayout.TextField("WP Prefix", script.preName);
        script.speed = EditorGUILayout.IntField("Speed", script.speed);
		script.batchCreating = EditorGUILayout.Toggle("Batch Creating", script.batchCreating);		

        m_preName = script.preName;
        m_folderName = script.folderName;
        m_speed = script.speed;        
		m_batchCreating = script.batchCreating;		
				
		if (m_lastFrameBatchCreating ==true && m_batchCreating==false)
		{			
			m_editMode = false;
		}
		
        if (m_editMode)
        {
			//2013-08-02 -B
			Tools.current = Tool.View;
			/*
            if (GUILayout.Button("Right Click in Scene View"))
            {
                                
            }
            */
			if (GUILayout.Button("Left Click in Scene View"))
            {
                                
            }
			//2013-08-02 -E
        }
        else
        {
            if (GUILayout.Button("Press for new Waypoint") || m_batchCreating)
            {
                m_editMode = true;             
                                      
                m_container = GameObject.Find(m_folderName);                
                if (m_container == null)
                {
                    waypointFolder = new GameObject();
                    waypointFolder.name = m_folderName;
                    m_container = waypointFolder;                    
                }   				
                
            }
			
        }
		
		m_lastFrameBatchCreating = m_batchCreating;
		
    }


}
