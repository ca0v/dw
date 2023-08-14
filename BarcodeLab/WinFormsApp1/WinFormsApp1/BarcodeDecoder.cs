using IronBarCode;
using IronSoftware.Drawing;
using System.Drawing;

namespace WinFormsApp1
{

	internal class BarcodeDecoder
	{
		public BarcodeReaderOptions BarcodeOptions { get; }

		public BarcodeDecoder()
		{
			// read the API key from app.config
			var apiKey = System.Configuration.ConfigurationManager.AppSettings["IronSoftware.Barcode.ApiKey"];
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

			var appsettings = new System.Configuration.AppSettingsReader();

			// if there exists barcode.expect-multiple-barcodes in app settings, use it
			barcodeOptions.ExpectMultipleBarcodes = TryGetValue(appsettings, "barcode.expect-multiple-barcodes", barcodeOptions.ExpectMultipleBarcodes);
			barcodeOptions.ExpectBarcodeTypes = StringToEnum<BarcodeEncoding>(TryGetValue(appsettings, "barcode.expect-barcode-types", barcodeOptions.ExpectBarcodeTypes.ToString()));
			barcodeOptions.Speed = StringToEnum<ReadingSpeed>(TryGetValue(appsettings, "barcode.speed", barcodeOptions.Speed.ToString()));
			barcodeOptions.MaxParallelThreads = TryGetValue(appsettings, "barcode.max-parallel-threads", barcodeOptions.MaxParallelThreads);
			barcodeOptions.RemoveFalsePositive = TryGetValue(appsettings, "barcode.remove-false-positive", barcodeOptions.RemoveFalsePositive);
			barcodeOptions.CropArea = new CropRectangle
			{
				X = TryGetValue(appsettings, "barcode.crop-area.top", barcodeOptions.CropArea.X),
				Y = TryGetValue(appsettings, "barcode.crop-area.left", barcodeOptions.CropArea.Y),
				Width = TryGetValue(appsettings, "barcode.crop-area.width", barcodeOptions.CropArea.Width),
				Height = TryGetValue(appsettings, "barcode.crop-area.height", barcodeOptions.CropArea.Height)
			};
			barcodeOptions.UseCode39ExtendedMode = TryGetValue(appsettings, "barcode.use-code39-extended-mode", barcodeOptions.UseCode39ExtendedMode);

		}

		private static T TryGetValue<T>(System.Configuration.AppSettingsReader appsettings, string key, T defaultValue)
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

		private static T StringToEnum<T>(string value)
		{
			return (T)System.Enum.Parse(typeof(T), value);
		}

		async public Task<string> ImageAsBarcode(System.Drawing.Image image)
		{
			var results = await BarcodeReader.ReadAsync(image, BarcodeOptions);
			if (results == null || !results.Any()) return "";
			var values = results.Values();
			return string.Join(',', values.ToArray());
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