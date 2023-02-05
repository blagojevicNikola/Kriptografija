using Kriptografija_Projekat_.Service;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            try
            {
                string content = File.ReadAllText(_filePath);
                string signature = File.ReadAllText(_signaturePath);


                //Pkcs1Encoding encryptEngine = new Pkcs1Encoding(new RsaEngine());
                //encryptEngine.Init(false, keyPair.Private);
                //string newSignature = Convert.ToBase64String(cryptoService.Sha256RsaSign(Convert.FromBase64String(content), keyPair.Private));

                //if(signature!=newSignature)
                //{
                //    return null;
                //}
                bool verified = cryptoService.Sha256RsaVerify(Convert.FromBase64String(content), Convert.FromBase64String(signature), keyPair);
                if (!verified)
                {
                    return null;
                }
                byte[] decResult = cryptoService.AesSystemDecrypt(content);

                //ISigner signer = SignerUtilities.GetSigner("SHA256WITHRSA");
                //signer.BlockUpdate(decResult, 0, decResult.Length);
                //signer.Init(false, keyPair.Public);
                return decResult;
            }catch(FormatException)
            {
                Debug.WriteLine("Greska u base64!");
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
