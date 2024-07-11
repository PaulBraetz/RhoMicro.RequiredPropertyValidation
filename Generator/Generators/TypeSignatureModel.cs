﻿#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace RhoMicro.RequiredPropertyValidation;

using System.Text;

using Microsoft.CodeAnalysis;

sealed record TypeSignatureModel
{
    public required String FullDisplayName { get; init; }
    public required Boolean IsSealed { get; init; }
    public required String Name { get; init; }
    public required String TypeKind { get; init; }
    public required String RecordKeyword { get; init; }
    public required String HintName { get; init; }
    public TypeSignatureModel? ParentType { get; init; }
    public static TypeSignatureModel Create(ITypeSymbol type) => CreateCore(type, withHintName: true);
    static TypeSignatureModel CreateCore(ITypeSymbol type, Boolean withHintName)
    {
        var result = type != null
            ? new TypeSignatureModel()
            {
                Name = type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat),
                FullDisplayName = type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                TypeKind = type.TypeKind switch
                {
                    Microsoft.CodeAnalysis.TypeKind.Class => "class",
                    Microsoft.CodeAnalysis.TypeKind.Struct => "struct",
                    _ => String.Empty
                },
                RecordKeyword = type.IsRecord ? "record" : String.Empty,
                IsSealed = type.IsSealed,
                ParentType = CreateCore(type.ContainingType, withHintName: false),
                HintName = withHintName
                ? GetHintName(type)
                : String.Empty
            }
            : null;

        return result!;
    }

    static String GetHintName(ITypeSymbol type)
    {
        var resultBuilder = new StringBuilder(type.ContainingNamespace?.ToDisplayString() ?? String.Empty);
        var types = new List<ITypeSymbol>();
        do
        {
            types.Add(type);
            type = type.ContainingType;
        } while(type is not null);

        for(var i = types.Count - 1; i >= 0; i--)
        {
            _ = resultBuilder.Append('_').Append(types[i].Name);
        }

        if(type is INamedTypeSymbol { TypeParameters: [.., { }] parameters })
        {
            _ = resultBuilder.Append("_of");
            foreach(var p in parameters)
            {
                _ = resultBuilder.Append('_').Append(p.Name);
            }
        }

        var result = resultBuilder.ToString();

        return result;
    }
}
