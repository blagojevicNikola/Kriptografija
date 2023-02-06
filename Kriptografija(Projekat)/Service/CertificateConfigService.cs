using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Tls;
using Org.BouncyCastle.X509;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Cmp;
using System.Collections;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Utilities.Encoders;
using System.Configuration;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Parameters;

namespace Kriptografija_Projekat_.Service
{
    public class CertificateConfigService
    {
        
        public CertificateConfigService()
        {
           
        }

        private string CreateCSR(string username, string mail, out AsymmetricCipherKeyPair p)
        {
            RsaKeyPairGenerator rsaGen = new RsaKeyPairGenerator();
            rsaGen.Init(new KeyGenerationParameters(new Org.BouncyCastle.Security.SecureRandom(), 2048));
            AsymmetricCipherKeyPair pair = rsaGen.GenerateKeyPair();
            p = pair;
            var attrs = new Dictionary<DerObjectIdentifier, string>();
            attrs[X509Name.CN] = username;
            attrs[X509Name.C] = "BA";
            attrs[X509Name.O] = "ETF";
            attrs[X509Name.L] = "BL";
            attrs[X509Name.ST] = "RS";
            attrs[X509Name.E] = mail;
            var ord = new List<DerObjectIdentifier>();
            ord.Add(X509Name.CN);
            ord.Add(X509Name.C);
            ord.Add(X509Name.O);
            ord.Add(X509Name.L);
            ord.Add(X509Name.ST);
            ord.Add(X509Name.E);

            var subject = new X509Name(ord, attrs);

            var pkcs10CertificationRequest = new Pkcs10CertificationRequest
                    (PkcsObjectIdentifiers.Sha256WithRsaEncryption.Id, subject, pair.Public, null, pair.Private);
            
            string csr = Convert.ToBase64String(pkcs10CertificationRequest.GetEncoded());
            return csr;
        }

