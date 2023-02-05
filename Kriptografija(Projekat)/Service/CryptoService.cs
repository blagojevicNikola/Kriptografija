using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
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
            Sha256Digest dgst = new Sha256Digest();
            RsaDigestSigner signer = new RsaDigestSigner(dgst);
            //ISigner signer = SignerUtilities.GetSigner("SHA256WITHRSA");
            signer.BlockUpdate(input, 0, input.Length);
            signer.Init(true, (RsaKeyParameters)privateKey);
            byte[] signResult = signer.GenerateSignature();
            return signResult;
        }

        public bool Sha256RsaVerify(byte[] input, byte[] signature, AsymmetricCipherKeyPair keyPair)
        {
            byte[] currentSignature = Sha256RsaSign(input, keyPair.Private);
            //ISigner signer = SignerUtilities.GetSigner("SHA256WITHRSA");
            Sha256Digest dgst = new Sha256Digest();
            RsaDigestSigner signer = new RsaDigestSigner(dgst);
            signer.BlockUpdate(input, 0, input.Length);
            signer.Init(false, (RsaKeyParameters)keyPair.Public);
            bool verified = signer.VerifySignature(signature);
            
            return verified;
        }

        private bool compareArrays(ReadOnlySpan<byte> a1, ReadOnlySpan<byte> a2)
        {
            return a1.SequenceEqual(a2);
        }
    }
}
