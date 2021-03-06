using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Ruokalista
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            try
            {
                GetRuoka();
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", ex.Message, "OK");
            }
        }


        public void GetRuoka()
        {
            var url = "https://peda.net/isokyro/ylakoulu/hyv%C3%A4-tiet%C3%A4%C3%A4/kouluruokailu:atom";

            char quote = '\u0022';

            WebClient client = new WebClient();

            string http = client.DownloadString(url);

            var basehtml = GetBetween(http, "<entry>", "</entry>");

            var updated = GetBetween(basehtml, "<updated>", "</updated>");

            DateTime datetimeupdate = DateTime.MinValue;
            try
            {
                datetimeupdate = DateTime.Parse(updated);
            }
            catch (Exception)
            {
                DisplayAlert("Error", "virhe päivämäärn lataamisessa", "OK");
            }

            var content = GetBetween(basehtml, $"<content type={quote}html{quote}>", "</content>");


            var Title = GetBetween(content, "&lt;p&gt;&lt;b&gt;", "&lt;br/&gt;&#10;");

            TitleLabel.Text = Title;

            var ruoka = content.Replace("&lt;br/&gt;&#10;", "\n");
            ruoka = ruoka.Replace("&lt;p&gt;&lt;b&gt;" + Title, string.Empty);
            ruoka = ruoka.Replace("&lt;br/&gt;&#10;", string.Empty);
            ruoka = ruoka.Replace("&lt;/b&gt;", string.Empty);
            ruoka = ruoka.Replace("&lt;/p&gt;&#10;", string.Empty);
            ruoka = ruoka.Replace("\n\n\n", string.Empty);
            ruoka = ruoka.Replace("\n", "\n\n");
            ruoka = ruoka.Replace("&lt;b&gt;", " ");

            updatedlabel.Text = "päivitetty: " + datetimeupdate.ToString("dd/MM/yyyy HH:mm");
            RuokaLabel.Text = ruoka;

            if(TitleLabel.Text == "")
            {
                TitleLabel.Text = "Ruokalista Error";
            }
        }

        public static string GetBetween(string strSource, string strStart, string strEnd)
        {
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                int Start, End;
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                return strSource.Substring(Start, End - Start);
            }

            return "";
        }
    }
}
