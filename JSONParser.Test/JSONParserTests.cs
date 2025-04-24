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

        [Fact]
        public void ParseJson_ArrayBoolTypes_BoolTypesResult()
        {
            var result = JSON.Parse("[true, false]");
            var value = Assert.IsType<object[]>(result);
            var b1 = Assert.IsType<bool>(value[0]);
            var b2 = Assert.IsType<bool>(value[1]);
            Assert.True(b1);
            Assert.False(b2);
        }

        [Fact]
        public void ParseJson_ArrayNumberTypes_NumberTypesResult()
        {
            var result = JSON.Parse("[1, 69, 420]");
            var value = Assert.IsType<object[]>(result);
            var b1 = Assert.IsType<int>(value[0]);
            var b2 = Assert.IsType<int>(value[1]);
            var b3 = Assert.IsType<int>(value[2]);
            Assert.Equal(1, b1);
            Assert.Equal(69, b2);
            Assert.Equal(420, b3);
        }
    }
}
