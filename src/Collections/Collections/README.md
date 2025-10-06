# Intercode.Toolbox.Collections

A trimmable, AOT-compatible .NET library that provides classes that define generic collections and collection-related utilities that are not found in the BCL.

---

## `MutableLookup` collection classes
The mutable lookup collections provide a multi-value dictionary-like interface that allows for the storage of multiple values for a single key.
Note that these collections are not thread-safe and should not be used in multi-threaded scenarios without proper synchronization.

The classes were inspired by the similarly named classes in the [MutableLookup](https://github.com/jussik/MutableLookup/) package.

---

### `MutableLookup<TKey, TValue>`

The `MutableLookup` abstract class provides the base functionality for the mutable lookup collections. A custom mutable lookup class that 
must derive from it and implement the following abstract methods:

- `AddValue`: Adds a value to the underlying container; must return `true` if the value was added, `false` otherwise.
- `AddValues`: Adds a collection of values to the underlying container.
- `TrimExcess`: Sets the capacity of the underlying conntainer to what it would be if it had been originally initialized with all its entries.

---

### `MutableListLookup<TKey, TValue>`

The `MutableListLookup` class is a mutable lookup that uses a `List<TValue>` as the underlying collection.

To add an value to the lookup, call the `Add` method with the key and value as arguments:

```csharp
var lookup = new MutableListLookup<string, string>();
lookup.Add( "A", "One" );
lookup.Add( "A", "Two" );
lookup.Add( "B", "Three" );
```

In the previous example, the lookup will contain two groupings: one with the key **A** and values **One** and **Two**,
and another with the key **B** and value **Three**.

By default, the `MutableListLookup` class uses the key's default equality comparer to compare keys. To use a custom equality comparer, pass it as an argument to the constructor:

```csharp
var lookup = new MutableListLookup<string, string>( StringComparer.OrdinalIgnoreCase );
lookup.Add( "A", "One" );
lookup.Add( "a", "Two" );
```

In this case, the lookup will contain a single grouping with the key **A** and values **One** and **Two**.

Note that the `ListLookup`'s `Add` method will always return `true`, because a list has no restrictions on the number of times a value can be added to it.

To conditionally add a value only of the key is not present in the lookup, use the `TryAdd` method:

```csharp
var lookup = new MutableListLookup<string, string>( StringComparer.OrdinalIgnoreCase );
lookup.TryAdd( "a", "One" ); // returns true
lookup.TryAdd( "a", "Two" ); // return false
```

---

### `MutableHashSetLookup<TKey, TValue>`

The `MutableHashSetLookup` class is a mutable lookup that uses a `HashSet<TValue>` as the underlying collection
and will not contain duplicate values for the same key. `MutableHashSetLookup` has constructors that allow you to specify the equality comparer for the keys and values.

To add a value to the lookup, call the `Add` method with the key and value as arguments:

```csharp
var lookup = new MutableHashSetLookup<string, string>();
lookup.Add( "A", "One" );
lookup.Add( "A", "Two" );
lookup.Add( "A", "Two" );
```

In the previous example, the lookup will contain a single grouping with the key **A** and values **One** and **Two**.
The first two calls to `Add` will return `true`, while the third call will return `false`.

As with the `MutableListLookup`, a value can be conditionally added only of the key is not present in the lookup using the `TryAdd` method:

```csharp
var lookup = new MutableHashSetLookup<string, string>();
lookup.TryAdd( "a", "One" ); // returns true
lookup.TryAdd( "a", "Two" ); // return false
```

---

### Helpers

Several extension methods are provided to simplify the creation of mutable lookups from an existing `IEnumerable<T>`:

- `ToMutableListLookup`: Create a `MutableListLookup<TKey, TValue>` from an `IEnumerable<TValue>` using a key selector. 
  There are several overloads that set the key comparer, and the value selector.
- `ToMutableHashSetLookup`: Create a `MutableListLookup<TKey, TValue>` from an `IEnumerable<TValue>` using a key selector and a value selector.
  There are several overloads that set the key and value comparers, and the value selector.

  ```csharp
  var lookup1 = people.ToMutableListLookup( p => p.LastName ); // Mutable lookup of people keyed by their last name.
  var lookup2 = people.ToMutableListLookup( p => p.LastName, p => p.FirstName ); // Mutable lookup of people's names keyed by their last name.
  var lookup3 = people.ToMutableHashSetLookup( p => p.LastName, p => p.FirstName ); // Mutable lookup of people's names keyed by their last name, with no duplicate first names.
  ```

---

## Extension methods

- `EmptyIfNull<T>( source )`: Returns an empty array if *source* is `null`. This method has overloads for arrays, enumerables, and collections.
- `AsArray<T>( T? item)`: Converts the specified item to an array; if the *item* is null, the returned array will be empty.
- `Batch<T>( source, batchSize)`: Batches the elements of the source into groups of the specified batch size. It has an overload to optimize batching for `IList<T>`.

Examples:
```csharp
int[]? nullarray = null;
var emptyarray = nullarray.EmptyIfNull(); // emptyarray is an empty array.

var item = 42;
var array = item.AsArray(); // array contains a single element: 42.

var source = Enumerable.Range( 1, 10 );
var batches = source.Batch( 3 ); // batches contains three groups: { 1, 2, 3 }, { 4, 5, 6 }, { 7, 8, 9 }, and { 10 }.
```

---

## License

This project is licensed under the [MIT License](LICENSE).
