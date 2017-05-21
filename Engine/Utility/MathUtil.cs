using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public class MathUtil {

    // http://docs.unity3d.com/Documentation/Manual/RandomNumbers.html

    public static Vector3 RandomVector3() {
        return new Vector3(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
    }

    // This gives a point inside a cube with sides one unit long. 
    // The cube can be scaled simply by multiplying the X, Y and Z 
    // components of the vector by the desired side lengths. If one of 
    // the axes is set to zero, the point will always lie within a single 
    // plane. For example, picking a random point on the "ground" is usually 
    // a matter of setting the X and Z components randomly and setting the Y 
    // component to zero.

    // When the volume is a sphere (ie, when you want a random point
    // within a given radius from a point of origin), you can use 
    // Random.insideUnitSphere multiplied by the desired radius:-

    public static Vector3 RandomVector3InSphere(float radius) {
        return UnityEngine.Random.insideUnitSphere * radius;
    }

    // Note that if you set one of the resulting vector's components to zero, 
    // you will *not* get a correct random point within a circle. 
    // Although the point is indeed random and lies within the right radius, 
    // the probability is heavily biased toward the edge of the circle and so 
    // points will be spread very unevenly. You should use Random.insideUnitCircle
    // for this task instead:-


    public static Vector3 RandomVector3InCircle(float radius) {
        return UnityEngine.Random.insideUnitSphere * radius;
    }

    // RANGES
    
    public static bool IsVector3OutOfRange(Vector3 pos, Vector3 min, Vector3 max, Vector3 bounds) {
        return !IsVector3InRange(pos, min, max, bounds);
    }

    public static bool IsVector3InRange(Vector3 pos, Vector3 min, Vector3 max, Vector3 bounds) {
        if (pos.x > max.x + bounds.x) {
            return false;
        }
        else if (pos.y > max.y + bounds.y) {
            return false;
        }
        else if (pos.z > max.z + bounds.z) {
            return false;
        }
        else if (pos.x < min.x + bounds.x) {
            return false;
        }
        else if (pos.y < min.y + bounds.y) {
            return false;
        }
        else if (pos.z < min.z + bounds.z) {
            return false;
        }

        return true;
    }

    //

    public static Vector3 LerpPercent(Vector3 start, Vector3 end, float percent) {

        return (start + percent * (end - start));
    }

    // Add items that add up to 1 and choose probabilistically

    public static T ChooseProbability<T>(List<T> items, List<float> probs) {
        int index = ChooseProbability(probs);
        if (items != null) {
            if (items.Count > index) {
                return items[index];
            }
            if (items.Count > 1)
                return items[0];
        }

        return default(T);
    }

    public static int ChooseProbability(List<float> probs) {
        float total = 0;

        foreach (float elem in probs) {
            total += elem;
        }

        float randomPoint = UnityEngine.Random.value * total;

        for (int i = 0; i < probs.Count; i++) {
            if (randomPoint < probs[i])
                return i;
            else
                randomPoint -= probs[i];
        }

        return probs.Count - 1;
    }

    public static List<T> Shuffle<T>(List<T> deck) {
        for (int i = 0; i < deck.Count; i++) {
            var temp = deck[i];
            var randomIndex = UnityEngine.Random.Range(0, deck.Count);
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }
        return deck;
    }

    public static T ChooseSet<T>(List<T> points, int numRequired) {
        List<T> result = new List<T>(numRequired);

        int numToChoose = numRequired;

        for (int numLeft = points.Count; numLeft > 0; numLeft--) {
            // Adding 0.0 is simply to cast the integers to float for the division.
            float prob = numToChoose + 0.0f / numLeft + 0.0f;

            if (UnityEngine.Random.value <= prob) {
                numToChoose--;
                result[numToChoose] = points[numLeft - 1];

                if (numToChoose == 0)
                    break;
            }
        }

        return default(T);
    }

    public static float Hermite(float start, float end, float value) {
        return Mathf.Lerp(start, end, value * value * (3.0f - 2.0f * value));
    }

    public static float Sinerp(float start, float end, float value) {
        return Mathf.Lerp(start, end, Mathf.Sin(value * Mathf.PI * 0.5f));
    }

    public static float Coserp(float start, float end, float value) {
        return Mathf.Lerp(start, end, 1.0f - Mathf.Cos(value * Mathf.PI * 0.5f));
    }

    public static float Berp(float start, float end, float value) {
        value = Mathf.Clamp01(value);
        value = (Mathf.Sin(value * Mathf.PI * (0.2f + 2.5f * value * value * value)) * Mathf.Pow(1f - value, 2.2f) + value) * (1f + (1.2f * (1f - value)));
        return start + (end - start) * value;
    }

    public static float SmoothStep(float x, float min, float max) {
        x = Mathf.Clamp(x, min, max);
        float v1 = (x - min) / (max - min);
        float v2 = (x - min) / (max - min);
        return -2 * v1 * v1 * v1 + 3 * v2 * v2;
    }

    public static float Lerp(float start, float end, float value) {
        return ((1.0f - value) * start) + (value * end);
    }

    public static Vector3 NearestPoint(Vector3 lineStart, Vector3 lineEnd, Vector3 point) {
        Vector3 lineDirection = Vector3.Normalize(lineEnd - lineStart);
        float closestPoint = Vector3.Dot((point - lineStart), lineDirection) / Vector3.Dot(lineDirection, lineDirection);
        return lineStart + (closestPoint * lineDirection);
    }

    public static Vector3 NearestPointStrict(Vector3 lineStart, Vector3 lineEnd, Vector3 point) {
        Vector3 fullDirection = lineEnd - lineStart;
        Vector3 lineDirection = Vector3.Normalize(fullDirection);
        float closestPoint = Vector3.Dot((point - lineStart), lineDirection) / Vector3.Dot(lineDirection, lineDirection);
        return lineStart + (Mathf.Clamp(closestPoint, 0.0f, Vector3.Magnitude(fullDirection)) * lineDirection);
    }

    public static float Bounce(float x) {
        return Mathf.Abs(Mathf.Sin(6.28f * (x + 1f) * (x + 1f)) * (1f - x));
    }

    // test for value that is near specified float (due to floating point inprecision)
    // all thanks to Opless for this!
    public static bool Approx(float val, float about, float range) {
        return ((Mathf.Abs(val - about) < range));
    }

    // test if a Vector3 is close to another Vector3 (due to floating point inprecision)
    // compares the square of the distance to the square of the range as this
    // avoids calculating a square root which is much slower than squaring the range
    public static bool Approx(Vector3 val, Vector3 about, float range) {
        return ((val - about).sqrMagnitude < range * range);
    }

    /*
      * CLerp - Circular Lerp - is like lerp but handles the wraparound from 0 to 360.
      * This is useful when interpolating eulerAngles and the object
      * crosses the 0/360 boundary.  The standard Lerp function causes the object
      * to rotate in the wrong direction and looks stupid. Clerp fixes that.
      */

    public static float Clerp(float start, float end, float value) {
        float min = 0.0f;
        float max = 360.0f;
        float half = Mathf.Abs((max - min) / 2.0f);//half the distance between min and max
        float retval = 0.0f;
        float diff = 0.0f;

        if ((end - start) < -half) {
            diff = ((max - start) + end) * value;
            retval = start + diff;
        }
        else if ((end - start) > half) {
            diff = -((max - end) + start) * value;
            retval = start + diff;
        }
        else
            retval = start + (end - start) * value;

        // LogUtil.Log("Start: "  + start + "   End: " + end + "  Value: " + value + "  Half: " + half + "  Diff: " + diff + "  Retval: " + retval);
        return retval;
    }

    public static int Modulo(int x, int m) {
        m = Mathf.Abs(m);
        return (x % m + m) % m;
    }

    public static Vector2 Circle(float angle, float radius) {
        return new Vector2(radius * Mathf.Cos(angle),
                           radius * -Mathf.Sin(angle));
    }

    public static float Hypotenuse(float a, float b) {
        return MathUtil.SquareRoot(HypotenuseSquared(a, b));
    }

    public static float HypotenuseSquared(float a, float b) {
        return MathUtil.Square(a) + MathUtil.Square(b);
    }

    public static float SquareRoot(float val) {
        return Mathf.Sqrt(val);
    }

    public static float Square(float val) {
        return val * val;
    }

    public static float SquareSign(float val) {
        return Square(val) * Mathf.Sign(val);
    }

    public static float Cube(float val) {
        return val * val * val;
    }

    public static int Clamp(int val, int min, int max) {
        return Math.Max(Math.Min(val, max), min);
    }

    public static float MirrorUnitClamp(float val) {
        return Mathf.Clamp(val, -1, 1);
    }

    public static float UnitClamp(float val) {
        return Mathf.Clamp(val, 0f, 1f);
    }

    public static float MPSToMPH(float metersPerSecond) {
        return metersPerSecond * 2.23693629f;
    }

    public static float MPHToMPS(float milesPerHour) {
        return milesPerHour * 0.44704f;
    }

    public static float KMHToMPH(float kilometersPerHour) {
        return kilometersPerHour / 1.609f;
    }

    public static float RPMToAV(float rpm) {
        return rpm * ((2f * Mathf.PI) / 60f);
    }

    public static float AVToRPM(float angularVelocity) {
        return angularVelocity * (60f / (2f * Mathf.PI));
    }

    public static float NormalizeAngleHalf(float angle) {
        angle = NormalizeAngle(angle);
        if (angle > 180f)
            return -(360f - angle);
        return angle;
    }

    public static float NormalizeAngle(float angle) {
        return ((angle % 360f) + 360f) % 360f;
    }

    public static float ClampAngle(float a, float min, float max) {
        while (max < min)
            max += 360f;
        while (a > max)
            a -= 360f;
        while (a < min)
            a += 360f;

        if (a > max) {
            if (a - (max + min) * 0.5f < 180f)
                return max;
            else
                return min;
        }
        else
            return a;
    }

    public static Vector3 CardinalAngles(Vector3 pos1, Vector3 pos2) {

        // Adjust both positions to be relative to our origin point (pos1)
        pos2 -= pos1;
        pos1 -= pos1;

        Vector3 angles = Vector3.zero;

        // Rotation to get from World +Z to pos2, rotated around World X (degrees up from Z axis)
        angles.x = Vector3.Angle(Vector3.forward, pos2 - Vector3.right * pos2.x);

        // Rotation to get from World +Z to pos2, rotated around World Y (degrees right? from Z axis)
        angles.y = Vector3.Angle(Vector3.forward, pos2 - Vector3.up * pos2.y);

        // Rotation to get from World +X to pos2, rotated around World Z (degrees up from X axis)
        angles.z = Vector3.Angle(Vector3.right, pos2 - Vector3.forward * pos2.z);

        return angles;
    }

    public static float ContAngle(Vector3 fwd, Vector3 targetDir, Vector3 upDir) {
        var angle = Vector3.Angle(fwd, targetDir);

        if (AngleDir(fwd, targetDir, upDir) == -1) {
            return 360 - angle;
        }
        else {
            return angle;
        }
    }

    //returns -1 when to the left, 1 to the right, and 0 for forward/backward
    public static float AngleDir(Vector3 fwd, Vector3 targetDir, Vector3 up) {

        Vector3 perp = Vector3.Cross(fwd, targetDir);

        float dir = Vector3.Dot(perp, up);

        if (dir > 0.0) {
            return 1.0f;
        }
        else if (dir < 0.0) {
            return -1.0f;
        }
        else {
            return 0.0f;
        }
    }
    

    // random range

    public static Vector3 RandomRange(float min, float max) {
        return RandomRange(min, max, min, max, min, max);
    }

    // x

    public static Vector3 RandomRangeX(float val) {
        return RandomRange(val, val, 0, 0, 0, 0);
    }

    public static Vector3 RandomRangeX(float min, float max) {
        return RandomRange(min, max, 0, 0, 0, 0);
    }

    // y

    public static Vector3 RandomRangeY(float val) {
        return RandomRange(0, 0, val, val, 0, 0);
    }

    public static Vector3 RandomRangeY(float min, float max) {
        return RandomRange(0, 0, min, max, 0, 0);
    }


    // z

    public static Vector3 RandomRangeZ(float val) {
        return RandomRange(0, 0, 0, 0, val, val);
    }

    public static Vector3 RandomRangeZ(float min, float max) {
        return RandomRange(0, 0, 0, 0, min, max);
    }

    // all

    public static Vector3 RandomRange(
        float minX = 0, float maxX = 0,
        float minY = 0, float maxY = 0,
        float minZ = 0, float maxZ = 0) {

        float rangeX = UnityEngine.Random.Range(minX, maxX);
        float rangeY = UnityEngine.Random.Range(minY, maxY);
        float rangeZ = UnityEngine.Random.Range(minZ, maxZ);

        return Vector3.zero.WithX(rangeX).WithY(rangeY).WithZ(rangeZ);
    }

    // all

    public static Vector3 RandomRangeConstrain(
        float minX = 0, float maxX = 0) {

        float range = UnityEngine.Random.Range(minX, maxX);

        return Vector3.zero.WithX(range).WithY(range).WithZ(range);
    }


}