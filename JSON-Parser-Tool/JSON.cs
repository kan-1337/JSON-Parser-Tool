using System.Text;

namespace JSON_Parser_Tool;
public class JSON
{
    public static object? Parse(string json)
    {
        return InternalParse(json, 0).Result;
    }

    public static ParsedValue ParseValue(string json, int position)
    {
        object result = null;
        var i = position;
        while (i < json.Length)
        {
            var c = json[i];

            if (c == ' ')
            {
                i++;
                break;
            }

            if (c == 'n')
            {
                return new ParsedValue(4, null);
            }

            if (c == 't')
            {
                return new ParsedValue(4, true);
            }

            if (c == 'f')
            {
                return new ParsedValue(5, false);
            }

            if (c is '0' or '1' or '2' or '3' or '4' or '5' or '6' or '7' or '8' or '9')
            {
                var parsedNumber = ParseNumber(json, i);
                return new ParsedValue(parsedNumber.Count, parsedNumber.Result);
            }

            if (c == '"')
            {
                var parsedString = ParseString(json, i);
                return new ParsedValue(parsedString.Count, parsedString.Result);
            }

            throw new Exception($"Unexpected character '{c}' at index {i}");
        }

        if (result is null)
        {
            return new ParsedValue(i, null);
        }

        return new ParsedValue(i - position, result);
    }

    private static JsonIntResult ParseNumber(string json, int position)
    {
        StringBuilder result = new StringBuilder();

        for (int i = position; i < json.Length; i++)
        {
            var c = json[i];
            if (c is '0' or '1' or '2' or '3' or '4' or '5' or '6' or '7' or '8' or '9')
            {
                result.Append(c);
            }
            else
            {
                break;
            }
        }

        var isParsed = int.TryParse(result.ToString(), out var number);
        if (!isParsed)
        {
            throw new Exception($"Unable to parse number at position {position}");
        }
        return new JsonIntResult(result.Length, number);
    }

    private static JsonArrayResult ParseArray(string json, int position)
    {
        List<object?> result = new List<object?>();
        var i = position + 1; // Skip the opening bracket
        while (i < json.Length)
        {
            var c = json[i];

            if (c is ' ' or ',')
            {
                i++;
                continue;
            }

            if (c == ']') // Closing bracket indicates end of array
            {
                i++;
                break;
            }

            var internalParse = InternalParse(json, i);
            result.Add(internalParse.Result);
            i += internalParse.Count;
        }

        return new JsonArrayResult(i - position, result.ToArray());
    }

    private static JsonStringResult ParseString(string json, int position)
    {
        StringBuilder result = new StringBuilder();
        var i = position + 1; // Skip the opening quote
        while (i < json.Length)
        {
            var c = json[i];
            if (c == '"')
            {
                i++;
                break;
            }
            if(c is '\\')
            {
                i++;
                result.Append(json[i]);
            }
            else
            {
                result.Append(c);
            }
            i++;
        }
        return new JsonStringResult(i - position, result.ToString());
    }

    private static JsonInternalResult InternalParse(string json, int position)
    {
        var i = position;
        while (i < json.Length)
        {
            var c = json[i];

            if (c == ' ')
            {
                i++;
                continue;
            }

            if (c == '[')
            {
                var result = ParseArray(json, i);
                return new JsonInternalResult(result.Count, result.Result);
            }
            else
            {
                var parsedValue = ParseValue(json, i);
                return new JsonInternalResult(parsedValue.Count, parsedValue.Result);
            }
        }
        throw new Exception($"Invalid JSON \n {json}");
    }
}

public record JsonIntResult(int Count, int Result);
public record JsonInternalResult(int Count, object? Result);
public record JsonArrayResult(int Count, object[] Result);
public record JsonStringResult(int Count, string Result);

public record struct ParsedValue(int Count, object? Result);