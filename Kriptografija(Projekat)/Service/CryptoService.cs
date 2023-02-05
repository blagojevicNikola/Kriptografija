using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Prng.Drbg;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Kriptografija_Projekat_.Service
{
    public class CryptoService
    {
        public CryptoService() { }

        public byte[] Aes128CBCDecrypt(byte[] input, byte[] key)
        {
            CbcBlockCipher blockCipher = new CbcBlockCipher(new AesEngine());
            PaddedBufferedBlockCipher cipher = new PaddedBufferedBlockCipher(blockCipher);
            cipher.Init(false, new ParametersWithIV(new KeyParameter(key), key, 0, key.Length));
            byte[] inputArr = new byte[cipher.GetOutputSize(input.Length)];
            int len = cipher.ProcessBytes(input, inputArr, 0);
            cipher.DoFinal(inputArr, len);
            return inputArr;
        }

        public byte[] Aes128CBCEncrypt(byte[] input, byte[] key)
        {
            CbcBlockCipher blockCipher = new CbcBlockCipher(new AesEngine());
            PaddedBufferedBlockCipher cipher = new PaddedBufferedBlockCipher(blockCipher);
            cipher.Init(true, new ParametersWithIV(new KeyParameter(key), key, 0, key.Length));
            byte[] symmetricOutput = new byte[cipher.GetOutputSize(input.Length)];
            int len = cipher.ProcessBytes(input, symmetricOutput, 0);
            cipher.DoFinal(symmetricOutput, len);
            return symmetricOutput;
        }

        public byte[] AesSystemEncrypt(byte[] clearBytes)
        {
            string EncryptionKey = "sigurnostsigurnostsigurnostsigurnost";
            using (Aes encryptor = Aes.Create())
            {
                encryptor.Key = Encoding.Unicode.GetBytes(EncryptionKey).Skip(0).Take(32).ToArray();
                encryptor.IV = Encoding.Unicode.GetBytes(EncryptionKey).Skip(0).Take(16).ToArray();
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    return ms.ToArray();
                }
            }
        }

        public byte[] AesSystemDecrypt(string input)
        {
            string EncryptionKey = "sigurnostsigurnostsigurnostsigurnost";
            byte[] cipherBytes = Convert.FromBase64String(input);
            using (Aes encryptor = Aes.Create())
            {
                encryptor.Key = Encoding.Unicode.GetBytes(EncryptionKey).Skip(0).Take(32).ToArray();
                encryptor.IV = Encoding.Unicode.GetBytes(EncryptionKey).Skip(0).Take(16).ToArray();
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    return ms.ToArray();
                }
            }
        }

        public byte[] RsaEncryptWithPublic(byte[] input, AsymmetricKeyParameter publicKey)
        {
            Pkcs1Encoding encryptEngine = new Pkcs1Encoding(new RsaEngine());
            encryptEngine.Init(true, publicKey);
            byte[] encResult = encryptEngine.ProcessBlock(input, 0, input.Length);
            return encResult;
        }

        public byte[] RsaDecryptWithPrivate(byte[] input, AsymmetricKeyParameter privateKey)
        {
            Pkcs1Encoding encryptEngine = new Pkcs1Encoding(new RsaEngine());
            encryptEngine.Init(false, privateKey);
            byte[] decResult = encryptEngine.ProcessBlock(input, 0, input.Length);
            return decResult;
        }

        public byte[] Sha256RsaSign(byte[] input, AsymmetricKeyParameter privateKey)
        {
            //Sha512Digest dgst = new Sha512Digest();
            //RsaDigestSigner signer = new RsaDigestSigner(dgst);
            ////ISigner signer = SignerUtilities.GetSigner("SHA256WITHRSA");
            //signer.BlockUpdate(input, 0, input.Length);
            //signer.Init(true, (RsaKeyParameters)privateKey);
            //byte[] signResult = signer.GenerateSignature();
            Sha512Digest dgst = new Sha512Digest();
            dgst.BlockUpdate(input, 0, input.Length);
            byte[] hashed = new byte[dgst.GetDigestSize()];
            dgst.DoFinal(hashed);
            byte[] signResult = RsaEncryptWithPrivate(hashed, privateKey);
            return signResult;
        }

        public bool Sha256RsaVerify(byte[] input, byte[] signature, AsymmetricCipherKeyPair keyPair)
        {
            
            //ISigner signer = SignerUtilities.GetSigner("SHA256WITHRSA");
            Sha512Digest dgst = new Sha512Digest();
            //RsaDigestSigner signer = new RsaDigestSigner(dgst);
            byte[] temp1 = new byte[dgst.GetDigestSize()];
            dgst.BlockUpdate(input, 0, input.Length);
            dgst.DoFinal(temp1);
            byte[] temp2 = RsaDecryptWithPublic(signature, keyPair.Public);
            //signer.BlockUpdate(input, 0, input.Length);
            //signer.Init(false, (RsaKeyParameters)keyPair.Public);
            //bool verified = signer.VerifySignature(signature);
            bool verified = compareArrays(temp1, temp2);
            return verified;
        }

        public byte[] RsaDecryptWithPublic(byte[] input, AsymmetricKeyParameter publicKey)
        {
            Pkcs1Encoding encryptEngine = new Pkcs1Encoding(new RsaEngine());
            encryptEngine.Init(false, publicKey);
            byte[] decResult = encryptEngine.ProcessBlock(input, 0, input.Length);
            return decResult;
        }

        public byte[] RsaEncryptWithPrivate(byte[] input, AsymmetricKeyParameter privateKey)
        {
            Pkcs1Encoding encryptEngine = new Pkcs1Encoding(new RsaEngine());
            encryptEngine.Init(true, privateKey);
            byte[] decResult = encryptEngine.ProcessBlock(input, 0, input.Length);
            return decResult;
        }

        //public bool Sha256RsaSystemVerify(byte[] input, byte[] signature, AsymmetricCipherKeyPair keyPair)
        //{
        //    bool success = false;
        //    using (var rsa = new RSACryptoServiceProvider())
        //    {
        //        try
        //        {
        //            rsa.ImportRSAPrivateKey(keyPair.;

        //            SHA512Managed Hash = new SHA512Managed();

        //            byte[] hashedData = Hash.ComputeHash(signedBytes);

        //            success = rsa.VerifyData(bytesToVerify, CryptoConfig.MapNameToOID("SHA512"), signedBytes);
        //        }
        //        catch (CryptographicException e)
        //        {
        //            Console.WriteLine(e.Message);
        //        }
        //        finally
        //        {
        //            rsa.PersistKeyInCsp = false;
        //        }
        //    }
        //    return success;
        //}

        private bool compareArrays(ReadOnlySpan<byte> a1, ReadOnlySpan<byte> a2)
        {
            return a1.SequenceEqual(a2);
        }
    }
}
