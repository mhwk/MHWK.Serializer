# StructSerializer

POC for deserializing readonly structs from json through code generation.


## Example

```c#
public readonly struct Foo {
    public readonly string Bar;
    
    public Foo(string bar) {
        Bar = bar;
    }
}

public static class Program {
    public static void Main() {
        var foo = FooSerializer.Instance.Deserialize(
            @"{""Bar"": ""Baz""}"
        );
        
        // foo.Bar == "Baz"
    }
}
```