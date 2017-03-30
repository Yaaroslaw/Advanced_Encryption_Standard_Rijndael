﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace AES
{
    class Program
    {
        public static void Main()
        {
            try
            {
                string line = "";
                int counter = 0;

                using (AesCryptoServiceProvider myAes = new AesCryptoServiceProvider())
                {

                    // Read the file and display it line by line.
                    using (StreamReader file = new StreamReader("FileToEncrypt.txt"))
                    using (FileStream byteWriter = new FileStream("EncryptedData.txt", FileMode.Append, FileAccess.Write))
                    using (StreamWriter resultFile = new StreamWriter("DecryptedData.txt"))
                    {
                        while ((line = file.ReadLine()) != null)
                        {
                            Console.WriteLine(line);
                            counter++;
                            var words = line.Split();
                            // Writes a block of bytes to this stream using data from
                            // a byte array.
                            byte[] encrypted = EncryptStringToBytes_Aes(words[1], myAes.Key, myAes.IV);
                            byteWriter.Write(encrypted, 0, encrypted.Length);
                            resultFile.Write(Convert.ToBase64String(encrypted));
                            // Decrypt the bytes to a string.
                            string roundtrip = DecryptStringFromBytes_Aes(encrypted, myAes.Key, myAes.IV);
                            //Base64Encode("WHAAAAT");
                            Console.WriteLine(Convert.ToBase64String(encrypted));
                            Console.WriteLine(Base64Decode(Convert.ToBase64String(encrypted)));
                            
                            Console.WriteLine("Round Trip: {0}", roundtrip);
                            resultFile.WriteLine(words[0] + encrypted);

                        }
                    }

                }

            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
            }

        }


        //Encrypt string using AesCryptoServiceProvider

        static byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;
            // Create an AesCryptoServiceProvider object
            // with the specified key and IV.
            using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {

                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                        //Console.WriteLine(plainText);
                    }
                }
            }


            // Return the encrypted bytes from the memory stream.
            return encrypted;

        }
       
        
        // Decrypt string using AesCryptoServiceProvider
        static string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an AesCryptoServiceProvider object
            // with the specified key and IV.
            using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }

            }

            return plaintext;

        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

    }
}