#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace RhoMicro.RequiredPropertyValidation;

using System.Collections.Generic;
using System.Diagnostics;

using Microsoft.CodeAnalysis;

using RhoMicro.CodeAnalysis.Library;

sealed record TypeModel
{
    public required String Namespace { get; init; }
    public required TypeSignatureModel Signature { get; init; }
    public required EquatableList<String> RequiredProperties { get; init; }
    public required String? BaseType { get; init; }
    public static TypeModel Create(ITypeSymbol t, IEnumerable<IPropertySymbol> requiredPropertySymbols, CancellationToken ct)
    {
        var signature = TypeSignatureModel.Create(t);
        var requiredProperties = requiredPropertySymbols.Select(t => t.Name).ToEquatableList(ct);
        var @namespace = t.ContainingNamespace?.ToDisplayString(
            SymbolDisplayFormat.FullyQualifiedFormat.WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted)) ??
            String.Empty;

        var result = new TypeModel()
        {
            BaseType = t.BaseType?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            Namespace = @namespace,
            RequiredProperties = requiredProperties,
            Signature = signature
        };

        return result;
    }
}
