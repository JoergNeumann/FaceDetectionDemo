using FaceDetectionDemo.Helpers;
using Windows.UI.Xaml.Media;

namespace FaceDetectionDemo
{
    public class CapturedImageInfo : ModelBase
    {
        private string _fileName;
        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; OnPropertyChanged(); }
        }

        private ImageSource _imageSource;
        public ImageSource ImageSource
        {
            get { return _imageSource; }
            set { _imageSource = value; OnPropertyChanged(); }
        }
    }
}
