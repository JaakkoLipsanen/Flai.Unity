
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
            return executeOn ? this.Execute() : this.ExecuteOff();
        }

        public bool Execute()
        {
            this.OnExecuteOnCalled();
            if (!this.CanExecute)
            {
                return false;
            }

            return this.ExecuteInner();
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

        protected abstract bool ExecuteInner();
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
}
