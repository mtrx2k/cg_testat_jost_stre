
namespace GravityDemo
{
    using UnityEngine;

    [ExecuteInEditMode]
    public sealed class GravitationalField : GravitationalObject
    {
        private const int DefaultDimensions = 8;



        [SerializeField, HideInInspector]
        private GravitationalBodyManager bodies;

        [SerializeField, HideInInspector] private int width  = DefaultDimensions;
        [SerializeField, HideInInspector] private int height = DefaultDimensions;
        [SerializeField, HideInInspector] private int depth  = DefaultDimensions;
        [SerializeField, HideInInspector] private int margin = DefaultDimensions;


        [SerializeField] private int _width  = DefaultDimensions;
        [SerializeField] private int _height = DefaultDimensions;
        [SerializeField] private int _depth  = DefaultDimensions;
        [SerializeField] private int _margin = DefaultDimensions;


        [SerializeField, HideInInspector] private ComputeShader gravitationalField;
        [SerializeField, HideInInspector] private ComputeShader gravitationalFieldVelocity;
        [SerializeField, HideInInspector] private Material      pointsMaterial;
        [SerializeField, HideInInspector] private Material      gridMaterial;

        [SerializeField] private bool drawPoints = false;
        [SerializeField] private bool drawGrid   = true;

        private ComputeBuffer pointBuffer;
        private ComputeBuffer gridBuffer;

        private int computePointPositionsKernel;
        private int computeDisplacementKernel;
        private int computeGridKernel;
        private int computeVelocityKernel;



        public int Width  { get { return width;  } set { width  = Mathf.Max(1, value); } }
        public int Height { get { return height; } set { height = Mathf.Max(1, value); } }
        public int Depth  { get { return depth;  } set { depth  = Mathf.Max(1, value); } }
        public int Margin { get { return margin; } set { margin = Mathf.Max(0, value); } }

        private int W          { get { return width  + 1; } }
        private int H          { get { return height + 1; } }
        private int D          { get { return depth  + 1; } }
        private int ThreadsX   { get { return W;          } }
        private int ThreadsY   { get { return H;          } }
        private int ThreadsZ   { get { return D;          } }
        private int PointCount { get { return W * H * D;  } }

        private void OnEnable()
        {
            if (bodies == null)
            {
                bodies =
                new GameObject().AddComponent<GravitationalBodyManager>();
                bodies.transform.parent = transform;
            }

            LoadResource("GravitationalField",         ref gravitationalField);
            LoadResource("GravitationalFieldVelocity", ref gravitationalFieldVelocity);
            LoadResource("GravitationalFieldPoints",   ref pointsMaterial);
            LoadResource("GravitationalFieldGrid",     ref gridMaterial);

            computePointPositionsKernel = gravitationalField.FindKernel("ComputePointPositions");
            computeDisplacementKernel   = gravitationalField.FindKernel("ComputeDisplacement");
            computeGridKernel           = gravitationalField.FindKernel("ComputeGrid");

            computeVelocityKernel = gravitationalFieldVelocity.FindKernel("ComputeVelocity");
        }

        private void OnDisable()
        {
            ReleaseComputeBuffer(ref pointBuffer);
            ReleaseComputeBuffer(ref gridBuffer);
        }



        private void OnDestroy()
        {
            Resources.UnloadAsset(pointsMaterial);
            Resources.UnloadAsset(gridMaterial);
        }


        private void OnRenderObject()
        {
            ValidatePointBuffer();
            ValidateGridBuffer();

            if (bodies.Count > 0)
                gravitationalField.SetBuffer(computeDisplacementKernel, "body_buffer", bodies.Buffer);
            gravitationalField.SetInt("body_count", bodies.Count);
            gravitationalField.SetBuffer(computeDisplacementKernel, "point_buffer", pointBuffer);
            gravitationalField.Dispatch(computeDisplacementKernel, ThreadsX, ThreadsY, ThreadsZ);

            if (drawPoints)
                DrawField(pointsMaterial);

            if (drawGrid)
            {
                gravitationalField.Dispatch(computeGridKernel, ThreadsX, ThreadsY, ThreadsZ);
                DrawField(gridMaterial);
            }

            ComputeVelocity();
        }



        private void ValidatePointBuffer()
        {
            if (ValidateComputeBuffer(PointCount, sizeof(float) * 3 * 2, ref pointBuffer))
            {
                gravitationalField.SetInt   ("w", W);
                gravitationalField.SetInt   ("h", H);
                gravitationalField.SetInt   ("d", D);
                gravitationalField.SetVector("offset", new Vector3(width, height, depth) * 0.5f);
                gravitationalField.SetBuffer(computePointPositionsKernel, "point_buffer", pointBuffer);
                gravitationalField.Dispatch(computePointPositionsKernel, ThreadsX, ThreadsY, ThreadsZ);

                pointsMaterial.SetBuffer("point_buffer", pointBuffer);
            }
        }

        private void ValidateGridBuffer()
        {
            if (ValidateComputeBuffer(PointCount, sizeof(uint) * 3, ref gridBuffer))
            {
                gravitationalField.SetBuffer(computeGridKernel, "point_buffer", pointBuffer);
                gravitationalField.SetBuffer(computeGridKernel, "grid_buffer",  gridBuffer);

                gridMaterial.SetBuffer("point_buffer", pointBuffer);
                gridMaterial.SetBuffer("grid_buffer",  gridBuffer);
            }
        }

        private void DrawField(Material material)
        {
            material.SetPass(0);
            material.SetMatrix("object_to_world", transform.localToWorldMatrix);
            Graphics.DrawProceduralNow(MeshTopology.Points, PointCount);
        }

        private void ComputeVelocity()
        {
            if (Application.isPlaying)
            {
                if (bodies.Count > 0)
                {
                    gravitationalFieldVelocity.SetInt   (                       "w",            W);
                    gravitationalFieldVelocity.SetInt   (                       "h",            H);
                    gravitationalFieldVelocity.SetInt   (                       "d",            D);
                    gravitationalFieldVelocity.SetInt   (                       "margin",       margin);
                    gravitationalFieldVelocity.SetFloat (                       "delta_time",   Time.deltaTime);
                    gravitationalFieldVelocity.SetBuffer(computeVelocityKernel, "point_buffer", pointBuffer);
                    gravitationalFieldVelocity.SetBuffer(computeVelocityKernel, "body_buffer",  bodies.Buffer);
                    gravitationalFieldVelocity.Dispatch(computeVelocityKernel, bodies.Count, 1, 1);
                }
            }
        }
    }
}