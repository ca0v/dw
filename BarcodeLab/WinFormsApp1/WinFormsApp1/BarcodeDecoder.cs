using IronBarCode;
using IronSoftware.Drawing;
using System.Drawing;

namespace WinFormsApp1
{

	internal class BarcodeDecoder
	{
		public static string ImageAsBarcode(System.Drawing.Image image)
		{
			var myOptionsExample = new BarcodeReaderOptions
			{
				Speed = ReadingSpeed.Balanced,
				ExpectMultipleBarcodes = false,
				ExpectBarcodeTypes = BarcodeEncoding.PDF417,
				MaxParallelThreads = 2,
				RemoveFalsePositive = false,
				CropArea = new CropRectangle(),
				UseCode39ExtendedMode = true
			};

			var results = BarcodeReader.Read(image, myOptionsExample).Values();
			return string.Join(',', results.ToArray());
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