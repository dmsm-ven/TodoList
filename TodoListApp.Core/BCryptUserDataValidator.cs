namespace TodoListApp.Core;

public class BCryptUserDataValidator : IUserDataEncryptValidator
{
    public bool Verify(string userPassword, string expectedPasswordHash)
    {
        return BCrypt.Net.BCrypt.Verify(userPassword, expectedPasswordHash);
    }
}
