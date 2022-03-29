using System;
using System.Collections.Generic;
using System.Text;

namespace Week10_4_ChainOfResponsibility
{

    //The Handler abstract class
    public abstract class IHandler
    {
        protected IHandler successor;

        public void SetSuccessor(IHandler successor)
        {
            this.successor = successor;
        }

        public abstract void HandleRequest(object param);
    }

    // A 'Concrete Handler' class
    public class ConcreteHandler1 : IHandler
    {
        public override void HandleRequest(object param)
        {
            //if(condition)
            //{
            //      Do Something!
            //}else if (successor != null)
            //{
            //      successor.HandleRequest();
            //}
        }
    }
}
