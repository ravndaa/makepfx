using System.Security.Cryptography.X509Certificates;

namespace ravndaa.certutils
{
    public static class CertUtils
    {
        public static async Task CreatePFX(string certPath, string secretPath, string password, string outfile)
        {
            X509Certificate2 cert = X509Certificate2.CreateFromPemFile(@$"{certPath}", @$"{secretPath}");

            var file = cert.Export(X509ContentType.Pfx, password);
            await File.WriteAllBytesAsync(@$"{outfile}", file);
        }
    }
}