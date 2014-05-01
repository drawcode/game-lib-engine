using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineCalculation {
//public class SplineCalculation : GameObjectBehavior{
        
    public static List<Vector3> NewCatmullRom(List<Transform> nodes, int slices, bool loop) {
            
        List <Vector3> result = new List <Vector3>();
        if (nodes.Count >= 2) {

            result.Add(GetPosition(nodes[0]));
            int last = nodes.Count - 1;
            
            for (int current = 0; (!loop && current < last)||(loop && current <= last); current++) {

                int previous = (current == 0) ? ((loop) ? last : current) : current - 1;
                int start = current;
                int end = (current == last) ? ((loop) ? 0 : current) : current + 1;
                int next = (end == last) ? ((loop) ? 0 : end) : end + 1;
                int stepCount = slices + 1;
                
                for (int step = 1; step <= stepCount; step++) {
                    
                    result.Add(CatmullRom(GetPosition(nodes[previous]),
                                     GetPosition(nodes[start]),
                                     GetPosition(nodes[end]),
                                     GetPosition(nodes[next]),
                                     step, stepCount)); 
                    
                }
                
            }
            
        }
        return result;
    }
    
    static Vector3 CatmullRom(Vector3 previous, Vector3 start, Vector3 end, Vector3 next,
                                float elapsedTime, float duration) {
        // References used:
        // p.266 GemsV1
        //
        // tension is often set to 0.5 but you can use any reasonable value:
        // http://www.cs.cmu.edu/~462/projects/assn2/assn2/catmullRom.pdf
        //
        // bias and tension controls:
        // http://local.wasp.uwa.edu.au/~pbourke/miscellaneous/interpolation/

        float percentComplete = elapsedTime / duration;
        float percentCompleteSquared = percentComplete * percentComplete;
        float percentCompleteCubed = percentCompleteSquared * percentComplete;

        return previous * (-0.5f * percentCompleteCubed +
            percentCompleteSquared -
            0.5f * percentComplete) +
            start * (1.5f * percentCompleteCubed +
            -2.5f * percentCompleteSquared + 1.0f) +
            end * (-1.5f * percentCompleteCubed +
            2.0f * percentCompleteSquared +
            0.5f * percentComplete) +
            next * (0.5f * percentCompleteCubed -
            0.5f * percentCompleteSquared);
    }

    static Vector3 GetPosition(Transform t) {
        return t.position;
    }
    
}
