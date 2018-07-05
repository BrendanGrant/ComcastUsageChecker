using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;

namespace ComcastUsageChecker
{
    class Program
    {
        static string username = "PUT_YOUR_COMCAST_ADDRESS_HERE@comcast.net";
        static string password = "COMCAST_PASSWORD";
        static void Main(string[] args)
        {
            Console.WriteLine("Lets try getting our Comcast data usage");

            //ShouldWork_But_Fails_Due_To_Bad_Redirect().GetAwaiter().GetResult();
            Dont_Follow_Redirects_And_Craft_Broken_Url_To_Get_Usage().GetAwaiter().GetResult();

            Console.ReadLine();
        }

        private static async Task ShouldWork_But_Fails_Due_To_Bad_Redirect()
        {
            try
            {
                var cc = new CookieContainer();
                var h = new HttpClientHandler()
                {
                    CookieContainer = cc,
                    //Proxy = new WebProxy("localhost", 8888), //For monitoring traffic with Fiddler
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                    AllowAutoRedirect = true,
                };
                var c = new HttpClient(h);

                c.DefaultRequestHeaders.Add("User-Agent", "python-requests/2.18.4");
                c.DefaultRequestHeaders.Add("Accept", "*/*");
                c.DefaultRequestHeaders.Add("Connection", "keep-alive");

                Console.WriteLine("Finding req_id for login...");
                var initialResponse = await c.GetAsync("https://customer.xfinity.com/oauth/force_connect/?continue=%23%2Fdevices");
                Debug.Assert(initialResponse.StatusCode == HttpStatusCode.OK);
                var body = await initialResponse.Content.ReadAsStringAsync();

                var d = new HtmlAgilityPack.HtmlDocument();
                d.LoadHtml(body);
                var dd = d.DocumentNode.Descendants("input");
                var reqId = dd.Where(n => n.Attributes.Contains("name") && n.Attributes["name"].Value == "reqId").FirstOrDefault()?.Attributes["value"]?.Value;

                Console.WriteLine("Found req_id = {0}", reqId);

                var content = new FormUrlEncodedContent(new[]
                 {
                       new KeyValuePair<string, string>("user", username),
                       new KeyValuePair<string, string>("passwd", password),
                       new KeyValuePair<string, string>("reqId", reqId),
                       new KeyValuePair<string, string>("deviceAuthn", "false"),
                       new KeyValuePair<string, string>("s", "oauth"),
                       new KeyValuePair<string, string>("forceAuthn", "1"),
                       new KeyValuePair<string, string>("r",  "comcast.net"),
                       new KeyValuePair<string, string>("ipAddrAuthn", "false"),
                       new KeyValuePair<string, string>("Continue", "https://oauth.xfinity.com/oauth/authorize?client_id=my-account-web&prompt=login&redirect_uri=https%3A%2F%2Fcustomer.xfinity.com%2Foauth%2Fcallback&response_type=code&state=%23%2Fdevices&response=1"),
                       new KeyValuePair<string, string>("passive", "false"),
                       new KeyValuePair<string, string>("client_id", "my-account-web"),
                       new KeyValuePair<string, string>("lang", "en"),
                   });
                var payload = content.ToString();

                Console.WriteLine("Posting to login...");
                var postResult = await c.PostAsync("https://login.xfinity.com/login", content);
                Debug.Assert(postResult.StatusCode == HttpStatusCode.OK);
                var postResponseContent = await postResult.Content.ReadAsStringAsync();

                Console.WriteLine("Fetching internet usage JSON...");
                var getResult = await c.GetAsync("https://customer.xfinity.com/apis/services/internet/usage");
                Debug.Assert(getResult.StatusCode == HttpStatusCode.OK);
                var datausage = await getResult.Content.ReadAsStringAsync();

                var usage = JsonConvert.DeserializeObject<DataUsage>(datausage);
                DisplayUsage(usage);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error during operation: {0}", ex);
            }
        }

