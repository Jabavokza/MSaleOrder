using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MSaleOrder
{
    public static class Extensions
    {
        public static void Invoke<TControlType>(this TControlType control, Action<TControlType> del)
            where TControlType : Control
        {
            if (control.InvokeRequired)
                control.Invoke(new Action(() => del(control)));
            else
                del(control);
        }
        public static void Invoke(this System.Windows.Forms.Control @this, Action action)
        {
            if (@this == null) return;//throw new ArgumentNullException("@this");
            if (action == null) return;//throw new ArgumentNullException("action");
            if (@this.InvokeRequired)
            {
                @this.Invoke(action);
            }
            else
            {
                action();
            }
        }
    }
}
