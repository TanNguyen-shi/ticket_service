using System.Text.Json;
using Ticketing.Application.Model.DTOs;
using Ticketing.Infrastructure.DTOs;
using Ticketing.Infrastructure.Extensions;
using Ticketing.Infrastructure.Helpers;
using Ticketing.Infrastructure.Repositories;

namespace Ticketing.Domain.Domain
{
    public interface IBaseService<TEntity, TPrimaryKey>
    {
        Task<ResponseMessage<TPrimaryKey>> InsertAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task<ResponseMessage<bool>> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task<ResponseMessage<bool>> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);

        Task<ResponseMessage<TResponse?>> GetAsync<TResponse, TRequest>(
            TRequest request,
            long? userLogin,
            CancellationToken cancellationToken = default);

        Task<ResponseMessage<IEnumerable<TResponse>>> GetPagedAsync<TResponse, TRequest>(
            TRequest request,
            long? userLogin,
            CancellationToken cancellationToken = default);

        Task<ResponseMessage<IEnumerable<TResponse>>> GetAllAsync<TResponse, TRequest>(
            TRequest request,
            long? userLogin,
            CancellationToken cancellationToken = default);

    }

    public abstract class BaseService<TRepo, TEntity>(TRepo repository, Enum entityType)
        where TEntity : class
        where TRepo : IGenericRepository<TEntity>
    {
        protected readonly string _entityName = entityType.GetDisplayName();
        protected readonly Enum _entityType = entityType;
        protected readonly TRepo _repository = repository;

        protected virtual TRequest NormalizeRequest<TRequest>(TRequest request)
        {
            //Xử lý 

            return request;
        }

        protected virtual Dictionary<string, object?> BuildParams<TRequest>(TRequest request, long? userLogin)
        {
            var dict = ParamExtensions.ToDictionary(request, userLogin);
            return new Dictionary<string, object?>(dict);
        }

        public virtual async Task<ResponseMessage<TResponse?>> GetAsync<TResponse, TRequest>(
            TRequest request,
            long? userLogin,
            CancellationToken cancellationToken = default)
        {
            request = NormalizeRequest(request);
            var param = BuildParams(request, userLogin);

            return await ExecuteAsync(
                async () => await _repository.GetAsync<TResponse>(param, cancellationToken),
                successMessage: "Lấy dữ liệu thành công",
                warningMessage: "Không tìm thấy dữ liệu",
                errorMessage: "Lấy dữ liệu thất bại",
                logData: request);
        }

        public virtual async Task<ResponseMessage<IEnumerable<TResponse>>> GetAllAsync<TResponse, TRequest>(
            TRequest request,
            long? userLogin,
            CancellationToken cancellationToken = default)
        {
            request = NormalizeRequest(request);
            var param = BuildParams(request, userLogin);

            return await ExecuteCollectionAsync(
                async () => await _repository.GetAllAsync<TResponse>(param, cancellationToken),
                successMessage: "Lấy dữ liệu thành công",
                warningMessage: "Không có dữ liệu",
                errorMessage: "Lấy dữ liệu thất bại",
                logData: request);
        }

        public virtual async Task<ResponseMessage<IEnumerable<TResponse>>> GetPagedAsync<TResponse, TRequest>(
            TRequest request,
            long? userLogin,
            CancellationToken cancellationToken = default)
        {
            request = NormalizeRequest(request);
            var param = BuildParams(request, userLogin);

            return await ExecuteCollectionAsync(
                async () => await _repository.GetPagedAsync<TResponse>(param, cancellationToken),
                successMessage: "Lấy dữ liệu thành công",
                warningMessage: "Không có dữ liệu",
                errorMessage: "Lấy dữ liệu thất bại",
                logData: request);
        }


        public virtual async Task<ResponseMessage<long>> InsertAsync<TRequest>(
            TRequest request,
            long? userLogin,
            CancellationToken cancellationToken = default)
        {
            request = NormalizeRequest(request);
            var param = BuildParams(request, userLogin);

            return await ExecuteAsync(
                async () =>
                {
                    var result = await _repository.InsertAsync(param, cancellationToken);
                    return result.ToLong();
                },
                successMessage: "Thêm mới dữ liệu thành công",
                warningMessage: "Không thể thêm dữ liệu",
                errorMessage: "Thêm mới dữ liệu thất bại",
                logData: request);
        }

        public virtual async Task<ResponseMessage<bool>> UpdateAsync<TRequest>(
            TRequest request,
            long? userLogin,
            CancellationToken cancellationToken = default)
        {
            request = NormalizeRequest(request);
            var param = BuildParams(request, userLogin);

            return await ExecuteAsync(
                async () =>
                {
                    var result = await _repository.UpdateAsync(param, cancellationToken);
                    return result.ToBool();
                },
                successMessage: "Cập nhật dữ liệu thành công",
                warningMessage: "Không thể cập nhật dữ liệu",
                errorMessage: "Cập nhật dữ liệu thất bại",
                logData: request);
        }

        public virtual async Task<ResponseMessage<bool>> DeleteAsync<TRequest>(
            TRequest request,
            long? userLogin,
            CancellationToken cancellationToken = default)
        {
            request = NormalizeRequest(request);
            var param = BuildParams(request, userLogin);

            return await ExecuteAsync(
                async () =>
                {
                    var result = await _repository.DeleteAsync(param, cancellationToken);
                    return result.ToBool();
                },
                successMessage: "Xóa dữ liệu thành công",
                warningMessage: "Không thể xóa dữ liệu",
                errorMessage: "Xóa dữ liệu thất bại",
                logData: request);
        }

        protected async Task<ResponseMessage<T>> ExecuteAsync<T>(
            Func<Task<T>> action,
            string successMessage,
            string warningMessage,
            string errorMessage,
            object? logData = null)
        {
            try
            {
                var result = await action();

                if (!HasValue(result))
                {
                    return ResponseMessage<T>.Warning(warningMessage);
                }

                return ResponseMessage<T>.Success(result, successMessage);
            }
            catch (Exception ex)
            {
                LogError(errorMessage, ex, logData);
                return ResponseMessage<T>.Error(errorMessage);
            }
        }

        protected async Task<ResponseMessage<IEnumerable<T>>> ExecuteCollectionAsync<T>(
            Func<Task<IEnumerable<T>>> action,
            string successMessage,
            string warningMessage,
            string errorMessage,
            object? logData = null)
        {
            try
            {
                var result = (await action()).ToList();

                if (result.Count == 0)
                {
                    return ResponseMessage<IEnumerable<T>>.Warning(warningMessage);
                }

                return ResponseMessage<IEnumerable<T>>.Success(result, successMessage);
            }
            catch (Exception ex)
            {
                LogError(errorMessage, ex, logData);
                return ResponseMessage<IEnumerable<T>>.Error(errorMessage);
            }
        }

        protected virtual bool HasValue<T>(T? value)
        {
            if (value is null)
                return false;

            if (value is string s)
                return !string.IsNullOrWhiteSpace(s);

            if (value is bool b)
                return b;

            if (value is int i)
                return i != 0;

            if (value is long l)
                return l != 0;

            return true;
        }

        protected virtual void LogError(string message, Exception ex, object? logData = null)
        {
            try
            {
                var payload = logData is null ? string.Empty : JsonSerializer.Serialize(logData);
                Console.WriteLine($"[{_entityName}] {message} - {ex.Message} - {payload}");
            }
            catch
            {
                Console.WriteLine($"[{_entityName}] {message} - {ex.Message}");
            }
        }
    }
}