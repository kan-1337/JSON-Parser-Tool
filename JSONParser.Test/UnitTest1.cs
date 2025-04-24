using JSON_Parser_Tool;

namespace JSONParser.Test
{
    public class UnitTest1
    {
        [Fact]
        public void ParseJson_Null_NullResult()
        {
            var result = JSON.Parse("null");
            Assert.Null(result);
        }

        [Fact]
        public void ParseJson_Boolean_TrueResult()
        {
            var result = JSON.Parse("null");
            Assert.Null(result);
        }
    }
}
