
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
//using ImsPosLibrary.Models;

namespace GTransfer.CommandBehavior
{
    class ItemButton : Button
    {
      // private Product _product;
        private TextBlock _tbItemName;        
        private StackPanel _stackPanel;
        private Image _image;
               
        public enum ImagePosition
        {
            BeforeText, AfterText
        }


        public ImageSource ImageSource
        {
            get { return _image.Source; }
            set { _image.Source = value; _image.Width = (value == null) ? 0 : this.Height; }
        }        
        public ImagePosition ImageOrder { get; set; }

     //  public Product product { get { return _product; } set { _product = value; _tbItemName.Text = value.DESCA; } }

        public ItemButton()
        {
            this.Width = 120;
            this.Height = 100;
            this.Margin = new Thickness(10, 5, 10, 5);
            _stackPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new System.Windows.Thickness(0), VerticalAlignment = System.Windows.VerticalAlignment.Center, HorizontalAlignment = System.Windows.HorizontalAlignment.Center };
            _image = new Image() { Height = this.Height, Width = 0, Stretch = Stretch.Fill };
            _tbItemName = new TextBlock { Width= this.Width-10, VerticalAlignment = System.Windows.VerticalAlignment.Center, FontWeight = System.Windows.FontWeights.SemiBold, FontSize = 14, Margin = new System.Windows.Thickness(8, 0, 0, 0), TextWrapping = TextWrapping.WrapWithOverflow };
            _stackPanel.Children.Add(_image);
            _stackPanel.Children.Add(_tbItemName);
            this.Content = _stackPanel;
            this.TouchDown += ItemButton_TouchDown;
        }

        void ItemButton_TouchDown(object sender, System.Windows.Input.TouchEventArgs e)
        {
            this.Command.Execute(this.CommandParameter);
        }
    }
}
