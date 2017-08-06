using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GTransfer.CommandBehavior
{
    public static class VisualClass
    {
        public static Color MainThemeStartColor { get; set; }
        public static Color MainThemeEndColor { get; set; }
        public static Point MainThemeStartPoint { get; set; }
        public static Point MainThemeEndPoint { get; set; }
        public static double MainThemeOffset { get; set; }
        public static GridLength BillingFormBarcodeWidth { get; set; }
        public static GridLength BillingFormGridWidth { get; set; }

        public static ImageSource CompanyLogo { get; set; }
        static VisualClass()
        {
            MainThemeStartColor = Colors.Blue;
            MainThemeEndColor = Colors.White;
            MainThemeStartPoint = new Point(0, 0);
            MainThemeEndPoint = new Point(1, 0);
            MainThemeOffset = 0.9d;
            BillingFormBarcodeWidth = new GridLength(60, GridUnitType.Star);
            BillingFormGridWidth = new GridLength(40, GridUnitType.Star);
           // CompanyLogo = new System.Windows.Media.Imaging.BitmapImage(new Uri("pack://application:,,,/Images/IMS Logo.png"));
            if (File.Exists(Environment.CurrentDirectory + "\\Theme.dat"))
            {
                string[] colors = File.ReadAllLines(Environment.CurrentDirectory + "\\Theme.dat");
                for (int i = 0; i < colors.Length; i++)
                {
                    switch (i)
                    {
                        case 0:
                            MainThemeStartColor = (Color)ColorConverter.ConvertFromString(colors[0]);
                            break;
                        case 1:
                            MainThemeEndColor = (Color)ColorConverter.ConvertFromString(colors[1]);
                            break;
                        case 2:
                            MainThemeEndPoint = (colors[2] == "Horizontal") ? new Point(1, 0) : new Point(0, 1);
                            break;
                        case 3:
                            double offset = 0;
                            double.TryParse(colors[3], out offset);
                            MainThemeOffset = offset;
                            break;
                       // case 4:
                        //    CompanyLogo = new System.Windows.Media.Imaging.BitmapImage(new Uri(colors[4]));
                          //  break;
                    }
                }
            }
        }
    }
}
