using System.Diagnostics;
using AForge.Video;
using AForge.Video.DirectShow;
using SixLabors.ImageSharp.Processing.Processors.Transforms;

namespace WinFormsApp1;

public class VideoCapture
{
    private static FilterInfo[] AsArray(FilterInfoCollection items)
    {
        var result = new FilterInfo[items.Count];
        for (var i = 0; i < items.Count; i++)
        {
            result[i] = items[i];
        }
        return result;
    }

    internal static FilterInfo[] Devices()
    {
        var items = AsArray(new FilterInfoCollection(FilterCategory.VideoInputDevice));
        return items;
    }

    internal static VideoSourceWrapper WatchDevice(string monikerString)
    {
        var videoSource = new VideoCaptureDevice(monikerString);
        // pick resolution where width is 1280, grayscale
        var idealResWidth = Configuration.TryGetValue("barcode.ideal-video-width", 1024);
        var capabilities = videoSource.VideoCapabilities.OrderBy(c => c.FrameSize.Width).ThenBy(c => c.BitCount);
        var ideal = capabilities.FirstOrDefault(c => c.FrameSize.Width == idealResWidth);
        if (ideal == null)
        {
            ideal = capabilities.FirstOrDefault(c => c.FrameSize.Width > idealResWidth);
        }
        if (ideal == null)
        {
            ideal = capabilities.FirstOrDefault(videoSource.VideoCapabilities[0]);
        }
        if (ideal == null) throw new Exception("No suitable resolution found");
        Trace.WriteLine($"Using resolution {ideal.FrameSize.Width}x{ideal.FrameSize.Height}");
        videoSource.VideoResolution = ideal;
        var result = new VideoSourceWrapper(videoSource);
        return result;
    }
}

internal class VideoSourceWrapper
{
    private readonly VideoCaptureDevice videoSource;
    public bool IsWatching { get; private set; }

    public VideoSourceWrapper(VideoCaptureDevice videoSource)
    {
        this.videoSource = videoSource;
        videoSource.VideoSourceError += (s, e) =>

        videoSource.SnapshotFrame += (s, e) =>
        {
        };

        videoSource.PlayingFinished += (s, e) =>
        {
            this.IsWatching = false;
        };

        videoSource.NewFrame += (s, e) =>
        {
            this.IsWatching = true;
        };

        videoSource.VideoSourceError += (s, e) =>
        {
            this.IsWatching = false;
        };

    }

    internal void Start(Action<Bitmap> onFrame)
    {
        if (IsWatching) throw new Exception("Already watching");
        this.videoSource.NewFrame += (s, e) =>
        {
            onFrame(e.Frame);
        };
        this.videoSource.Start();
    }

    internal void Stop()
    {
        this.videoSource.SignalToStop();
    }
}