namespace MicroServiceBase.Contract
{
    public interface IResponce : IRMSSerializable
    {
        string ErrorMsg { get; set; }
        ServiceOperationResult Result { get; set; }
    }

    public enum ServiceOperationResult
    {
        OK,
        Warning,
        DbUnavailable,
        DbError,
        HasNoRights,
        BadParams,
        InternalServiceError,
        ExternalServiceError,
        ExtServUnavailable
    }
}