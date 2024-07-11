#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace Tests;

using RhoMicro.RequiredPropertyValidation;

public partial class GeneratorTests : TestBase
{
    readonly partial struct StructFoo
    {
        public required Object Bar { get; init; }
    }
    partial class Bar : Foo
    {
        public required Object Baz { get; set; }
    }
    partial class Foo
    {
        public required Object Bar1 { get; init; }
        public required Int32? Bar2 { get; init; }
        public required Object? NullableBar { get; init; }
        public Object? NullableNonRequiredBar { get; init; }
    }
    [Fact]
    public void StructRootThrowsOnNull()
    {
        var instance = Activator.CreateInstance<StructFoo>();
        var validator = GetValidator();
        _ = Assert.Throws<RequiredPropertiesValidationException>(() => validator.Validate(instance));
    }
    [Fact]
    public void StructRootDoesNotThrowOnNonNull()
    {
        var instance = new StructFoo() { Bar = new() };
        var validator = GetValidator();
        try
        {
            validator.Validate(instance);
        } catch(Exception ex)
        {
            Assert.Fail(ex.Message);
        }
    }
    [Fact]
    public void RootThrowsOnNull()
    {
        var instance = Activator.CreateInstance<Foo>();
        var validator = GetValidator();
        _ = Assert.Throws<RequiredPropertiesValidationException>(() => validator.Validate(instance));
    }
    [Fact]
    public void RootDoesNotThrowOnNonNull()
    {
        var instance = new Foo()
        {
            Bar1 = new(),
            Bar2 = new(),
            NullableBar = null
        };
        var validator = GetValidator();
        try
        {
            validator.Validate(instance);
        } catch(Exception ex)
        {
            Assert.Fail(ex.Message);
        }
    }
    [Fact]
    public void ChildThrowsOnNull()
    {
        var instance = Activator.CreateInstance<Bar>();
        var validator = GetValidator();
        _ = Assert.Throws<RequiredPropertiesValidationException>(() => validator.Validate(instance));
    }
    [Fact]
    public void ChildDoesNotThrowOnNonNull()
    {
        var instance = new Bar()
        {
            Bar1 = new(),
            Bar2 = new(),
            Baz = new(),
            NullableBar = new()
        };
        var validator = GetValidator();
        try
        {
            validator.Validate(instance);
        } catch(Exception ex)
        {
            Assert.Fail(ex.Message);
        }
    }
    [Fact]
    public void ChildThrowsOnRootNull()
    {
        var instance = Activator.CreateInstance<Bar>();
        instance.Baz = new();
        var validator = GetValidator();
        _ = Assert.Throws<RequiredPropertiesValidationException>(() => validator.Validate(instance));
    }
}