        public Org.BouncyCastle.X509.X509Certificate SignCert(string username, string mail)
        {
            AsymmetricCipherKeyPair csrPair;
            char[] csr = this.CreateCSR(username, mail, out csrPair).ToCharArray();
            byte[] csrEncode = Convert.FromBase64CharArray(csr, 0, csr.Length);
            Pkcs10CertificationRequest pk10Holder = new Pkcs10CertificationRequest(csrEncode);
            CryptoApiRandomGenerator randomGenerator = new CryptoApiRandomGenerator();
            SecureRandom random = new SecureRandom(randomGenerator);

            X509V3CertificateGenerator certificateGenerator = new X509V3CertificateGenerator();

            // Serial Number
            BigInteger serialNumber = new BigInteger(File.ReadAllText(ConfigurationManager.AppSettings["Serial"]!));
            File.WriteAllText(ConfigurationManager.AppSettings["Serial"]!, (serialNumber.IntValue + 1).ToString());
            certificateGenerator.SetSerialNumber(serialNumber);
            Org.BouncyCastle.X509.X509Certificate issuer = new Org.BouncyCastle.X509.X509Certificate(File.ReadAllBytes(ConfigurationManager.AppSettings["Path"]!));
            X509Name issuerDetails = issuer.IssuerDN;
            certificateGenerator.SetIssuerDN(issuerDetails);
            certificateGenerator.SetSubjectDN(pk10Holder.GetCertificationRequestInfo().Subject);

            DateTime notBefore = DateTime.UtcNow.Date;
            DateTime notAfter = notBefore.AddMonths(6);

            certificateGenerator.SetNotBefore(notBefore);
            certificateGenerator.SetNotAfter(notAfter);
            certificateGenerator.SetPublicKey(pk10Holder.GetPublicKey());

            AsymmetricCipherKeyPair issuerPrivKey;
            using (var reader = File.OpenText(ConfigurationManager.AppSettings["Main"]! + @"\ca_key.pem"))
                issuerPrivKey = (AsymmetricCipherKeyPair)new PemReader(reader).ReadObject();

            ISignatureFactory signatureFactory = new Asn1SignatureFactory("SHA256WITHRSA", issuerPrivKey.Private, random);
            certificateGenerator.AddExtension(X509Extensions.KeyUsage, true, new KeyUsage(KeyUsage.DigitalSignature | KeyUsage.NonRepudiation | KeyUsage.DataEncipherment));
            Org.BouncyCastle.X509.X509Certificate newCertificate = certificateGenerator.Generate(signatureFactory);
            File.WriteAllBytes(ConfigurationManager.AppSettings["Main"]! + @"\" + serialNumber + ".der", newCertificate.GetEncoded());
            TextWriter tw = new StringWriter();
            PemWriter pw = new PemWriter(tw);
            pw.WriteObject(csrPair.Private);
            pw.Writer.Flush();
            File.WriteAllText(ConfigurationManager.AppSettings["Users"]! + @"\" + username + @"\" + username + ".pem", tw.ToString());
            tw.Close();
            return newCertificate;
        }

        public void CreateSelfSigned()
        {
            //RSA key = RSA.Create(4096);
            //var req = new System.Security.Cryptography.X509Certificates.CertificateRequest("cn=novi ca", key, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            //req.CertificateExtensions.Add(new X509BasicConstraintsExtension(true, false, 0, false));
            //var cert = req.CreateSelfSigned(DateTimeOffset.Now, DateTimeOffset.Now.AddYears(5));
            //File.WriteAllBytes(@"C:\Users\Lenovo PC\Desktop\ca2.pfx", cert.Export(X509ContentType.Pfx, "sigurnost"));
            RsaKeyPairGenerator rsaGen = new RsaKeyPairGenerator();
            rsaGen.Init(new KeyGenerationParameters(new Org.BouncyCastle.Security.SecureRandom(), 4096));
            AsymmetricCipherKeyPair pair = rsaGen.GenerateKeyPair();
            X509V3CertificateGenerator gen = new X509V3CertificateGenerator();
            
            var attrs = new Dictionary<DerObjectIdentifier, string>();
            attrs[X509Name.CN] = "CA";
            attrs[X509Name.C] = "BA";
            attrs[X509Name.O] = "ETF";
            attrs[X509Name.L] = "BL";
            attrs[X509Name.ST] = "RS";
            attrs[X509Name.E] = "etf@mail.com";

            var ord = new List<DerObjectIdentifier>();
            ord.Add(X509Name.CN);
            ord.Add(X509Name.C);
            ord.Add(X509Name.O);
            ord.Add(X509Name.L);
            ord.Add(X509Name.ST);
            ord.Add(X509Name.E);

            gen.SetSerialNumber(BigInteger.One);
            gen.SetIssuerDN(new X509Name(ord, attrs));
            gen.SetNotBefore(DateTime.UtcNow.AddDays(0));
            gen.SetNotAfter(DateTime.UtcNow.AddYears(5));
            gen.SetSubjectDN(new X509Name(ord, attrs));
            gen.SetPublicKey(pair.Public);
            X509ExtensionsGenerator extGen = new X509ExtensionsGenerator();
            gen.AddExtension(X509Extensions.KeyUsage, true, new KeyUsage(KeyUsage.CrlSign | KeyUsage.DigitalSignature | KeyUsage.NonRepudiation));
            Org.BouncyCastle.X509.X509Certificate cert = gen.Generate(new Asn1SignatureFactory("SHA256WITHRSA", pair.Private, null));
            File.WriteAllBytes(ConfigurationManager.AppSettings["Main"]+@"\ca1.der", cert.GetEncoded());
            TextWriter tw = new StringWriter();
            PemWriter pw = new PemWriter(tw);
            pw.WriteObject(pair.Private);
            pw.Writer.Flush();
            File.WriteAllText(ConfigurationManager.AppSettings["Main"]! + @"\ca_key.pem", tw.ToString());
            tw.Close();
        }

        public Org.BouncyCastle.X509.X509Certificate? ValidateCertificate(string certPath)
        {
            X509Crl crl = new X509Crl(File.ReadAllBytes(ConfigurationManager.AppSettings["CRL"]! + @"\CertRevocationList.crl"));
            Org.BouncyCastle.X509.X509Certificate cert = new Org.BouncyCastle.X509.X509Certificate(File.ReadAllBytes(certPath));
            if(crl.IsRevoked(cert))
            {
                return null;
            }
            bool[] usage = new bool[] { true, true, false, true, false, false, false, false, false};
            bool[] certUsage = cert.GetKeyUsage();
            if(!compareArrays(usage,certUsage))
            {
                return null;
            }
            Sha256Digest dgst = new Sha256Digest();
            RsaDigestSigner signer = new RsaDigestSigner(dgst);
            AsymmetricCipherKeyPair issuerPrivKey;
            using (var reader = File.OpenText(ConfigurationManager.AppSettings["Main"]! + @"\ca_key.pem"))
                issuerPrivKey = (AsymmetricCipherKeyPair)new PemReader(reader).ReadObject();
            signer.Init(false, (RsaKeyParameters)issuerPrivKey.Public);
            byte[] actual = cert.GetSignature();
            byte[] toCheck = cert.GetTbsCertificate();
            signer.BlockUpdate(toCheck, 0, toCheck.Length);
            bool isSignatureValid = signer.VerifySignature(actual);
            bool isTimeValid = true;
            if(DateTime.Now > cert.NotAfter || DateTime.Now < cert.NotBefore)
            {
                isTimeValid = false;
            }

            if (isSignatureValid && isTimeValid)
                return cert;
            else
                return null;
        }

        public void BuildCRL()
        {
            Org.BouncyCastle.X509.X509Certificate issuer = new Org.BouncyCastle.X509.X509Certificate(File.ReadAllBytes(ConfigurationManager.AppSettings["Path"]!));
            AsymmetricCipherKeyPair issuerPrivKey;
            using (var reader = File.OpenText(ConfigurationManager.AppSettings["Main"]! + @"\ca_key.pem"))
                issuerPrivKey = (AsymmetricCipherKeyPair)new PemReader(reader).ReadObject();

            X509V2CrlGenerator gen = new X509V2CrlGenerator();
            gen.SetIssuerDN(issuer.IssuerDN);
            gen.SetNextUpdate(DateTime.Now.AddMonths(3));
            gen.SetThisUpdate(DateTime.Now);
            string signatureAlgorithm = "SHA256withRSA";
            ISignatureFactory signatureFactory = new Asn1SignatureFactory(signatureAlgorithm, issuerPrivKey.Private);
            X509Crl crl = gen.Generate(signatureFactory);
            byte[] temp = crl.GetEncoded();

            File.WriteAllBytes(ConfigurationManager.AppSettings["CRL"]! + @"\CertRevocationList.crl", temp);
        }

        public void RevokeCertificate(Org.BouncyCastle.X509.X509Certificate cert)
        {
            X509Crl crl = new X509Crl(File.ReadAllBytes(ConfigurationManager.AppSettings["CRL"]! + @"\CertRevocationList.crl"));
            Org.BouncyCastle.X509.X509Certificate issuer = new Org.BouncyCastle.X509.X509Certificate(File.ReadAllBytes(ConfigurationManager.AppSettings["Path"]!));
            AsymmetricCipherKeyPair issuerPrivKey;
            using (var reader = File.OpenText(ConfigurationManager.AppSettings["Main"]! + @"\ca_key.pem"))
                issuerPrivKey = (AsymmetricCipherKeyPair)new PemReader(reader).ReadObject();

            X509V2CrlGenerator gen = new X509V2CrlGenerator();
            gen.SetIssuerDN(issuer.IssuerDN);
            gen.SetNextUpdate(DateTime.Now.AddMonths(3));
            gen.SetThisUpdate(DateTime.Now);
            string signatureAlgorithm = "SHA256withRSA";
            ISignatureFactory signatureFactory = new Asn1SignatureFactory(signatureAlgorithm, issuerPrivKey.Private);
            gen.AddCrl(crl);
            gen.AddCrlEntry(cert.SerialNumber, DateTime.Now, CrlReason.CertificateHold);
            X509Crl newCrl = gen.Generate(signatureFactory);
            byte[] temp = newCrl.GetEncoded();
            File.WriteAllBytes(ConfigurationManager.AppSettings["CRL"]! + @"\CertRevocationList.crl", temp);
        }

        private bool compareArrays(ReadOnlySpan<bool> a1, ReadOnlySpan<bool> a2)
        {
            return a1.SequenceEqual(a2);
        }
    }
}
