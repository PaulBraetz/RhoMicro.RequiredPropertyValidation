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
    void AssertEx<T>(T instance, params String[] propertyNames)
    {
        var validator = GetValidator(useReflection: false);
        var ex = Assert.Throws<RequiredPropertiesValidationException>(() => validator.Validate(instance));
        foreach(var name in propertyNames)
            Assert.Contains(name, ex.NullProperties);
        Assert.Equal(propertyNames.Length, ex.NullProperties.Count);
    }
    [Fact]
    public void StructRootThrowsOnNull()
    {
        var instance = Activator.CreateInstance<StructFoo>();
        AssertEx(instance, "Bar");
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
        AssertEx(instance, "Bar1");
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
        AssertEx(instance, "Bar1", "Baz");
    }
    [Fact]
    public void ChildAsObjectThrowsOnNull()
    {
        var instance = Activator.CreateInstance<Bar>();
        AssertEx((Object)instance, "Bar1", "Baz");
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
        AssertEx(instance, "Bar1");
    }
}
