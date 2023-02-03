using Kriptografija_Projekat_.Service;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kriptografija_Projekat_.Model
{
    public class Segment
    {
        private string _filePath;
        private string _signaturePath;

        public Segment(string filePath, string signaturePath)
        {
            _filePath = filePath;
            _signaturePath = signaturePath;
        }

        public byte[]? Load(AsymmetricCipherKeyPair keyPair, CryptoService cryptoService)
        {
            byte[] content = Convert.FromBase64String(File.ReadAllText(_filePath));
            byte[] signature = Convert.FromBase64String(File.ReadAllText(_signaturePath));

            //Pkcs1Encoding encryptEngine = new Pkcs1Encoding(new RsaEngine());
            //encryptEngine.Init(false, keyPair.Private);
            byte[] decResult = cryptoService.Aes128CBCDecrypt(content, Encoding.UTF8.GetBytes("sigurnostsigurno"));

            //ISigner signer = SignerUtilities.GetSigner("SHA256WITHRSA");
            //signer.BlockUpdate(decResult, 0, decResult.Length);
            //signer.Init(false, keyPair.Public);
            bool verified = cryptoService.Sha256RsaVerify(decResult, signature, keyPair.Public);
            if (verified)
            {
                return decResult;
            }
            else
            {
                return null;
            }
        }

        override
        public string ToString() 
        {
            return _filePath + "|" + _signaturePath;
        }
    }
}
