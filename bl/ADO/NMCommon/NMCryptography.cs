
// NMCryptography.cs

// * http://nexmi.com
// * NoErp Project - Nexmi Open ERP
// * Copyright (C) 2012, Nguyễn Quang Tuyến (tuyen.nq@nexmi.com), AUTHOR.txt (http://nexmi.com/about)
// * This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation; 
//   either version 2 of the License, or (at your option) any later version. This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
//   without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details. 
//   You should have received a copy of the GNU General Public License along with this library; if not, write to the Free Software Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
// *

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace NEXMI
{
    public  class NMCryptography
    {   
        public static string RSAEncryptWithPublic(string cleartext)
        {
            String XmlPrivateKeyString = "<RSAKeyValue><Modulus>7q0JvkFA3cLJ1iZg30X2v0nIZ5nbnLMBX+m7ppJSfi9+yfKAHBe9xkkqezDsBPginQUQOOZ6DYTU3JQzjqOHblMZW7s6ga3PtIUhmASOZqrgxzQNM2036/Stkm26ECp+/ncLXpR4T8OX3su6IbrTjW8tUL49vG3Cx6BQV6yzpwE=</Modulus><Exponent>AQAB</Exponent><P>+wwghH/KUVo81rBFHrv6fWho/lGrgzzMgKqoQsa/+8/2v69AZO+6qA5Npb4bLqVE/iQM4AFaRv/hoxpZzpULvQ==</P><Q>82JuXqgOWUBvADiXD/qqeHOs+kXT1sUukimwZ1EML1+SEaPpaUWLUcFiBHUYDEghmJT2BAhuRaGT5RPov6c6lQ==</Q><DP>fkbCUIOK+9K9CWxOyD/bppsPPOVNtUyuvQWab0RTq1EXF6IKbPsc042mRvlR6OuewYrWoxJrG414LLeDduMHrQ==</DP><DQ>J2YKudX4pKgSj/WZNqP8To1jNgxxs3WuCUkoXkE3eL++1I41La5tVCm65T/TScGvdpS+kn6O40S5vT6Q0d2PVQ==</DQ><InverseQ>0TMEdaNASg4mI+3DBI47YJSWoMM9Q4Cr67cWFxlu+lq2FaV4Ks98/qGPiFUnN9EKSAPd3x1pKDIAMgY0s7KTng==</InverseQ><D>pOrj5JZqH9FMbbCvFxfc22g8FXn2iVwXAxMaGorYfWCMFzZ81uyAsNY1iFHRg82A/UZWYzq980FpY9DznwfXt0oHyusyWZ5hstHPWdr07mAKEfyn4/myN/OWiZdoM9viJ8SOd8jW4ZiPviFIftvZR9Yg5vpzE3zn7TXHtxgKHME=</D></RSAKeyValue>";
            CspParameters cspParams = new CspParameters(1);
            RSACryptoServiceProvider RsaProvider = new RSACryptoServiceProvider(4096, cspParams);
            RsaProvider.FromXmlString(XmlPrivateKeyString);
            byte[] plainbytes = Encoding.Unicode.GetBytes(cleartext);
            byte[] cipherbytes = RsaProvider.Encrypt(plainbytes, false);
            return Convert.ToBase64String(cipherbytes);
        }

        public static string RSAEncryptWithPrivate(string cleartext)
        {
            String XmlPrivateKeyString = "<RSAKeyValue><Modulus>7q0JvkFA3cLJ1iZg30X2v0nIZ5nbnLMBX+m7ppJSfi9+yfKAHBe9xkkqezDsBPginQUQOOZ6DYTU3JQzjqOHblMZW7s6ga3PtIUhmASOZqrgxzQNM2036/Stkm26ECp+/ncLXpR4T8OX3su6IbrTjW8tUL49vG3Cx6BQV6yzpwE=</Modulus><Exponent>AQAB</Exponent><P>+wwghH/KUVo81rBFHrv6fWho/lGrgzzMgKqoQsa/+8/2v69AZO+6qA5Npb4bLqVE/iQM4AFaRv/hoxpZzpULvQ==</P><Q>82JuXqgOWUBvADiXD/qqeHOs+kXT1sUukimwZ1EML1+SEaPpaUWLUcFiBHUYDEghmJT2BAhuRaGT5RPov6c6lQ==</Q><DP>fkbCUIOK+9K9CWxOyD/bppsPPOVNtUyuvQWab0RTq1EXF6IKbPsc042mRvlR6OuewYrWoxJrG414LLeDduMHrQ==</DP><DQ>J2YKudX4pKgSj/WZNqP8To1jNgxxs3WuCUkoXkE3eL++1I41La5tVCm65T/TScGvdpS+kn6O40S5vT6Q0d2PVQ==</DQ><InverseQ>0TMEdaNASg4mI+3DBI47YJSWoMM9Q4Cr67cWFxlu+lq2FaV4Ks98/qGPiFUnN9EKSAPd3x1pKDIAMgY0s7KTng==</InverseQ><D>pOrj5JZqH9FMbbCvFxfc22g8FXn2iVwXAxMaGorYfWCMFzZ81uyAsNY1iFHRg82A/UZWYzq980FpY9DznwfXt0oHyusyWZ5hstHPWdr07mAKEfyn4/myN/OWiZdoM9viJ8SOd8jW4ZiPviFIftvZR9Yg5vpzE3zn7TXHtxgKHME=</D></RSAKeyValue>";
            CspParameters cspParams = new CspParameters(1);
            RSACryptoServiceProvider RsaProvider = new RSACryptoServiceProvider(4096, cspParams);
            RsaProvider.FromXmlString(XmlPrivateKeyString);
            byte[] plainbytes = Encoding.Unicode.GetBytes(cleartext);
            byte[] cipherbytes = RsaProvider.Encrypt(plainbytes, false);
            return Convert.ToBase64String(cipherbytes);
        }

        public static string RSADecrypt(string ciphertext)
        {
            string cleartext = "";
            try
            {
                String XmlPrivateKeyString = "<RSAKeyValue><Modulus>7q0JvkFA3cLJ1iZg30X2v0nIZ5nbnLMBX+m7ppJSfi9+yfKAHBe9xkkqezDsBPginQUQOOZ6DYTU3JQzjqOHblMZW7s6ga3PtIUhmASOZqrgxzQNM2036/Stkm26ECp+/ncLXpR4T8OX3su6IbrTjW8tUL49vG3Cx6BQV6yzpwE=</Modulus><Exponent>AQAB</Exponent><P>+wwghH/KUVo81rBFHrv6fWho/lGrgzzMgKqoQsa/+8/2v69AZO+6qA5Npb4bLqVE/iQM4AFaRv/hoxpZzpULvQ==</P><Q>82JuXqgOWUBvADiXD/qqeHOs+kXT1sUukimwZ1EML1+SEaPpaUWLUcFiBHUYDEghmJT2BAhuRaGT5RPov6c6lQ==</Q><DP>fkbCUIOK+9K9CWxOyD/bppsPPOVNtUyuvQWab0RTq1EXF6IKbPsc042mRvlR6OuewYrWoxJrG414LLeDduMHrQ==</DP><DQ>J2YKudX4pKgSj/WZNqP8To1jNgxxs3WuCUkoXkE3eL++1I41La5tVCm65T/TScGvdpS+kn6O40S5vT6Q0d2PVQ==</DQ><InverseQ>0TMEdaNASg4mI+3DBI47YJSWoMM9Q4Cr67cWFxlu+lq2FaV4Ks98/qGPiFUnN9EKSAPd3x1pKDIAMgY0s7KTng==</InverseQ><D>pOrj5JZqH9FMbbCvFxfc22g8FXn2iVwXAxMaGorYfWCMFzZ81uyAsNY1iFHRg82A/UZWYzq980FpY9DznwfXt0oHyusyWZ5hstHPWdr07mAKEfyn4/myN/OWiZdoM9viJ8SOd8jW4ZiPviFIftvZR9Yg5vpzE3zn7TXHtxgKHME=</D></RSAKeyValue>";
                CspParameters cspParams = new CspParameters(1);
                RSACryptoServiceProvider RsaProvider = new RSACryptoServiceProvider(4096, cspParams);
                RsaProvider.FromXmlString(XmlPrivateKeyString);
                byte[] cipherbytes = Convert.FromBase64String(ciphertext);
                byte[] plain = RsaProvider.Decrypt(cipherbytes, false);
                cleartext = System.Text.Encoding.Unicode.GetString(plain);
            }
            catch
            {
                throw;
            }

            return cleartext;
        }

        public static string ECBEncrypt(string strToEncrypt)
        {
            String strKey = "JVL";
            try
            {
                TripleDESCryptoServiceProvider objDESCrypto = new TripleDESCryptoServiceProvider();
                MD5CryptoServiceProvider objHashMD5 = new MD5CryptoServiceProvider();

                byte[] byteHash, byteBuff;
                string strTempKey = strKey;

                byteHash = objHashMD5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(strTempKey));
                objHashMD5 = null;
                objDESCrypto.Key = byteHash;
                objDESCrypto.Mode = CipherMode.ECB; //CBC, CFB

                byteBuff = ASCIIEncoding.ASCII.GetBytes(strToEncrypt);
                return Convert.ToBase64String(objDESCrypto.CreateEncryptor().TransformFinalBlock(byteBuff, 0, byteBuff.Length));
            }
            catch (Exception ex)
            {
                return "Wrong Input. " + ex.Message;
            }
        }

        public static string ECBEncrypt(string strToEncrypt, string strKey)
        {
            try
            {
                TripleDESCryptoServiceProvider objDESCrypto = new TripleDESCryptoServiceProvider();
                MD5CryptoServiceProvider objHashMD5 = new MD5CryptoServiceProvider();

                byte[] byteHash, byteBuff;
                string strTempKey = strKey;

                byteHash = objHashMD5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(strTempKey));
                objHashMD5 = null;
                objDESCrypto.Key = byteHash;
                objDESCrypto.Mode = CipherMode.ECB; //CBC, CFB

                byteBuff = ASCIIEncoding.ASCII.GetBytes(strToEncrypt);
                return Convert.ToBase64String(objDESCrypto.CreateEncryptor().TransformFinalBlock(byteBuff, 0, byteBuff.Length));
            }
            catch (Exception ex)
            {
                return "Wrong Input. " + ex.Message;
            }
        }

        public static string ECBDecrypt(string strEncrypted, string strKey)
        {
            try
            {
                TripleDESCryptoServiceProvider objDESCrypto = new TripleDESCryptoServiceProvider();
                MD5CryptoServiceProvider objHashMD5 = new MD5CryptoServiceProvider();

                byte[] byteHash, byteBuff;
                string strTempKey = strKey;

                byteHash = objHashMD5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(strTempKey));
                objHashMD5 = null;
                objDESCrypto.Key = byteHash;
                objDESCrypto.Mode = CipherMode.ECB; //CBC, CFB

                byteBuff = Convert.FromBase64String(strEncrypted);
                string strDecrypted = ASCIIEncoding.ASCII.GetString(objDESCrypto.CreateDecryptor().TransformFinalBlock(byteBuff, 0, byteBuff.Length));
                objDESCrypto = null;

                return strDecrypted;
            }
            catch (Exception ex)
            {
                return "Wrong Input. " + ex.Message;
            }
        }

        public static string ECBDecrypt(string strEncrypted)
        {
            String strKey = "JVL";
            try
            {
                TripleDESCryptoServiceProvider objDESCrypto = new TripleDESCryptoServiceProvider();
                MD5CryptoServiceProvider objHashMD5 = new MD5CryptoServiceProvider();

                byte[] byteHash, byteBuff;
                string strTempKey = strKey;

                byteHash = objHashMD5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(strTempKey));
                objHashMD5 = null;
                objDESCrypto.Key = byteHash;
                objDESCrypto.Mode = CipherMode.ECB; //CBC, CFB

                byteBuff = Convert.FromBase64String(strEncrypted);
                string strDecrypted = ASCIIEncoding.ASCII.GetString(objDESCrypto.CreateDecryptor().TransformFinalBlock(byteBuff, 0, byteBuff.Length));
                objDESCrypto = null;

                return strDecrypted;
            }
            catch (Exception ex)
            {
                return "Wrong Input. " + ex.Message;
            }
        }

        public static string base64Encode(string data)
        {
            try
            {
                byte[] encData_byte = new byte[data.Length];
                encData_byte = System.Text.Encoding.UTF8.GetBytes(data);
                string encodedData = Convert.ToBase64String(encData_byte);
                return encodedData;
            }
            catch
            {
                //throw new Exception("Error in base64Encode" + e.Message);
                return "";
            }
        }

        public static string base64Decode(string data)
        {
            try
            {
                System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
                System.Text.Decoder utf8Decode = encoder.GetDecoder();

                byte[] todecode_byte = Convert.FromBase64String(data);
                int charCount = utf8Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
                char[] decoded_char = new char[charCount];
                utf8Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
                string result = new String(decoded_char);
                return result;
            }
            catch
            {
                //throw new Exception("Error in base64Decode" + e.Message);
                return "";
            }
        }
    }
}
