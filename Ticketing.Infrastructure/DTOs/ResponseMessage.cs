using System.Runtime.CompilerServices;
using Ticketing.Infrastructure.DTOs;
using Ticketing.Infrastructure.Extensions;

namespace Ticketing.Application.Model.DTOs
{
    public class MessageStatus
    {
        public const string success = "success";
        public const string error = "error";
        public const string warning = "warning";
    }


    public interface IResponseMessage
    {
        public bool issuccess { get; }
        public string status { get; set; }
        public string message { get; set; }
        public int errorcode { get; set; }
        public IEnumerable<BaseError> errors { get; set; }
    }

    public class ResponseMessage<DataType> : IResponseMessage
    {
        public void SetError(
            KeyValuePair<int, string> message,
            string messageCustom = "")
        {
            this.message = message.Value + messageCustom;
            this.errorcode = message.Key;
            this.status = MessageStatus.error;
        }

        public void SetError(
            int erorCode,
            string message)
        {
            this.message = string.Format("{0}: {1}", erorCode, message);
            this.errorcode = erorCode;
            this.status = MessageStatus.error;
        }

        public void SetWarning(
            KeyValuePair<int, string> message,
            string messageCustom = "")
        {
            this.message = message.Value + messageCustom;
            this.errorcode = message.Key;
            this.status = MessageStatus.warning;
        }

        public void SetWarning(
            int erorCode,
            string message)
        {
            this.message = string.Format("{0}: {1}", erorCode, message);
            this.errorcode = erorCode;
            this.status = MessageStatus.warning;
        }

        public void SetMessageWarning(
            string message)
        {
            this.message = message;
            this.status = MessageStatus.warning;
        }

        public void SetMessageSuccess(
            string message)
        {
            this.message = message;
            this.status = MessageStatus.success;
        }

        public void SetMessageSuccess(DataType data, string message = "Lấy dữ liệu thành công")
        {
            this.data = data;
            this.message = message;
            this.status = MessageStatus.success;
        }

        public void setMessage(ResponseMessage<DataType> response)
        {
            if (response == null)
            {
                this.message = "Không có dữ liệu";
                this.status = MessageStatus.error;
            }
            else
            {
                this.message = response.message;
                this.errorcode = response.errorcode;
                this.status = response.status;
                this.data = response.data;
            }
        }

        public void setMessage<T>(ResponseMessage<T> response)
        {
            if (response == null)
            {
                this.message = "Không có dữ liệu";
                this.status = MessageStatus.error;
            }
            else
            {
                this.message = response.message;
                this.errorcode = response.errorcode;
                this.status = response.status;
            }
        }

        public void setMessage<T>(ResponseMessage<T> response, out T output)
        {
            if (response == null)
            {
                this.message = "Không có dữ liệu";
                this.status = MessageStatus.error;
                output = default;
            }
            else
            {
                this.message = response.message;
                this.errorcode = response.errorcode;
                this.status = response.status;
                output = response.data;
            }
        }

        public T getMessage<T>(ResponseMessage<T> response)
        {
            T rs = default;
            if (response == null)
            {
                this.message = "Không có dữ liệu";
                this.status = MessageStatus.error;
            }
            else
            {
                this.message = response.message;
                this.errorcode = response.errorcode;
                this.status = response.status;
                rs = response.data;
            }

            return rs;
        }

        public void SetMessageError(string message)
        {
            this.message = message;
            this.status = MessageStatus.error;
        }

        public DataType setMessage(DataType value, DataType result, string ok, string error)
        {
            if (EqualityComparer<DataType>.Default.Equals(result, value))
            {
                this.message = ok;
                this.status = MessageStatus.success;
            }
            else
            {
                this.message = error;
                this.status = MessageStatus.error;
            }

            return result;
        }

        public ResponseMessage<DataType> MessageError(string message, BaseError error = null)
        {
            this.message = message;
            this.status = MessageStatus.error;
            if (error is not null)
            {
                this.errors = (this.errors ?? []).Append(error);
            }

            return this;
        }

        public ResponseMessage<DataType> MessageSuccess(DataType data, string message = "Xử lý thành công")
        {
            this.data = data;
            this.message = message;
            this.status = MessageStatus.success;
            return this;
        }

