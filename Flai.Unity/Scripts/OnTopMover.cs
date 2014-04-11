using Flai.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Flai.Scripts
{
    // todo: awful name, name it better
    public class OnTopMover : FlaiScript
    {
        public Direction2D AllowedDirection = Direction2D.Down;
        public bool DrawDebug = true;

        private HashSet<GameObject> _gameObjectsOnTop = new HashSet<GameObject>();
        private float _previousScaleY;
        private float _previousPositionY;
        private float _scaleMultiplier = 1f;

        public bool HasAny
        {
            get { return _gameObjectsOnTop.Count > 0; }
        }

        public int Count
        {
            get { return _gameObjectsOnTop.Count; }
        }

        public void ForceUpdate()
        {
            this.Update();
        }

        protected override void Awake()
        {
            _previousScaleY = this.Scale2D.Y;
            _previousPositionY = this.Position2D.Y;

            var boxCollider = this.Get<BoxCollider2D>();
            if (boxCollider != null)
            {
                _scaleMultiplier = boxCollider.size.y;
            }
        }

        protected override void Update()
        {
            float changeInUnitsScale = (this.Scale2D.Y - _previousScaleY) * _scaleMultiplier;
            float changeInUnitsPosition = (this.Position2D.Y - _previousPositionY);
            float changeInUnits = changeInUnitsScale + changeInUnitsPosition;

            const float HorizontalInflateAmount = 0.05f;
            const float VerticalInflateAmount = 0.15f;
            Rect currentBounds = this.collider2D.GetBoundsHack().AsInflated(HorizontalInflateAmount, VerticalInflateAmount);
            _gameObjectsOnTop.RemoveWhere(go => go == null);
            _gameObjectsOnTop.RemoveWhere(go => !go.collider2D.GetBoundsHack().AsInflated(HorizontalInflateAmount, VerticalInflateAmount).Overlaps(currentBounds));

            foreach (GameObject other in _gameObjectsOnTop)
            {
                other.SetPosition2D(other.GetPosition2D() + Vector2f.UnitY * changeInUnits);
            }

            _previousScaleY = this.Scale.y;
            _previousPositionY = this.Position2D.Y;

            if (this.DrawDebug)
            {
                FlaiDebug.DrawRectangleOutlines(currentBounds);
            }
        }

        protected override void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.contacts.Any(contact => Vector2f.Distance(contact.normal, this.AllowedDirection.ToUnitVector()) < 0.001f))
            {
                _gameObjectsOnTop.Add(collision.gameObject);
            }
        }

        protected override void OnCollisionExit2D(Collision2D collision)
        {
            _gameObjectsOnTop.Remove(collision.gameObject);
        }
    }
}
