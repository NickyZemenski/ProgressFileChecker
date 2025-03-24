using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ServiceApp
{
    public class RequestFileUploadModel
    {
        public string Id { get; set; }

        public string hashtype { get; set; } = "sha-256";
        public string hash { get; set; }
        public string file { get; set; }
        public string comments { get; set; } = "upload by api";

        public Dictionary<string, string> ToParameters => new Dictionary<string, string>() { { "Id",Id},{ "hashtype",hashtype},{ "hash", hash },{ "comments", comments }  };
        public static string GetFileHash(string filePath)
        {
            // Convert the input string to a byte array and compute the hash.
            byte[] data = SHA256.Create().ComputeHash(File.ReadAllBytes(filePath));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            var sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();

        }

    }


  
}
