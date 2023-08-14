using System.Media;
using System.Text;

namespace WinFormsApp1
{
	public partial class Form1 : Form
	{
		private readonly BarcodeDecoder barcodeDecoder = new BarcodeDecoder();
		PictureBox pictureBoxVideoCapture;
		VideoSourceWrapper? videoCapture;

		public Form1()
		{
			InitializeComponent();
		}

		public static void PlaySuccess()
		{
			var waveFile = @"./Assets/success.wav";
			var player = new SoundPlayer(waveFile);
			player.Play();
		}

		// on closing, close the video device
		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			videoCapture?.Stop();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			if (true == this.videoCapture?.IsWatching)
			{
				videoCapture.Stop();
				this.videoCapture = null;
				return;
			}

			var items = VideoCapture.Devices();
			if (items.Length == 0) throw new Exception("No Video Input Device found");

			if (this.comboBoxInputDevice.SelectedIndex < 0)
			{
				this.comboBoxInputDevice.SelectedIndex = 0;
			}


			var desiredItem = items.FirstOrDefault(i => i.Name == this.comboBoxInputDevice.SelectedItem.ToString());
			if (desiredItem == null) throw new Exception($"Camera not found");

			videoCapture = VideoCapture.WatchDevice(desiredItem.MonikerString);
			videoCapture.Start(OnFrame);
		}

		private bool IsProcessingBarcode = false;
		private bool IsFirstRun = true;

		private async void OnFrame(Bitmap frame)
		{
			if (!this.InvokeRequired) throw new Exception("We should not be on the UI thread");

			// set this.pictureBoxVideoCapture Size to match the frame
			if (IsFirstRun)
			{
				IsFirstRun = false;
				this.Invoke(() =>
				{
					this.pictureBoxVideoCapture.Size = frame.Size;
					// turn off any scaling on the picture box
					this.pictureBoxVideoCapture.SizeMode = PictureBoxSizeMode.Normal;
				});
			}

			var bitmapForUx = (Bitmap)frame.Clone();
			this.pictureBoxVideoCapture.Image?.Dispose();
			this.pictureBoxVideoCapture.Image = bitmapForUx;

			if (IsProcessingBarcode) return;
			try
			{
				IsProcessingBarcode = true;
				var bitmapForBarcode = (Bitmap)frame.Clone();
				var barcode = await barcodeDecoder.ImageAsBarcode(bitmapForBarcode);
				bitmapForBarcode.Dispose();
				if (!string.IsNullOrEmpty(barcode))
				{
					// display the barcode value, but we are not on the UI thread, so...
					this.Invoke((MethodInvoker)delegate
					{
						var priorBarcode = this.textBoxBarcode.Text;
						if (priorBarcode == barcode) return;
						this.textBoxBarcode.Text = barcode;
						PlaySuccess();
						var tableData = BarcodeDecoder.DecodeDriverLicense(barcode);
						// create a table control to hold this data
						ShowDriverInfoInTable(tableData);
					});
				}
			}
			finally
			{
				IsProcessingBarcode = false;
			}
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			var comboBoxInputDevice = this.comboBoxInputDevice;
			var items = VideoCapture.Devices();
			for (int i = 0; i < items.Count(); i++)
			{
				comboBoxInputDevice.Items.Add(items[i].Name);
			}
			if (comboBoxInputDevice.Items.Count > 0)
				comboBoxInputDevice.SelectedIndex = 0;
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