namespace DevStore.Billing.DevsPay
{
    public class DevsPayService(string apiKey, string encryptionKey)
    {
        public readonly string ApiKey = apiKey;
        public readonly string EncryptionKey = encryptionKey;
    }
}