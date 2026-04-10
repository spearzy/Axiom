using Microsoft.CodeAnalysis;

namespace Axiom.Analyzers.NunitMigration;

internal sealed class NunitAssertMigrationSymbols
{
    private NunitAssertMigrationSymbols(
        Compilation compilation,
        INamedTypeSymbol? nunitAssertType,
        INamedTypeSymbol? nunitIsType,
        INamedTypeSymbol? nunitDoesType,
        INamedTypeSymbol? nunitHasType,
        INamedTypeSymbol? constraintExpressionType,
        INamedTypeSymbol? countConstraintExpressionType,
        INamedTypeSymbol? resolveConstraintType,
        INamedTypeSymbol? enumerableType,
        INamedTypeSymbol? genericEnumerableType,
        INamedTypeSymbol? asyncEnumerableType,
        INamedTypeSymbol? actionType,
        INamedTypeSymbol? funcType,
        INamedTypeSymbol? taskType,
        INamedTypeSymbol? genericTaskType,
        INamedTypeSymbol? valueTaskType,
        INamedTypeSymbol? genericValueTaskType,
        INamedTypeSymbol? spanType,
        INamedTypeSymbol? readOnlySpanType,
        INamedTypeSymbol? memoryType,
        INamedTypeSymbol? readOnlyMemoryType)
    {
        Compilation = compilation;
        NunitAssertType = nunitAssertType;
        NunitIsType = nunitIsType;
        NunitDoesType = nunitDoesType;
        NunitHasType = nunitHasType;
        ConstraintExpressionType = constraintExpressionType;
        CountConstraintExpressionType = countConstraintExpressionType;
        ResolveConstraintType = resolveConstraintType;
        EnumerableType = enumerableType;
        GenericEnumerableType = genericEnumerableType;
        AsyncEnumerableType = asyncEnumerableType;
        ActionType = actionType;
        FuncType = funcType;
        TaskType = taskType;
        GenericTaskType = genericTaskType;
        ValueTaskType = valueTaskType;
        GenericValueTaskType = genericValueTaskType;
        SpanType = spanType;
        ReadOnlySpanType = readOnlySpanType;
        MemoryType = memoryType;
        ReadOnlyMemoryType = readOnlyMemoryType;
    }

    public Compilation Compilation { get; }
    public INamedTypeSymbol? NunitAssertType { get; }
    private INamedTypeSymbol? NunitIsType { get; }
    private INamedTypeSymbol? NunitDoesType { get; }
    private INamedTypeSymbol? NunitHasType { get; }
    private INamedTypeSymbol? ConstraintExpressionType { get; }
    private INamedTypeSymbol? CountConstraintExpressionType { get; }
    private INamedTypeSymbol? ResolveConstraintType { get; }
    private INamedTypeSymbol? EnumerableType { get; }
    private INamedTypeSymbol? GenericEnumerableType { get; }
    private INamedTypeSymbol? AsyncEnumerableType { get; }
    private INamedTypeSymbol? ActionType { get; }
    private INamedTypeSymbol? FuncType { get; }
    private INamedTypeSymbol? TaskType { get; }
    private INamedTypeSymbol? GenericTaskType { get; }
    private INamedTypeSymbol? ValueTaskType { get; }
    private INamedTypeSymbol? GenericValueTaskType { get; }
    private INamedTypeSymbol? SpanType { get; }
    private INamedTypeSymbol? ReadOnlySpanType { get; }
    private INamedTypeSymbol? MemoryType { get; }
    private INamedTypeSymbol? ReadOnlyMemoryType { get; }

    public bool IsEnabled =>
        NunitAssertType is not null &&
        NunitIsType is not null &&
        NunitDoesType is not null &&
        NunitHasType is not null &&
        ConstraintExpressionType is not null &&
        CountConstraintExpressionType is not null &&
        ResolveConstraintType is not null;

