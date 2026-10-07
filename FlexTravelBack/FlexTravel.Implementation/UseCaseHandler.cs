using FlexTravel.Application.Dto;
using FlexTravel.Application.UseCases;
using FlexTravel.Implementation.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;
using System.Diagnostics;

namespace FlexTravel.Implementation
{
    public class UseCaseHandler
    {
        private readonly IUser _user;
        private readonly IUseCaseLogger _useCaseLogger;

        public UseCaseHandler(IUser user, IUseCaseLogger useCaseLogger)
        {
            _user = user;
            _useCaseLogger = useCaseLogger;
        }

        public void HandleCommand<TRequest>(ICommand<TRequest> command, TRequest data)
        {
            HandleLoggingAndAuthorization(command, data);

            var sw = Stopwatch.StartNew();
            command.Execute(data);
            sw.Stop();

            Console.WriteLine("Command name: " + command.UseCaseName + ", Duration: " + sw.ElapsedMilliseconds + "ms");
        }

        public TResponse HandleCommand<TRequest, TResponse>(ICommand<TRequest, TResponse> command, TRequest data)
        {
            HandleLoggingAndAuthorization(command, data);

            var sw = Stopwatch.StartNew();
            var response = command.Execute(data);
            sw.Stop();

            Console.WriteLine("Command name: " + command.UseCaseName + ", Duration: " + sw.ElapsedMilliseconds + "ms");

            return response;
        }

        public TResponse HandleQuery<TRequest, TResponse>(IQuery<TRequest, TResponse> query, TRequest data)
        {
            HandleLoggingAndAuthorization(query, data);

            var sw = Stopwatch.StartNew();
            var response = query.Execute(data);
            sw.Stop();

            Console.WriteLine("Query name: " + query.UseCaseName + ", Duration: " + sw.ElapsedMilliseconds + "ms");

            return response;
        }

        private void HandleLoggingAndAuthorization<TRequest>(IUseCase useCase, TRequest data)
        {
            var isAuthorized = _user.UseCaseIds.Contains(useCase.UseCaseId);
            var log = new UseCaseLogDto
            {
                User = _user.Identity,
                ExecutionDateTime = DateTime.UtcNow,
                UseCaseName = useCase.UseCaseName,
                UserId = _user.Id,
                Data = JsonConvert.SerializeObject(data, LogSerializerSettings),
                IsAuthorized = isAuthorized
            };

            if (!isAuthorized || useCase is not ISkipAuditLog)
            {
                _useCaseLogger.Log(log);
            }

            if (!isAuthorized)
            {
                throw new ForbiddenUseCaseExecutionException(useCase.UseCaseName, _user.Identity);
            }
        }

        private static readonly JsonSerializerSettings LogSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new SensitiveDataContractResolver()
        };

        private class SensitiveDataContractResolver : DefaultContractResolver
        {
            protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
            {
                var property = base.CreateProperty(member, memberSerialization);

                if (property.PropertyName != null && (property.PropertyName.Contains("Password", StringComparison.OrdinalIgnoreCase)
                    || property.PropertyName.Contains("Token", StringComparison.OrdinalIgnoreCase)))
                {
                    property.ValueProvider = new MaskedValueProvider();
                }

                if (property.PropertyType == typeof(byte[]))
                {
                    property.PropertyType = typeof(string);
                    property.ValueProvider = new SizeValueProvider(property.ValueProvider);
                }

                return property;
            }
        }

        private class SizeValueProvider : IValueProvider
        {
            private readonly IValueProvider _inner;

            public SizeValueProvider(IValueProvider inner)
            {
                _inner = inner;
            }

            public object GetValue(object target) => _inner.GetValue(target) is byte[] bytes ? $"{bytes.Length} bytes" : null;

            public void SetValue(object target, object value)
            {
            }
        }

        private class MaskedValueProvider : IValueProvider
        {
            public object GetValue(object target) => "***";

            public void SetValue(object target, object value)
            {
            }
        }
    }
}
