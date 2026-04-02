using System.Collections;

namespace Axiom.Assertions.Equivalency;

internal static partial class EquivalencyEngine
{
    private static void CompareEnumerable<TPathMode>(
        IEnumerable actualEnumerable,
        IEnumerable expectedEnumerable,
        string path,
        string rootPath,
        TPathMode pathMode,
        EquivalencyOptions options,
        List<EquivalencyDifference> differences,
        HashSet<ReferencePair> visitedPairs)
        where TPathMode : struct, IExpectedPathMode<TPathMode>
    {
        IEqualityComparer? collectionItemComparer = null;
        var hasCollectionItemComparer =
            options.HasCollectionItemComparers &&
            TryGetConfiguredCollectionItemComparer(path, rootPath, options, out collectionItemComparer);

        if (options.CollectionOrder == EquivalencyCollectionOrder.Any)
        {
            // Any-order matching needs random access and "used index" tracking.
            // Keep list materialisation only for this mode.
            var actualItems = actualEnumerable.Cast<object?>().ToList();
            var expectedItems = expectedEnumerable.Cast<object?>().ToList();
            CompareEnumerableAnyOrder(
                actualItems,
                expectedItems,
                path,
                rootPath,
                pathMode,
                options,
                differences,
                hasCollectionItemComparer ? collectionItemComparer : null);
            return;
        }

        // Ordered mode walks both sequences once to avoid eager list allocations.
        var actualEnumerator = actualEnumerable.GetEnumerator();
        var expectedEnumerator = expectedEnumerable.GetEnumerator();
        try
        {
            var index = 0;
            while (true)
            {
                var hasActual = actualEnumerator.MoveNext();
                var hasExpected = expectedEnumerator.MoveNext();
                if (!hasActual || !hasExpected)
                {
                    if (!hasActual && !hasExpected)
                    {
                        return;
                    }

                    if (hasExpected)
                    {
                        AddDifference(
                            differences,
                            $"{path}[{index}]",
                            pathMode.GetIndexedMode(index).DifferenceExpectedPath,
                            expectedEnumerator.Current,
                            null,
                            EquivalencyDifferenceKind.CollectionItemMissingOnActual);
                        index++;

                        while (expectedEnumerator.MoveNext())
                        {
                            AddDifference(
                                differences,
                                $"{path}[{index}]",
                                pathMode.GetIndexedMode(index).DifferenceExpectedPath,
                                expectedEnumerator.Current,
                                null,
                                EquivalencyDifferenceKind.CollectionItemMissingOnActual);
                            index++;
                        }
                    }
                    else
                    {
                        AddDifference(
                            differences,
                            $"{path}[{index}]",
                            pathMode.ClearExpectedPath().DifferenceExpectedPath,
                            null,
                            actualEnumerator.Current,
                            EquivalencyDifferenceKind.CollectionItemExtraOnActual);
                        index++;

                        while (actualEnumerator.MoveNext())
                        {
                            AddDifference(
                                differences,
                                $"{path}[{index}]",
                                pathMode.ClearExpectedPath().DifferenceExpectedPath,
                                null,
                                actualEnumerator.Current,
                                EquivalencyDifferenceKind.CollectionItemExtraOnActual);
                            index++;
                        }
                    }

                    return;
                }

                var itemPath = $"{path}[{index}]";
                var actualItem = actualEnumerator.Current;
                var expectedItem = expectedEnumerator.Current;
                if (hasCollectionItemComparer)
                {
                    if (!collectionItemComparer!.Equals(actualItem, expectedItem))
                    {
                        var indexedMode = pathMode.GetIndexedMode(index);
                        AddDifference(
                            differences,
                            itemPath,
                            indexedMode.DifferenceExpectedPath,
                            expectedItem,
                            actualItem,
                            EquivalencyDifferenceKind.ValueMismatch,
                            "values differ");
                    }
                }
                else
                {
                    CompareNode(
                        actualItem,
                        expectedItem,
                        itemPath,
                        rootPath,
                        pathMode.GetIndexedMode(index),
                        options,
                        differences,
                        visitedPairs);
                }

                index++;
            }
        }
        finally
        {
            (actualEnumerator as IDisposable)?.Dispose();
            (expectedEnumerator as IDisposable)?.Dispose();
        }
    }

    private static void CompareEnumerableAnyOrder<TPathMode>(
        List<object?> actualItems,
        List<object?> expectedItems,
        string path,
        string rootPath,
        TPathMode pathMode,
        EquivalencyOptions options,
        List<EquivalencyDifference> differences,
        IEqualityComparer? collectionItemComparer)
        where TPathMode : struct, IExpectedPathMode<TPathMode>
    {
        var usedActualIndexes = new bool[actualItems.Count];

        for (var expectedIndex = 0; expectedIndex < expectedItems.Count; expectedIndex++)
        {
            var expectedItem = expectedItems[expectedIndex];
            var matched = false;
            for (var actualIndex = 0; actualIndex < actualItems.Count; actualIndex++)
            {
                if (usedActualIndexes[actualIndex])
                {
                    continue;
                }

                var isEquivalent = collectionItemComparer is not null
                    ? collectionItemComparer.Equals(actualItems[actualIndex], expectedItem)
                    : ItemsEquivalentDeep(
                        actualItems[actualIndex],
                        expectedItem,
                        $"{path}[{expectedIndex}]",
                        rootPath,
                        pathMode.GetIndexedMode(expectedIndex),
                        options);
                if (!isEquivalent)
                {
                    continue;
                }

                usedActualIndexes[actualIndex] = true;
                matched = true;
                break;
            }

            if (!matched)
            {
                AddDifference(
                    differences,
                    $"{path}[{expectedIndex}]",
                    pathMode.GetIndexedMode(expectedIndex).DifferenceExpectedPath,
                    expectedItem,
                    null,
                    EquivalencyDifferenceKind.ExpectedCollectionItemNotFound);
            }
        }

        for (var actualIndex = 0; actualIndex < actualItems.Count; actualIndex++)
        {
            if (usedActualIndexes[actualIndex])
            {
                continue;
            }

            AddDifference(
                differences,
                $"{path}[{actualIndex}]",
                pathMode.ClearExpectedPath().DifferenceExpectedPath,
                null,
                actualItems[actualIndex],
                EquivalencyDifferenceKind.ActualCollectionContainsExtraItem);
        }
    }

    private static bool ItemsEquivalentDeep<TPathMode>(
        object? actual,
        object? expected,
        string path,
        string rootPath,
        TPathMode pathMode,
        EquivalencyOptions options)
        where TPathMode : struct, IExpectedPathMode<TPathMode>
    {
        var localDifferences = new List<EquivalencyDifference>();
        var localVisitedPairs = new HashSet<ReferencePair>(ReferencePairComparer.Instance);
        CompareNode(actual, expected, path, rootPath, pathMode, options, localDifferences, localVisitedPairs);
        return localDifferences.Count == 0;
    }
}
