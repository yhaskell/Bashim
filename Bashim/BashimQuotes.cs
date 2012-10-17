using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Bashim
{
    public class BashOrgRu
    {
        public class Quote
        {
            public string id { get; set; }
            public string quote { get; set; }
        }

        static string Convert(string s)
        {
            return s.Replace("&gt;", ">").Replace("&lt;", "<").Replace("&nbsp;", " ").Replace("&amp;", "&").Replace("&quot;", "\"");
        }

        IEnumerable<Quote> Quotes(string s, bool force = false)
        {
            int index = s.IndexOf("<a href=\"/quote/");
            if (force) index = 1;
            while (index != -1)
            {

                string iNum = s.Substring(index + 16, 6);

                int qstart = s.IndexOf("<div class=\"text\">", index);
                int qend = s.IndexOf("</div>", qstart);

                string quote = s.Substring(qstart + 18, qend - qstart - 18).Replace("<br>", "\n").Replace("<br />", "\n");

                yield return new Quote { id = iNum, quote = Convert(quote) };

                index = s.IndexOf("<a href=\"/quote/", qend);
            }
            yield break;
        }

        IEnumerable<Quote> AbyssTopQuotes(string s)
        {
            int index = 0;
            int num = 1;
            while (index != -1)
            {                
                int qstart = s.IndexOf("<div class=\"text\">", index);
                int qend = s.IndexOf("</div>", qstart);

                string quote = s.Substring(qstart + 18, qend - qstart - 18).Replace("<br>", "\n").Replace("<br />", "\n");

                yield return new Quote { id = (num++).ToString(), quote = Convert(quote) };

                index = s.IndexOf("<div class=\"quote", qend);
            }
            yield break;
        }

        public int PagesCount { get; set; }

        public async Task<IEnumerable<Quote>> GetNewQuotes(int pageId = -1)
        {
            HttpClient http = new HttpClient();

            var link = pageId >0 ? "http://bash.im/index/" + pageId.ToString() : "http://bash.im/";

            var httpResult = await http.GetAsync(link);

            IEnumerable<Quote> result = null;

            var enc = Encoding.GetEncoding("windows-1251");
            var byteArray = await httpResult.Content.ReadAsByteArrayAsync();
            var d = enc.GetString(byteArray, 0, byteArray.Length).Substring(16);

            Regex regex = new Regex(@"min=""1"" max=""(\d*)"" value=""(\d*)""");

            var u = regex.Match(d);

            var max = u.Groups[1].Value;
            var value = u.Groups[2].Value;

            PagesCount = int.Parse(max);

            var begin = d.IndexOf("<div class=\"quote\">");
            var end = d.IndexOf("<div class=\"pager\">", begin);

            var s = d.Substring(begin, end - begin);

            result = Quotes(s);

            return result;
        }

        private int? nextRandomSeed;

        public async Task<IEnumerable<Quote>> GetRandomQuotes()
        {
            HttpClient http = new HttpClient();

            var link = nextRandomSeed != null ? "http://bash.im/random?" + nextRandomSeed.ToString() : "http://bash.im/random";

            var httpResult = await http.GetAsync(link);

            IEnumerable<Quote> result = null;

            var enc = Encoding.GetEncoding("windows-1251");
            var byteArray = await httpResult.Content.ReadAsByteArrayAsync();
            var d = enc.GetString(byteArray, 0, byteArray.Length).Substring(16);

            var begin = d.IndexOf("<div class=\"quote\">");
            var end = d.IndexOf(@"<a href=""/random", begin);

            var s = d.Substring(begin, end - begin);

            Regex regex = new Regex(@"/random\?(\d*)");

            var u = regex.Match(d);

            var seed = u.Groups[1].Value;
            nextRandomSeed = int.Parse(seed);

            result = Quotes(s);

            return result;
        }

        public int? nextAbyssSeed;

        public async Task<IEnumerable<Quote>> GetAbyss()
        {
            HttpClient http = new HttpClient();

            var link = nextAbyssSeed != null ? "http://bash.im/abyss?" + nextRandomSeed.ToString() : "http://bash.im/abyss";

            var httpResult = await http.GetAsync(link);

            IEnumerable<Quote> result = null;

            var enc = Encoding.GetEncoding("windows-1251");
            var byteArray = await httpResult.Content.ReadAsByteArrayAsync();
            var d = enc.GetString(byteArray, 0, byteArray.Length).Substring(16);

            var begin = d.IndexOf("<div class=\"quote\">");
            var end = d.IndexOf("<div class=\"quote more\">", begin);

            var s = d.Substring(begin, end - begin);

            Regex regex = new Regex(@"/abyss\?(\d*)");

            var u = regex.Match(d);

            var seed = u.Groups[1].Value;
            nextAbyssSeed = int.Parse(seed);

            result = Quotes(s);

            return result;
        }

        public async Task<IEnumerable<Quote>> GetAbyssTop()
        {
            HttpClient http = new HttpClient();

            var link = "http://bash.im/abysstop";

            var httpResult = await http.GetAsync(link);

            IEnumerable<Quote> result = null;

            var enc = Encoding.GetEncoding("windows-1251");
            var byteArray = await httpResult.Content.ReadAsByteArrayAsync();
            var d = enc.GetString(byteArray, 0, byteArray.Length).Substring(16);

            var begin = d.IndexOf("<div class=\"quote\">");
            var end = d.IndexOf("<div class=\"inside\">", begin);

            var s = d.Substring(begin, end - begin);
            
            result = AbyssTopQuotes(s);

            return result;
        }
    }
}
