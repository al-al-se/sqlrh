using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
using System.Text;
using Microsoft.Extensions.Options;

public class MyPasswordHasher : IPasswordHasher<SqlrhUser>
{
    IOptions<PasswordHasherOptions> _options;

    public MyPasswordHasher(IOptions<PasswordHasherOptions> o)
    {
        _options = o;
    }

    public string GenerateSaltedPassword(SqlrhUser u, string password)
    {
        return $"{password}{1000 - password.Length}{_options.Value.Salt}{u.Login}";
    }

    public string HashPassword(SqlrhUser u, string password)
    {
        using (SHA256 mySHA256 = SHA256Managed.Create())
        {
            string saltedPassword = GenerateSaltedPassword(u,password);
            byte[] hash = mySHA256.ComputeHash(
                Encoding.UTF8.GetBytes(saltedPassword.ToString()));

            StringBuilder hashSB = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                hashSB.Append(hash[i].ToString("x2"));
            }
            return hashSB.ToString();
        }
    }

    public PasswordVerificationResult VerifyHashedPassword(
      SqlrhUser u, string hashedPassword, string providedPassword)
    {
        string saltedPassword = GenerateSaltedPassword(u,providedPassword);
        if (hashedPassword == HashPassword(u,saltedPassword))
            return PasswordVerificationResult.Success;
        else
            return PasswordVerificationResult.Failed;
    }
}