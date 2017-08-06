using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
//using ImsPosLibrary;
//using ImsPosLibrary.Helpers;

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;


namespace GTransfer.CommandBehavior
{
    public class MouseDoubleClick 
    {
        public static DependencyProperty CommandProperty =
      DependencyProperty.RegisterAttached("Command",
      typeof(IAccount),
      typeof(MouseDoubleClick),
      new UIPropertyMetadata(CommandChanged));

        public static DependencyProperty CommandParameterProperty =
            DependencyProperty.RegisterAttached("CommandParameter",
                                                typeof(object),
                                                typeof(MouseDoubleClick),
                                                new UIPropertyMetadata(null));

        public static void SetCommand(DependencyObject target, IAccount value)
        {
            target.SetValue(CommandProperty, value);
        }

        public static void SetCommandParameter(DependencyObject target, object value)
        {
            target.SetValue(CommandParameterProperty, value);
        }
        public static object GetCommandParameter(DependencyObject target)
        {
            return target.GetValue(CommandParameterProperty);
        }

        private static void CommandChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            Control control = target as Control;
            if (control != null)
            {
                if ((e.NewValue != null) && (e.OldValue == null))
                {
                    control.MouseDoubleClick += OnMouseDoubleClick;
                }
                else if ((e.NewValue == null) && (e.OldValue != null))
                {
                    control.MouseDoubleClick -= OnMouseDoubleClick;
                }
            }
        }

        private static void OnMouseDoubleClick(object sender, RoutedEventArgs e)
        {
            Control control = sender as Control;
            IAccount command = (IAccount)control.GetValue(CommandProperty);
            object commandParameter = control.GetValue(CommandParameterProperty);
            command.Execute(commandParameter);
        }
    }

    public interface IAccount
    {
        string ACNAME
        {
            get;
            set;
        }

        void Execute(object commandParameter);
    }
}
