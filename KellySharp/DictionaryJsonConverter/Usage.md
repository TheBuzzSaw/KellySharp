# Dictionary JSON Converter

In transitioning from `Newtonsoft.Json` to `System.Text.Json`, users lost the ability convert dictionaries with non-`string` keys. This little library is intended to restore some of that functionality. It works with the following dictionary types:

- `IDictionary<TKey, TValue>`
- `Dictionary<TKey, TValue>`
- `SortedDictionary<TKey, TValue>`
- `ImmutableDictionary<TKey, TValue>`
- `ImmutableSortedDictionary<TKey, TValue>`

By default, it supports converting the following `TKey` types:

- Integer types: `sbyte`, `short`, `int`, `long`, `BigInteger`
- Unsigned integer types: `byte`, `ushort`, `uint`, `ulong`
- Floating point types: `float`, `double`, `decimal`
- `Guid`

Support for more `TKey` types can be added by the user.

## Simple Usage

To understand the problem itself, consider the following code.

    var raw = "{\"49fc2162-744a-4a42-b685-ea1e30ce2a2f\": 99}";
    var dictionary = JsonSerializer.Deserialize<Dictionary<Guid, int>>(raw);

This will throw a `NotSupportedException` in response to the fact that the dictionary specifies `Guid` for its `TKey`. This generally makes sense as JSON objects can only ever have strings for keys. However, it places unnecessary burden on the user to convert everything in two steps: first to string keys, and then parsed to some other type.

    var options = new JsonSerializerOptions();
    options.Converters.Add(DictionaryJsonConverterFactory.Default);
    var raw = "{\"49fc2162-744a-4a42-b685-ea1e30ce2a2f\": 99}";
    // Remember to add the options!
    var dictionary = JsonSerializer.Deserialize<Dictionary<Guid, int>>(raw, options);

The dictionary now deserializes successfully.
