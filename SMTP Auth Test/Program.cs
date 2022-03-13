using System.Net;
using HtmlAgilityPack;

namespace HttpClientStatus;

class Program
{
    static async Task Main(string[] args)
    {

        //Create random secret key string
        Random random = new Random();
        string rand_sec_code = "";
        char letters;
        for (int i = 0; i < 10; i++)
        {
            int rand_num = random.Next(0, 21);
            letters = Convert.ToChar(rand_num + 65);
            rand_sec_code = rand_sec_code + letters + rand_num;

        }

        //Create auth_pair url
        string app_name = "PasswordManager";
        string app_info = "IAANSEC";
        string security_code = rand_sec_code;
        string auth_pair = "https://www.authenticatorapi.com/pair.aspx?" + "AppName=" + app_name + "&AppInfo=" + app_info + "&SecretCode=" + security_code;


        //Get request to auth_pair url
        using var client = new HttpClient();
        var html = await client.GetStringAsync(auth_pair);

        //Load auth_pair html 
        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(html);
        string qr_img_url = htmlDocument.DocumentNode
            .SelectSingleNode("//img")
            .Attributes["src"].Value;
        Console.WriteLine(qr_img_url);



        //Download QR Code img
        HttpWebRequest lxRequest = (HttpWebRequest)WebRequest.Create(qr_img_url);

        // returned values are returned as a stream, then read into a string
        String lsResponse = string.Empty;
        using (HttpWebResponse lxResponse = (HttpWebResponse)lxRequest.GetResponse())
        {
            using (BinaryReader reader = new BinaryReader(lxResponse.GetResponseStream()))
            {
                Byte[] lnByte = reader.ReadBytes(1 * 1024 * 1024 * 10);
                using (FileStream lxFS = new FileStream("auth.jpg", FileMode.Create))
                {
                    lxFS.Write(lnByte, 0, lnByte.Length);
                }
            }
        }

        Console.WriteLine(auth_pair);



        //Validation
        Console.WriteLine("Enter Pin: ");
        string pin = Console.ReadLine();

        string validate = "https://www.authenticatorapi.com/Validate.aspx?" + "Pin=" + pin + "&SecretCode=" + security_code;

        Console.WriteLine(validate);

        var result = await client.GetStringAsync(validate);
        Console.WriteLine("\n" + result);

    }
}

