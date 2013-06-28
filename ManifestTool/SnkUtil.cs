using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;

namespace Ionic.AppUpdater
{
    public sealed class SnkUtil
    {
        private const int MagicPrivateIndex = 8;
        private const int MagicPublicIndex = 20; 
        private const int MagicSize = 4; 

        private static byte[] GetFileBytes(string path)
        {
            Stream stm = File.OpenRead(path);
            using(stm)
            {
                byte[] buffer = new byte[stm.Length];
                stm.Read(buffer, 0, buffer.Length);
                return buffer;
            }
        } 

        private static byte[] Copy(byte[] src, int index, int size)
        {
            if ((src == null) || (src.Length < (index + size)))
                return null; 

            byte[] dest = new byte[size];
            Array.Copy(src, index, dest, 0, size); 
            return dest;
        } 

        private static bool Check(byte[] bytes, byte[] check, int index)
        {
            if (bytes == null)
                throw(new ArgumentNullException("bytes"));
            if (check == null)
                throw(new ArgumentNullException("check"));
            if (bytes.Length < index + check.Length)
                throw(new ArgumentException("array bounds violation: index + check.Length"));

            for (int i = 0; i < check.Length; i++)
            {
                if (bytes[i + index] != check[i])
                    return false; 
            } 
            return true; 
        }

        /// <summary>
        /// Check that RSA1 is in header (public key only).
        /// </summary>
        /// <param name="keypair"></param>
        /// <returns></returns>
        private static bool CheckRSA1(byte[] bytes)
        {
            // Check that RSA1 is in header.
            // R S A 1 
            byte[] check = new byte[] {0x52, 0x53, 0x41, 0x31};
            return Check(bytes, check, MagicPublicIndex);
        } 

        /// <summary>
        /// Check that RSA2 is in header (public and private key).
        /// </summary>
        /// <param name="keypair"></param>
        /// <returns></returns>
        private static bool CheckRSA2(byte[] bytes)
        {
            // Check that RSA2 is in header.
            // R S A 2 
            byte[] check = new byte[] {0x52, 0x53, 0x41, 0x32};
            return Check(bytes, check, MagicPrivateIndex);
        } 

        /// <summary>
        /// Returns RSAParameters from byte[].
        /// Example to get rsa public key from assembly:
        /// byte[] pubkey = System.Reflection.Assembly.GetExecutingAssembly().GetName().GetPublicKey();
        /// RSAParameters p = SnkUtil.GetRSAParameters(pubkey); 
        /// </summary>
        /// <param name="keypair"></param>
        /// <returns></returns>
        private static RSAParameters GetRSAParameters(byte[] bytes)
        {
            if ((bytes == null) || (bytes.Length == 0))
                throw new ArgumentNullException("bytes"); 

            bool pubonly = (bytes.Length == 160);

            if (pubonly && !CheckRSA1(bytes))
                return new RSAParameters();

            if (!pubonly && !CheckRSA2(bytes))
                return new RSAParameters();

            RSAParameters parameters = new RSAParameters();

            int index = pubonly ? MagicPublicIndex : MagicPrivateIndex;
            index += MagicSize + 4;
            int size = 4;
            parameters.Exponent = Copy(bytes, index, size);
            Array.Reverse(parameters.Exponent);

            index += size;
            size = 128;
            parameters.Modulus = Copy(bytes, index, size);
            Array.Reverse(parameters.Modulus); 

            if (pubonly)
                return parameters;

            // Figure private params 
            // Must reverse order (little vs. big endian issue)

            index += size;
            size = 64; 
            parameters.P = Copy(bytes, index, size);
            Array.Reverse(parameters.P); 

            index += size;
            size = 64;
            parameters.Q = Copy(bytes, index, size); 
            Array.Reverse(parameters.Q); 

            index += size;
            size = 64;
            parameters.DP = Copy(bytes, index, size);
            Array.Reverse(parameters.DP); 

            index += size;
            size = 64;
            parameters.DQ = Copy(bytes, index, size);
            Array.Reverse(parameters.DQ); 

            index += size;
            size = 64;
            parameters.InverseQ = Copy(bytes, index, size);
            Array.Reverse(parameters.InverseQ); 

            index += size;
            size = 128;
            parameters.D = Copy(bytes, index, size);
            Array.Reverse(parameters.D); 

            return parameters;
        }

        private static RSACryptoServiceProvider GetRsaProvider(byte[] bytes)
        {
            if (bytes == null)
                throw new ArgumentNullException("bytes"); 

            RSAParameters parameters = GetRSAParameters(bytes); 

            // Must set KeyNumber to AT_SIGNATURE for strong
            // name keypair to be correctly imported. 
            // CspParameters cp = new CspParameters(); 
            // cp.KeyNumber = 2; // AT_SIGNATURE 

            // RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(1024, cp);
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.ImportParameters(parameters);
            return rsa;
        }

        /// <summary>
        /// Returns RSA object from *.snk key file.
        /// </summary>
        /// <param name="path">Path to snk file.</param>
        /// <returns>RSACryptoServiceProvider</returns>
        public static RSACryptoServiceProvider GetRsaProvider(string path)
        {
            if (path == null)
                throw new ArgumentNullException("path"); 

            byte[] bytes = GetFileBytes(path);
            if (bytes == null)
                throw new Exception("Invalid SNK file."); 

            RSACryptoServiceProvider rsa = GetRsaProvider(bytes);
            return rsa;
        } 

        public static RSACryptoServiceProvider GetRsaProvider(System.Reflection.Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly"); 

            byte[] bytes = assembly.GetName().GetPublicKey();
            if (bytes.Length == 0)
                throw new Exception("No public key in assembly."); 

            RSAParameters parameters = GetRSAParameters(bytes); 
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.ImportParameters(parameters);
            return rsa;
        }
    }

}