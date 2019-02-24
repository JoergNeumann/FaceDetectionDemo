using FaceDetectionDemo.Helpers;
using System.Collections.ObjectModel;

namespace FaceDetectionDemo
{
    public class MainPageViewModel : ModelBase
    {
        public MainPageViewModel()
        {
            this.TrainingPhotos = new ObservableCollection<CapturedImageInfo>();
            this.Status = "Enter a name and take 3 photos!";
        }

        private ObservableCollection<CapturedImageInfo> _trainingPhotos;
        public ObservableCollection<CapturedImageInfo> TrainingPhotos
        {
            get { return _trainingPhotos; }
            set { _trainingPhotos = value; OnPropertyChanged(); }
        }

        private string _trainingPersonName;
        public string TrainingPersonName
        {
            get { return _trainingPersonName; }
            set { _trainingPersonName = value; OnPropertyChanged(); }
        }

        private CapturedImageInfo _testPhoto;
        public CapturedImageInfo TestPhoto
        {
            get { return _testPhoto; }
            set { _testPhoto = value; OnPropertyChanged(); }
        }

        private string _detectedPersonName;
        public string DetectedPersonName
        {
            get { return _detectedPersonName; }
            set { _detectedPersonName = value; OnPropertyChanged(); }
        }

        private string _detectedPersonAttributes;
        public string DetectedPersonAttributes
        {
            get { return _detectedPersonAttributes; }
            set { _detectedPersonAttributes = value; OnPropertyChanged(); }
        }

        private string _confidence;
        public string Confidence
        {
            get { return _confidence; }
            set { _confidence = value; OnPropertyChanged(); }
        }

        private string _status;
        public string Status
        {
            get { return _status; }
            set { _status = value; OnPropertyChanged(); }
        }
    }
}
