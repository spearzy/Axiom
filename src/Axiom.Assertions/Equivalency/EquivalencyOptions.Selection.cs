using System.Linq.Expressions;

namespace Axiom.Assertions.Equivalency;

public sealed partial class EquivalencyOptions
{
    public EquivalencyOptions IgnoreMember(string memberName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(memberName);
        _ignoredMemberNames.Add(memberName);
        return this;
    }

    public EquivalencyOptions IgnorePath(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        _ignoredPaths.Add(path);
        return this;
    }

    public EquivalencyOptions Ignore<TSubject>(Expression<Func<TSubject, object?>> memberSelector)
    {
        ArgumentNullException.ThrowIfNull(memberSelector);

        _ignoredPaths.Add(EquivalencySelectorPath.Create(memberSelector, nameof(memberSelector)));
        return this;
    }

    public EquivalencyOptions Ignore<TSubject>(params Expression<Func<TSubject, object?>>[] memberSelectors)
    {
        ArgumentNullException.ThrowIfNull(memberSelectors);

        foreach (var memberSelector in memberSelectors)
        {
            if (memberSelector is null)
            {
                throw new ArgumentNullException(nameof(memberSelectors), "memberSelectors must not contain null entries.");
            }

            _ignoredPaths.Add(EquivalencySelectorPath.Create(memberSelector, nameof(memberSelectors)));
        }

        return this;
    }

    // Restrict equivalency to one member branch, e.g. "Name" or "Address.Postcode".
    public EquivalencyOptions OnlyCompareMember(string memberPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(memberPath);
        _onlyComparedMembers.Add(memberPath.Trim());
        return this;
    }

    public EquivalencyOptions OnlyCompareMembers(params string[] memberPaths)
    {
        ArgumentNullException.ThrowIfNull(memberPaths);
        foreach (var memberPath in memberPaths)
        {
            OnlyCompareMember(memberPath);
        }

        return this;
    }

    public EquivalencyOptions OnlyCompare<TSubject>(Expression<Func<TSubject, object?>> memberSelector)
    {
        ArgumentNullException.ThrowIfNull(memberSelector);

        _onlyComparedMembers.Add(EquivalencySelectorPath.Create(memberSelector, nameof(memberSelector)));
        return this;
    }

    public EquivalencyOptions OnlyCompare<TSubject>(params Expression<Func<TSubject, object?>>[] memberSelectors)
    {
        ArgumentNullException.ThrowIfNull(memberSelectors);

        foreach (var memberSelector in memberSelectors)
        {
            if (memberSelector is null)
            {
                throw new ArgumentNullException(nameof(memberSelectors), "memberSelectors must not contain null entries.");
            }

            _onlyComparedMembers.Add(EquivalencySelectorPath.Create(memberSelector, nameof(memberSelectors)));
        }

        return this;
    }
}
