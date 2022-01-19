// See https://aka.ms/new-console-template for more information
using System.CommandLine;
using ravndaa.certutils;

var rootCommand = new RootCommand { Description = "Create pfx file from pem and a secret." };

var pfxargCert = new Option<string>("-cert", "Certificate file");
pfxargCert.AddAlias("-c");
pfxargCert.IsRequired = true;
var pfxargSecret = new Option<string>("-secret", "Secret key file");
pfxargSecret.AddAlias("-s");
pfxargSecret.IsRequired = true;
var pfxargPassword = new Option<string>("-password", "PFX password");
pfxargPassword.AddAlias("-p");
pfxargPassword.IsRequired = true;
var pfxargOutput = new Option<string>("-outfile", "PFX filename");
pfxargOutput.AddAlias("-o");
pfxargOutput.IsRequired = true;

rootCommand.AddOption(pfxargCert);
rootCommand.AddOption(pfxargSecret);
rootCommand.AddOption(pfxargPassword);
rootCommand.AddOption(pfxargOutput);

rootCommand.SetHandler(async (string cert, string secret, string password, string outfile) => { await CertUtils.CreatePFX(cert, secret, password, outfile); }, pfxargCert, pfxargSecret, pfxargPassword, pfxargOutput);

return rootCommand.Invoke(args);
