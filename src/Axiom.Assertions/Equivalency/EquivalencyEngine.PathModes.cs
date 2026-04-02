
namespace Axiom.Assertions.Equivalency;

internal static partial class EquivalencyEngine
{
    private interface IExpectedPathMode<TSelf>
        where TSelf : struct, IExpectedPathMode<TSelf>
    {
        string DifferenceExpectedPath { get; }

        TSelf GetExpectedMemberMode(
            string path,
            string rootPath,
            string actualMemberName,
            EquivalencyOptions options,
            out string expectedMemberName);

        TSelf GetActualMemberMode(
            string path,
            string rootPath,
            string expectedMemberName,
            EquivalencyOptions options,
            out string actualMemberName);

        TSelf GetIndexedMode(int index);

        TSelf ClearExpectedPath();
    }

    private readonly struct NoMappingPathMode : IExpectedPathMode<NoMappingPathMode>
    {
        public string DifferenceExpectedPath => string.Empty;

        public NoMappingPathMode GetExpectedMemberMode(
            string path,
            string rootPath,
            string actualMemberName,
            EquivalencyOptions options,
            out string expectedMemberName)
        {
            expectedMemberName = actualMemberName;
            return default;
        }

        public NoMappingPathMode GetActualMemberMode(
            string path,
            string rootPath,
            string expectedMemberName,
            EquivalencyOptions options,
            out string actualMemberName)
        {
            actualMemberName = expectedMemberName;
            return default;
        }

        public NoMappingPathMode GetIndexedMode(int index)
        {
            return default;
        }

        public NoMappingPathMode ClearExpectedPath()
        {
            return default;
        }
    }

    private readonly struct MappedPathMode : IExpectedPathMode<MappedPathMode>
    {
        private readonly string _expectedPath;

        public MappedPathMode(string expectedPath)
        {
            _expectedPath = expectedPath;
        }

        public static MappedPathMode Root => new(string.Empty);

        public string DifferenceExpectedPath => _expectedPath;

        public MappedPathMode GetExpectedMemberMode(
            string path,
            string rootPath,
            string actualMemberName,
            EquivalencyOptions options,
            out string expectedMemberName)
        {
            var actualRelativePath = ToRelativePath(path, rootPath);
            var actualMemberPath = AppendPath(actualRelativePath, actualMemberName);
            var expectedMemberPath = ResolveExpectedMemberPath(actualMemberPath, _expectedPath, actualMemberName, options);
            expectedMemberName = GetDirectChildMemberName(expectedMemberPath, _expectedPath);
            return new MappedPathMode(expectedMemberPath);
        }

        public MappedPathMode GetActualMemberMode(
            string path,
            string rootPath,
            string expectedMemberName,
            EquivalencyOptions options,
            out string actualMemberName)
        {
            var actualRelativePath = ToRelativePath(path, rootPath);
            var expectedMemberPath = AppendPath(_expectedPath, expectedMemberName);
            var actualMemberPath = ResolveActualMemberPath(expectedMemberPath, actualRelativePath, expectedMemberName, options);
            actualMemberName = GetDirectChildMemberName(actualMemberPath, actualRelativePath);
            return new MappedPathMode(expectedMemberPath);
        }

        public MappedPathMode GetIndexedMode(int index)
        {
            return new MappedPathMode(AppendIndex(_expectedPath, index));
        }

        public MappedPathMode ClearExpectedPath()
        {
            return Root;
        }
    }
}
