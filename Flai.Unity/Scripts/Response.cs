using System;

namespace Flai.Scripts
{
    public abstract class Response : FlaiScript
    {
        public abstract void Execute();
        public virtual void ExecuteOff()
        {
            throw new NotImplementedException("");
        }
    }
}
