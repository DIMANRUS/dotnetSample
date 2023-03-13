using System.Diagnostics;
using Windows.Media;

namespace ScreenCapture;
public sealed partial class MainPage
{
    // Capture API objects.
    private SizeInt32 _lastSize;
    private GraphicsCaptureItem _item;
    private Direct3D11CaptureFramePool _framePool;
    private GraphicsCaptureSession _session;

    // Non-API related members.
    private CanvasDevice _canvasDevice;
    private CompositionGraphicsDevice _compositionGraphicsDevice;
    private Compositor _compositor;
    private CompositionDrawingSurface _surface;
    private CanvasBitmap _currentFrame;
    private string _screenshotFilename = "test.png";

    public MainPage()
    {
        InitializeComponent();
    }

    private void Setup()
    {
        _canvasDevice = new CanvasDevice();

        _compositionGraphicsDevice = CanvasComposition.CreateCompositionGraphicsDevice(
            Window.Current.Compositor,
            _canvasDevice);

        _compositor = Window.Current.Compositor;

        _surface = _compositionGraphicsDevice.CreateDrawingSurface(
            new Size(400, 400),
            DirectXPixelFormat.B8G8R8A8UIntNormalized,
            DirectXAlphaMode.Premultiplied);    // This is the only value that currently works with
                                                // the composition APIs.

        var visual = _compositor.CreateSpriteVisual();
        visual.RelativeSizeAdjustment = Vector2.One;
        var brush = _compositor.CreateSurfaceBrush(_surface);
        brush.HorizontalAlignmentRatio = 0.5f;
        brush.VerticalAlignmentRatio = 0.5f;
        brush.Stretch = CompositionStretch.Uniform;
        visual.Brush = brush;
        ElementCompositionPreview.SetElementChildVisual(this, visual);
    }

    public async Task StartCaptureAsync()
    {
        //var picker = new GraphicsCapturePicker();
        //GraphicsCaptureItem item = await picker.PickSingleItemAsync();

        //if (item != null)
        //{
        //    StartCaptureInternal(item);
        //}
        AppRecordingManager manager = AppRecordingManager.GetDefault();
        var status = manager.GetStatus();
        if (status.CanRecord || status.CanRecordTimeSpan)
        {
            var result = await manager.SaveScreenshotToFilesAsync(ApplicationData.Current.LocalFolder, "screnshot", AppRecordingSaveScreenshotOption.HdrContentVisible, manager.SupportedScreenshotMediaEncodingSubtypes);
            Debug.WriteLine(result.Succeeded);
            if (result.Succeeded)
            {
                foreach (var item in result.SavedScreenshotInfos)
                {
                    Debug.WriteLine(item.File.DisplayName);
                }
            }
            else
            {
                Debug.WriteLine(result.ExtendedError.Message);
            }
        }
    }

    private void StartCaptureInternal(GraphicsCaptureItem item)
    {
        // Stop the previous capture if we had one.
        StopCapture();

        _item = item;
        _lastSize = _item.Size;

        _framePool = Direct3D11CaptureFramePool.Create(
           _canvasDevice, // D3D device
           DirectXPixelFormat.B8G8R8A8UIntNormalized, // Pixel format
           2, // Number of frames
           _item.Size); // Size of the buffers

        _framePool.FrameArrived += (s, a) =>
        {
            // The FrameArrived event is raised for every frame on the thread
            // that created the Direct3D11CaptureFramePool. This means we
            // don't have to do a null-check here, as we know we're the only
            // one dequeueing frames in our application.  

            // NOTE: Disposing the frame retires it and returns  
            // the buffer to the pool.

            using (var frame = _framePool.TryGetNextFrame())
            {
                ProcessFrame(frame);
            }
        };

        _item.Closed += (s, a) =>
        {
            StopCapture();
        };

        _session = _framePool.CreateCaptureSession(_item);
        _session.StartCapture();
    }

    public void StopCapture()
    {
        _session?.Dispose();
        _framePool?.Dispose();
        _item = null;
        _session = null;
        _framePool = null;
    }

    private void ProcessFrame(Direct3D11CaptureFrame frame)
    {
        // Resize and device-lost leverage the same function on the
        // Direct3D11CaptureFramePool. Refactoring it this way avoids
        // throwing in the catch block below (device creation could always
        // fail) along with ensuring that resize completes successfully and
        // isn’t vulnerable to device-lost.
        bool needsReset = false;
        bool recreateDevice = false;

        if ((frame.ContentSize.Width != _lastSize.Width) ||
            (frame.ContentSize.Height != _lastSize.Height))
        {
            needsReset = true;
            _lastSize = frame.ContentSize;
        }

        try
        {
            // Take the D3D11 surface and draw it into a  
            // Composition surface.

            // Convert our D3D11 surface into a Win2D object.
            CanvasBitmap canvasBitmap = CanvasBitmap.CreateFromDirect3D11Surface(
                _canvasDevice,
                frame.Surface);

            _currentFrame = canvasBitmap;

            // Helper that handles the drawing for us.
            FillSurfaceWithBitmap(canvasBitmap);
        }

        // This is the device-lost convention for Win2D.
        catch (Exception e) when (_canvasDevice.IsDeviceLost(e.HResult))
        {
            // We lost our graphics device. Recreate it and reset
            // our Direct3D11CaptureFramePool.  
            needsReset = true;
            recreateDevice = true;
        }

        if (needsReset)
        {
            ResetFramePool(frame.ContentSize, recreateDevice);
        }
    }

    private void FillSurfaceWithBitmap(CanvasBitmap canvasBitmap)
    {
        CanvasComposition.Resize(_surface, canvasBitmap.Size);

        using (var session = CanvasComposition.CreateDrawingSession(_surface))
        {
            session.Clear(Colors.Transparent);
            session.DrawImage(canvasBitmap);
        }
    }

    private void ResetFramePool(SizeInt32 size, bool recreateDevice)
    {
        do
        {
            try
            {
                if (recreateDevice)
                {
                    _canvasDevice = new CanvasDevice();
                }

                _framePool.Recreate(
                    _canvasDevice,
                    DirectXPixelFormat.B8G8R8A8UIntNormalized,
                    2,
                    size);
            }
            // This is the device-lost convention for Win2D.
            catch (Exception e) when (_canvasDevice.IsDeviceLost(e.HResult))
            {
                _canvasDevice = null;
                recreateDevice = true;
            }
        } while (_canvasDevice == null);
    }

    private async void Button_ClickAsync(object sender, RoutedEventArgs e)
    {
        await StartCaptureAsync();
    }

    private async void ScreenshotButton_ClickAsync(object sender, RoutedEventArgs e)
    {
        await SaveImageAsync(_screenshotFilename, _currentFrame);
    }

    private async Task SaveImageAsync(string filename, CanvasBitmap frame)
    {
        StorageFolder pictureFolder = KnownFolders.SavedPictures;

        StorageFile file = await pictureFolder.CreateFileAsync(
            filename,
            CreationCollisionOption.ReplaceExisting);

        using (var fileStream = await file.OpenAsync(FileAccessMode.ReadWrite))
        {
            await frame.SaveAsync(fileStream, CanvasBitmapFileFormat.Png, 1f);
        }
    }
}