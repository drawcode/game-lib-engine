// Version 1.4
// ©2011 Starscene Software. All rights reserved. Redistribution of source code without permission not allowed.

using UnityEngine;

namespace Engine.Graphics.Vector {

    public enum LineType { Continuous, Discrete }

    public enum Joins { Fill, Weld, None }

    public class VectorMaterial {
        public static Material defaultLineMaterial;
    }

    public class VectorLine {
        public Vector2[] points2;
        public Vector3[] points3;

        public float lineWidth {
            get { return lineWidths[0] * 2; }
            set {
                if (lineWidths.Length == 1) {
                    lineWidths[0] = value * .5f;
                }
                else {
                    float thisWidth = value * .5f;
                    for (int i = 0; i < lineWidths.Length; i++) {
                        lineWidths[i] = thisWidth;
                    }
                }
            }
        }

        public float[] lineWidths;
        public float capLength = 0.0f;
        private int m_depth = 0;

        public int depth {
            get { return m_depth; }
            set { m_depth = Mathf.Clamp(value, 0, 100); }
        }

        public GameObject vectorObject;
        public MeshFilter meshFilter;
        public Mesh mesh;
        public Vector3[] lineVertices;
        public Vector2[] lineUVs;
        public Color[] lineColors;
        public bool smoothWidth = false;
        public bool active = true;
        private int m_layer = -1;

        public int layer {
            get { return m_layer; }
            set {
                m_layer = value;
                if (m_layer < 0) m_layer = 0;
                else if (m_layer > 31) m_layer = 31;
                vectorObject.layer = m_layer;
            }
        }

        private bool m_continuousLine;

        public bool continuousLine {
            get { return m_continuousLine; }
        }

        private bool m_fillJoins;
        private bool m_weldJoins;

        public bool weldJoins {
            get { return m_weldJoins; }
        }

        private bool m_isPoints;

        public bool isPoints {
            get { return m_isPoints; }
        }

        private int m_maxDrawIndex = 0;
        private int m_minDrawIndex = 0;

        public int maxDrawIndex {
            get { return m_maxDrawIndex; }
            set {
                m_maxDrawIndex = value;
                if (m_maxDrawIndex < 0) m_maxDrawIndex = 0;
                else {
                    if (points2 == null) {
                        if (m_maxDrawIndex > points3.Length - 1) m_maxDrawIndex = points3.Length - 1;
                    }
                    else {
                        if (m_maxDrawIndex > points2.Length - 1) m_maxDrawIndex = points2.Length - 1;
                    }
                }
            }
        }

        public int minDrawIndex {
            get { return m_minDrawIndex; }
            set {
                m_minDrawIndex = value;
                if (!m_continuousLine && m_minDrawIndex % 2 != 0) {	// No odd numbers for discrete lines
                    m_minDrawIndex++;
                }
                if (m_minDrawIndex < 0) m_minDrawIndex = 0;
                else {
                    if (points2 == null) {
                        if (m_minDrawIndex > points3.Length - 1) m_minDrawIndex = points3.Length - 1;
                    }
                    else {
                        if (m_minDrawIndex > points2.Length - 1) m_minDrawIndex = points2.Length - 1;
                    }
                }
            }
        }

        // Vector3 constructors
        public VectorLine(string lineName, Vector3[] linePoints, Material lineMaterial, float width) {
            points3 = linePoints;
            SetupMesh(ref lineName, lineMaterial, null, ref width, LineType.Discrete, Joins.None, false, false);
        }

        public VectorLine(string lineName, Vector3[] linePoints, Color color, Material lineMaterial, float width) {
            points3 = linePoints;
            Color[] colors = SetColor(color, LineType.Discrete, linePoints.Length, false);
            SetupMesh(ref lineName, lineMaterial, colors, ref width, LineType.Discrete, Joins.None, false, false);
        }

        public VectorLine(string lineName, Vector3[] linePoints, Color[] colors, Material lineMaterial, float width) {
            points3 = linePoints;
            SetupMesh(ref lineName, lineMaterial, colors, ref width, LineType.Discrete, Joins.None, false, false);
        }

