namespace YoJsonParser;

public static class YoJson
{
    public static object? Parse(string json)
    {
        return InternalParse(json, 0).Value;
    }

    private static (int Count, object? Value) InternalParse(string json, int position)
    {
        int i = position;
        while (i < json.Length)
        {
            var c = json[i];

            if (c == ' ')
            {
                i++;
                continue;
            }

            if (c == '{')
            {
                var (inc, obj) = ParseObject(json, i);
                return (i - position + inc, obj);
            }

            if (c == '[')
            {
                var (inc, arr) = ParseArray(json, i);
                return (i - position + inc, arr);
            }

            var (vInc, val) = ParseValue(json, i);
            return (i - position + vInc, val);
        }

        throw new Exception($"failed to parse json: {json}");
    }

    private static (int Count, Dictionary<string, object?> Value) ParseObject(
        string json,
        int position
    )
    {
        Dictionary<string, object?> result = new();
        int i = position + 1;
        while (i < json.Length)
        {
            var c = json[i];
            if (c is ' ' or ',' or '\n' or '\r')
            {
                i++;
                continue;
            }

            if (c is '}')
            {
                i++;
                break;
            }

            if (c is '"')
            {
                var (inc, key, val) = ParseObjectProperty(json, i);
                i += inc;
                result[key] = val;
                continue;
            }

            throw new Exception($"unknow token {c}");
        }
        return (i - position, result);
    }

    private static (int Count, string Key, object? Value) ParseObjectProperty(
        string json,
        int position
    )
    {
        int i = position;
        var (keyInc, key) = ParseString(json, i);
        i += keyInc;

        while (json[i] != ':')
        {
            i++;
        }

        i++;
        var (valueInc, value) = InternalParse(json, i);
        i += valueInc;

        return (i - position, key, value);
    }

    private static (int Count, object?[] Value) ParseArray(string json, int position)
    {
        List<object?> result = new();
        int i = position + 1;
        while (i < json.Length)
        {
            var c = json[i];

            if (c is ' ' or ',' or '\n' or '\r')
            {
                i++;
                continue;
            }

            if (c is ']')
            {
                i++;
                break;
            }

            var (inc, val) = InternalParse(json, i);
            result.Add(val);
            i += inc;
        }

        return (i - position, result.ToArray());
    }

    private static (int Count, object? Value) ParseValue(string json, int position)
    {
        int i = position;
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
                return (i - position + 4, null);
            }

            if (c == 't')
            {
                return (i - position + 4, true);
            }

            if (c == 'f')
            {
                return (i - position + 5, false);
            }

            if (c is '0' or '1' or '2' or '3' or '4' or '5' or '6' or '7' or '8' or '9')
            {
                var (inc, number) = ParseNumber(json, i);
                return (i - position + inc, number);
            }

            if (c is '"')
            {
                var (inc, str) = ParseString(json, i);
                return (i - position + inc, str);
            }

            throw new Exception("unknow character");
        }

        throw new Exception("unknow character");
    }

    private static (int Count, int Result) ParseNumber(string json, int position)
    {
        string result = "";
        for (int i = position; i < json.Length; i++)
        {
            var c = json[i];
            if (c is '0' or '1' or '2' or '3' or '4' or '5' or '6' or '7' or '8' or '9')
            {
                result += c;
            }
            else
            {
                break;
            }
        }

        return (result.Length, int.Parse(result));
    }

    private static (int Count, string Result) ParseString(string json, int position)
    {
        string result = "";
        int i = position + 1;
        while (i < json.Length)
        {
            var c = json[i];
            if (c is '"')
            {
                i++;
                break;
            }

            if (c is '\\')
            {
                i++;
                result += json[i];
            }
            else
            {
                result += c;
            }

            i++;
        }

        return (i - position, result);
    }
}
