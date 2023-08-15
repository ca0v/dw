using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using AForge.Video;
using AForge.Video.DirectShow;
using SixLabors.ImageSharp.Processing.Processors.Transforms;

namespace WinFormsApp1
{

    public static class ThreadExtension
    {
        public static void Abort(this Thread thread)
        {
        }
    }

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

        internal static VideoSourceWrapper WatchDevice(string monikerString, AForge.Controls.VideoSourcePlayer player)
        {
            var videoSource = new VideoCaptureDevice(monikerString);

            var idealFrameRate = Configuration.TryGetValue("barcode.ideal-video-framerate", 10);
            var idealResWidth = Configuration.TryGetValue("barcode.ideal-video-width", 1024);

            var capabilities = videoSource.VideoCapabilities.OrderBy(c => c.FrameSize.Width).ThenBy(c => c.BitCount);
            var idealCaps = capabilities.Where(c => c.FrameSize.Width == idealResWidth);
            if (!idealCaps.Any())
            {
                idealCaps = capabilities.Where(c => c.FrameSize.Width > idealResWidth);
            }
            if (!idealCaps.Any())
            {
                idealCaps = capabilities;
            }

            if (!idealCaps.Any()) throw new Exception("No suitable resolution found");

            var ideal = idealCaps.OrderBy(c => c.AverageFrameRate).Where(c => c.AverageFrameRate >= idealFrameRate).FirstOrDefault();
            if (ideal == null)
            {
                ideal = idealCaps.First();
            }

            Trace.WriteLine($"Using resolution {ideal.FrameSize.Width}x{ideal.FrameSize.Height}");
            videoSource.VideoResolution = ideal;
            return new VideoSourceWrapper(player, videoSource);
        }
    }

    internal class VideoSourceWrapper
    {
        private AForge.Controls.VideoSourcePlayer videoSourcePlayer;
        public bool IsWatching { get; private set; }

        public VideoSourceWrapper(AForge.Controls.VideoSourcePlayer player, VideoCaptureDevice videoSource)
        {
            this.videoSourcePlayer = player;
            videoSourcePlayer.VideoSource = videoSource;

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
            this.videoSourcePlayer.NewFrame += (object s, ref Bitmap image) =>
            {
                onFrame(image);
            };
            this.videoSourcePlayer.Start();
        }

        internal void Stop()
        {
            // throws Thread abort is not supported on this platform.
            this.videoSourcePlayer.Stop();
        }
    }
}