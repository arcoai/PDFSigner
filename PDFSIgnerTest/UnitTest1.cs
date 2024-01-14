using PDFSigner;
using System.Reflection.PortableExecutable;

namespace PDFSignerTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var signer = new PDFSigner.PDFSigner(new PDFSignerConfiguration {
                CertificateFilePath = "C:\\ruta\\al\\certificado.p12",
                CertificatePassword = "clave-del-certificado",
                VisibleSignature = true
            });
            signer.Sign(
                File.ReadAllBytes("C:\\ruta\\al\\fichero.pdf"),
                "C:\\ruta\\al\\fichero-firmado.pdf"
                );

            Assert.IsTrue(true);
        }
    }
}