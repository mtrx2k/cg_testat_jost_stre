
namespace GravityDemo
{
    using System;
    using UnityEngine;

    [ExecuteInEditMode]
    public sealed partial class GravitationalBody : MonoBehaviour
    {

        public struct Data
        {

            public Vector4 position;
            public Vector3 velocity;
            public float   mass;


            public static int Size
            {
                get
                {
                    return sizeof(float) * 4 +
                           sizeof(float) * 3 +
                           sizeof(float);
                }
            }

        }


        [SerializeField] private float mass         = 1;
        [SerializeField] private float initialSpeed = 1;
        [SerializeField] private Color color        = Color.white;
        [SerializeField] private bool  render       = true;

        private bool altered;

        public float Mass
        {
            get { return mass; }
            set
            {
                if (mass != value)
                {
                    mass    = value;
                    altered = true;
                }
            }
        }

        public float InitialSpeed
        {
            get { return initialSpeed; }
            set { initialSpeed = value; }
        }

        public Color Color
        {
            get { return color; }
            set { color = value; }
        }

        public bool Render
        {
            get { return render; }
            set { render = value; }
        }



        public event Action<GravitationalBody> OnAltered   = delegate { };
        public event Action<GravitationalBody> OnDestroyed = delegate { }; 

        private void Awake()
        {
            name = typeof(GravitationalBody).Name;
        }


        private void OnValidate()
        {
            altered = true;
        }


        private void OnDestroy()
        {
            OnDestroyed(this);
        }

        private void Update()
        {
            float scale = Mathf.Max(1, mass * 0.005f);
            if (transform.localScale.x != scale ||
                transform.localScale.y != scale ||
                transform.localScale.z != scale)
                transform.localScale = Vector3.one * scale;

            if (!Application.isPlaying)
            {
                if (transform.hasChanged)
                {
                    transform.hasChanged = false;
                    altered              = true;
                }

                if (altered)
                {
                    altered = false;
                    OnAltered(this);
                }
            }
        }

    }
}