        public VectorLine(string lineName, Vector3[] linePoints, Material lineMaterial, float width, LineType lineType) {
            points3 = linePoints;
            SetupMesh(ref lineName, lineMaterial, null, ref width, lineType, Joins.None, false, false);
        }

        public VectorLine(string lineName, Vector3[] linePoints, Color color, Material lineMaterial, float width, LineType lineType) {
            points3 = linePoints;
            Color[] colors = SetColor(color, lineType, linePoints.Length, false);
            SetupMesh(ref lineName, lineMaterial, colors, ref width, lineType, Joins.None, false, false);
        }

        public VectorLine(string lineName, Vector3[] linePoints, Color[] colors, Material lineMaterial, float width, LineType lineType) {
            points3 = linePoints;
            SetupMesh(ref lineName, lineMaterial, colors, ref width, lineType, Joins.None, false, false);
        }

        public VectorLine(string lineName, Vector3[] linePoints, Material lineMaterial, float width, LineType lineType, Joins joins) {
            points3 = linePoints;
            SetupMesh(ref lineName, lineMaterial, null, ref width, lineType, joins, false, false);
        }

        public VectorLine(string lineName, Vector3[] linePoints, Color color, Material lineMaterial, float width, LineType lineType, Joins joins) {
            points3 = linePoints;
            Color[] colors = SetColor(color, lineType, linePoints.Length, false);
            SetupMesh(ref lineName, lineMaterial, colors, ref width, lineType, joins, false, false);
        }

        public VectorLine(string lineName, Vector3[] linePoints, Color[] colors, Material lineMaterial, float width, LineType lineType, Joins joins) {
            points3 = linePoints;
            SetupMesh(ref lineName, lineMaterial, colors, ref width, lineType, joins, false, false);
        }

        // Vector2 constructors
        public VectorLine(string lineName, Vector2[] linePoints, Material lineMaterial, float width) {
            points2 = linePoints;
            SetupMesh(ref lineName, lineMaterial, null, ref width, LineType.Discrete, Joins.None, true, false);
        }

        public VectorLine(string lineName, Vector2[] linePoints, Color color, Material lineMaterial, float width) {
            points2 = linePoints;
            Color[] colors = SetColor(color, LineType.Discrete, linePoints.Length, false);
            SetupMesh(ref lineName, lineMaterial, colors, ref width, LineType.Discrete, Joins.None, true, false);
        }

        public VectorLine(string lineName, Vector2[] linePoints, Color[] colors, Material lineMaterial, float width) {
            points2 = linePoints;
            SetupMesh(ref lineName, lineMaterial, colors, ref width, LineType.Discrete, Joins.None, true, false);
        }

        public VectorLine(string lineName, Vector2[] linePoints, Material lineMaterial, float width, LineType lineType) {
            points2 = linePoints;
            SetupMesh(ref lineName, lineMaterial, null, ref width, lineType, Joins.None, true, false);
        }

        public VectorLine(string lineName, Vector2[] linePoints, Color color, Material lineMaterial, float width, LineType lineType) {
            points2 = linePoints;
            Color[] colors = SetColor(color, lineType, linePoints.Length, false);
            SetupMesh(ref lineName, lineMaterial, colors, ref width, lineType, Joins.None, true, false);
        }

        public VectorLine(string lineName, Vector2[] linePoints, Color[] colors, Material lineMaterial, float width, LineType lineType) {
            points2 = linePoints;
            SetupMesh(ref lineName, lineMaterial, colors, ref width, lineType, Joins.None, true, false);
        }

        public VectorLine(string lineName, Vector2[] linePoints, Material lineMaterial, float width, LineType lineType, Joins joins) {
            points2 = linePoints;
            SetupMesh(ref lineName, lineMaterial, null, ref width, lineType, joins, true, false);
        }

        public VectorLine(string lineName, Vector2[] linePoints, Color color, Material lineMaterial, float width, LineType lineType, Joins joins) {
            points2 = linePoints;
            Color[] colors = SetColor(color, lineType, linePoints.Length, false);
            SetupMesh(ref lineName, lineMaterial, colors, ref width, lineType, joins, true, false);
        }

