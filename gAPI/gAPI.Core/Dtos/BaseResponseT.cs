using gAPI.Core.Enums;
using System.Net.Mail;

namespace gAPI.Core.Dtos;

public class BaseResponseT<T> : BaseResponse
{
    public BaseResponseT()
    {
    }

    public BaseResponseT(T response)
    {
        Response = response;
    }

    public T? Response { get; set; }

    public T GaurdIfNull()
    { 
        if (Response == null || Error.HasValue)
            throw new Exception(Enum.GetName(
                Error.HasValue
                ? Error.Value
                : BaseResponseErrorEnum.ErrorNotSpecified));
        return Response;
    }

}