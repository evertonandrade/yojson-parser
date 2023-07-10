using Xunit;

namespace YoJsonParser.Tests;

public class YoJsonTests
{
    [Fact]
    public void ParseNull()
    {
        var result = YoJson.Parse("null");

        Assert.Null(result);
    }

    [Theory]
    [InlineData("true", true)]
    [InlineData("false", false)]
    public void ParseBool(string json, bool expected)
    {
        var result = YoJson.Parse(json);

        var value = Assert.IsType<bool>(result);
        Assert.Equal(expected, value);
    }

    [Theory]
    [InlineData(" 0", 0)]
    [InlineData("1", 1)]
    [InlineData("10", 10)]
    [InlineData("69", 69)]
    [InlineData("256", 256)]
    public void ParseNumbers(string json, int expected)
    {
        var result = YoJson.Parse(json);

        var value = Assert.IsType<int>(result);
        Assert.Equal(expected, value);
    }

    [Theory]
    [InlineData(" \"one\"", "one")]
    [InlineData(" \"two\"", "two")]
    [InlineData("\" \\\"three\\\" \"", " \"three\" ")]
    public void ParseString(string json, string expected)
    {
        var result = YoJson.Parse(json);

        var value = Assert.IsType<string>(result);
        Assert.Equal(expected, value);
    }

    [Fact]
    public void ParseArrayOfBool()
    {
        var result = YoJson.Parse("[true, false]");

        var value = Assert.IsType<object[]>(result);
        var b1 = Assert.IsType<bool>(value[0]);
        var b2 = Assert.IsType<bool>(value[1]);
        Assert.True(b1);
        Assert.False(b2);
    }

    [Fact]
    public void ParseArrayOfNumber()
    {
        var result = YoJson.Parse("[1, 69, 420]");

        var value = Assert.IsType<object[]>(result);
        var n1 = Assert.IsType<int>(value[0]);
        var n2 = Assert.IsType<int>(value[1]);
        var n3 = Assert.IsType<int>(value[2]);
        Assert.Equal(1, n1);
        Assert.Equal(69, n2);
        Assert.Equal(420, n3);
    }

    [Fact]
    public void ParseArrayOfString()
    {
        var result = YoJson.Parse("[\"one\", \"two\", \"\\\"three\\\"\"]");

        var value = Assert.IsType<object[]>(result);
        var s1 = Assert.IsType<string>(value[0]);
        var s2 = Assert.IsType<string>(value[1]);
        var s3 = Assert.IsType<string>(value[2]);
        Assert.Equal("one", s1);
        Assert.Equal("two", s2);
        Assert.Equal("\"three\"", s3);
    }

    [Fact]
    public void ParseArrayOfDynamic()
    {
        var result = YoJson.Parse("[null, false, 1, \"one\"]");

        var value = Assert.IsType<object[]>(result);
        var n = value[0];
        var b = Assert.IsType<bool>(value[1]);
        var i = Assert.IsType<int>(value[2]);
        var s = Assert.IsType<string>(value[3]);
        Assert.Null(n);
        Assert.False(b);
        Assert.Equal(1, i);
        Assert.Equal("one", s);
    }

    [Fact]
    public void ParseArrayNested()
    {
        var result = YoJson.Parse("[null [false, 1, \"one\"]]");

        var value = Assert.IsType<object[]>(result);
        var n = value[0];
        Assert.Null(n);

        var nestedValue = Assert.IsType<object[]>(value[1]);
        var b = Assert.IsType<bool>(nestedValue[0]);
        var i = Assert.IsType<int>(nestedValue[1]);
        var s = Assert.IsType<string>(nestedValue[2]);
        Assert.False(b);
        Assert.Equal(1, i);
        Assert.Equal("one", s);
    }

    [Fact]
    public void ParseEmptyObject()
    {
        var result = YoJson.Parse("{}");

        var value = Assert.IsType<Dictionary<string, object>>(result);
        Assert.Empty(value);
    }

    [Fact]
    public void ParseFlatObject()
    {
        const string json = """
        {
            "a": null,
            "b": true,
            "c": 360,
            "d": "no scope"
        }    
        """;
        var result = YoJson.Parse(json);

        var value = Assert.IsType<Dictionary<string, object>>(result);
        Assert.NotEmpty(value);
        Assert.Null(value["a"]);

        var b = Assert.IsType<bool>(value["b"]);
        Assert.True(b);

        var n = Assert.IsType<int>(value["c"]);
        Assert.Equal(360, n);

        var s = Assert.IsType<string>(value["d"]);
        Assert.Equal("no scope", s);
    }

    [Fact]
    public void ParseNestedObject()
    {
        const string json = """
        {
            "a": null,
            "b": {
                "b": true,
                "c": 360,
                "d": "no scope"
            }
        }    
        """;
        var result = YoJson.Parse(json);

        var value = Assert.IsType<Dictionary<string, object>>(result);
        Assert.NotEmpty(value);
        Assert.Null(value["a"]);

        var nestedValue = Assert.IsType<Dictionary<string, object>>(value["b"]);

        var b = Assert.IsType<bool>(nestedValue["b"]);
        Assert.True(b);

        var n = Assert.IsType<int>(nestedValue["c"]);
        Assert.Equal(360, n);

        var s = Assert.IsType<string>(nestedValue["d"]);
        Assert.Equal("no scope", s);
    }

    [Fact]
    public void ParseNestedObjectsAndArrays()
    {
        const string json = """
        {
            "a": null,
            "b": {
                "b": [
                        {
                            "b": true,
                            "c": 360,
                            "d": "no scope"
                        }
                    ],
                "c": 360,
                "d": "no scope"
            }
        }   
        """;
        var result = YoJson.Parse(json);

        var value = Assert.IsType<Dictionary<string, object>>(result);
        Assert.NotEmpty(value);
        Assert.Null(value["a"]);

        value = Assert.IsType<Dictionary<string, object>>(value["b"]);
        Assert.Equal(360, value["c"]);
        Assert.Equal("no scope", value["d"]);

        var arr = Assert.IsType<object[]>(value["b"]);
        value = Assert.IsType<Dictionary<string, object>>(arr[0]);

        var b = Assert.IsType<bool>(value["b"]);
        Assert.True(b);

        Assert.Equal(360, value["c"]);

        Assert.Equal("no scope", value["d"]);
    }
}
