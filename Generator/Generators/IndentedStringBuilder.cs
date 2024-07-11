namespace RhoMicro.CodeAnalysis.Library.Text;
using System;

using RhoMicro.RequiredPropertyValidation;

partial class IndentedStringBuilder
{
    public IndentedStringBuilder AppendTypeSignature(TypeSignatureModel signature) =>
        Append("partial ")
        .Append(signature.RecordKeyword)
        .Append(' ')
        .Append(signature.TypeKind)
        .Append(' ')
        .Append(signature.Name);
    public IndentedStringBuilder AppendValidationInterface(String arg) =>
        Append("global::").Append(typeof(IValidateRequiredProperties<>).Namespace).Append('.').Append(nameof(IValidateRequiredProperties<Object>)).Append('<').Append(arg).Append('>');
    public IndentedStringBuilder AppendBaseConversion(String baseType, Boolean invert = false)
    {
        AppendCore("this is ");

        if(invert)
            AppendCore("not ");

        AppendValidationInterface(baseType).AppendCore(" baseImplementation");

        return this;
    }
}
