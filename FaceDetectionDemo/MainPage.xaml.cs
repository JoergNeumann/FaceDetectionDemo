using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace FaceDetectionDemo
{
    public sealed partial class MainPage : Page
    {
        private const string subscriptionKey = "------------------"; // Hier eigenen API Key eintragen!
        private const string faceEndpoint = "https://westeurope.api.cognitive.microsoft.com"; // Hier ggf. andere Region URI angeben!

        private string _groupId = "bdd23070-9b03-4ea6-a767-604b280d3980"; // Guid.NewGuid().ToString();
        private string _groupName = "MyGroup";
        private StorageFolder _imageFolder;
        private MainPageViewModel _viewModel;
        private CameraCaptureService _cameraService;
        private FaceRecognitionService _recognitionService;

        public MainPage()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            _viewModel = this.DataContext as MainPageViewModel;
            _cameraService = new CameraCaptureService();
            _recognitionService = new FaceRecognitionService(subscriptionKey, faceEndpoint);
            _imageFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("ProfilePhotoFolder", CreationCollisionOption.OpenIfExists);
        }

        private async void TakePhotoButton_Click(object sender, RoutedEventArgs e)
        {
            var response = await _cameraService.CapturePhoto(_imageFolder);
            if (response != null)
            {
                _viewModel.TrainingPhotos.Add(response);
            }
        }

        private void TrainModel_Click(object sender, RoutedEventArgs e)
        {
            var t1 = _recognitionService.TrainPersonGroup(_groupId, _groupName, _viewModel.TrainingPersonName, _imageFolder.Path);
            Task.WhenAll(t1).Wait(5000);
            _viewModel.Status = "Training done.";
        }

        private async void DetectButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.TestPhoto = await _cameraService.CapturePhoto(_imageFolder);
            if (_viewModel.TestPhoto != null)
            {
                var info = await _recognitionService.IdentifyPerson(_groupId, _viewModel.TestPhoto.FileName);
                _viewModel.DetectedPersonName = "Detected: " +  (info.Name != null 
                    ? info.Name + " (" + info.Confidence + ")" 
                    : "(unknown)");
                _viewModel.DetectedPersonAttributes = info.Attributes;
            }
        }
    }
}
