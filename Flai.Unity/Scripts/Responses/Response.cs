
namespace Flai.Scripts.Responses
{
    public abstract class Response : FlaiScript
    {
        public virtual bool CanExecute
        {
            get { return true; }
        }

        public virtual bool IsToggleable
        {
            get { return false; }
        }

        public bool Execute(bool executeOn)
        {
            return executeOn ? this.ExecuteOn() : this.ExecuteOff();
        }

        public bool ExecuteOn()
        {
            this.OnExecuteOnCalled();
            if (!this.CanExecute)
            {
                return false;
            }

            return this.ExecuteOnInner();
        }

        public bool ExecuteOff()
        {
            this.OnExecuteOffCalled();
            if (!this.CanExecute)
            {
                return false;
            }

            return this.ExecuteOffInner();
        }

        public bool ExecuteToggle()
        {
            this.OnExecuteToggleCalled();
            if (!this.CanExecute || !this.IsToggleable)
            {
                return false;
            }

            return this.ExecuteToggleInner();
        }

        protected abstract bool ExecuteOnInner();
        protected virtual bool ExecuteOffInner()
        {
            return false;
        }

        protected virtual bool ExecuteToggleInner()
        {
            return false;
        }

        protected virtual void OnExecuteOnCalled() { }
        protected virtual void OnExecuteOffCalled() { }
        protected virtual void OnExecuteToggleCalled() { }
    }

    public abstract class ToggleableResponse : Response
    {
        public sealed override bool IsToggleable
        {
            get { return true; }
        }

        protected abstract override bool ExecuteToggleInner();
    }
}
