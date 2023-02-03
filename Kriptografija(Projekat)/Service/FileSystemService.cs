using Kriptografija_Projekat_.Model;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Tls;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Kriptografija_Projekat_.Service
{
    public class FileSystemService
    {
        Random rand;
        public FileSystemService() 
        { 
            rand = new Random(); 
        }

        public UserFile? AddFile(User user, string filePath)
        {
            if (!File.Exists(filePath))
                return null;
            CryptoService cryptoService = new CryptoService();
            string fileName = Path.GetFileName(filePath);
            UserFile newFile = new UserFile(fileName);
            byte[] array = File.ReadAllBytes(filePath);
            int numOfSegments = array.Length / (rand.Next(7) + 4);
            List<byte[]> chunks = array.Chunk(numOfSegments).ToList();
            int tmp = 0;
            chunks.ForEach(chunk =>
            {
                newFile.AddSegment(writeSegment(user.Username, (tmp++).ToString(), chunk, user.KeyPair, cryptoService));
            });
            return newFile;
        }


        private Segment writeSegment(string username, string numOfChunk, byte[] arr, AsymmetricCipherKeyPair keyPair, CryptoService cryptoService)
        {
            string directoryName = ConfigurationManager.AppSettings["FS"]! + @"\" + numOfChunk;
            string chunkName = username+ "_" + numOfChunk;
            if (!File.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }

            Sha256Digest alg = new Sha256Digest();
            byte[] chunkNameArray = Encoding.UTF8.GetBytes(chunkName);
            alg.BlockUpdate(chunkNameArray, 0, chunkNameArray.Length);
            byte[] output = new byte[alg.GetDigestSize()];
            alg.DoFinal(output, 0);
            string segmentName = Convert.ToBase64String(output);
            segmentName = segmentName.Replace(@"/", "");

            //ISigner signer = SignerUtilities.GetSigner("SHA256WITHRSA");
            //signer.BlockUpdate(arr, 0, arr.Length);
            //signer.Init(true, keyPair.Private);
            byte[] signResult = cryptoService.Sha256RsaSign(arr, keyPair.Private);

            //Pkcs1Encoding encryptEngine = new Pkcs1Encoding(new RsaEngine());
            //encryptEngine.Init(true, keyPair.Public);
            byte[] encResult = cryptoService.Aes128CBCEncrypt(arr, Encoding.UTF8.GetBytes("sigurnostsigurno"));

            string segmentFilePath = directoryName + @"\" + segmentName + ".cry";
            string segmentSignaturePath = directoryName + @"\" + segmentName + ".sig";
            File.WriteAllText(segmentFilePath, Convert.ToBase64String(encResult));
            File.WriteAllText(segmentSignaturePath, Convert.ToBase64String(signResult));

            return new Segment(segmentFilePath, segmentSignaturePath);
        }
    }
}
