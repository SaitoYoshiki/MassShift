using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

public class MyCryptography : MonoBehaviour {

	private static readonly string EncryptKey = "ko3kdi39ks03kd0l2jlsdjfsdlfjsdlk";
	private static readonly int EncryptPasswordCount = 16;
	private static readonly string PasswordChars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
	private static readonly int PasswordCharsLength = PasswordChars.Length;


	//暗号化
	public static void Encrypt(string json, out string iv, out string base64) {

		byte[] src = Encoding.UTF8.GetBytes(json);
		byte[] dst;
		EncryptAes(src, out iv, out dst);
		base64 = Convert.ToBase64String(dst);
		
	}

	//復号化
	public static void Decrypt(string iv,string base64, out string json) {

		byte[] src = Convert.FromBase64String(base64);
		byte[] dst;
		DecryptAes(src, iv, out dst);
		json = Encoding.UTF8.GetString(dst).Trim('\0');

	}





	public static void EncryptAes(byte[] src, out string iv, out byte[] dst) {
		iv = CreatePassword(EncryptPasswordCount);
		dst = null;
		using (RijndaelManaged rijndael = new RijndaelManaged()) {
			rijndael.Padding = PaddingMode.PKCS7;
			rijndael.Mode = CipherMode.CBC;
			rijndael.KeySize = 256;
			rijndael.BlockSize = 128;

			byte[] key = Encoding.UTF8.GetBytes(EncryptKey);
			byte[] vec = Encoding.UTF8.GetBytes(iv);

			using (ICryptoTransform encryptor = rijndael.CreateEncryptor(key, vec))
			using (MemoryStream ms = new MemoryStream())
			using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write)) {
				cs.Write(src, 0, src.Length);
				cs.FlushFinalBlock();
				dst = ms.ToArray();
			}
		}
	}

	/// <summary>
	/// AES複合化
	/// </summary>
	public static void DecryptAes(byte[] src, string iv, out byte[] dst) {
		dst = new byte[src.Length];
		using (RijndaelManaged rijndael = new RijndaelManaged()) {
			rijndael.Padding = PaddingMode.PKCS7;
			rijndael.Mode = CipherMode.CBC;
			rijndael.KeySize = 256;
			rijndael.BlockSize = 128;

			byte[] key = Encoding.UTF8.GetBytes(EncryptKey);
			byte[] vec = Encoding.UTF8.GetBytes(iv);

			using (ICryptoTransform decryptor = rijndael.CreateDecryptor(key, vec))
			using (MemoryStream ms = new MemoryStream(src))
			using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read)) {
				cs.Read(dst, 0, dst.Length);
			}
		}
	}

	/// <summary>
	/// パスワード生成
	/// </summary>
	/// <param name="count">文字列数</param>
	/// <returns>パスワード</returns>
	public static string CreatePassword(int count) {
		StringBuilder sb = new StringBuilder(count);
		for (int i = count - 1; i >= 0; i--) {
			char c = PasswordChars[UnityEngine.Random.Range(0, PasswordCharsLength)];
			sb.Append(c);
		}
		return sb.ToString();
	}

}
