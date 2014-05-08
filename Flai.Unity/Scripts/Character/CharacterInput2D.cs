using Flai.Input;
using UnityEngine;

namespace Flai.Scripts.Character
{
    [RequireComponent(typeof(CharacterController2D))]
    public class CharacterInput2D : FlaiScript
    {
        private CharacterController2D _characterController2D;
        public bool IsControllingEnabled = true;

        public string MoveLeftButton = "Move Left";
        public KeyCode MoveLeftAlternativeKey = KeyCode.A;

        public string MoveRightButton = "Move Right";
        public KeyCode MoveRightAlternativeKey = KeyCode.D;

        public string JumpButton = "Jump";
        public KeyCode JumpAlternativeKey = KeyCode.Space;

        protected override void Awake()
        {
            _characterController2D = this.Get<CharacterController2D>();
        }

        protected override void Update()
        {
            if (this.IsControllingEnabled)
            {
                if (FlaiInput.IsButtonOrKeyPressed(this.MoveLeftButton, this.MoveLeftAlternativeKey))
                {
                    _characterController2D.MoveLeft();
                }

                if (FlaiInput.IsButtonOrKeyPressed(this.MoveRightButton, this.MoveRightAlternativeKey))
                {
                    _characterController2D.MoveRight();
                }

                if (FlaiInput.IsButtonOrKeyPressed(this.JumpButton, this.JumpAlternativeKey))
                {
                    _characterController2D.Jump();
                }
            }
        }
    }
}
