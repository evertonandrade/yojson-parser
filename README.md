# YoJson: A simple JSON parser
The lib that parsing a json string to an object in C#

## Example
A simple example on how to parse JSON from a string literal.

```csharp
const string json = """{ "number": 42, "string": "yes", "list": ["for", "sure", 42] }""";
var obj = YoJson.Parse(json);
```