        public VectorLine(string lineName, Vector2[] linePoints, Color[] colors, Material lineMaterial, float width, LineType lineType, Joins joins) {
            points2 = linePoints;
            SetupMesh(ref lineName, lineMaterial, colors, ref width, lineType, joins, true, false);
        }

        // Points constructors
        protected VectorLine(bool usePoints, string lineName, Vector2[] linePoints, Material lineMaterial, float width) {
            points2 = linePoints;
            SetupMesh(ref lineName, lineMaterial, null, ref width, LineType.Continuous, Joins.None, true, true);
        }

        protected VectorLine(bool usePoints, string lineName, Vector2[] linePoints, Color color, Material lineMaterial, float width) {
            points2 = linePoints;
            Color[] colors = SetColor(color, LineType.Continuous, linePoints.Length, true);
            SetupMesh(ref lineName, lineMaterial, colors, ref width, LineType.Continuous, Joins.None, true, true);
        }

        protected VectorLine(bool usePoints, string lineName, Vector2[] linePoints, Color[] colors, Material lineMaterial, float width) {
            points2 = linePoints;
            SetupMesh(ref lineName, lineMaterial, colors, ref width, LineType.Continuous, Joins.None, true, true);
        }

        protected VectorLine(bool usePoints, string lineName, Vector3[] linePoints, Material lineMaterial, float width) {
            points3 = linePoints;
            SetupMesh(ref lineName, lineMaterial, null, ref width, LineType.Continuous, Joins.None, false, true);
        }

        protected VectorLine(bool usePoints, string lineName, Vector3[] linePoints, Color[] colors, Material lineMaterial, float width) {
            points3 = linePoints;
            SetupMesh(ref lineName, lineMaterial, colors, ref width, LineType.Continuous, Joins.None, false, true);
        }

        protected VectorLine(bool usePoints, string lineName, Vector3[] linePoints, Color color, Material lineMaterial, float width) {
            points3 = linePoints;
            Color[] colors = SetColor(color, LineType.Continuous, linePoints.Length, true);
            SetupMesh(ref lineName, lineMaterial, colors, ref width, LineType.Continuous, Joins.None, false, true);
        }

        private Color[] SetColor(Color color, LineType lineType, int size, bool usePoints) {
            if (!usePoints) {
                size = lineType == LineType.Continuous ? size - 1 : size / 2;
            }
            Color[] colors = new Color[size];
            for (int i = 0; i < size; i++) {
                colors[i] = color;
            }
            return colors;
        }

        protected void SetupMesh(ref string lineName, Material useMaterial, Color[] colors, ref float width, LineType lineType, Joins joins, bool use2Dlines, bool usePoints) {
            m_fillJoins = (joins == Joins.Fill ? true : false);
            m_continuousLine = (lineType == LineType.Continuous ? true : false);
            if (m_fillJoins && !continuousLine) {
                LogUtil.LogError("VectorLine: Must use LineType.Continuous if using Joins.Fill for \"" + lineName + "\"");
                return;
            }
            if ((use2Dlines && points2 == null) || (!use2Dlines && points3 == null)) {
                LogUtil.LogError("VectorLine: the points array is null for \"" + lineName + "\"");
                return;
            }
            int pointsLength = use2Dlines ? points2.Length : points3.Length;
            if (!usePoints && pointsLength < 2) {
                LogUtil.LogError("The points array must contain at least two points");
                return;
            }
            if (!continuousLine && pointsLength % 2 != 0) {
                LogUtil.LogError("VectorLine: Must have an even points array length for \"" + lineName + "\" when using LineType.Discrete");
                return;
            }

            lineWidths = new float[1];
            lineWidths[0] = width * .5f;
            m_isPoints = usePoints;
            bool useSegmentColors = (colors != null) ? true : false;
            m_weldJoins = (joins == Joins.Weld) ? true : false;

            if (!usePoints) {
                if (continuousLine) {
                    if (useSegmentColors && colors.Length != pointsLength - 1) {
                        LogUtil.LogWarning("VectorLine: Length of color array for \"" + lineName + "\" must be length of points array minus one...disabling segment colors");
                        useSegmentColors = false;
                    }
                }
                else {
                    if (useSegmentColors && colors.Length != pointsLength / 2) {
                        LogUtil.LogWarning("VectorLine: Length of color array for \"" + lineName + "\" must be exactly half the length of points array...disabling segment colors");
                        useSegmentColors = false;
                    }
                }
            }
            else {
                if (useSegmentColors && colors.Length != pointsLength) {
                    LogUtil.LogWarning("VectorLine: Length of color array for \"" + lineName + "\" must be the same length as the points array...disabling segment colors");
                    useSegmentColors = false;
                }
            }

            if (useMaterial == null) {
                if (VectorMaterial.defaultLineMaterial == null) {
                    //VectorMaterial.defaultLineMaterial = new Material("Shader \"Vertex Colors/Alpha\" {SubShader {Cull Off ZWrite On Blend SrcAlpha OneMinusSrcAlpha Pass {BindChannels {Bind \"Color\", color Bind \"Vertex\", vertex}}}}");
                }
                useMaterial = VectorMaterial.defaultLineMaterial;
            }

            mesh = new Mesh();
            mesh.name = lineName;
            vectorObject = new GameObject("Vector " + lineName, typeof(MeshRenderer));
            vectorObject.layer = Vector.vectorLayer;
            meshFilter = (MeshFilter)vectorObject.AddComponent(typeof(MeshFilter));
            vectorObject.GetComponent<Renderer>().material = useMaterial;
            meshFilter.mesh = mesh;
            BuildMesh(pointsLength, use2Dlines, useSegmentColors, colors);
        }

