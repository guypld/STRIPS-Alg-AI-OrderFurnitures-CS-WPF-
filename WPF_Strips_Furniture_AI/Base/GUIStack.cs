using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WPF_Strips_Furniture_AI.Tools;

namespace WPF_Strips_Furniture_AI.Base
{
    public class GUIStack<T> : Stack<T>
    {
        //public override void Pop()
        
        public void Push(T item)
        {
            base.Push(item);
            if (item is IList)
            {
                var act = new ActionDescription();
                act.Action = "";
                foreach (var i in (IList)item)
                {
                    act.Action += i.ToString() + ",";
                }


                Model.Instance.PushToGUIStack(act);
                //Thread.Sleep(10);
            }
        }

        public T Pop()
        {
            Model.Instance.PopFromGUIStack();
            return base.Pop();
        }
    }
}
