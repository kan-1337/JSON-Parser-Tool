using JSON_Parser_Tool;

namespace JSONParser.Test
{
    public class JSONParserTests
    {
        [Fact]
        public void ParseJson_WithNull_NullResult()
        {
            var result = JSON.Parse("null");
            Assert.Null(result);
        }

        [Theory]
        [InlineData("true", true)]
        [InlineData("false", false)]
        public void ParseJson_WithBoolean_TrueAndFalseResult(string json, bool expected)
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
        public void ParseJson_WithIntNumber_IntNumberResult(string json, int expected)
        {
            var result = JSON.Parse(json);
            var value = Assert.IsType<int>(result);
            Assert.Equal(expected, value);
        }

        [Theory]
        [InlineData("\"one\"", "one")]
        [InlineData(" \" two \"", " two ")]
        [InlineData(" \" \\\" three \\\" \"", " \" three \" ")]
        public void ParseJson_WithStringNumber_StringNumberResult(string json, string expected)
        {
            var result = JSON.Parse(json);
            var value = Assert.IsType<string>(result);
            Assert.Equal(expected, value);
        }

        [Fact]
        public void ParseJson_WithArrayBoolTypes_BoolTypesResult()
        {
            var result = JSON.Parse("[true, false]");
            var value = Assert.IsType<object[]>(result);
            var b1 = Assert.IsType<bool>(value[0]);
            var b2 = Assert.IsType<bool>(value[1]);
            Assert.True(b1);
            Assert.False(b2);
        }

        [Fact]
        public void ParseJson_WithArrayNumberTypes_NumberTypesResult()
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

        [Fact]
        public void ParseJson_ArrayStringTypes_StringTypesResult()
        {
            var result = JSON.Parse("[ \"one\", \"two\", \"\\\"three\\\"\"]");
            var value = Assert.IsType<object[]>(result);
            var b1 = Assert.IsType<string>(value[0]);
            var b2 = Assert.IsType<string>(value[1]);
            var b3 = Assert.IsType<string>(value[2]);
            Assert.Equal("one", b1);
            Assert.Equal("two", b2);
            Assert.Equal("\"three\"", b3);
        }

        [Fact]
        public void ParseJson_WithArrayTypes_ArrayTypesResult()
        {
            var result = JSON.Parse("[ null, 1 , false, \"one\"]");
            var value = Assert.IsType<object[]>(result);
            var b1 = Assert.IsType<int>(value[1]);
            var b2 = Assert.IsType<bool>(value[2]);
            var b3 = Assert.IsType<string>(value[3]);
            Assert.Null(value[0]);
            Assert.Equal(1, b1);
            Assert.False(b2);
            Assert.Equal("one", b3);
        }

        [Fact]
        public void ParseJson_WithNestedTypes_NestedResult()
        {
            var result = JSON.Parse("[ null, [1, false, \"one\"]]");
            var value = Assert.IsType<object[]>(result);
            Assert.Null(value[0]);

            value = Assert.IsType<object[]>(value[1]);

            Assert.Equal(1, Assert.IsType<int>(value[0]));
            Assert.False(Assert.IsType<bool>(value[1]));
            Assert.Equal("one", Assert.IsType<string>(value[2]));
        }
    }
}
