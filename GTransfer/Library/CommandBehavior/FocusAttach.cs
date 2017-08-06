using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GTransfer.CommandBehavior
{
    public class FocusAttacher
    {
        private static readonly DependencyProperty FocusAttachProperty = DependencyProperty.RegisterAttached("FocusAttach", 
            typeof(bool), typeof(FocusAttacher), new PropertyMetadata(false, FocusChanged));

        public static bool GetFocus(DependencyObject  d)
        {
            return (bool)d.GetValue(FocusAttachProperty);
        }

        public static  void  SetFocus(DependencyObject d,bool value)
        {
            d.SetValue(FocusAttachProperty, value);
        }
        private static void FocusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if((bool)e.NewValue==true  )
            {
                ((UIElement)d).Focus();
            }
        }

        
    }

    
}
