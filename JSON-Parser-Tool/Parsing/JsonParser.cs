using JSON_Parser_Tool.Exceptions;
using JSON_Parser_Tool.Models;
using System.Globalization;
using System.Text;

namespace JSON_Parser_Tool
{
    public class JsonParser
    {
        public static object? Parse(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                throw new JsonParseException("Input JSON cannot be null or empty.");

            var parsed = ParseElement(json, 0);
            return parsed.Result;
        }

        private static JsonInternalResult ParseElement(string json, int position)
        {
            int i = position;

            while (i < json.Length)
            {
                char c = json[i];

                if (char.IsWhiteSpace(c))
                {
                    i++;
                    continue;
                }

                return c switch
                {
                    '{' => ParseObject(json, i, position),
                    '[' => ParseArray(json, i, position),
                    _ => ParseSimpleValue(json, i, position)
                };
            }

            throw new JsonParseException($"Invalid JSON starting at position {position}.");
        }

        private static JsonInternalResult ParseSimpleValue(string json, int i, int originalPosition)
        {
            char c = json[i];

            if (c == 'n' && json.Substring(i, 4) == "null")
                return new JsonInternalResult(4, null);

            if (c == 't' && json.Substring(i, 4) == "true")
                return new JsonInternalResult(4, true);

            if (c == 'f' && json.Substring(i, 5) == "false")
                return new JsonInternalResult(5, false);

            if (char.IsDigit(c) || c == '-')
                return ParseNumber(json, i, originalPosition);

            if (c == '"')
            {
                var parsedString = ParseString(json, i, originalPosition);
                return new JsonInternalResult(parsedString.Count, parsedString.Result);
            }

            throw new JsonParseException($"Unexpected character '{c}' at position {i}.");
        }

        private static JsonInternalResult ParseNumber(string json, int i, int originalPosition)
        {
            int start = i;
            var sb = new StringBuilder();

            if (json[i] == '-')
            {
                sb.Append(json[i]);
                i++;
            }

            bool hasDecimal = false;

            while (i < json.Length)
            {
                if (char.IsDigit(json[i]))
                {
                    sb.Append(json[i]);
                }
                else if (json[i] == '.' && !hasDecimal)
                {
                    hasDecimal = true;
                    sb.Append(json[i]);
                }
                else
                {
                    break;
                }
                i++;
            }

            var numberStr = sb.ToString();

            if (hasDecimal)
            {
                if (double.TryParse(numberStr, NumberStyles.Float, CultureInfo.InvariantCulture, out var floatResult))
                    return new JsonInternalResult(i - originalPosition, floatResult);

                throw new JsonParseException($"Invalid float format at {start}");
            }
            else
            {
                if (int.TryParse(numberStr, out var intResult))
                    return new JsonInternalResult(i - originalPosition, intResult);

                throw new JsonParseException($"Invalid integer format at {start}");
            }
        }

        private static JsonStringResult ParseString(string json, int position, int basePosition)
        {
            if (json[position] != '"')
                throw new JsonParseException($"Expected '\"' at position {position} but found '{json[position]}'");

            StringBuilder result = new StringBuilder();
            int i = position + 1; // Skip the opening quote

            while (i < json.Length)
            {
                char c = json[i];

                if (c == '"')
                {
                    i++; // Move past the closing quote
                    break;
                }

                if (c == '\\')
                {
                    i++;
                    if (i >= json.Length)
                        throw new JsonParseException("Unexpected end of input after escape character.");

                    char next = json[i];
                    switch (next)
                    {
                        case '\\': result.Append('\\'); break;
                        case '"': result.Append('"'); break;
                        case 'n': result.Append('\n'); break;
                        case 't': result.Append('\t'); break;
                        case 'r': result.Append('\r'); break;
                        case 'b': result.Append('\b'); break;
                        case 'f': result.Append('\f'); break;
                        default:
                            throw new JsonParseException($"Unsupported escape sequence: \\{next}");
                    }
                }
                else
                {
                    result.Append(c);
                }

                i++;
            }

            return new JsonStringResult(i - basePosition, result.ToString());
        }

        private static JsonInternalResult ParseArray(string json, int i, int originalPosition)
        {
            List<object?> list = new();
            i++; // Skip '['

            while (i < json.Length)
            {
                while (i < json.Length && (char.IsWhiteSpace(json[i]) || json[i] == ',' || json[i] == '\n' || json[i] == '\r'))
                {
                    i++;
                }

                if (i < json.Length && json[i] == ']')
                {
                    i++;
                    break;
                }

                var element = ParseElement(json, i);
                list.Add(element.Result);
                i += element.Count;
            }

            return new JsonInternalResult(i - originalPosition, list.ToArray());
        }

        private static JsonInternalResult ParseObject(string json, int i, int originalPosition)
        {
            Dictionary<string, object?> dict = new();
            i++; // Skip '{'

            while (i < json.Length)
            {
                while (i < json.Length && (char.IsWhiteSpace(json[i]) || json[i] == ',' || json[i] == '\n' || json[i] == '\r'))
                {
                    i++;
                }

                if (i < json.Length && json[i] == '}')
                {
                    i++;
                    break;
                }

                if (json[i] != '"')
                    throw new JsonParseException($"Expected '\"' at position {i} inside object.");

                var keyParsed = ParseString(json, i, i);
                var key = (string)keyParsed.Result!;
                i += keyParsed.Count;

                while (i < json.Length && json[i] != ':')
                {
                    i++;
                }

                if (i >= json.Length)
                    throw new JsonParseException($"Unexpected end of object after key '{key}'.");

                i++; // skip ':'

                while (i < json.Length && char.IsWhiteSpace(json[i]))
                {
                    i++;
                }

                var valueParsed = ParseElement(json, i);
                dict[key] = valueParsed.Result;
                i += valueParsed.Count;
            }

            return new JsonInternalResult(i - originalPosition, dict);
        }
    }
}
