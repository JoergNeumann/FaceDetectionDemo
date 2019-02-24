using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;

namespace FaceDetectionDemo
{
    public class CameraCaptureService
    {
        public async Task<CapturedImageInfo> CapturePhoto(StorageFolder imageFolder)
        {
            var captureUI = new CameraCaptureUI();
            captureUI.PhotoSettings.Format = CameraCaptureUIPhotoFormat.Jpeg;
            captureUI.PhotoSettings.CroppedSizeInPixels = new Size(200, 200);
            var photo = await captureUI.CaptureFileAsync(CameraCaptureUIMode.Photo);
            if (photo != null)
            {
                var stream = await photo.OpenAsync(FileAccessMode.Read);
                var decoder = await BitmapDecoder.CreateAsync(stream);
                var softwareBitmap = await decoder.GetSoftwareBitmapAsync();
                var softwareBitmapBGR8 = SoftwareBitmap.Convert(softwareBitmap, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
                var bitmapSource = new SoftwareBitmapSource();
                await bitmapSource.SetBitmapAsync(softwareBitmapBGR8);

                string fileName = Guid.NewGuid().ToString() + ".jpg";
                await photo.CopyAsync(imageFolder, fileName, NameCollisionOption.ReplaceExisting);

                var response = new CapturedImageInfo
                {
                    FileName = Path.Combine(imageFolder.Path, fileName),
                    ImageSource = bitmapSource
                };

                return response;
            }
            return null;
        }
    }
}
