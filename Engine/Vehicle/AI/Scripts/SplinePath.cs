using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SplinePath : GameObjectBehavior {   

    public int steps = 5;
    public bool loop = true;
    public Color color = Color.white;
    [HideInInspector]
    public List <Transform>
        path;
    private Vector3[] pathPositions;
    [HideInInspector]
    public List<Vector3>
        sequence;
    //private bool isLoaded = false;
    
    protected virtual void Awake() {
        
        FillSequence();
    }
    
    protected virtual void OnDrawGizmos() {
    
        FillSequence();
        DrawGizmos();       
                
    }
    
    protected void DrawGizmos() {
        if (sequence != null) {         
                    
            int count;
            int c = 0;
            count = sequence.Count;
            for (c = 0; c < count; c++) {
                if (c < count - 1) {
                    Debug.DrawLine(sequence[c], sequence[c + 1], color);
                }
                else if (loop) {
                    Debug.DrawLine(sequence[count - 1], sequence[0], color);
                }
            }
        }
    }
    
    protected void FillSequence() {
        
        sequence = SplineCalculation.NewCatmullRom(path, steps, loop);
        
    }
            
}
