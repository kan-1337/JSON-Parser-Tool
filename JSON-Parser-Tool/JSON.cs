namespace JSON_Parser_Tool;
public class JSON
{
    public static object? Parse(string json)
    {
        object result = null;
        for (int i = 0; i < json.Length; i++)
        {
            var c = json[i];
            if (c == 'n')
            {
                result = null;
                i += 4;
                continue;
            }
        }
        return result;
    }
}
