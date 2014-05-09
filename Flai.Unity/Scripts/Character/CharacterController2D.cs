using System.Collections.Generic;
using Flai.Inspector;
using UnityEngine;

namespace Flai.Scripts.Character
{
    public class CharacterController2D : FlaiScript
    {
        #region Fields

        protected float _timeInAir = float.MaxValue / 2f; // really big value at the start so that it is larger than JumpTimeBias
        protected bool _isJumping = false;

        protected int _moveDirection = 0;
        protected bool _jumpNextFrame = false;

        public float HorizontalSpeedDrag = 0.85f;
        public float AccelerationPower = 20;
        public float AirSpeedDrag = 1f;
        public float Speed = 10;
        public float JumpForce = 750;
        public float JumpTimeBias = 0.1f;
        public bool SetFacingDirectionAutomatically = false;
        public bool SetGroundDirectionAutomatically = false;

        public List<string> IgnoredLayers = new List<string>();

        #endregion

        #region Properties

        public virtual LayerMaskF IgnoreMask
        {
            get { return LayerMaskF.FromNames(this.IgnoredLayers).Inverse; }
        }

        [ShowInInspector]
        public virtual bool IsOnGround
        {
            get
            {
                BoxCollider2D boxCollider = this.BoxCollider2D;
                Vector2f center = boxCollider.GetBoundsHack().Center + this.GroundDirection.ToUnitVector() * (boxCollider.size.y / 2f + 0.01f); // todo: remove GetBoundsHack
                Vector2f left = center - Vector2f.UnitX * boxCollider.size / 2f;
                Vector2f right = center + Vector2f.UnitX * boxCollider.size / 2f;

                const float MaxDistance = 0.01f;
                Vector2f direction = this.GroundDirection.ToUnitVector();
                return Physics2D.Raycast(center, direction, MaxDistance, this.IgnoreMask) || // three points to check ground
                    Physics2D.Raycast(left, direction, MaxDistance, this.IgnoreMask) ||
                    Physics2D.Raycast(right, direction, MaxDistance, this.IgnoreMask);
            }
        }

        [ShowInInspector]
        public bool CanJump
        {
            get { return !_isJumping && !_jumpNextFrame && (this.IsOnGround || _timeInAir <= this.JumpTimeBias); }
        }

        [ShowInInspector]
        public Vector2f Velocity
        {
            get { return this.Rigidbody2D.velocity; }
        }

        // todo: in Unity4.5 "mirror" flag should be coming. dunno if mirroring vertically works too? also setter missing atm
        [ShowInInspector]
        public virtual VerticalDirection GroundDirection
        {
            get
            {
                float gravity = Physics2D.gravity.y * this.Rigidbody2D.gravityScale;
                return (gravity <= 0) ? VerticalDirection.Down : VerticalDirection.Up;
            }
        }

        // todo: in Unity4.5 "mirror" flag should be coming.
        [ShowInInspector(IsReadOnly = true)]
        public HorizontalDirection FacingDirection
        {
            get { return this.Scale.x > 0 ? HorizontalDirection.Right : HorizontalDirection.Left; }
            set { this.Scale2D = Vector2f.Abs(this.Scale2D) * new Vector2f(value == HorizontalDirection.Right ? 1 : -1, 1); }
        }

        #endregion

        #region Input Methods (Move/Jump etc)

        public void MoveLeft()
        {
            _moveDirection = FlaiMath.Clamp(_moveDirection - 1, -1, 1);
        }

        public void MoveRight()
        {
            _moveDirection = FlaiMath.Clamp(_moveDirection + 1, -1, 1);
        }

        [ShowInInspector]
        public bool Jump()
        {
            if (this.CanJump)
            {
                _jumpNextFrame = true;
                return true;
            }

            return false;
        }

        #endregion

        protected override void Update()
        {
            this.UpdateTimeInAir();
            this.Control();
        }

        private void UpdateTimeInAir()
        {
            if (this.IsOnGround && (!_isJumping || this.Velocity.Y <= 0))
            {
                _isJumping = false;
                _timeInAir = 0;
            }
            else
            {
                _timeInAir += Time.deltaTime;
            }
        }

        // bad name
        private void Control()
        {
            this.ApplyHorizontalSpeedDrag();
            float force = this.CalculateHorizontalForce();
            this.ApplyHorizontalVelocity(force);

            if (force == 0 && this.IsOnGround)
            {
                this.Rigidbody2D.velocity *= new Vector2f(0.7f, 1);
            }

            if (_jumpNextFrame)
            {
                this.JumpInner();
            }

            // set facing direction
            if (this.SetFacingDirectionAutomatically)
            {
                this.SetFacingDirection(force);
            }

            // flip upside down if anti gravity
            if (this.SetGroundDirectionAutomatically)
            {
                this.SetGroundDirection();
            }

            _jumpNextFrame = false;
            _moveDirection = 0;
        }

        protected virtual float CalculateHorizontalForce()
        {
            float force = _moveDirection * this.Speed;
            if (!this.IsOnGround)
            {
                force *= this.AirSpeedDrag;
            }

            return force;
        }

        protected virtual void JumpInner()
        {
            this.Position2D -= this.GroundDirection.ToUnitVector() * 0.01f;
            this.Rigidbody2D.AddForce(-this.GroundDirection.ToUnitVector() * this.JumpForce);
            _isJumping = true;
        }

        protected virtual void SetFacingDirection(float moveForce)
        {
            if (moveForce != 0)
            {
                this.FacingDirection = (moveForce > 0) ? HorizontalDirection.Right : HorizontalDirection.Left;
            }
        }

        protected virtual void SetGroundDirection()
        {
            int multiplier = (this.GroundDirection == VerticalDirection.Down) ? 1 : -1;
            this.Scale2D = new Vector2f(this.Scale2D.X, FlaiMath.Abs(this.Scale2D.Y) * multiplier);
        }

        protected virtual void ApplyHorizontalVelocity(float force)
        {
            this.Rigidbody2D.velocity += Vector2f.UnitX.ToVector2() * force * this.AccelerationPower * Time.deltaTime;
            this.Rigidbody2D.velocity = Vector2f.ClampX(this.Rigidbody2D.velocity, -this.Speed, this.Speed);
        }

        protected virtual float CalculateHorizontalSpeedDrag()
        {
            return this.HorizontalSpeedDrag;
        }

        private void ApplyHorizontalSpeedDrag()
        {
            this.Rigidbody2D.velocity *= new Vector2f(this.CalculateHorizontalSpeedDrag(), 1);
        }
    }
}