        private static async Task Dont_Follow_Redirects_And_Craft_Broken_Url_To_Get_Usage()
        {

            try
            {
                var cc = new CookieContainer();
                var h = new HttpClientHandler()
                {
                    CookieContainer = cc,
                    //Proxy = new WebProxy("localhost", 8888), //For monitoring traffic with Fiddler
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                    AllowAutoRedirect = false,
                };
                var c = new HttpClient(h);

                c.DefaultRequestHeaders.Add("User-Agent", "python-requests/2.18.4");
                c.DefaultRequestHeaders.Add("Accept", "*/*");
                c.DefaultRequestHeaders.Add("Connection", "keep-alive");

                Console.WriteLine("Finding req_id for login...");
                var initialResponse = await c.GetAsync("https://customer.xfinity.com/oauth/force_connect/?continue=%23%2Fdevices");
                if (initialResponse.StatusCode == HttpStatusCode.Redirect)
                {
                    initialResponse = await c.GetAsync(initialResponse.Headers.Location);
                    if (initialResponse.StatusCode == HttpStatusCode.Redirect)
                    {
                        initialResponse = await c.GetAsync(initialResponse.Headers.Location);
                        if (initialResponse.StatusCode == HttpStatusCode.Redirect)
                        {
                            initialResponse = await c.GetAsync(initialResponse.Headers.Location);
                        }
                    }
                }

                Debug.Assert(initialResponse.StatusCode == HttpStatusCode.OK);
                var body = await initialResponse.Content.ReadAsStringAsync();

                var d = new HtmlAgilityPack.HtmlDocument();
                d.LoadHtml(body);
                var dd = d.DocumentNode.Descendants("input");
                var reqId = dd.Where(n => n.Attributes.Contains("name") && n.Attributes["name"].Value == "reqId").FirstOrDefault()?.Attributes["value"]?.Value;

                Console.WriteLine("Found req_id = {0}", reqId);

                var content = new FormUrlEncodedContent(new[]
                 {
                       new KeyValuePair<string, string>("user", username),
                       new KeyValuePair<string, string>("passwd", password),
                       new KeyValuePair<string, string>("reqId", reqId),
                       new KeyValuePair<string, string>("deviceAuthn", "false"),
                       new KeyValuePair<string, string>("s", "oauth"),
                       new KeyValuePair<string, string>("forceAuthn", "1"),
                       new KeyValuePair<string, string>("r",  "comcast.net"),
                       new KeyValuePair<string, string>("ipAddrAuthn", "false"),
                       new KeyValuePair<string, string>("Continue", "https://oauth.xfinity.com/oauth/authorize?client_id=my-account-web&prompt=login&redirect_uri=https%3A%2F%2Fcustomer.xfinity.com%2Foauth%2Fcallback&response_type=code&state=%23%2Fdevices&response=1"),
                       new KeyValuePair<string, string>("passive", "false"),
                       new KeyValuePair<string, string>("client_id", "my-account-web"),
                       new KeyValuePair<string, string>("lang", "en"),
                });
                var payload = content.ToString();

                Console.WriteLine("Posting to login...");
                var postResult = await c.PostAsync("https://login.xfinity.com/login", content);
                if (postResult.StatusCode == HttpStatusCode.Redirect || postResult.StatusCode == HttpStatusCode.Found)
                {
                    //For reasons unknown, the server replies with an incomplete URL, but what is needed is easy enough to detect and add... though ugly.
                    if (!postResult.Headers.Location.ToString().Contains("client_id"))
                    {
                        var newQuery = "client_id=my-account-web&prompt=login&redirect_uri=https%3A%2F%2Fcustomer.xfinity.com%2Foauth%2Fcallback&response_type=code&state=%23%2Fdevices&response=1&" + postResult.Headers.Location.Query.ToString().TrimStart('?');

                        var newUri = new Uri($"https://{postResult.Headers.Location.Authority}{postResult.Headers.Location.AbsolutePath}?{newQuery}");

                        postResult = await c.GetAsync(newUri);

                        if (postResult.StatusCode == HttpStatusCode.Redirect || postResult.StatusCode == HttpStatusCode.Found)
                        {
                            postResult = await c.GetAsync(postResult.Headers.Location);

                            if (postResult.StatusCode == HttpStatusCode.Redirect || postResult.StatusCode == HttpStatusCode.Found)
                            {
                                postResult = await c.GetAsync(postResult.Headers.Location);
                            }
                        }
                    }
                }
                Debug.Assert(postResult.StatusCode == HttpStatusCode.OK);
                var postResponseContent = await postResult.Content.ReadAsStringAsync();

                Console.WriteLine("Fetching internet usage JSON");
                var getResult = await c.GetAsync("https://customer.xfinity.com/apis/services/internet/usage");
                Debug.Assert(getResult.StatusCode == HttpStatusCode.OK);
                var datausage = await getResult.Content.ReadAsStringAsync();

                Console.WriteLine("Got {0} bytes of response", datausage.Length);

                var usage = JsonConvert.DeserializeObject<DataUsage>(datausage);
                DisplayUsage(usage);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error during operation: {0}", ex);
            }
        }

        private static void DisplayUsage(DataUsage usage)
        {
            Console.WriteLine("courtesyUsed: {0}", usage.courtesyUsed);
            Console.WriteLine("courtesyRemaining: {0}", usage.courtesyRemaining);
            Console.WriteLine("courtesyAllowed: {0}", usage.courtesyAllowed);
            Console.WriteLine("inPaidOverage: {0}", usage.inPaidOverage);
            Console.WriteLine("months: {0}", usage.usageMonths.Count);

            foreach (var month in usage.usageMonths)
            {
                Console.WriteLine("\tpolicyName: {0}", month.policyName);
                Console.WriteLine("\tstartDate: {0}", month.startDate);
                Console.WriteLine("\tendDate: {0}", month.endDate);
                Console.WriteLine("\thomeUsage: {0}", month.homeUsage);
                Console.WriteLine("\tallowableUsage: {0}", month.allowableUsage);
                Console.WriteLine("\tunitOfMeasure: {0}", month.unitOfMeasure);
                Console.WriteLine("\tdevices:");
                foreach (var device in month.devices)
                {
                    Console.WriteLine("\t\tdeviceId: {0}", device.id);
                    Console.WriteLine("\t\tusage: {0}", device.usage);
                }
                Console.WriteLine("\tadditionalBlocksUsed: {0}", month.additionalBlocksUsed);
                Console.WriteLine("\tadditionalCostPerBlock: {0}", month.additionalCostPerBlock);
                Console.WriteLine("\tadditionalUnitsPerBlock: {0}", month.additionalUnitsPerBlock);
                Console.WriteLine("\tadditionalIncluded: {0}", month.additionalIncluded);
                Console.WriteLine("\tadditionalUsed: {0}", month.additionalUsed);
                Console.WriteLine("\tadditionalPercentUsed: {0}", month.additionalPercentUsed);
                Console.WriteLine("\tadditionalRemaining: {0}", month.additionalRemaining);
                Console.WriteLine("\tbillableOverage: {0}", month.billableOverage);
                Console.WriteLine("\toverageCharges: {0}", month.overageCharges);
                Console.WriteLine("\toverageUsed: {0}", month.overageUsed);
                Console.WriteLine("\tcurrentCreditAmount: {0}", month.currentCreditAmount);
                Console.WriteLine("\tmaxCreditAmount: {0}", month.maxCreditAmount);
                Console.WriteLine("\tpolicy: {0}", month.policy);

                Console.WriteLine("-------------------------------");

            }
        }
    }
}