        public ResponseMessage<DataType> MessageWarning(string message)
        {
            this.message = message;
            this.status = MessageStatus.warning;
            return this;
        }

        public ResponseMessage<DataType> Message(string ok, string error)
        {
            if (ObjectExtensions.HasValue(data))
            {
                this.message = ok;
                this.status = MessageStatus.success;
            }
            else
            {
                this.message = error;
                this.status = MessageStatus.error;
            }

            return this;
        }

        public ResponseMessage<DataType> Message(string ok, string error, DataType data = default)
        {
            if (ObjectExtensions.HasValue(data))
            {
                this.message = ok;
                this.data = data;
                this.status = MessageStatus.success;
            }
            else
            {
                this.message = error;
                this.status = MessageStatus.error;
            }

            return this;
        }


        public static ResponseMessage<DataType> Success(string message)
        {
            bool temp = true;
            return new ResponseMessage<DataType>
            {
                data = typeof(DataType) == typeof(bool) ? Unsafe.As<bool, DataType>(ref temp) : default!,
                status = MessageStatus.success,
                message = message
            };
        }

        public static ResponseMessage<DataType> Success(DataType data,
            string message = "Xử lý thành công") => new ResponseMessage<DataType>
            { data = data, status = MessageStatus.success, message = message };

        public static ResponseMessage<DataType> Create(DataType value, DataType result, string ok, string error)
        {
            var isok = EqualityComparer<DataType>.Default.Equals(result, value);
            return new ResponseMessage<DataType>
            {
                data = result,
                message = isok ? ok : error,
                status = isok ? MessageStatus.success : MessageStatus.error
            };
        }

        public static ResponseMessage<DataType> Create(DataType data,
            string successMessage = "Xử lý thành công",
            string errorMessage = "Lấy dữ liệu thất bại")
        {
            var success = ObjectExtensions.HasValue(data);
            return new ResponseMessage<DataType>
            {
                data = data,
                message = success ? successMessage : errorMessage,
                status = success ? MessageStatus.success : MessageStatus.error
            };
        }

        public bool TryGetData(
            out DataType data,
            out string msg)
        {
            msg = this.message ?? string.Empty;
            if (this.status == MessageStatus.success)
            {
                data = this.data;
                return true;
            }

            data = default;
            return false;
        }

        public static ResponseMessage<DataType> Error(string message, IEnumerable<BaseError> errors = null) =>
            new ResponseMessage<DataType> { message = message, errors = errors, status = MessageStatus.error };

        public static ResponseMessage<DataType> Error(string message, BaseError error) =>
            new ResponseMessage<DataType> { message = message, errors = [error], status = MessageStatus.error };

        public static ResponseMessage<DataType> Error(string message, string exceptionMessage) =>
            new ResponseMessage<DataType> { message = message, errors = [new("Exception", exceptionMessage)], status = MessageStatus.error };

        public static ResponseMessage<DataType> Warning(string message) => new ResponseMessage<DataType> { message = message, status = MessageStatus.warning };

        /// <summary>
        /// Thành công ghi dữ liệu 
        /// Dùng cho các thao tác ghi dữ liệu: Create, Update, Delete, Import, Init, Upload, Download
        /// </summary>
        /// <returns></returns>
        public static ResponseMessage<DataType> SuccessCommand(DataType data, string message = "Khởi tạo dữ liệu thành công") =>
            new ResponseMessage<DataType> { data = data, status = MessageStatus.success, message = message };

        public string status { get; set; } = MessageStatus.success;
        public string message { get; set; }

        public virtual DataType data { get; set; }
        public int errorcode { get; set; }
        public IEnumerable<BaseError> errors { get; set; }
        public bool issuccess => status == MessageStatus.success;
    }

    public class ResponseMessage : IResponseMessage
    {
        public bool issuccess => status == MessageStatus.success;
        public string status { get; set; } = MessageStatus.success;
        public string message { get; set; }
        public int errorcode { get; set; }
        public object data { get; set; }
        public IEnumerable<BaseError> errors { get; set; }
    }
}