using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GetImagesFromGoogleSearch
{
    public partial class frmMain : Form
    {
        string imagesPath = @"C:\Users\user\Desktop\Hackton\TF_Test\images";
        public frmMain()
        {
            InitializeComponent();
        }

        static string getHtmlContent(string url)
        {
            string toReturn;
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36";

            using (var response = webRequest.GetResponse())
            {
                using (var content = response.GetResponseStream())
                {
                    using (var reader = new StreamReader(content, Encoding.UTF8))
                    {
                        toReturn = reader.ReadToEnd();
                    }
                }
            }
            return toReturn;
        }

        static byte[] getImageFromUrl(string url)
        {
            var webClient = new WebClient();
            return webClient.DownloadData(url);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Enabled = false;
            string pathToSaveTo = Path.Combine(imagesPath, txtSearch.Text.Trim().Replace(" ", "_"));
            string htmlContent = getHtmlContent("https://www.google.co.il/search?q="+ txtSearch.Text.Trim().Replace(" ", "+") + "&safe=off&source=lnms&tbm=isch&sa=X&ved=0ahUKEwi4v9X98bDUAhVJtBQKHc-SDhgQ_AUIBigB&biw=2411&bih=1598");
            int i = 1;
            List<String> imageUrls = Regex.Matches(htmlContent, "(?<=\"ou\":\").*?(?=\\?|\")", RegexOptions.IgnoreCase | RegexOptions.Singleline).Cast<Match>().Select(item => item.Value).ToList();
            foreach (string m in imageUrls)
            {
                progressBar1.Value = (int)Math.Round((double)i / (double)imageUrls.Count() * 100.0);
                Application.DoEvents();
                try
                {
                    byte[] imageData = getImageFromUrl(m);
                    if (!Directory.Exists(pathToSaveTo))
                    {
                        Directory.CreateDirectory(pathToSaveTo);
                    }
                    using (FileStream fs = new FileStream(Path.Combine(pathToSaveTo, i.ToString() + Path.GetExtension(m)), FileMode.CreateNew))
                    {
                        fs.Write(imageData, 0, imageData.Length);
                        fs.Dispose();
                    }
                }
                catch { }
                i++;
            }
            this.Enabled = true;
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            txtProductName.Text = txtSearch.Text.Trim().Replace(" ", "_");
        }
    }
}
