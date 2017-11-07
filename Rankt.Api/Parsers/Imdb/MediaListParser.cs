using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace TrakkerApp.Api.Parsers.Imdb
{
    public class MediaListParser
    {
        //Pass in List Id
        //Get List Title, and number of movies in List

        public int ListSize { get; set; }
        public string ListId { get; set; }
        public string ListName { get; set; }
        public string ListDescription { get; set; }
        public List<string> MediaList;

        public MediaListParser(string id, string listName)
        {
            ListId = id;
            ListName = listName;
            ListSize = 0;
            MediaList = new List<string>();
        }

        public async Task GetListOfMedia()
        {
            int startingPoint = 1;

            while (ListSize == 0 || ListSize > MediaList.Count)
            {
                if (MediaList.Count > startingPoint)
                {
                    startingPoint += 100;
                }

                var url = "http://www.imdb.com/list/" + ListId + "/?start=" + startingPoint;
                var web = new HtmlWeb();
                var doc = await web.LoadFromWebAsync(url);

                if (ListSize == 0)
                {
                    try
                    {
                        var listName = doc.DocumentNode
                            .Descendants("div")
                            .First(x => x.Attributes["class"] != null &&
                                        x.Attributes["class"].Value.Equals("description"));

                        ListDescription = listName.InnerHtml;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error in IMDB parser.  No Description. Message " + e.Message);
                    }

                    var desc = doc.DocumentNode
                        .Descendants("div")
                        .First(x => x.Attributes["class"] != null && x.Attributes["class"].Value.Equals("desc"));

                    ListSize = Convert.ToInt32(desc.Attributes["data-size"].Value);
                }
                var tempDiv = doc.DocumentNode
                    .Descendants("div")
                    .Where(x => x.Attributes["class"] != null &&
                                x.Attributes["class"].Value.Equals("hover-over-image zero-z-index"));

                foreach (var div in tempDiv)
                {
                    MediaList.Add(div.Attributes["data-const"].Value);
                }
            }
        }
    }
}