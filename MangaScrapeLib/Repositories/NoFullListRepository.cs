﻿using MangaScrapeLib.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MangaScrapeLib.Repositories
{
    public abstract class NoFullListRepository : Repository
    {
        protected abstract Uri GetSearchUri(string query);
        protected abstract Series[] GetSeriesFromSearch(string searchPageHtml);

        public NoFullListRepository(string name, string uriString, string mangaIndexPageStr, SeriesMetadataSupport seriesMetadata, string iconFileName) : base(name, uriString, mangaIndexPageStr, seriesMetadata, iconFileName)
        {
        }

        public override async Task<ISeries[]> SearchSeriesAsync(string query)
        {
            var lowercaseQuery = query.ToLowerInvariant();
            var searchUri = GetSearchUri(lowercaseQuery);

            var html = await Client.GetStringAsync(searchUri);
            var output = GetSeriesFromSearch(html);
            output = output.OrderBy(d => d.Title).ToArray();
            return output;
        }
    }
}
