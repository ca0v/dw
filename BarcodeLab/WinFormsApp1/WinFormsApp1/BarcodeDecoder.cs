using IronBarCode;
using IronSoftware.Drawing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace WinFormsApp1
{

	internal class Configuration
	{
		private static System.Configuration.AppSettingsReader appsettings = new System.Configuration.AppSettingsReader();
		internal static string IronSoftwareBarcodeApiKey { get; } = TryGetValue("IronSoftware.Barcode.ApiKey", "");

		internal static T TryGetValue<T>(string key, T defaultValue)
		{
			try
			{
				return (T)appsettings.GetValue(key, typeof(T));
			}
			catch
			{
				return defaultValue;
			}
		}

	}

	internal class BarcodeDecoder
	{
		public BarcodeReaderOptions BarcodeOptions { get; }

		public BarcodeDecoder()
		{
			// read the API key from app.config
			var apiKey = Configuration.IronSoftwareBarcodeApiKey;
			if (string.IsNullOrEmpty(apiKey))
			{
				throw new System.Exception("IronSoftware.Barcode.ApiKey is not set in app.config");
			}
			// set the API key
			License.LicenseKey = apiKey;

			// if not licensed, report to user
			if (!License.IsLicensed)
			{
				throw new System.Exception("IronSoftware.Barcode.ApiKey is not valid");
			}

			var barcodeOptions = this.BarcodeOptions = new BarcodeReaderOptions
			{
				Speed = ReadingSpeed.Balanced,
				ExpectMultipleBarcodes = false,
				ExpectBarcodeTypes = BarcodeEncoding.PDF417,
				MaxParallelThreads = 4,
				RemoveFalsePositive = false,
				CropArea = new CropRectangle(),
				UseCode39ExtendedMode = true
			};

			// if there exists barcode.expect-multiple-barcodes in app settings, use it
			barcodeOptions.ExpectMultipleBarcodes = Configuration.TryGetValue("barcode.expect-multiple-barcodes", barcodeOptions.ExpectMultipleBarcodes);
			barcodeOptions.ExpectBarcodeTypes = StringToEnum<BarcodeEncoding>(Configuration.TryGetValue("barcode.expect-barcode-types", barcodeOptions.ExpectBarcodeTypes.ToString()));
			barcodeOptions.Speed = StringToEnum<ReadingSpeed>(Configuration.TryGetValue("barcode.speed", barcodeOptions.Speed.ToString()));
			barcodeOptions.MaxParallelThreads = Configuration.TryGetValue("barcode.max-parallel-threads", barcodeOptions.MaxParallelThreads);
			barcodeOptions.RemoveFalsePositive = Configuration.TryGetValue("barcode.remove-false-positive", barcodeOptions.RemoveFalsePositive);
			barcodeOptions.CropArea = new CropRectangle
			{
				X = Configuration.TryGetValue("barcode.crop-area.top", barcodeOptions.CropArea.X),
				Y = Configuration.TryGetValue("barcode.crop-area.left", barcodeOptions.CropArea.Y),
				Width = Configuration.TryGetValue("barcode.crop-area.width", barcodeOptions.CropArea.Width),
				Height = Configuration.TryGetValue("barcode.crop-area.height", barcodeOptions.CropArea.Height)
			};
			barcodeOptions.UseCode39ExtendedMode = Configuration.TryGetValue("barcode.use-code39-extended-mode", barcodeOptions.UseCode39ExtendedMode);

		}

		private static T StringToEnum<T>(string value)
		{
			return (T)System.Enum.Parse(typeof(T), value);
		}

		async public Task<string> ImageAsBarcode(Bitmap image)
		{
			// convert the bitmap to a byte array
			byte[] imageAsBytes = ImageToByteArray(image);
			var results = await BarcodeReader.ReadAsync(imageAsBytes, BarcodeOptions);
			if (results == null || !results.Any()) return "";
			var values = results.Values();
			return string.Join(",", values.ToArray());
		}

		private byte[] ImageToByteArray(Bitmap image)
		{
			using (var stream = new System.IO.MemoryStream())
			{
				image.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
				return stream.ToArray();
			}
		}

		public static Dictionary<string, string> DecodeDriverLicense(string barcode)
		{
			return DecodeAamvaBarcode(barcode);
		}

		private static Dictionary<string, string> DecodeAamvaBarcode(string barcode)
		{
			// from https://www.aamva.org/getmedia/99ac7057-0f4d-4461-b0a2-3a5532e1b35c/AAMVA-2020-DLID-Card-Design-Standard.pdf
			var keyCodes = new Dictionary<string, string>
			{
				{ "DCA", "Vehicle Class"},
				{ "DCB", "Restriction Codes"},
				{ "DCD", "Endorsement Codes"},
				{ "DBA", "Expiration Date"},
				{ "DCS", "Family Name" },
				{ "DAC", "First Name"},
				{ "DAD", "Middle Name(s)"},
				{ "DBD", "Issue Date"},
				{ "DBB", "Date of Birth"},
				{ "DBC", "Sex"},
				{ "DAY", "Eye Color"},
				{ "DAU", "Height"},
				{ "DAG", "Address – Street 1"},
				{ "DAI", "Address – City"},
				{ "DAJ", "Address – Jurisdiction Code"},
				{ "DAK", "Address – Postal Code"},
				{ "DAQ", "Customer ID Number"},
				{ "DCF", "Document Discriminator"},
				{ "DCG", "Country Identification"},
				{ "DDE", "Family name truncation"},
				{ "DDF", "First name truncation"},
				{ "DDG", "Middle name truncation"},
				{"DAH", "Address – Street 2"},
				{"DAZ", "Hair Color"},
				{"DCI", "Place of birth"},
				{"DCJ", "Audit information"},
				{"DCK", "Inventory control number"},
				{"DAW", "Weight (pounds)"},
				// etc.
			};

			var result = new Dictionary<string, string>();
			var lines = barcode.Split('\n');
			foreach (var line in lines)
			{
				if (line.Length < 3) continue;
				var keyCode = line.Substring(0, 3);
				if (keyCodes.ContainsKey(keyCode))
				{
					result.Add(keyCodes[keyCode], line.Substring(3));
				}
			}
			return result;
		}
	}
}