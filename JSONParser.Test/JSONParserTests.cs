using JSON_Parser_Tool;

namespace JSONParser.Test
{
    public class JSONParserTests
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
        public void ParseJson_Boolean_TrueAndFalseResult(string json, bool expected)
        {
            var result = JSON.Parse(json);
            var value = Assert.IsType<bool>(result);
            Assert.Equal(expected, value);
        }

        [Theory]
        [InlineData("0", 0)]
        [InlineData("1", 1)]
        [InlineData("10", 10)]
        [InlineData("42", 42)]
        [InlineData("69", 69)]
        [InlineData("420", 420)]
        public void ParseJson_IntNumber_IntNumberResult(string json, int expected)
        {
            var result = JSON.Parse(json);
            var value = Assert.IsType<int>(result);
            Assert.Equal(expected, value);
        }

        [Theory]
        [InlineData("\"one\"", "one")]
        [InlineData(" \" two \"", " two ")]
        [InlineData(" \" \\\" three \\\" \"", " \" three \" ")]
        public void ParseJson_StringNumber_StringNumberResult(string json, string expected)
        {
            var result = JSON.Parse(json);
            var value = Assert.IsType<string>(result);
            Assert.Equal(expected, value);
        }
    }
}
