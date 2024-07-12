#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace RhoMicro.RequiredPropertyValidation;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using RhoMicro.CodeAnalysis.Library;
using RhoMicro.CodeAnalysis.Library.Text;

[Generator(LanguageNames.CSharp)]
public sealed class Generator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var provider = context.CompilationProvider
            .Select(GetModels)
            .SelectMany((l, ct) => l)
            .Select(GetSource);

        context.RegisterSourceOutput(provider, (ctx, t) => ctx.AddSource(t.hintName, t.source));
    }

    private static (String hintName, String source) GetSource(TypeModel m, CancellationToken ct)
    {
        var builder = new IndentedStringBuilder(new()
        {
            AmbientCancellationToken = ct,
            GeneratorName = "RhoMicro.RequiredPropertyValidation.Generator",
            PrependNullableEnable = true,
            PrependMarkerComment = true,
            //PrependWarningDisablePragma = true
        }).Append((b) =>
        {
            if(m!.Namespace is not [.., { }])
                return;

            _ = b.Append("namespace ").Append(m.Namespace).AppendLine(';');
        });

        var sigs = new List<TypeSignatureModel>();
        var currentSig = m!.Signature;
        do
        {
            sigs.Add(currentSig);
            currentSig = currentSig.ParentType;
        } while(currentSig is not null);

        for(var i = sigs.Count - 1; i > 0; i--)
        {
            _ = builder.AppendTypeSignature(sigs[i]).OpenBracesBlock();
        }

        var source = builder.AppendTypeSignature(m.Signature).Append(" : ").AppendValidationInterface(m.Signature.Name)
            .OpenBracesBlock()
                .Append("void ").AppendValidationInterface().Append('.').Append(nameof(IValidateRequiredProperties.GetNullPropertyNames)).AppendLine("(global::System.Collections.Generic.HashSet<global::System.String> nullProperties) =>")
                .Indent()
                    .Append("((").AppendValidationInterface(m.Signature.FullDisplayName).Append(")this).").Append(nameof(IValidateRequiredProperties<Object>.GetNullPropertyNames)).AppendLine("(nullProperties);")
                .Detent()
                .Append("void ").AppendValidationInterface(m.Signature.FullDisplayName).Append('.').Append(nameof(IValidateRequiredProperties<Object>.GetNullPropertyNames)).Append("(global::System.Collections.Generic.HashSet<global::System.String> nullProperties)")
                .OpenBracesBlock()
                    .Append(b =>
                    {
                        if(m.BaseType is not null)
                        {
                            _ = b.Append("if(").AppendBaseConversion(m.BaseType).AppendLine(')')
                            .Indent()
                                .Append("baseImplementation.").Append(nameof(IValidateRequiredProperties<Object>.GetNullPropertyNames)).AppendLine("(nullProperties);")
                            .Detent();
                        }

                        foreach(var property in m.RequiredProperties)
                        {
                            _ = b.AppendLine()
                            .Append("if(this.").Append(property).AppendLine(" is null)")
                            .Indent()
                                .Append("_ = nullProperties.Add(\"").Append(property).AppendLine("\");")
                            .Detent();
                        }
                    })
                .CloseBlock()
                .Append("global::System.Boolean ").AppendValidationInterface().Append('.').Append(nameof(IValidateRequiredProperties.IsValid)).AppendLine(" =>")
                .Indent()
                    .Append("((").AppendValidationInterface(m.Signature.FullDisplayName).Append(")this).").Append(nameof(IValidateRequiredProperties<Object>.IsValid)).AppendLine(';')
                .Detent()
                .Append("global::System.Boolean ").AppendValidationInterface(m.Signature.FullDisplayName).Append('.').Append(nameof(IValidateRequiredProperties<Object>.IsValid)).AppendLine(" =>")
                .Indent()
                    .Append(b =>
                    {
                        if(m.BaseType is not null)
                        {
                            b.Append('(').AppendBaseConversion(m.BaseType, invert: true)
                            .Append(" || baseImplementation.").Append(nameof(IValidateRequiredProperties<Object>.IsValid)).AppendLine(')')
                            .AppendCore("&& ");
                        }

                        b.Append("this.").Append(m.RequiredProperties[0]).AppendCore(" is not null");

                        for(var i = 1; i < m.RequiredProperties.Count; i++)
                        {
                            b.AppendLine().Append("&& this.").Append(m.RequiredProperties[i]).AppendCore(" is not null");
                        }

                        b.AppendCore(';');
                    })
                .Detent()
            .CloseAllBlocks()
            .ToString();

        var hintName = m.Signature.HintName;

        return (hintName, source);
    }

    private static EquatableList<TypeModel> GetModels(Compilation c, CancellationToken ct)
    {
        var result = c.GetSymbolsWithName(n => true, SymbolFilter.Type, ct)
            .OfType<ITypeSymbol>()
            .Where(t => t.TypeKind is TypeKind.Class or TypeKind.Struct)
            .Where(t =>
            {
                var hasPartialDeclarations = t.DeclaringSyntaxReferences.Select(r => r.GetSyntax(ct))
                    .Where(s => s is TypeDeclarationSyntax
                    {
                        Modifiers: [.., { RawKind: (Int32)SyntaxKind.PartialKeyword }]
                    } and not InterfaceDeclarationSyntax)
                    .Any();

                return hasPartialDeclarations;
            })
            .Select(t =>
            {
                ct.ThrowIfCancellationRequested();

                var requiredPropertySymbols = t.GetMembers()
                    .OfType<IPropertySymbol>()
                    .Where(p => p is
                    {
                        IsRequired: true,
                        Type.IsReferenceType: true,
                        IsDefinition: true,
                        NullableAnnotation: NullableAnnotation.NotAnnotated
                    });

                if(!requiredPropertySymbols.Any())
                    return null;

                ct.ThrowIfCancellationRequested();
                var result = TypeModel.Create(t, requiredPropertySymbols, ct);

                return result;
            })
            .Where(m => m is not null)
            .ToEquatableList(ct);

        return result!;
    }
}
