namespace PDFSigner
{
    public class PDFSignerConfiguration
    {
        public string CertificateFilePath { get; set; } = string.Empty;
        public string CertificatePassword { get; set; } = string.Empty;
        public string SignatureReason { get; set; } = "Firma electrónica";
        public string SignatureLocation { get; set; } = string.Empty;
        public bool VisibleSignature { get; set; } = false;

        public bool HasSignatureReason => !string.IsNullOrEmpty(SignatureReason);
        public bool HasSignatureLocation => !string.IsNullOrEmpty(SignatureLocation);
    }
}