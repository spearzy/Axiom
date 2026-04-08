using System.Reflection;

namespace Axiom.Assertions.Equivalency;

internal static partial class EquivalencyEngine
{
    private static string ResolveExpectedMemberName(string actualMemberName, EquivalencyOptions options)
    {
        return options.TryGetMappedExpectedMemberName(actualMemberName, out var expectedMemberName)
            ? expectedMemberName
            : actualMemberName;
    }

    private static string ResolveActualMemberName(string expectedMemberName, EquivalencyOptions options)
    {
        return options.TryGetMappedActualMemberName(expectedMemberName, out var actualMemberName)
            ? actualMemberName
            : expectedMemberName;
    }

    private static string ResolveExpectedMemberPath(
        string actualMemberPath,
        string currentExpectedPath,
        string actualMemberName,
        EquivalencyOptions options)
    {
        RecordExpectedPathHelperProbe(ref _resolveExpectedMemberPathProbeCount);

        if (options.TryGetMappedExpectedMemberPath(actualMemberPath, out var expectedMemberPath))
        {
            return expectedMemberPath;
        }

        return AppendPath(currentExpectedPath, ResolveExpectedMemberName(actualMemberName, options));
    }

    private static string ResolveActualMemberPath(
        string expectedMemberPath,
        string currentActualPath,
        string expectedMemberName,
        EquivalencyOptions options)
    {
        RecordExpectedPathHelperProbe(ref _resolveActualMemberPathProbeCount);

        if (options.TryGetMappedActualMemberPath(expectedMemberPath, out var actualMemberPath))
        {
            return actualMemberPath;
        }

        return AppendPath(currentActualPath, ResolveActualMemberName(expectedMemberName, options));
    }

    private static string AppendPath(string parentPath, string childSegment)
    {
        RecordExpectedPathHelperProbe(ref _appendPathProbeCount);
        return parentPath.Length == 0 ? childSegment : $"{parentPath}.{childSegment}";
    }

    private static string AppendIndex(string path, int index)
    {
        RecordExpectedPathHelperProbe(ref _appendIndexProbeCount);
        return path.Length == 0 ? $"[{index}]" : $"{path}[{index}]";
    }

    private static string GetDirectChildMemberName(string fullPath, string parentPath)
    {
        RecordExpectedPathHelperProbe(ref _getDirectChildMemberNameProbeCount);

        if (parentPath.Length == 0)
        {
            return ExtractFirstMemberSegment(fullPath);
        }

        if (fullPath.StartsWith($"{parentPath}.", StringComparison.Ordinal))
        {
            return ExtractFirstMemberSegment(fullPath[(parentPath.Length + 1)..]);
        }

        throw new InvalidOperationException(
            $"Configured member mapping '{fullPath}' does not align with expected parent path '{parentPath}'.");
    }

    private static string ExtractFirstMemberSegment(string path)
    {
        var separatorIndex = path.IndexOf('.');
        return separatorIndex >= 0 ? path[..separatorIndex] : path;
    }

    private static void RecordExpectedPathHelperProbe(ref int counter)
    {
        if (!_expectedPathHelperProbeEnabled)
        {
            return;
        }

        Interlocked.Increment(ref counter);
    }

    private static ComparableMemberSet GetComparableMembers(Type type, EquivalencyOptions options)
    {
        var cacheKey = new ComparableMemberCacheKey(type, options.IncludePublicProperties, options.IncludePublicFields);
        return ComparableMemberCache.GetOrAdd(cacheKey, static key => BuildComparableMembers(key.Type, key.IncludePublicProperties, key.IncludePublicFields));
    }

    private static ComparableMemberSet BuildComparableMembers(Type type, bool includePublicProperties, bool includePublicFields)
    {
        var members = new Dictionary<string, Func<object, object?>>(StringComparer.Ordinal);

        if (includePublicProperties)
        {
            foreach (var property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if (property.GetMethod is null || property.GetMethod.IsStatic)
                {
                    continue;
                }

                if (property.GetIndexParameters().Length > 0)
                {
                    continue;
                }

                members.TryAdd(property.Name, property.GetValue);
            }
        }

        if (includePublicFields)
        {
            foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.Public))
            {
                if (field.IsStatic)
                {
                    continue;
                }

                members.TryAdd(field.Name, field.GetValue);
            }
        }

        var orderedNames = members.Keys.ToArray();
        Array.Sort(orderedNames, StringComparer.Ordinal);
        return new ComparableMemberSet(members, orderedNames);
    }

    private static bool IsPathIgnored(string path, string rootPath, EquivalencyOptions options)
    {
        if (options.IgnoredPaths.Contains(path))
        {
            return true;
        }

        var relativePath = ToRelativePath(path, rootPath);
        if (relativePath.Length > 0 && options.IgnoredPaths.Contains(relativePath))
        {
            return true;
        }

        foreach (var ignoredPath in options.IgnoredPaths)
        {
            if (PathMatchesOrIsChild(path, ignoredPath))
            {
                return true;
            }

            if (relativePath.Length > 0 && PathMatchesOrIsChild(relativePath, ignoredPath))
            {
                return true;
            }
        }

        return false;
    }

    private static bool PathMatchesOrIsChild(string currentPath, string configuredPath)
    {
        if (currentPath.Equals(configuredPath, StringComparison.Ordinal))
        {
            return true;
        }

        return currentPath.StartsWith($"{configuredPath}.", StringComparison.Ordinal) ||
               currentPath.StartsWith($"{configuredPath}[", StringComparison.Ordinal);
    }

    private static bool IsPathIncluded(string path, string rootPath, EquivalencyOptions options)
    {
        if (options.OnlyComparedMembers.Count == 0)
        {
            return true;
        }

        var relativePath = ToRelativePath(path, rootPath);
        if (relativePath.Length == 0)
        {
            return true;
        }

        foreach (var includedMember in options.OnlyComparedMembers)
        {
            if (PathMatchesOrContains(relativePath, includedMember))
            {
                return true;
            }
        }

        return false;
    }

    private static bool PathMatchesOrContains(string currentPath, string includedMember)
    {
        if (currentPath.Equals(includedMember, StringComparison.Ordinal))
        {
            return true;
        }

        if (currentPath.StartsWith($"{includedMember}.", StringComparison.Ordinal) ||
            currentPath.StartsWith($"{includedMember}[", StringComparison.Ordinal))
        {
            return true;
        }

        return includedMember.StartsWith($"{currentPath}.", StringComparison.Ordinal) ||
               includedMember.StartsWith($"{currentPath}[", StringComparison.Ordinal);
    }

    private static string ToRelativePath(string path, string rootPath)
    {
        if (path.Equals(rootPath, StringComparison.Ordinal))
        {
            return string.Empty;
        }

        if (path.StartsWith($"{rootPath}.", StringComparison.Ordinal))
        {
            return path[(rootPath.Length + 1)..];
        }

        if (path.StartsWith($"{rootPath}[", StringComparison.Ordinal))
        {
            return path[rootPath.Length..];
        }

        // Fallback for unusual path roots; preserving current path keeps matching predictable.
        return path;
    }
}
