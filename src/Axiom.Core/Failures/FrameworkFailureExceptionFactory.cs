using System.Linq.Expressions;
using System.Reflection;

namespace Axiom.Core.Failures;

internal static class FrameworkFailureExceptionFactory
{
    internal static Func<string, Exception> Create(FrameworkFailureStrategyDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);

        var exceptionType = ResolveExceptionType(definition.ExceptionTypeName, definition.AssemblyNames);
        if (exceptionType is null)
        {
            var message = string.Format(
                FailureStrategyMessages.MissingFrameworkTypeTemplate,
                definition.StrategyName,
                definition.ExceptionTypeName);
            return _ => new InvalidOperationException(message);
        }

        var messageConstructor = exceptionType.GetConstructor([typeof(string)]);
        if (messageConstructor is null)
        {
            var message = string.Format(
                FailureStrategyMessages.MissingStringConstructorTemplate,
                definition.StrategyName,
                exceptionType.FullName ?? definition.ExceptionTypeName);
            return _ => new InvalidOperationException(message);
        }

        return BuildMessageConstructorFactory(messageConstructor);
    }

    private static Func<string, Exception> BuildMessageConstructorFactory(ConstructorInfo messageConstructor)
    {
        // Compile constructor invocation once so repeated failures avoid reflection Invoke cost.
        var message = Expression.Parameter(typeof(string), "message");
        var createException = Expression.New(messageConstructor, message);
        var castToException = Expression.Convert(createException, typeof(Exception));
        return Expression.Lambda<Func<string, Exception>>(castToException, message).Compile();
    }

    private static Type? ResolveExceptionType(string exceptionTypeName, IReadOnlyList<string> assemblyNames)
    {
        // Fast path: direct assembly-qualified lookup against known candidate assembly names.
        foreach (var assemblyName in assemblyNames)
        {
            var qualifiedType = Type.GetType($"{exceptionTypeName}, {assemblyName}", throwOnError: false);
            if (qualifiedType is not null)
            {
                return qualifiedType;
            }
        }

        // Fallback: inspect already-loaded assemblies in the current AppDomain.
        foreach (var loadedAssembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            var type = loadedAssembly.GetType(exceptionTypeName, throwOnError: false, ignoreCase: false);
            if (type is not null)
            {
                return type;
            }
        }

        // Final attempt: load optional candidate assemblies explicitly, then resolve type from them.
        foreach (var assemblyName in assemblyNames)
        {
            try
            {
                var assembly = Assembly.Load(new AssemblyName(assemblyName));
                var type = assembly.GetType(exceptionTypeName, throwOnError: false, ignoreCase: false);
                if (type is not null)
                {
                    return type;
                }
            }
            catch (FileNotFoundException)
            {
                // Optional dependency not available in this project; keep searching candidates.
            }
            catch (FileLoadException)
            {
                // Optional dependency not available in this project; keep searching candidates.
            }
            catch (BadImageFormatException)
            {
                // Optional dependency not available in this project; keep searching candidates.
            }
        }

        return null;
    }
}
