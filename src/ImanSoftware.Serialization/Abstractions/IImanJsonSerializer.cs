using System;
using System.Collections.Generic;
using System.Text;

namespace ImanSoftware.Serialization;

/// <summary>
/// Contract for JSON serialization and deserialization.
/// Provides a unified abstraction over System.Text.Json or other libraries.
/// </summary>
public interface IImanJsonSerializer
{
    /// <summary>
    /// Serializes an object to a JSON string.
    /// </summary>
    string Serialize<T>(T value);

    /// <summary>
    /// Serializes an object to a JSON string with custom options.
    /// </summary>
    string Serialize<T>(T value, object? options = null);

    /// <summary>
    /// Deserializes a JSON string to an object of type T.
    /// </summary>
    T? Deserialize<T>(string json);

    /// <summary>
    /// Deserializes a JSON string to an object of type T with custom options.
    /// </summary>
    T? Deserialize<T>(string json, object? options = null);
}