using AspectInjector.Broker;
using System.Reflection;

namespace JiraReport.Client.Aspects
{
    [Aspect(Scope.Global), AttributeUsage(AttributeTargets.Method)]
    [Injection(typeof(ExceptionStateBindAttribute))]
    public class ExceptionStateBindAttribute : Attribute
    {
        private static readonly MethodInfo _asyncHandler = typeof(ExceptionStateBindAttribute).GetMethod(nameof(ExceptionStateBindAttribute.WrapAsync), BindingFlags.NonPublic | BindingFlags.Static);
        private static readonly MethodInfo _syncHandler = typeof(ExceptionStateBindAttribute).GetMethod(nameof(ExceptionStateBindAttribute.WrapSync), BindingFlags.NonPublic | BindingFlags.Static);
        private static readonly Type _voidTaskResult = Type.GetType("System.Threading.Tasks.VoidTaskResult");

        [Advice(Kind.Around, Targets = Target.Any)]
        public object Handle(
            [Argument(Source.Instance)] object instance,
            [Argument(Source.Target)] Func<object[], object> target,
            [Argument(Source.Arguments)] object[] args,
            [Argument(Source.ReturnType)] Type retType
            )
        {
            if (instance is null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            if (retType is null)
            {
                throw new ArgumentNullException(nameof(retType));
            }

            if (instance is not IExceptionStateLogger logger)
            {
                throw new InvalidOperationException($"This attribute is supported only for {typeof(IExceptionStateLogger)}. Actual type: {instance.GetType().FullName}");
            }

            if (typeof(Task).IsAssignableFrom(retType))
            {
                var syncResultType = retType.IsConstructedGenericType ? retType.GenericTypeArguments[0] : _voidTaskResult;
                return _asyncHandler.MakeGenericMethod(syncResultType).Invoke(this, new object[] { target, args, logger });
            }
            else
            {
                retType = retType == typeof(void) ? typeof(object) : retType;
                return _syncHandler.MakeGenericMethod(retType).Invoke(this, new object[] { target, args, logger });
            }
        }

        private static T WrapSync<T>(Func<object[], object> target, object[] args, IExceptionStateLogger logger)
        {
            try
            {
                return (T)target(args);
            }
            catch (Exception ex)
            {
                logger.Log(ex);
            }

            return default;
        }

        private static async Task<T> WrapAsync<T>(Func<object[], object> target, object[] args, IExceptionStateLogger logger)
        {
            try
            {
                return await ((Task<T>)target(args)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                logger.Log(ex);
            }

            return default;
        }
    }
}
