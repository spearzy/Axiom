namespace Axiom.Assertions.Equivalency;

public sealed partial class EquivalencyOptions
{
    internal EquivalencyOptions Clone()
    {
        var clone = new EquivalencyOptions
        {
            CollectionOrder = CollectionOrder,
            RequireStrictRuntimeTypes = RequireStrictRuntimeTypes,
            MaxDifferences = MaxDifferences,
            StringComparison = StringComparison,
            IncludePublicProperties = IncludePublicProperties,
            IncludePublicFields = IncludePublicFields,
            FailOnMissingMembers = FailOnMissingMembers,
            FailOnExtraMembers = FailOnExtraMembers,
            IgnoreExpectedNullMemberValues = IgnoreExpectedNullMemberValues,
            IgnoreActualNullMemberValues = IgnoreActualNullMemberValues,
            FloatTolerance = FloatTolerance,
            DoubleTolerance = DoubleTolerance,
            HalfTolerance = HalfTolerance,
            DecimalTolerance = DecimalTolerance,
            DateOnlyTolerance = DateOnlyTolerance,
            DateTimeTolerance = DateTimeTolerance,
            DateTimeOffsetTolerance = DateTimeOffsetTolerance,
            TimeOnlyTolerance = TimeOnlyTolerance,
            TimeSpanTolerance = TimeSpanTolerance,
        };

        foreach (var member in _ignoredMemberNames)
        {
            clone._ignoredMemberNames.Add(member);
        }

        foreach (var path in _ignoredPaths)
        {
            clone._ignoredPaths.Add(path);
        }

        foreach (var memberPath in _onlyComparedMembers)
        {
            clone._onlyComparedMembers.Add(memberPath);
        }

        foreach (var typeComparer in _typeComparers)
        {
            clone._typeComparers[typeComparer.Key] = typeComparer.Value;
        }

        foreach (var pathComparer in _pathComparers)
        {
            clone._pathComparers[pathComparer.Key] = pathComparer.Value;
        }

        foreach (var collectionItemComparer in _collectionItemComparers)
        {
            clone._collectionItemComparers[collectionItemComparer.Key] = collectionItemComparer.Value;
        }

        foreach (var memberNameMapping in _actualToExpectedMemberNames)
        {
            clone._actualToExpectedMemberNames[memberNameMapping.Key] = memberNameMapping.Value;
        }

        foreach (var reverseMapping in _expectedToActualMemberNames)
        {
            clone._expectedToActualMemberNames[reverseMapping.Key] = reverseMapping.Value;
        }

        foreach (var pathMapping in _actualToExpectedMemberPaths)
        {
            clone._actualToExpectedMemberPaths[pathMapping.Key] = pathMapping.Value;
        }

        foreach (var reversePathMapping in _expectedToActualMemberPaths)
        {
            clone._expectedToActualMemberPaths[reversePathMapping.Key] = reversePathMapping.Value;
        }

        return clone;
    }
}