        public void Resize(Vector3[] linePoints) {
            if (points2 != null) {
                LogUtil.LogError("Must supply a Vector2 array instead of a Vector3 array for this line");
                return;
            }
            points3 = linePoints;
            RebuildMesh(false, linePoints.Length);
            maxDrawIndex = maxDrawIndex;	// Make sure it's clamped properly
        }

        public void Resize(Vector2[] linePoints) {
            if (points3 != null) {
                LogUtil.LogError("Must supply a Vector3 array instead of a Vector2 array for this line");
                return;
            }
            points2 = linePoints;
            RebuildMesh(true, linePoints.Length);
            maxDrawIndex = maxDrawIndex;	// Make sure it's clamped properly
        }

        public void Resize(int newSize) {
            if (points2 != null) {
                points2 = new Vector2[newSize];
            }
            else {
                points3 = new Vector3[newSize];
            }
            RebuildMesh(points2 != null, newSize);
            maxDrawIndex = maxDrawIndex;	// Make sure it's clamped properly
        }

        private void RebuildMesh(bool use2Dlines, int pointsLength) {
            if (!continuousLine && pointsLength % 2 != 0) {
                LogUtil.LogError("VectorLine.Resize: Must have an even points array length for \"" + vectorObject.name + "\" when using LineType.Discrete");
                return;
            }

            GameObject.Destroy(mesh);
            mesh = new Mesh();
            meshFilter.mesh = mesh;
            Color[] colors = null;
            if (lineColors != null) {
                colors = SetColor(lineColors[0], continuousLine ? LineType.Continuous : LineType.Discrete, pointsLength, isPoints);
            }
            if (lineWidths.Length > 1) {
                float thisWidth = lineWidth;
                lineWidths = new float[pointsLength];
                lineWidth = thisWidth;
            }
            BuildMesh(pointsLength, use2Dlines, colors != null, colors);
        }

