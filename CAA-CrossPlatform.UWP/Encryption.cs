using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

//helped by reading this article
//http://en.gravatar.com/andrasnemes

namespace CAA_CrossPlatform.UWP
{
    public class Encryption
    {
        public string Salt { get; }
        public string Digest { get; set; }

        public Encryption(string salt, string digest)
        {
            Salt = salt;
            Digest = digest;
        }

        //create hash from password + salt
        public static Encryption CreateHashSalt(string password)
        {
            byte[] saltBytes = GenerateRandomCryptographicBytes();
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            List<byte> passwordSaltBytes = new List<byte>();
            passwordSaltBytes.AddRange(passwordBytes);
            passwordSaltBytes.AddRange(saltBytes);
            byte[] digestBytes = SHA512.Create().ComputeHash(passwordSaltBytes.ToArray());
            return new Encryption(Convert.ToBase64String(saltBytes), Convert.ToBase64String(digestBytes));
        }

        //create random salt
        static byte[] GenerateRandomCryptographicBytes()
        {
            RNGCryptoServiceProvider rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            byte[] randomBytes = new byte[64];
            rngCryptoServiceProvider.GetBytes(randomBytes);
            return randomBytes;
        }

        //check hash and salt
        public static bool CheckHashSalt(string password, string digest, string salt)
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] saltBytes = Convert.FromBase64String(salt);
            List<byte> passwordSaltBytes = new List<byte>();
            passwordSaltBytes.AddRange(passwordBytes);
            passwordSaltBytes.AddRange(saltBytes);
            byte[] digestBytes = SHA512.Create().ComputeHash(passwordSaltBytes.ToArray());

            if (digest == Convert.ToBase64String(digestBytes))
                return true;

            return false;
        }
    }
}
