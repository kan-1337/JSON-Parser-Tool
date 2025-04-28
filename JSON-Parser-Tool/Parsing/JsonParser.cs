using JSON_Parser_Tool.Exceptions;
using JSON_Parser_Tool.Models;
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
                return ParseString(json, i, originalPosition);

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

            while (i < json.Length && char.IsDigit(json[i]))
            {
                sb.Append(json[i]);
                i++;
            }

            if (!int.TryParse(sb.ToString(), out var number))
                throw new JsonParseException($"Invalid number format starting at {start}.");

            return new JsonInternalResult(i - originalPosition, number);
        }

        private static JsonInternalResult ParseString(string json, int i, int originalPosition)
        {
            i++; // Skip the opening quote
            var sb = new StringBuilder();

            while (i < json.Length)
            {
                if (json[i] == '"')
                {
                    i++; // skip closing quote
                    break;
                }
                if (json[i] == '\\')
                {
                    i++;
                    if (i < json.Length)
                        sb.Append(json[i]);
                }
                else
                {
                    sb.Append(json[i]);
                }
                i++;
            }

            return new JsonInternalResult(i - originalPosition, sb.ToString());
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
