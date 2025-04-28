using JSON_Parser_Tool;
using JSON_Parser_Tool.Parsing;

namespace JSONParser.Test
{
    public class JSONParserTests
    {
        [Fact]
        public void ParseJson_WithNull_NullResult()
        {
            var result = JsonParser.Parse("null");
            Assert.Null(result);
        }

        [Theory]
        [InlineData("true", true)]
        [InlineData("false", false)]
        public void ParseJson_WithBoolean_TrueAndFalseResult(string json, bool expected)
        {
            var result = JsonParser.Parse(json);
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
            var result = JsonParser.Parse(json);
            var value = Assert.IsType<int>(result);
            Assert.Equal(expected, value);
        }

        [Theory]
        [InlineData("\"one\"", "one")]
        [InlineData(" \" two \"", " two ")]
        [InlineData(" \" \\\" three \\\" \"", " \" three \" ")]
        public void ParseJson_WithStringNumber_StringNumberResult(string json, string expected)
        {
            var result = JsonParser.Parse(json);
            var value = Assert.IsType<string>(result);
            Assert.Equal(expected, value);
        }

        [Fact]
        public void ParseJson_WithArrayBoolTypes_BoolTypesResult()
        {
            var result = JsonParser.Parse("[true, false]");
            var value = Assert.IsType<object[]>(result);
            var b1 = Assert.IsType<bool>(value[0]);
            var b2 = Assert.IsType<bool>(value[1]);
            Assert.True(b1);
            Assert.False(b2);
        }

        [Fact]
        public void ParseJson_WithArrayNumberTypes_NumberTypesResult()
        {
            var result = JsonParser.Parse("[1, 69, 420]");
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
            var result = JsonParser.Parse("[ \"one\", \"two\", \"\\\"three\\\"\"]");
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
            var result = JsonParser.Parse("[ null, 1 , false, \"one\"]");
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
            var result = JsonParser.Parse("[ null, [1, false, \"one\"]]");
            var value = Assert.IsType<object[]>(result);
            Assert.Null(value[0]);

            value = Assert.IsType<object[]>(value[1]);

            Assert.Equal(1, Assert.IsType<int>(value[0]));
            Assert.False(Assert.IsType<bool>(value[1]));
            Assert.Equal("one", Assert.IsType<string>(value[2]));
        }

        [Fact]
        public void ParseJson_WitEmptyhObject_EmptyResult()
        {
            var result = JsonParser.Parse("{}");
            var value = Assert.IsType<Dictionary<string, object>>(result);
            Assert.Empty(value);
        }

        [Fact]
        public void ParseJson_WithFlatObject_ReturnsCorrectDictionary()
        {
            var json = """
                {
                    "key1": null,
                    "key2": true,
                    "key3": 420,
                    "key4": "Trump"
                }
                """;
            var result = JsonParser.Parse(json);
            var value = Assert.IsType<Dictionary<string, object>>(result);
            Assert.NotEmpty(value);
            Assert.Null(value["key1"]);
            Assert.True(Assert.IsType<bool>(value["key2"]));
            Assert.Equal(420, value["key3"]);
            Assert.Equal("Trump", value["key4"]);
        }

        [Fact]
        public void ParseJson_WithNestedObjectsAndArrays_ReturnsCorrectNestedStructure()
        {
            var json = """
                {
                    "key1": null,
                    "key2": {
                        "key3": [
                        {
                            "key4": "Falafel",
                            "key5": 69,
                            "key6": "Elon"
                        }],
                        "key7": true,
                        "key8": 420,
                        "key9": "Trump"
                    },
                }
                """;
            var result = JsonParser.Parse(json);

            var root = Assert.IsType<Dictionary<string, object>>(result);

            Assert.NotEmpty(root);
            Assert.Null(root["key1"]);

            var key2 = Assert.IsType<Dictionary<string, object>>(root["key2"]);

            Assert.True((bool)key2["key7"]);
            Assert.Equal(420, (int)key2["key8"]);
            Assert.Equal("Trump", (string)key2["key9"]);

            var key3 = Assert.IsType<object[]>(key2["key3"]);
            Assert.Single(key3);

            var firstItem = Assert.IsType<Dictionary<string, object>>(key3[0]);
            Assert.Equal("Falafel", firstItem["key4"]);
            Assert.Equal(69, firstItem["key5"]);
            Assert.Equal("Elon", firstItem["key6"]);
        }

        [Fact]
        public void ParseJson_WithIntegerValue_ReturnsCorrectInt()
        {
            var json = "42";
            var result = JsonParser.Parse(json);
            var value = Assert.IsType<int>(result);
            Assert.Equal(42, value);
        }

        [Fact]
        public void ParseJson_WithFloatValue_ReturnsCorrectDouble()
        {
            var json = "3.1415";
            var result = JsonParser.Parse(json);
            var value = Assert.IsType<double>(result);
            Assert.Equal(3.1415, value, 5); // 5 digits precision
        }

        [Fact]
        public void ParseJson_WithNegativeFloat_ReturnsCorrectDouble()
        {
            var json = "-0.99";
            var result = JsonParser.Parse(json);
            var value = Assert.IsType<double>(result);
            Assert.Equal(-0.99, value, 5);
        }

        [Fact]
        public void ParseJson_WithEscapedCharactersInString_ReturnsCorrectString()
        {
            var json = "\"Hello\\nWorld\\tTab\\\"Quote\\\"\"";
            var result = JsonParser.Parse(json);
            var value = Assert.IsType<string>(result);
            Assert.Equal("Hello\nWorld\tTab\"Quote\"", value);
        }

        [Fact]
        public void ParseJson_WithArrayContainingFloatsAndInts_ReturnsCorrectArray()
        {
            var json = "[1, 2.5, 3, 4.75]";
            var result = JsonParser.Parse(json);
            var array = Assert.IsType<object[]>(result);

            Assert.Equal(4, array.Length);
            Assert.Equal(1, array[0]);
            Assert.Equal(2.5, (double)array[1], 5);
            Assert.Equal(3, array[2]);
            Assert.Equal(4.75, (double)array[3], 5);
        }

        [Fact]
        public void ParseJson_WithNestedObjectWithFloatAndEscapedString_ReturnsCorrectStructure()
        {
            var json = """
            {
                "number": 123.456,
                "message": "Line1\nLine2",
                "valid": true
            }
            """;


            var result = JsonParser.Parse(json);
            var dict = Assert.IsType<Dictionary<string, object>>(result);

            Assert.True(dict.ContainsKey("number"));
            Assert.True(dict.ContainsKey("message"));
            Assert.True(dict.ContainsKey("valid"));

            Assert.Equal(123.456, (double)dict["number"], 5);
            Assert.Equal("Line1\nLine2", dict["message"]);
            Assert.Equal(true, dict["valid"]);
        }
    }
}