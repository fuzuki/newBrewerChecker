using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace newbrewers
{
    /// <summary>
    /// 国税局のサイトから酒類等製造免許の新規取得者名等一覧を取得する
    /// </summary>
    /// 

    class BrewerUtil
    {
        private static readonly string[] TaxAgencyUrls = {
            "http://www.nta.go.jp/about/organization/sapporo/sake/menkyo/seizo.htm",
            "http://www.nta.go.jp/about/organization/sendai/sake/menkyo/seizo.htm",
            "http://www.nta.go.jp/about/organization/kantoshinetsu/sake/menkyo/seizo.htm",
            "http://www.nta.go.jp/about/organization/tokyo/sake/menkyo/seizo.htm",
            "http://www.nta.go.jp/about/organization/kanazawa/sake/menkyo/seizo.htm",
            "http://www.nta.go.jp/about/organization/nagoya/sake/menkyo/seizo.htm",
            "http://www.nta.go.jp/about/organization/osaka/sake/menkyo/seizo.htm",
            "http://www.nta.go.jp/about/organization/hiroshima/sake/menkyo/seizo.htm",
            "http://www.nta.go.jp/about/organization/takamatsu/sake/menkyo/seizo.htm",
            "http://www.nta.go.jp/about/organization/fukuoka/sake/menkyo/seizo.htm",
            "http://www.nta.go.jp/about/organization/kumamoto/sake/menkyo/seizo.htm",
            "http://www.nta.go.jp/about/organization/okinawa/sake/menkyo/seizo.htm" };


        private static readonly string BaseUrl = "http://www.nta.go.jp";
        private static readonly string PrefecturePat = "<td [a-z]+=\"nowrap\" rowspan=\"\\d+\"><a href=\"(/about/organization/[a-z]+/sake/menkyo/seizo/[a-z]+\\.htm)\">(.+)</a></td>"; // "<a href=\"\\./20\\d{6}/(20\\d{6}[a-z]\\d{5})/20\\d{6}[a-z]\\d{5}0000f\\.html\">(.+)<br>(.+)</a>";
//        private static readonly string PrefecturePat = "<td class="nowrap" rowspan="30"><a href="/about/organization/sapporo/sake/menkyo/seizo/index.htm">北海道</a></td>"
        private static readonly string DatePat = "<li><a href=\"(/about/organization/[a-z]+/sake/menkyo/seizo/data/[a-z]\\d+/\\d{2}/[a-z]+\\.htm)\">(.+)</a></li>";
        private static readonly string BrewerPat = "<tbody>(.+)</tbody>";


        /// <summary>
        /// 国税庁の酒類等製造免許の新規取得者名等一覧を返す。
        /// </summary>
        /// <returns></returns>
        public static List<Brewer> getBrewers()
        {
            var list = new List<Brewer>();
            var prefs = getPrefestureUrls();
            foreach(var p in prefs)
            {
                var pref = p[0];
//                 Console.WriteLine(pref);
                var regdate = getDateUrls(p[1]);
                foreach(var d in regdate)
                {
                    var blist = getBrewers(d[1]);
                    foreach(var b in blist)
                    {
                        list.Add(new Brewer(pref, b));
                    }

                }
            }
            return list;
        }

        /// <summary>
        /// 県ごとの新規登録者のurlを返す
        /// </summary>
        /// <returns></returns>
        public static List<string[]> getPrefestureUrls()
        {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            var wc = new System.Net.WebClient();
 //            wc.Encoding = System.Text.Encoding.UTF8;

            var list = new List<string[]>();

            foreach (var u in TaxAgencyUrls)
            {
                try
                {
                    var lines = wc.DownloadString(u).Split('\n');
                    foreach (var l in lines)
                    {
                        var m = System.Text.RegularExpressions.Regex.Match(l, PrefecturePat);
                        if (m.Success)
                        {
//                            Console.WriteLine(m.Groups[1].Value + ":" + m.Groups[2].Value);
                            string[] t = { m.Groups[2].Value, BaseUrl + m.Groups[1].Value };
                            list.Add(t);
                        }
                    }
                }
                catch (System.Net.WebException)
                {
                    Console.WriteLine("something wrong happen: " + u);
                    list.Clear();
                }
                System.Threading.Thread.Sleep(50);//連続ダウンロードを控えるため
            }
            return list;
        }


        /// <summary>
        /// 月ごとの新規登録urlを取得
        /// </summary>
        /// <param name="url">指定の県の</param>
        /// <returns></returns>
        public static List<string[]> getDateUrls(string url)
        {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            var wc = new System.Net.WebClient();

            var list = new List<string[]>();

            try
            {
                var lines = wc.DownloadString(url).Split('\n');
                foreach (var l in lines)
                {
                    var m = System.Text.RegularExpressions.Regex.Match(l, DatePat);
                    if (m.Success)
                    {
//                        Console.WriteLine(m.Groups[1].Value + ":" + m.Groups[2].Value);
                        string[] t = { m.Groups[2].Value, BaseUrl + m.Groups[1].Value };
                        list.Add(t);
                    }
                }
            }
            catch (System.Net.WebException)
            {
                Console.WriteLine("something wrong happen: " + url);
                list.Clear();
            }
            System.Threading.Thread.Sleep(50);//連続ダウンロードを控えるため

            return list;
        }


        /// <summary>
        /// 新規登録者を返す
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static List<string[]> getBrewers(string url)
        {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            var wc = new System.Net.WebClient();

            var list = new List<string[]>();

            try
            {
                var lines = wc.DownloadString(url).Replace("\n","");
                var m = System.Text.RegularExpressions.Regex.Match(lines, BrewerPat);
//                if (m.Success && !m.Groups[1].Value.Contains("該当なし"))
                if (m.Success)
                    {
//                    Console.WriteLine(m.Groups[1].Value.Replace("</tr>", "\n"));
                        var bl = m.Groups[1].Value.Replace("<tr>", "").Replace("</tr>", "\n").Trim().Split('\n');
                    foreach(var b in bl)
                    {
                        if(b.Trim().Length > 0 && !b.Contains("該当なし") && !b.Contains("以下余白") && !b.Contains("税務署名") && !b.Contains("&nbsp;"))
                        {
                            //getBrewer(b.Replace("<tr>", "").Trim());
                            var t = b.Replace("<td>", "").Replace("<td class=\"left\">", "").Replace("<td nowrap=\"nowrap\">", "").Replace("<td align=\"left\">", "").Replace("<td class=\"center\">", "").Replace("<td class=\"nowrap\">", "").Replace("<br />", "///").Replace("<br>", "///").Replace("</td>", "\n").Trim().Split('\n');
                            for(var i = 0; i < t.Length; i++)
                            {
                                t[i] = t[i].Trim();
                            }
//                            Console.WriteLine(string.Join(",",t));
                            list.Add(t);
                        }
                    }

                }
            }
            catch (System.Net.WebException)
            {
                Console.WriteLine("something wrong happen: " + url);
                list.Clear();
            }
            System.Threading.Thread.Sleep(50);//連続ダウンロードを控えるため

            return list;
        }

        /// <summary>
        /// 指定のパスに一覧をcsv形式で出力する
        /// </summary>
        /// <param name="path"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static void writeList(string path, List<Brewer> list) 
        {
            using (var sw = new System.IO.StreamWriter(path, false))
            {
                foreach(var b in list)
                {
                    sw.WriteLine(b.ToString());
                }
            }

        }
    }
}
