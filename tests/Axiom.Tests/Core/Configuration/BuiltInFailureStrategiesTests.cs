using System.Reflection;
using System.Reflection.Emit;
using System.Linq;

namespace Axiom.Tests.Core.Configuration;

public sealed class BuiltInFailureStrategiesTests : IDisposable
{
    private static readonly Type? XunitExceptionType = ResolveType(
        FrameworkFailureStrategyDefinitions.Xunit.ExceptionTypeName,
        FrameworkFailureStrategyDefinitions.Xunit.AssemblyNames);

    public static TheoryData<string, string, string[]> BuiltInStrategies { get; } = new()
    {
        {
            FrameworkFailureStrategyDefinitions.Xunit.StrategyName,
            FrameworkFailureStrategyDefinitions.Xunit.ExceptionTypeName,
            FrameworkFailureStrategyDefinitions.Xunit.AssemblyNames.ToArray()
        },
        {
            FrameworkFailureStrategyDefinitions.NUnit.StrategyName,
            FrameworkFailureStrategyDefinitions.NUnit.ExceptionTypeName,
            FrameworkFailureStrategyDefinitions.NUnit.AssemblyNames.ToArray()
        },
        {
            FrameworkFailureStrategyDefinitions.MSTest.StrategyName,
            FrameworkFailureStrategyDefinitions.MSTest.ExceptionTypeName,
            FrameworkFailureStrategyDefinitions.MSTest.AssemblyNames.ToArray()
        }
    };

    public void Dispose()
    {
        AxiomServices.Reset();
    }

    [Theory]
    [MemberData(nameof(BuiltInStrategies))]
    public void Assertions_UseConfiguredBuiltInFailureStrategy_OutsideBatch(
        string strategyName,
        string exceptionTypeName,
        string[] assemblyNames)
    {
        EnsureFrameworkExceptionTypeIsAvailable(exceptionTypeName, assemblyNames);
        var strategy = CreateStrategy(strategyName);
        AxiomServices.Configure(c => c.FailureStrategy = strategy);

        const int value = 42;
        var ex = Assert.ThrowsAny<Exception>(() => value.Should().Be(7));

        Assert.Equal(exceptionTypeName, ex.GetType().FullName);
        Assert.Equal("Expected value to be 7, but found 42.", ex.Message);
    }

    [Theory]
    [MemberData(nameof(BuiltInStrategies))]
    public void BatchDispose_UsesConfiguredBuiltInFailureStrategy(
        string strategyName,
        string exceptionTypeName,
        string[] assemblyNames)
    {
        EnsureFrameworkExceptionTypeIsAvailable(exceptionTypeName, assemblyNames);
        var strategy = CreateStrategy(strategyName);
        AxiomServices.Configure(c => c.FailureStrategy = strategy);

        var ex = Assert.ThrowsAny<Exception>(() =>
        {
            using var batch = Axiom.Core.Assert.Batch("numbers");
            42.Should().Be(7);
            99.Should().Be(100);
        });

        Assert.Equal(exceptionTypeName, ex.GetType().FullName);

        var message = ex.Message.Replace("\r\n", "\n", StringComparison.Ordinal);
        Assert.Contains("Batch 'numbers' failed with 2 assertion failure(s):", message);
        Assert.Contains("1) Expected 42 to be 7, but found 42.", message);
        Assert.Contains("2) Expected 99 to be 100, but found 99.", message);
    }

    [Theory]
    [MemberData(nameof(BuiltInStrategies))]
    public void Factory_ReturnsConfigurationError_WhenFrameworkExceptionTypeMissing(
        string strategyName,
        string exceptionTypeName,
        string[] assemblyNames)
    {
        var missingDefinition = new FrameworkFailureStrategyDefinition(
            StrategyName: strategyName,
            ExceptionTypeName: $"{exceptionTypeName}.Missing",
            AssemblyNames: assemblyNames);

        var factory = FrameworkFailureExceptionFactory.Create(missingDefinition);
        var ex = factory("ignored");

        var invalidOperationException = Assert.IsType<InvalidOperationException>(ex);
        Assert.Contains(missingDefinition.StrategyName, invalidOperationException.Message);
        Assert.Contains(missingDefinition.ExceptionTypeName, invalidOperationException.Message);
    }

