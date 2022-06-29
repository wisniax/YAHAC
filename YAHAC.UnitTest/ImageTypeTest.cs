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
        public void ImageTypeTest_EqualTest(AllItemsREPO.Item testItem)
        {
            var test = testItem.Texture.RawFormat.ToString();
            Assert.That(test, Is.EqualTo("Png") | Is.EqualTo("Gif"), "For item: " + testItem.id);
        }

        public static List<AllItemsREPO.Item> ItemPreparator()
        {
            List<AllItemsREPO.Item> items = new();
            items.AddRange(AllItemsREPO.itemRepo.items);
            return items;
        }
    }
}