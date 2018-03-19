using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Runtime.InteropServices;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.IO;
using System.Net.Http;
using System.Net;

namespace TroubleTicketDesign
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class Page1 : Page
    {
        //private const int SW_SHOWNORMAL = 1;
        //private const int SW_SHOWMINIMIZED = 2;
        //private const int SW_SHOWMAXIMIZED = 3;

        //double screenLeft = SystemParameters.VirtualScreenLeft;
        //double screenTop = SystemParameters.VirtualScreenTop;
        //double screenWidth = SystemParameters.VirtualScreenWidth;
        //double screenHeight = SystemParameters.VirtualScreenHeight;

        

        

        static HttpClient client = new HttpClient();


        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);


        public Page1()
        {
            InitializeComponent();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {


        }

        private void Button_Screenshot_Click(object sender, RoutedEventArgs e)
        {
            //Minimize window so we do not take a picture of it
            var window = Application.Current.MainWindow;
            window.WindowState = WindowState.Minimized;

            //Sleep so picture does not include window
            System.Threading.Thread.Sleep(500);

            //Make the image and save it
             var image = test();

            var byteArray = ImageToByteArray(image);
            

            callApi(byteArray);

            //SaveClipboardImageToFile(image);
            window.WindowState = WindowState.Normal;

        }


        private BitmapSource test()
        {
            BitmapSource result;

            Bitmap screenshotBitmap;

            screenshotBitmap = new System.Drawing.Bitmap((int)SystemParameters.PrimaryScreenWidth, (int)SystemParameters.PrimaryScreenHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(screenshotBitmap))
            {
                g.CopyFromScreen(0, 0, 0, 0, screenshotBitmap.Size);

            }

            IntPtr handle = IntPtr.Zero;
            try
            {
                handle = screenshotBitmap.GetHbitmap();
                result = Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                return result;
            }
            finally
            {
                DeleteObject(handle);
            }
        }

        public static void SaveClipboardImageToFile(BitmapSource image)
        {
            string filePath = "C:\\Temp\\test.png";
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(image));
                encoder.Save(fileStream);
            }
        }

        public static byte[] ImageToByteArray(BitmapSource bitmapSource)
        {
            byte[] bit;
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            //encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
            encoder.QualityLevel = 100;
            // byte[] bit = new byte[0];
            using (MemoryStream stream = new MemoryStream())
            {
                encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                encoder.Save(stream);
                bit = stream.ToArray();
                stream.Close();
            }

            return bit;
        }

        //static async void callApi(Byte[] byteArray)
        //{
        //     HttpClient client = new HttpClient();
        //    client.BaseAddress = new Uri("http://localhost:64195/");
        //    client.DefaultRequestHeaders.Accept.Clear();
        //    HttpContent screenshot = new ByteArrayContent(byteArray);
        //    HttpResponseMessage reponse = await client.PostAsync("api/image", screenshot);



        //}
        //static async void callApi(Byte[] byteArray)
        //{
        //    HttpClient client = new HttpClient();
        //    client.BaseAddress = new Uri("http://localhost:53555/");
        //    client.DefaultRequestHeaders.Accept.Clear();

        //    var photo_converted = Convert.ToBase64String(byteArray);
        //    var request = "api/image?photo=";
        //    request = request + photo_converted;

        //    HttpResponseMessage reponse = await client.PostAsync(request, null);

        //    var yay = "yay";



        //}
        public class Issue
        {
            public string Image { get; set; }
            
        }

        //public static async Task<string> callApi(byte[] image)
        //{

        //    HttpClient client = new HttpClient();
        //    StringContent queryString = new StringContent(Convert.ToBase64String(image));
        //    var uri = new Uri("http://localhost:53555/api/image");




        //    HttpResponseMessage response = await client.PostAsync(
        //        uri, queryString);

        //    response.EnsureSuccessStatusCode();
        //    string responseBody = await response.Content.ReadAsStringAsync();

        //    return responseBody;

        //} 

        public static void callApi(byte[] image)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://localhost:53555/api/image");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            string json = "{\"image\":\"";
            json = json + Convert.ToBase64String(image) + "\"}";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
            }

        }

        /*public bool callApi(byte[] image)
        {
            string url = "http://localhost:53555/api/image";
            var wc = new WebClient();
            wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            var parameters = new NameValueCollection()
            {
                { "image", Convert.ToBase64String(image) }
            };

            NameValueCollection myNameValueCollection = new NameValueCollection();
            myNameValueCollection.Add("Image", Convert.ToBase64String(image));

            var res = wc.UploadValues(url, myNameValueCollection);
            return true;
        }*/


    }
}
