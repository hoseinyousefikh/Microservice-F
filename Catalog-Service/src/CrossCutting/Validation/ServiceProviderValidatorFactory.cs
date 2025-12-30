using Catalog_Service.src.CrossCutting.Validation.Admin;
using FluentValidation;

namespace Catalog_Service.src.CrossCutting.Validation
{
    public class ServiceProviderValidatorFactory : IValidatorFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Dictionary<Type, Type> _validatorTypes;

        public ServiceProviderValidatorFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _validatorTypes = new Dictionary<Type, Type>();

            // Scan the assembly and cache validator types
            var validatorAssembly = typeof(CreateCategoryValidator).Assembly;
            foreach (var type in validatorAssembly.GetTypes())
            {
                if (type.IsClass && !type.IsAbstract)
                {
                    var baseType = type.BaseType;
                    while (baseType != null && baseType.IsGenericType)
                    {
                        if (baseType.GetGenericTypeDefinition() == typeof(AbstractValidator<>))
                        {
                            var modelType = baseType.GetGenericArguments()[0];
                            _validatorTypes[modelType] = type;
                            break;
                        }
                        baseType = baseType.BaseType;
                    }
                }
            }
        }

        public IValidator<T> GetValidator<T>()
        {
            return GetValidator(typeof(T)) as IValidator<T>;
        }

        public IValidator GetValidator(Type type)
        {
            if (_validatorTypes.TryGetValue(type, out var validatorType))
            {
                return _serviceProvider.GetService(validatorType) as IValidator;
            }
            return null;
        }
    }
}
