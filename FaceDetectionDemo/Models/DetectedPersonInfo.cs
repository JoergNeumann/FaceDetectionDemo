using FaceDetectionDemo.Helpers;

namespace FaceDetectionDemo
{
    public class DetectedPersonInfo : ModelBase
    {
        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged(); }
        }

        private string _attributes;
        public string Attributes
        {
            get { return _attributes; }
            set { _attributes = value; OnPropertyChanged(); }
        }

        private string _confidence;
        public string Confidence
        {
            get { return _confidence; }
            set { _confidence = value; OnPropertyChanged(); }
        }
    }
}
