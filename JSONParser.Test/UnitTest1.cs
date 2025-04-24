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

        [Theory]
        [InlineData("true", true)]
        [InlineData("false", false)]
        public void ParseJson_Boolean_TrueResult(string json, bool expected)
        {
            var result = JSON.Parse(json);
            var value = Assert.IsType<bool>(result);
            Assert.Equal(expected, value);
        }
    }
}
