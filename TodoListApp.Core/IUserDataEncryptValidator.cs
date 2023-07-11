namespace TodoListApp.Core
{
    public interface IUserDataEncryptValidator
    {
        bool Verify(string userPassword, string expectedPasswordHash);
    }
}
