using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.security;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.X509;
using PDFSigner.Exceptions;

namespace PDFSigner 
{
    public class PDFSigner
    {
        private PDFSignerConfiguration Configuration { get; set; }
        private Pkcs12Store KeyStore { get; set; }

        public PDFSigner(PDFSignerConfiguration configuration) 
        {
            Configuration = configuration;
            KeyStore = ReadCertificateFile();

            if (!HasPrivateKey())
                throw new PrivateKeyNotFoundException("Private key not found");
        }

        public void Sign(string inputFilePath, string outputFilePath)
        {
            Sign(
                File.ReadAllBytes(inputFilePath),
                outputFilePath
                );
        }

        public void Sign(byte[] inputFile, string outputFilePath)
        {
            var pdfStamper = GetPdfStamper(inputFile, outputFilePath);

            MakeSignature.SignDetached(
                GetSignatureAppearance(pdfStamper), 
                GetPrivateKeySignature(), 
                GetCertificateChain(), 
                null, 
                null,
                null, 
                0, 
                CryptoStandard.CMS
                );

            pdfStamper.Close();
        }

        private IExternalSignature GetPrivateKeySignature()
        {
            return new PrivateKeySignature(GetPrivateKey(), DigestAlgorithms.SHA256);
        }

        private string? GetAlias()
        {
            return KeyStore.Aliases
                .Cast<string>()
                .FirstOrDefault(entryAlias => KeyStore.IsKeyEntry(entryAlias));
        }

        private bool HasPrivateKey()
        {
            return GetAlias() != null;
        }

        private ICipherParameters GetPrivateKey()
        {
            return KeyStore.GetKey(GetAlias()).Key;
        }

        private X509Certificate GetCertificate()
        {
            X509CertificateEntry certificateEntry = KeyStore.GetCertificate(GetAlias());
            return certificateEntry.Certificate;
        }

        private X509Certificate[] GetCertificateChain()
        {
            return new X509Certificate[] { 
                GetCertificate()
            };
        }

        private PdfSignatureAppearance GetSignatureAppearance(PdfStamper pdfStamper)
        {
            PdfSignatureAppearance signatureAppearance = pdfStamper.SignatureAppearance;
            if (Configuration.HasSignatureReason)
                signatureAppearance.Reason = Configuration.SignatureReason;

            if (Configuration.HasSignatureLocation)
                signatureAppearance.Location = Configuration.SignatureLocation;

            if (Configuration.VisibleSignature)
            {
                float x = 30;
                float y = 80;

                signatureAppearance.Layer2Text = string.Format("Firmado por: {0}", KeyStore.GetCertificate(GetAlias()).Certificate.SubjectDN.ToString());
                signatureAppearance.Acro6Layers = false;
                signatureAppearance.Layer4Text = PdfSignatureAppearance.questionMark;
                signatureAppearance.SetVisibleSignature(
                    new Rectangle(x, y, x + 150, y + 60),
                    1,
                    "signature"
                    );
            }

            return signatureAppearance;
        }

        private PdfStamper GetPdfStamper(byte[] inputFile, string outputFilePath)
        {
            return PdfStamper.CreateSignature(
                new PdfReader(inputFile),
                new FileStream(outputFilePath, FileMode.Create),
                '\0',
                null,
                true
                );
        }

        private Pkcs12Store ReadCertificateFile()
        {
            CheckCertificateFile();
            return new Pkcs12Store(
                new FileStream(Configuration.CertificateFilePath, FileMode.Open, FileAccess.Read),
                Configuration.CertificatePassword.ToCharArray()
                );
        }

        private void CheckCertificateFile()
        {
            if (!File.Exists(Configuration.CertificateFilePath))
            {
                throw new FileNotFoundException(Configuration.CertificateFilePath);
            }
        }
    }
}
