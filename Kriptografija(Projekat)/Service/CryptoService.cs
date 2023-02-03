using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Linq;
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
            ISigner signer = SignerUtilities.GetSigner("SHA256WITHRSA");
            signer.BlockUpdate(input, 0, input.Length);
            signer.Init(true, privateKey);
            byte[] signResult = signer.GenerateSignature();
            return signResult;
        }

        public bool Sha256RsaVerify(byte[] input, byte[] signature, AsymmetricKeyParameter publicKey)
        {
            ISigner signer = SignerUtilities.GetSigner("SHA256WITHRSA");
            signer.BlockUpdate(input, 0, input.Length);
            signer.Init(false, publicKey);
            bool verified = signer.VerifySignature(signature);
            return verified;
        }
    }
}
