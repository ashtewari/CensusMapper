using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BingMapMVVM
{
    public class DeferredAction
    {
        private Action action;

        public DeferredAction(Action action)
        {
            this.action = action;
        }

        private Action Action
        {
            get { return this.action; }
        }

        public void Execute()
        {
            this.Action.Invoke();
        }
    }
}
