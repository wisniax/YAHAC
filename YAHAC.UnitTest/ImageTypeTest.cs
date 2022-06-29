using YAHAC.Core.ApiInstances;

namespace YAHAC.UnitTest
{
    public class ImageTypeTest
    {
        [SetUp]
        public void Setup()
        {
        }


        [TestCaseSource(nameof(ItemPreparator))]
        public void ImageType_IsPngOrGifTest(AllItemsREPO.Item testItem)
        {
            var test = testItem.Texture.RawFormat.ToString();
            Assert.That(test, Is.EqualTo("Png") | Is.EqualTo("Gif"), "For item: " + testItem.id);
        }

        [TestCaseSource(nameof(ItemPreparator))]
        public void ImageTopLeftPixel_IsTransparentTest(AllItemsREPO.Item testItem)
        {
            System.Drawing.Bitmap workingBitmap = (System.Drawing.Bitmap)testItem.Texture;

            System.Drawing.Rectangle rect = new(0,0,workingBitmap.Width,workingBitmap.Height);

            System.Drawing.Imaging.BitmapData bitmapData = workingBitmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            int renderCalcSize = Math.Abs(bitmapData.Stride) * bitmapData.Height;

            byte[] bitmapPtr = new byte[renderCalcSize];

            System.Runtime.InteropServices.Marshal.Copy(bitmapData.Scan0, bitmapPtr, 0, renderCalcSize);
           
            Assert.That(bitmapPtr[0], Is.EqualTo(0), "For item: " + testItem.id + " alpha is not 0!!!");
        }

        public static List<AllItemsREPO.Item> ItemPreparator()
        {
            List<AllItemsREPO.Item> items = new();
            items.AddRange(AllItemsREPO.itemRepo.items);
            return items;
        }
    }
}