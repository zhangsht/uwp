using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Xml;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace HW7
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            phoneHome.Text = "";
            cardType.Text = "";
            GetPhoneHomeXML(phoneNumer.Text);
        }
        private async void GetPhoneHomeXML(string tel)
        {
            try
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                // 创建一个HTTP client实例对象
                HttpClient httpClient = new HttpClient();
                // Add a user-agent header to the GET request. 
                var headers = httpClient.DefaultRequestHeaders;
                // The safe way to add a header value is to use the TryParseAdd method and verify the return value is true,
                // especially if the header value is coming from user input.
                string header = "ie Mozilla/5.0 (Windows NT 6.2; WOW64; rv:25.0) Gecko/20100101 Firefox/25.0";
                if (!headers.UserAgent.TryParseAdd(header))
                {
                    throw new Exception("Invalid header value: " + header);
                }
                string getXMLCode = "http://life.tenpay.com/cgi-bin/mobile/MobileQueryAttribution.cgi?chgmobile=" + tel;
                //发送GET请求
                HttpResponseMessage response = await httpClient.GetAsync(getXMLCode);
                //// 确保返回值为成功状态
                response.EnsureSuccessStatusCode();

                Byte[] getByte = await response.Content.ReadAsByteArrayAsync();
                //// UTF-8是Unicode的实现方式之一。这里采用UTF-8进行编码
                Encoding code = Encoding.GetEncoding("gb2312");
                string result = code.GetString(getByte, 0, getByte.Length);

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(result);
                
                //检查是否出现错误
                XmlNodeList NodeLists = doc.GetElementsByTagName("retmsg");
                
                if (NodeLists[0].InnerText != "error")
                {
                    NodeLists = doc.GetElementsByTagName("province");
                    if (NodeLists != null)
                        phoneHome.Text = NodeLists[0].InnerXml;
                    NodeLists = doc.GetElementsByTagName("supplier");
                    if (NodeLists != null)
                        cardType.Text = NodeLists[0].InnerXml;
                } else
                {
                    await new MessageDialog("手机号码有误").ShowAsync();
                }
            }
            catch (HttpRequestException e1)
            {
                var errorMessage = new MessageDialog(e1.ToString()).ShowAsync();
                   
            }
            
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            phoneHome.Text = "";
            cardType.Text = "";
            GetPhoneHomeJson(phoneNumer.Text);
        }
        private async void GetPhoneHomeJson(string tel)
        {
            try
            {
                // 创建一个HTTP client实例对象
                HttpClient httpClient = new HttpClient();
                // Add a user-agent header to the GET request. 
                var headers = httpClient.DefaultRequestHeaders;
                // The safe way to add a header value is to use the TryParseAdd method and verify the return value is true,
                // especially if the header value is coming from user input.
                string header = "ie Mozilla/5.0 (Windows NT 6.2; WOW64; rv:25.0) Gecko/20100101 Firefox/25.0";
                if (!headers.UserAgent.TryParseAdd(header))
                {
                    throw new Exception("Invalid header value: " + header);
                }
                headers.Add("apikey", "b17463955df3dece479c7c0513a317b1");

                string getJsonCode = "http://apis.baidu.com/apistore/mobilephoneservice/mobilephone?tel=" + tel;
                //发送GET请求
                HttpResponseMessage response = await httpClient.GetAsync(getJsonCode);
                // 确保返回值为成功状态
                response.EnsureSuccessStatusCode();

                Byte[] getByte = await response.Content.ReadAsByteArrayAsync();
                // UTF-8是Unicode的实现方式之一。这里采用UTF-8进行编码
                Encoding code = Encoding.GetEncoding("UTF-8");
                string result = code.GetString(getByte, 0, getByte.Length);
                //var i = new MessageDialog(result).ShowAsync();

                // 反序列化结果字符串
                JObject res = (JObject)JsonConvert.DeserializeObject(result);
                if (res["errNum"].ToString() != "0")
                    throw (new Exception("手机号码有误"));

                if (res["retData"] != null)
                {
                    phoneHome.Text = res["retData"]["province"].ToString();
                    cardType.Text = res["retData"]["carrier"].ToString();
                }
            }
            catch (HttpRequestException e1)
            {
                var errorMessage = new MessageDialog(e1.ToString()).ShowAsync();

            }
            catch (Exception e2)
            {
                var errorMessage = new MessageDialog(e2.ToString()).ShowAsync();
            }
        }

    }
}
