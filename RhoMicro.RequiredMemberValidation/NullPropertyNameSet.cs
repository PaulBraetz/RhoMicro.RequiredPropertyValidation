namespace RhoMicro.RequiredMemberValidation;

using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Represents an immutable set of property names.
/// </summary>
/// <param name="propertyNames">The set of property names to wrap.</param>
public sealed class NullPropertyNameSet(HashSet<String> propertyNames) : IReadOnlyCollection<String>
{
    /// <summary>
    /// Gets an empty instance.
    /// </summary>
    public static NullPropertyNameSet Empty { get; } = new(new(StringComparer.OrdinalIgnoreCase));
    /// <summary>
    /// Determines whether the <see cref="NullPropertyNameSet"/> contains the specified element.
    /// </summary>
    /// <param name="propertyName">
    /// The element to locate in the <see cref="NullPropertyNameSet"/> object.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the <see cref="NullPropertyNameSet"/> object contains the specified element; otherwise, <see langword="false"/>.
    /// </returns>
    public Boolean Contains(String propertyName) => propertyNames.Contains(propertyName);
    /// <inheritdoc/>
    public Int32 Count => ( (IReadOnlyCollection<String>)propertyNames ).Count;
    /// <inheritdoc/>
    public IEnumerator<String> GetEnumerator() => ( (IEnumerable<String>)propertyNames ).GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ( (IEnumerable)propertyNames ).GetEnumerator();
}
