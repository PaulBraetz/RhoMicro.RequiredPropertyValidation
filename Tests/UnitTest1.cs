#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace Tests;

using RhoMicro.RequiredMemberValidation;

public partial class UnitTest1
{
    partial class Bar : Foo
    {
        public required Object Baz { get; set; }
    }
    partial class Foo
    {
        public required Object Bar1 { get; init; }
        public required Object Bar2 { get; init; }
        public required Object? NullableBar { get; init; }
        public Object? NullableNonRequiredBar { get; init; }
    }
    [Fact]
    public void RootThrowsOnNull()
    {
        var instance = Activator.CreateInstance<Foo>();
        _ = Assert.Throws<RequiredPropertiesValidationException>(() => RequiredPropertyValidation.Validate(instance));
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
        try
        {
            RequiredPropertyValidation.Validate(instance);
        } catch(Exception ex)
        {
            Assert.Fail(ex.Message);
        }
    }
    [Fact]
    public void ChildThrowsOnNull()
    {
        var instance = Activator.CreateInstance<Bar>();
        _ = Assert.Throws<RequiredPropertiesValidationException>(() => RequiredPropertyValidation.Validate(instance));
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
        try
        {
            RequiredPropertyValidation.Validate(instance);
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
        _ = Assert.Throws<RequiredPropertiesValidationException>(() => RequiredPropertyValidation.Validate(instance));
    }
}