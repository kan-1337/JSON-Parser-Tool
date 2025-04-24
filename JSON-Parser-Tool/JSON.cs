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

            throw new Exception($"Unexpected character '{c}' at index {i}");
        }
        return result;
    }
}