    public static NunitAssertMigrationSymbols Create(Compilation compilation)
    {
        return new NunitAssertMigrationSymbols(
            compilation,
            compilation.GetTypeByMetadataName("NUnit.Framework.Assert"),
            compilation.GetTypeByMetadataName("NUnit.Framework.Is"),
            compilation.GetTypeByMetadataName("NUnit.Framework.Does"),
            compilation.GetTypeByMetadataName("NUnit.Framework.Has"),
            compilation.GetTypeByMetadataName("NUnit.Framework.Constraints.ConstraintExpression"),
            compilation.GetTypeByMetadataName("NUnit.Framework.Constraints.CountConstraintExpression"),
            compilation.GetTypeByMetadataName("NUnit.Framework.Constraints.IResolveConstraint"),
            compilation.GetTypeByMetadataName("System.Collections.IEnumerable"),
            compilation.GetTypeByMetadataName("System.Collections.Generic.IEnumerable`1"),
            compilation.GetTypeByMetadataName("System.Collections.Generic.IAsyncEnumerable`1"),
            compilation.GetTypeByMetadataName("System.Action"),
            compilation.GetTypeByMetadataName("System.Func`1"),
            compilation.GetTypeByMetadataName("System.Threading.Tasks.Task"),
            compilation.GetTypeByMetadataName("System.Threading.Tasks.Task`1"),
            compilation.GetTypeByMetadataName("System.Threading.Tasks.ValueTask"),
            compilation.GetTypeByMetadataName("System.Threading.Tasks.ValueTask`1"),
            compilation.GetTypeByMetadataName("System.Span`1"),
            compilation.GetTypeByMetadataName("System.ReadOnlySpan`1"),
            compilation.GetTypeByMetadataName("System.Memory`1"),
            compilation.GetTypeByMetadataName("System.ReadOnlyMemory`1"));
    }

    public bool IsNunitAssert(INamedTypeSymbol? containingType)
        => NunitAssertType is not null &&
           containingType is not null &&
           SymbolEqualityComparer.Default.Equals(containingType.OriginalDefinition, NunitAssertType);

    public bool IsResolveConstraint(ITypeSymbol type)
        => ResolveConstraintType is not null &&
           (SymbolEqualityComparer.Default.Equals(type, ResolveConstraintType) || ImplementsInterface(type, ResolveConstraintType));

    public bool IsIsType(ITypeSymbol? type)
        => NunitIsType is not null &&
           type is not null &&
           SymbolEqualityComparer.Default.Equals(type.OriginalDefinition, NunitIsType);

    public bool IsConstraintExpressionType(ITypeSymbol? type)
        => ConstraintExpressionType is not null &&
           type is not null &&
           SymbolEqualityComparer.Default.Equals(type.OriginalDefinition, ConstraintExpressionType);

    public bool IsDoesType(ITypeSymbol? type)
        => NunitDoesType is not null &&
           type is not null &&
           SymbolEqualityComparer.Default.Equals(type.OriginalDefinition, NunitDoesType);

    public bool IsHasType(ITypeSymbol? type)
        => NunitHasType is not null &&
           type is not null &&
           SymbolEqualityComparer.Default.Equals(type.OriginalDefinition, NunitHasType);

    public bool IsCountConstraintExpressionType(ITypeSymbol? type)
        => CountConstraintExpressionType is not null &&
           type is not null &&
           SymbolEqualityComparer.Default.Equals(type.OriginalDefinition, CountConstraintExpressionType);

    public bool IsEnumerableLike(ITypeSymbol type)
    {
        if (type.SpecialType == SpecialType.System_String)
        {
            return false;
        }

        if (type is IArrayTypeSymbol)
        {
            return true;
        }

        if (EnumerableType is not null &&
            SymbolEqualityComparer.Default.Equals(type, EnumerableType))
        {
            return true;
        }

        if (GenericEnumerableType is not null &&
            type is INamedTypeSymbol namedType &&
            SymbolEqualityComparer.Default.Equals(namedType.OriginalDefinition, GenericEnumerableType))
        {
            return true;
        }

        return ImplementsInterface(type, EnumerableType) || ImplementsInterface(type, GenericEnumerableType);
    }

    public bool IsAsyncEnumerableLike(ITypeSymbol type)
    {
        if (AsyncEnumerableType is null)
        {
            return false;
        }

        if (type is INamedTypeSymbol namedType &&
            SymbolEqualityComparer.Default.Equals(namedType.OriginalDefinition, AsyncEnumerableType))
        {
            return true;
        }

        return ImplementsInterface(type, AsyncEnumerableType);
    }

