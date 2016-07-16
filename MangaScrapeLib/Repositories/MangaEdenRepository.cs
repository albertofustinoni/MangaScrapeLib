﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MangaScrapeLib.Models;

namespace MangaScrapeLib.Repositories
{
    public class MangaEdenRepository : NoFullListRepository
    {
        private static readonly MangaEdenRepository instance = new MangaEdenRepository();
        public static MangaEdenRepository Instance { get { return instance; } }

        private MangaEdenRepository() : base("Manga Eden", "http://www.mangaeden.com/", "en/en-directory/", "MangaEden.png") { }

        protected override Uri GetSearchUri(string query)
        {
            var uriQuery = Uri.EscapeDataString(query);
            var output = new Uri(MangaIndexPage, string.Format("?title={0}", uriQuery));
            return output;
        }

        protected override Series[] GetSeriesFromSearch(string searchPageHtml)
        {
            var document = Parser.Parse(searchPageHtml);

            var table = document.QuerySelector("#mangaList");
            var rows = table.QuerySelectorAll("tr").Skip(1);
            var output = rows.Select(d =>
            {
                var links = d.QuerySelectorAll("a");
                var seriesLink = links.First();
                var updatesLink = links.Last();
                var updateText = updatesLink.TextContent.Replace('\n', ' ').Trim();
                var series = new Series(this, new Uri(RootUri, seriesLink.Attributes["href"].Value), seriesLink.TextContent.Trim()) { Updated = updateText };
                return series;
            }).OrderBy(d => d.Title).ToArray();

            return output;
        }

        internal override IChapter[] GetChapters(Series series, string seriesPageHtml)
        {
            throw new NotImplementedException();
        }

        internal override ISeries[] GetDefaultSeries(string mangaIndexPageHtml)
        {
            return GetSeriesFromSearch(mangaIndexPageHtml);
        }

        internal override Uri GetImageUri(string mangaPageHtml)
        {
            throw new NotImplementedException();
        }

        internal override IPage[] GetPages(Chapter chapter, string mangaPageHtml)
        {
            throw new NotImplementedException();
        }

        internal override void GetSeriesInfo(Series series, string seriesPageHtml)
        {
            throw new NotImplementedException();
        }
    }
}
