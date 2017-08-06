using GTransfer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GTransfer.Converters
{
    [ValueConversion(typeof(ITreeNode), typeof(ImageSource))]
    public class TreeNodeToImageConverter : IValueConverter {
        private const string UriFormat = "pack://application:,,,/Images/{0}";
        //private const string UriFormat = "../Images/{0}";
        private static readonly IDictionary<string, ImageSource> SuffixToImageMap = new Dictionary<string, ImageSource>();

        private static readonly ImageSource FolderSource = new BitmapImage(new Uri(String.Format(UriFormat, "closeicon.png")) );
        private static readonly ImageSource FolderExpanded = new BitmapImage(new Uri(String.Format(UriFormat, "openicon.png")));
        private static readonly ImageSource ItemSource = new BitmapImage(new Uri(String.Format(UriFormat, "file.png")));

        static TreeNodeToImageConverter() {
            SuffixToImageMap["Bill"] = new BitmapImage(new Uri(String.Format(UriFormat, "closeicon.png")));
            SuffixToImageMap["RV"] = new BitmapImage(new Uri(String.Format(UriFormat, "file.png")));
            //SuffixToImageMap[".png"] = SuffixToImageMap[".jpeg"] = SuffixToImageMap[".jpg"] = new BitmapImage(new Uri(String.Format(UriFormat, "Picture.png")));
            //SuffixToImageMap[".txt"] = new BitmapImage(new Uri(String.Format(UriFormat, "Text.png")));
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) 
        {

            var viewModel = (ITreeNode)value;
            if (viewModel == null)
                return Binding.DoNothing;

            if (viewModel.IsGroup && viewModel.IsExpanded)
                return FolderExpanded;
            if (viewModel.IsExpanded == false && viewModel.IsGroup)
                return FolderSource;
            if (viewModel.IsGroup == false)
                return ItemSource;
            else
            {
                //if(viewModel.Type==null)
                //{
                //    return SuffixToImageMap.s
                //}
                //var source = SuffixToImageMap.Where(kvp => viewModel.Group.EndsWith(kvp.Key)).Select(kvp => kvp.Value).FirstOrDefault();
                //return source ?? Binding.DoNothing;
                return Binding.DoNothing;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
