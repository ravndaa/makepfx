using System.CommandLine;
using System.Security;
using System.Security.Cryptography.X509Certificates;


static async Task CreatePFX(string certPath, string secretPath, string password, string outfile)
{
    if (string.IsNullOrEmpty(certPath)) return;
    if (string.IsNullOrEmpty(secretPath)) return;
    if (string.IsNullOrEmpty(password)) return;
    if (string.IsNullOrEmpty(outfile)) return;

    X509Certificate2 cert = X509Certificate2.CreateFromPemFile(@$"{certPath}", @$"{secretPath}");

    var file = cert.Export(X509ContentType.Pfx, password);
    await File.WriteAllBytesAsync(@$"{outfile}", file);
}

// stolen from: https://stackoverflow.com/questions/29201697/hide-replace-when-typing-a-password-c/29201791
static string GetPassword()
{
    string password = "";
    ConsoleKeyInfo info = Console.ReadKey(true);
    while (info.Key != ConsoleKey.Enter)
    {
        if (info.Key != ConsoleKey.Backspace)
        {
            Console.Write("*");
            password += info.KeyChar;
        }
        else if (info.Key == ConsoleKey.Backspace)
        {
            if (!string.IsNullOrEmpty(password))
            {
                // remove one character from the list of password characters
                password = password.Substring(0, password.Length - 1);
                // get the location of the cursor
                int pos = Console.CursorLeft;
                // move the cursor to the left by one character
                Console.SetCursorPosition(pos - 1, Console.CursorTop);
                // replace it with space
                Console.Write(" ");
                // move the cursor to the left by one character again
                Console.SetCursorPosition(pos - 1, Console.CursorTop);
            }
        }
        info = Console.ReadKey(true);
    }
    // add a new line because user pressed enter at the end of their password
    Console.WriteLine();
    return password;
}

var rootCommand = new RootCommand { Description = "Create pfx file from pem and a secret." };

var pfxargCert = new Option<string>("-cert", "Certificate file");
pfxargCert.AddAlias("-c");
pfxargCert.IsRequired = true;
var pfxargSecret = new Option<string>("-secret", "Secret key file");
pfxargSecret.AddAlias("-s");
pfxargSecret.IsRequired = true;
var pfxargPassword = new Option<string>("-password", "PFX password");
pfxargPassword.AddAlias("-p");
pfxargPassword.IsRequired = false;
var pfxargOutput = new Option<string>("-outfile", "PFX filename");
pfxargOutput.AddAlias("-o");
pfxargOutput.IsRequired = true;

rootCommand.AddOption(pfxargCert);
rootCommand.AddOption(pfxargSecret);
rootCommand.AddOption(pfxargPassword);
rootCommand.AddOption(pfxargOutput);

rootCommand.SetHandler(async (string cert, string secret, string password, string outfile) => {
    try
    {
        if (!File.Exists(cert)) { Console.WriteLine($"Can't find certificate file."); return; }
        if (!File.Exists(secret)) { Console.WriteLine($"Can't find secret key file."); return; }
        if (File.Exists(outfile)) { Console.WriteLine($"Outfile exists."); return; }
        string certpassord = string.Empty;
        if(string.IsNullOrEmpty(password))
        {
            Console.Write("Password: ");
            var pass1 = GetPassword();
            Console.Write("Confirm Password: ");
            var pass2 = GetPassword();
            if(pass1 != pass2) { Console.WriteLine("password is not matching"); return; }
            certpassord = pass1;
        }
        else
        {
            certpassord = password;
        } 

        await CreatePFX(cert, secret, certpassord, outfile);
    }
    catch (global::System.Exception)
    {
        return;
    }

}, pfxargCert, pfxargSecret, pfxargPassword, pfxargOutput);

return rootCommand.Invoke(args);