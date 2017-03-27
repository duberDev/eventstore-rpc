namespace EventStore.RPC.Server
{
    public static class UserCredentialsExtensions
    {
        public static ClientAPI.SystemData.UserCredentials ToUserCredentials(this UserCredentials userCredentials)
        {
            return userCredentials == null
                ? null
                : new ClientAPI.SystemData.UserCredentials(userCredentials.Username, userCredentials.Password);
        }
    }
}