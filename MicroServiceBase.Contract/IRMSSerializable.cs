namespace MicroServiceBase.Contract
{
    public interface IRMSSerializable
    {
        byte[] GetBytes();
        string GetString();
    }
}