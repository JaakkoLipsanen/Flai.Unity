using System;
using System.Collections.Generic;

namespace Flai.Scripts.Responses
{
    // executes all responses in children
    public class ExecuteChildrenResponse : Response
    {
        private static readonly Action<Response> ExecuteOnAction = r => r.Execute();
        private static readonly Action<Response> ExecuteOffAction = r => r.ExecuteOff();
        private static readonly Action<Response> ExecuteToggleAction = r => r.ExecuteToggle();
        public bool SearchRecursively = false; // true == search all children, false == search only one level

        protected override bool ExecuteInner()
        {
            this.Execute(ExecuteChildrenResponse.ExecuteOnAction);
            return true; // returns true always. todo: return only if at least one response was executed?
        }

        protected override bool ExecuteOffInner()
        {
            this.Execute(ExecuteChildrenResponse.ExecuteOffAction);
            return true; // returns true always. todo: return only if at least one response was executed?
        }


        protected override bool ExecuteToggleInner()
        {
            this.Execute(ExecuteChildrenResponse.ExecuteToggleAction);
            return true; // returns true always. todo: return only if at least one response was executed?
        }
    
        private void Execute(Action<Response> action)
        {
            List<Response> cache = new List<Response>(); 
            this.Transform.GetComponentsInChildren(cache, this.SearchRecursively);

            for (int i = 0; i < cache.Count; i++)
            {
                if (cache[i] != null)
                {
                    action(cache[i]);
                }
            }
        }
    }
}
