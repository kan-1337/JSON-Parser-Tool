using System.Text;

namespace JSON_Parser_Tool;
public class JSON
{
    public static object? Parse(string json)
    {
        object result = null;
        for (int i = 0; i < json.Length;)
        {
            var c = json[i];
            if (c == 'n')
            {
                result = null;
                i += 4;
                continue;
            }

            if (c == 't')
            {
                result = true;
                i += 4;
                continue;
            }

            if (c == 'f')
            {
                result = false;
                i += 5;
                continue;
            }

            if (c == ' ')
            {
                i++;
                continue;
            }

            if (c is '0' or '1' or '2' or '3' or '4' or '5' or '6' or '7' or '8' or '9')
            {
                var parsedNumber = ParseNumber(json, i);
                result = parsedNumber.Result;
                i += parsedNumber.Count;
                continue;
            }

            if (c == '"')
            {
                var stringResult = ParseString(json, i);
                result = stringResult.Result;
                i += stringResult.Count;
                continue;
            }

            throw new Exception($"Unexpected character '{c}' at index {i}");
        }
        return result;
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
}

public record JsonIntResult(int Count, int Result);
public record JsonStringResult(int Count, string Result);
