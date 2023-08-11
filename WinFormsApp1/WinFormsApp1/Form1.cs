using System.Text;
using AForge.Video.DirectShow;

namespace WinFormsApp1
{
	public partial class Form1 : Form
	{
		PictureBox pictureBoxVideoCapture;

		public Form1()
		{
			InitializeComponent();
		}

		private VideoCaptureDevice? videoCaptureDevice;

		// on closing, close the video device
		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (this.videoCaptureDevice != null)
			{
				this.videoCaptureDevice.SignalToStop();
				this.videoCaptureDevice = null;
			}
		}

		private void button1_Click(object sender, EventArgs e)
		{
			if (this.videoCaptureDevice != null)
			{
				this.videoCaptureDevice.SignalToStop();
				this.videoCaptureDevice = null;
				return;
			}

			var pictureBox = this.pictureBoxVideoCapture;
			var barcodeResult = this.textBoxBarcode;

			var items = AsArray(new FilterInfoCollection(FilterCategory.VideoInputDevice));
			if (items.Length == 0) throw new Exception("No Video Input Device found");

			if (this.comboBoxInputDevice.SelectedIndex < 0)
			{
				this.comboBoxInputDevice.SelectedIndex = 0;
			}


			var desiredItem = items.FirstOrDefault(i => i.Name == this.comboBoxInputDevice.SelectedItem.ToString());
			if (desiredItem == null) throw new Exception($"Camera not found");

			var videoSource = this.videoCaptureDevice = new VideoCaptureDevice(desiredItem.MonikerString);
			// pick resolution where width is 1280
			var targetRes = videoSource.VideoCapabilities.FirstOrDefault(c => c.FrameSize.Width == 1280, videoSource.VideoCapabilities[0]);
			videoSource.VideoResolution = targetRes;

			var timeOfLastScan = DateTime.Now.AddSeconds(-1);

			videoSource.VideoSourceError += VideoSource_VideoSourceError;

			videoSource.SnapshotFrame += (s, e) =>
			{
			};

			videoSource.PlayingFinished += (s, e) =>
			{
			};

			videoSource.NewFrame += (s, e) =>
			{
				// are we on the UI thread?
				if (!this.InvokeRequired) throw new Exception("Must not execute on the UI thread");

				// display the barcode value, but we are not on the UI thread, so...
				this.Invoke((MethodInvoker)delegate
				{
					var bitmap = (Bitmap)e.Frame.Clone();
					pictureBox.Image = bitmap;
				});

				// has one second passed yet?
				var timeSinceLastScan = DateTime.Now.Subtract(timeOfLastScan);
				if (timeSinceLastScan.TotalMilliseconds > 200)
				{
					// create a new thread to decode the barcode
					var bitmap = (Bitmap)e.Frame.Clone();
					var thread = new Thread(() =>
					{
						var barcode = BarcodeDecoder.ImageAsBarcode(bitmap);
						if (!string.IsNullOrEmpty(barcode))
						{
							// display the barcode value, but we are not on the UI thread, so...
							this.Invoke((MethodInvoker)delegate
							{
								barcodeResult.Text = barcode;
								var tableData = BarcodeDecoder.DecodeDriverLicense(barcode);
								// create a table control to hold this data
								ShowDriverInfoInTable(tableData);
							});
						}
						timeOfLastScan = DateTime.Now;
					});
					thread.Start();
				}

			};


			videoSource.Start();


		}

		private string AsString(Dictionary<string, string> dictionary)
		{
			var sb = new StringBuilder();
			foreach (var kvp in dictionary)
			{
				sb.AppendLine($"{kvp.Key}: {kvp.Value}");
			}
			return sb.ToString();
		}

		private FilterInfo[] AsArray(FilterInfoCollection items)
		{
			var result = new FilterInfo[items.Count];
			for (var i = 0; i < items.Count; i++)
			{
				result[i] = items[i];
			}
			return result;
		}

		private void VideoSource_VideoSourceError(object sender, AForge.Video.VideoSourceErrorEventArgs eventArgs)
		{
			throw new NotImplementedException();
		}

		private void button1_Click_1(object sender, EventArgs e)
		{
			var barcodeResult = this.textBoxBarcode;
			var photo = Bitmap.FromFile(@"C:\Users\calix\Pictures\barcode_pdf417\sc_dl.png");
			var pictureBox = this.pictureBoxVideoCapture;
			pictureBox.Image = photo;

			var barcode = BarcodeDecoder.ImageAsBarcode(photo);
			if (!string.IsNullOrEmpty(barcode))
			{
				barcodeResult.Text = barcode;
			}
			else
			{
				barcodeResult.Text = "Barcode not found";
			}

		}

		private void Form1_Load(object sender, EventArgs e)
		{
			var comboBoxInputDevice = this.comboBoxInputDevice;
			var items = new FilterInfoCollection(FilterCategory.VideoInputDevice);
			for (int i = 0; i < items.Count; i++)
			{
				{
					comboBoxInputDevice.Items.Add(items[i].Name);
				}
			}
		}

		private void ShowDriverInfoInTable(Dictionary<string, string> tableData)
		{
			var table = this.tableLayoutPanel1;
			table.ColumnCount = 2;
			table.RowCount = tableData.Count;
			var row = 0;
			// enable scrolling
			table.AutoScroll = true;
			foreach (var item in tableData)
			{
				var key = item.Key;
				var value = item.Value;
				var descriptionLabel = new Label
				{
					Text = key,
					// prevent wrapping text on the descriptionLabel
					MaximumSize = new System.Drawing.Size(200, 0),
					AutoSize = true
				};

				table.Controls.Add(descriptionLabel, 0, row);
				table.Controls.Add(new Label() { Text = value }, 1, row);
				row++;
			}
		}
	}
}