    public bool IsSpanOrMemoryLike(ITypeSymbol type)
    {
        if (type is not INamedTypeSymbol namedType)
        {
            return false;
        }

        var originalDefinition = namedType.OriginalDefinition;
        return (SpanType is not null && SymbolEqualityComparer.Default.Equals(originalDefinition, SpanType)) ||
               (ReadOnlySpanType is not null && SymbolEqualityComparer.Default.Equals(originalDefinition, ReadOnlySpanType)) ||
               (MemoryType is not null && SymbolEqualityComparer.Default.Equals(originalDefinition, MemoryType)) ||
               (ReadOnlyMemoryType is not null && SymbolEqualityComparer.Default.Equals(originalDefinition, ReadOnlyMemoryType));
    }

    public bool SupportsEqualityMigrationReceiver(ITypeSymbol type)
        => IsStringType(type) || !UsesSpecializedShouldReceiver(type);

    public bool SupportsNullMigrationReceiver(ITypeSymbol type)
        => IsStringType(type) || !UsesSpecializedShouldReceiver(type);

    public bool SupportsEmptyMigrationReceiver(ITypeSymbol type)
        => IsEnumerableLike(type) && !IsAsyncEnumerableLike(type);

    public bool SupportsReferenceEqualityMigrationReceiver(ITypeSymbol type)
        => type.IsReferenceType && !UsesSpecializedShouldReceiver(type);

    public bool SupportsCountMigrationReceiver(ITypeSymbol type)
        => IsEnumerableLike(type) && !IsAsyncEnumerableLike(type) && !IsStringType(type);

    public static bool IsNullableOrReferenceType(ITypeSymbol type)
    {
        if (type.IsReferenceType)
        {
            return true;
        }

        return type is INamedTypeSymbol namedType &&
               namedType.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T;
    }

    private bool UsesSpecializedShouldReceiver(ITypeSymbol type)
    {
        if (ActionType is not null && SymbolEqualityComparer.Default.Equals(type, ActionType))
        {
            return true;
        }

        if (IsStringType(type))
        {
            return true;
        }

        if (IsTaskLike(type) || IsAsyncEnumerableLike(type))
        {
            return true;
        }

        return IsFuncReturningTaskLike(type);
    }

    private static bool IsStringType(ITypeSymbol type)
        => type.SpecialType == SpecialType.System_String;

    private bool IsTaskLike(ITypeSymbol type)
    {
        if (TaskType is not null && SymbolEqualityComparer.Default.Equals(type, TaskType))
        {
            return true;
        }

        if (ValueTaskType is not null && SymbolEqualityComparer.Default.Equals(type, ValueTaskType))
        {
            return true;
        }

        if (type is not INamedTypeSymbol namedType)
        {
            return false;
        }

        var originalDefinition = namedType.OriginalDefinition;
        return (GenericTaskType is not null && SymbolEqualityComparer.Default.Equals(originalDefinition, GenericTaskType)) ||
               (GenericValueTaskType is not null && SymbolEqualityComparer.Default.Equals(originalDefinition, GenericValueTaskType));
    }

    private bool IsFuncReturningTaskLike(ITypeSymbol type)
    {
        if (FuncType is null ||
            type is not INamedTypeSymbol namedType ||
            !SymbolEqualityComparer.Default.Equals(namedType.OriginalDefinition, FuncType) ||
            namedType.TypeArguments.Length != 1)
        {
            return false;
        }

        return IsTaskLike(namedType.TypeArguments[0]);
    }

    private static bool ImplementsInterface(ITypeSymbol type, INamedTypeSymbol? interfaceType)
    {
        if (interfaceType is null)
        {
            return false;
        }

        foreach (var candidate in type.AllInterfaces)
        {
            var originalDefinition = candidate.OriginalDefinition;
            if (SymbolEqualityComparer.Default.Equals(candidate, interfaceType) ||
                SymbolEqualityComparer.Default.Equals(originalDefinition, interfaceType))
            {
                return true;
            }
        }

        return false;
    }
}