    [Fact]
    public void Factory_ReturnsConfigurationError_WhenFrameworkExceptionLacksStringConstructor()
    {
        const string assemblyName = "Axiom.Tests.NoStringCtor";
        const string typeName = "Axiom.Tests.NoStringCtorException";
        EnsureDynamicExceptionType(typeName, assemblyName, includeStringConstructor: false);

        var definition = new FrameworkFailureStrategyDefinition(
            StrategyName: "NoStringCtorStrategy",
            ExceptionTypeName: typeName,
            AssemblyNames: [assemblyName]);

        var factory = FrameworkFailureExceptionFactory.Create(definition);
        var ex = factory("ignored");

        var invalidOperationException = Assert.IsType<InvalidOperationException>(ex);
        Assert.Contains(definition.StrategyName, invalidOperationException.Message);
        Assert.Contains(definition.ExceptionTypeName, invalidOperationException.Message);
    }

    private static void EnsureFrameworkExceptionTypeIsAvailable(string exceptionTypeName, IReadOnlyList<string> assemblyNames)
    {
        if (ResolveType(exceptionTypeName, assemblyNames) is not null)
        {
            return;
        }

        // For frameworks not referenced by this test project, define an in-memory stand-in type
        // so strategy resolution can still be verified end-to-end without package collisions.
        EnsureDynamicExceptionType(
            exceptionTypeName,
            assemblyNames[0],
            includeStringConstructor: true);
    }

    private static void EnsureDynamicExceptionType(string typeName, string assemblyName, bool includeStringConstructor)
    {
        if (ResolveType(typeName, [assemblyName]) is not null)
        {
            return;
        }

        var dynamicAssembly = AssemblyBuilder.DefineDynamicAssembly(
            new AssemblyName(assemblyName),
            AssemblyBuilderAccess.Run);
        var moduleBuilder = dynamicAssembly.DefineDynamicModule($"{assemblyName}.dll");
        var typeBuilder = moduleBuilder.DefineType(
            typeName,
            TypeAttributes.Public | TypeAttributes.Class,
            typeof(Exception));

        if (includeStringConstructor)
        {
            var constructorBuilder = typeBuilder.DefineConstructor(
                MethodAttributes.Public,
                CallingConventions.Standard,
                [typeof(string)]);

            var il = constructorBuilder.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Call, typeof(Exception).GetConstructor([typeof(string)])!);
            il.Emit(OpCodes.Ret);
        }
        else
        {
            // Only a parameterless constructor: this intentionally misses the (string) constructor
            // to validate factory diagnostics for malformed framework exception types.
            var constructorBuilder = typeBuilder.DefineConstructor(
                MethodAttributes.Public,
                CallingConventions.Standard,
                Type.EmptyTypes);

            var il = constructorBuilder.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Call, typeof(Exception).GetConstructor(Type.EmptyTypes)!);
            il.Emit(OpCodes.Ret);
        }

        _ = typeBuilder.CreateType();
    }

    private static Type? ResolveType(string typeName, IReadOnlyList<string> assemblyNames)
    {
        // xUnit type is frequently used in this test class; use cached resolution when available.
        if (typeName == FrameworkFailureStrategyDefinitions.Xunit.ExceptionTypeName &&
            XunitExceptionType is not null)
        {
            return XunitExceptionType;
        }

        // Try direct assembly-qualified lookup first.
        foreach (var assemblyName in assemblyNames)
        {
            var candidate = Type.GetType($"{typeName}, {assemblyName}", throwOnError: false);
            if (candidate is not null)
            {
                return candidate;
            }
        }

        // Fallback to scanning assemblies already loaded in this process.
        foreach (var loadedAssembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            var candidate = loadedAssembly.GetType(typeName, throwOnError: false, ignoreCase: false);
            if (candidate is not null)
            {
                return candidate;
            }
        }

        return null;
    }

    private static IFailureStrategy CreateStrategy(string strategyName)
    {
        return strategyName switch
        {
            nameof(XunitFailureStrategy) => XunitFailureStrategy.Instance,
            nameof(NUnitFailureStrategy) => NUnitFailureStrategy.Instance,
            nameof(MSTestFailureStrategy) => MSTestFailureStrategy.Instance,
            _ => throw new ArgumentOutOfRangeException(nameof(strategyName), strategyName, "Unknown strategy kind.")
        };
    }
}
