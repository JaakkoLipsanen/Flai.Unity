
// ReSharper disable ConvertConditionalTernaryToNullCoalescing
using System.Collections.Generic;
using UnityEngine;

namespace Flai
{
    // lightweight extension of MonoBehaviour, use in cases where you have a tons of components. Doesn't implement any methods (Awake, Update, Collision etc)
    public abstract class ExtendedMonoBehaviour : MonoBehaviour
    {
        internal GameObject _gameObject = null;
        internal Transform _transform = null;

        public GameObject GameObject
        {
            get { return _gameObject == null ? (_gameObject = gameObject) : _gameObject; }
        }

        #region Components

        public Transform Transform
        {
            get { return _transform == null ? (_transform = transform) : _transform; }
        }

        public BoxCollider2D BoxCollider2D
        {
            get { return this.Get<BoxCollider2D>(); }
        }

        public SpriteRenderer SpriteRenderer
        {
            get { return this.Get<SpriteRenderer>(); }
        }

        public Animator Animator
        {
            get { return this.Get<Animator>(); }
        }

        #endregion

        #region Position/Rotation/Scale

        public Vector2f Position2D
        {
            get { return this.Transform.GetPosition2D(); }
            set { this.Transform.SetPosition2D(value); }
        }

        public Vector2f LocalPosition2D
        {
            get { return this.Transform.GetLocalPosition2D(); }
            set { this.Transform.SetLocalPosition2D(value); }
        }

        public Vector3 Position
        {
            get { return this.Transform.position; }
            set { this.Transform.position = value; }
        }

        public float Rotation2D
        {
            get { return this.Transform.GetRotation2D(); }
            set { this.Transform.SetRotation2D(value); }
        }

        public float LocalRotation2D
        {
            get { return this.Transform.GetLocalRotation2D(); }
            set { this.Transform.SetLocalRotation2D(value); }
        }

        public Vector2f RotationDirection2D
        {
            get { return FlaiMath.GetAngleVectorDeg(this.Rotation2D); }
            set
            {
                Ensure.True(value != Vector2f.Zero);
                this.Rotation2D = FlaiMath.GetAngleDeg(value);
            }
        }

        public Vector3 Rotation
        {
            get { return this.Transform.eulerAngles; }
            set { this.Transform.eulerAngles = value; }
        }

        public Vector3 LocalRotation
        {
            get { return this.Transform.localEulerAngles; }
            set { this.Transform.localEulerAngles = value; }
        }

        public Quaternion RotationQuaternion
        {
            get { return this.Transform.rotation; }
            set { this.Transform.rotation = value; }
        }

        public Quaternion LocalRotationQuaternion
        {
            get { return this.Transform.localRotation; }
            set { this.Transform.localRotation = value; }
        }

        public Vector2f Scale2D
        {
            get { return this.Transform.GetScale2D(); }
            set { this.Transform.SetScale2D(value); }
        }

        public Vector3 Scale
        {
            get { return this.Transform.localScale; }
            set { this.Transform.localScale = value; }
        }

        #endregion

        #region Parent/Childs

        public GameObject Parent
        {
            get { return this.Transform.GetParent(); }
            set { this.Transform.SetParent(value != null ? value.transform : null); }
        }

        public GameObject RootParent
        {
            get { return this.Transform.root.gameObject; }
        }

        public IEnumerable<GameObject> Children
        {
            get { return this.GetAllChildren(); }
        }

        #endregion

        #region Layer

        public int LayerIndex
        {
            get { return this.GameObject.layer; }
            set { this.GameObject.layer = value; }
        }

        public string LayerName
        {
            get { return Layer.NameFromIndex(this.LayerIndex); }
            set { this.LayerIndex = Layer.IndexFromName(value); }
        }

        #endregion
    }

    public abstract class FlaiScript : ExtendedMonoBehaviour
    {
        protected FlaiScript()
        {
        }

        protected virtual void Awake() { }
        protected virtual void Start() { }
        protected virtual void OnDestroy() { }

        protected virtual void Update() { }
        protected virtual void FixedUpdate() { }
        protected virtual void LateUpdate() { }

        protected virtual void OnCollisionEnter2D(Collision2D collision) { }
        protected virtual void OnCollisionStay2D(Collision2D collision) { }
        protected virtual void OnCollisionExit2D(Collision2D collision) { }

        protected virtual void OnTriggerEnter2D(Collider2D other) { }
        protected virtual void OnTriggerStay2D(Collider2D other) { }
        protected virtual void OnTriggerExit2D(Collider2D other) { }

        protected virtual void OnDrawGizmos() { }
        protected virtual void OnGUI() { }
    }
}