        private void BuildMesh(int pointsLength, bool use2Dlines, bool useSegmentColors, Color[] colors) {
            int vertLength = 0;
            int triLength = 0;
            bool addPoint = false;

            if (continuousLine) {
                if (!m_isPoints) {
                    vertLength = (pointsLength - 1) * 4;
                    triLength = (pointsLength - 1) * 6;
                }
                else {
                    vertLength = (pointsLength) * 4;
                    triLength = (pointsLength) * 6;
                }
                if (m_fillJoins) {
                    triLength += (pointsLength - 2) * 6;

                    // Add another join fill if the first point equals the last point (like with a square)
                    if ((use2Dlines && points2[0] == points2[points2.Length - 1]) || (!use2Dlines && points3[0] == points3[points3.Length - 1])) {
                        triLength += 6;
                        addPoint = true;
                    }
                }
            }
            else {
                vertLength = pointsLength * 2;
                triLength = pointsLength / 2 * 6;
            }
            if (vertLength > 65534) {
                LogUtil.LogError("VectorLine: exceeded maximum vertex count of 65534 for \"" + vectorObject.name + "\"...use fewer points");
                return;
            }

            lineVertices = new Vector3[vertLength];
            int[] newTriangles = new int[triLength];
            lineUVs = new Vector2[vertLength];

            int idx = 0;
            int end = 0;
            if (!m_isPoints) {
                end = continuousLine ? pointsLength - 1 : pointsLength / 2;
            }
            else {
                end = pointsLength;
            }

            for (int i = 0; i < end; i++) {
                lineUVs[idx] = new Vector2(0.0f, 1.0f);
                lineUVs[idx + 1] = new Vector2(0.0f, 0.0f);
                lineUVs[idx + 2] = new Vector2(1.0f, 1.0f);
                lineUVs[idx + 3] = new Vector2(1.0f, 0.0f);
                idx += 4;
            }

            if (useSegmentColors) {
                lineColors = new Color[vertLength];
                idx = 0;
                for (int i = 0; i < colors.Length; i++) {
                    lineColors[idx] = colors[i];
                    lineColors[idx + 1] = colors[i];
                    lineColors[idx + 2] = colors[i];
                    lineColors[idx + 3] = colors[i];
                    idx += 4;
                }
            }

            idx = 0;
            if (!m_isPoints) {
                end = continuousLine ? (pointsLength - 1) * 4 : pointsLength * 2;
            }
            else {
                end = pointsLength * 4;
            }
            for (int i = 0; i < end; i += 4) {
                newTriangles[idx] = i;
                newTriangles[idx + 1] = i + 2;
                newTriangles[idx + 2] = i + 1;

                newTriangles[idx + 3] = i + 2;
                newTriangles[idx + 4] = i + 3;
                newTriangles[idx + 5] = i + 1;
                idx += 6;
            }

            if (m_fillJoins) {
                end -= 2;
                int i = 0;
                for (i = 2; i < end; i += 4) {
                    newTriangles[idx] = i;
                    newTriangles[idx + 1] = i + 2;
                    newTriangles[idx + 2] = i + 1;

                    newTriangles[idx + 3] = i + 2;
                    newTriangles[idx + 4] = i + 3;
                    newTriangles[idx + 5] = i + 1;
                    idx += 6;
                }
                if (addPoint) {
                    newTriangles[idx] = i;
                    newTriangles[idx + 1] = 0;
                    newTriangles[idx + 2] = i + 1;

                    newTriangles[idx + 3] = 0;
                    newTriangles[idx + 4] = 1;
                    newTriangles[idx + 5] = i + 1;
                }
            }

            mesh.vertices = lineVertices;
            mesh.uv = lineUVs;
            if (useSegmentColors) {
                mesh.colors = lineColors;
            }
            mesh.triangles = newTriangles;
        }

        public void SetupVertexColors() {
            if (lineColors != null) return;
            lineColors = new Color[lineVertices.Length];
            mesh.colors = lineColors;
        }
    }

    public class VectorPoints : VectorLine {

        public VectorPoints(string name, Vector2[] points, Material material, float width)
            : base(true, name, points, material, width) {
        }

        public VectorPoints(string name, Vector2[] points, Color[] colors, Material material, float width)
            : base(true, name, points, colors, material, width) {
        }

        public VectorPoints(string name, Vector2[] points, Color color, Material material, float width)
            : base(true, name, points, color, material, width) {
        }

        public VectorPoints(string name, Vector3[] points, Material material, float width)
            : base(true, name, points, material, width) {
        }

        public VectorPoints(string name, Vector3[] points, Color[] colors, Material material, float width)
            : base(true, name, points, colors, material, width) {
        }

        public VectorPoints(string name, Vector3[] points, Color color, Material material, float width)
            : base(true, name, points, color, material, width) {
        }
    }
}