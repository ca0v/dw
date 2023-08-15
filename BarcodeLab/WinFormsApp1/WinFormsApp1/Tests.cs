using System.Drawing;

namespace WinFormsApp1.Tests
{
    public class BarcodeDecoderTests
    {
        async public void TestImageAsBarcode()
        {
            Form1.PlaySuccess();

            var photo = Bitmap.FromFile(@"C:\Users\calix\Pictures\barcode_pdf417\sc_dl.png") as Bitmap;
            var barcode = await new BarcodeDecoder().ImageAsBarcode(photo);
            if (string.IsNullOrEmpty(barcode))
            {
                throw new System.Exception("Barcode not found");
            }
        }
    }
}
