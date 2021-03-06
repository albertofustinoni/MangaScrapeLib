﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Html.Parser;
using MangaScrapeLib.Models;
using MangaScrapeLib.Tools;

namespace MangaScrapeLib.Repository
{
    internal abstract class RepositoryBase : IRepository
    {
        protected IWebClient WebClient { get; }
        protected IBrowsingContext BrowsingContext { get; }

        internal abstract Task<IReadOnlyList<IChapter>> GetChaptersAsync(Series input, CancellationToken token);
        internal abstract Task<IReadOnlyList<IPage>> GetPagesAsync(Chapter input, CancellationToken token);
        internal abstract Task<byte[]> GetImageAsync(Page input, CancellationToken token);

        private IReadOnlyList<ISeries> AvailableSeries { get; set; }

        protected static HtmlParser Parser { get; } = new HtmlParser();

        private string IconFileName { get; }
        private readonly Lazy<byte[]> icon;
        public byte[] Icon => icon.Value;

        public string Name { get; private set; }
        public Uri RootUri { get; private set; }

        public bool SupportsCover { get; }
        public bool SupportsAuthor { get; }
        public bool SupportsLastUpdateTime { get; }
        public bool SupportsTags { get; }
        public bool SupportsDescription { get; }

        protected RepositoryBase(IWebClient webClient, string name, string uriString, string iconFileName, bool supportsAllMetadata) :
            this(webClient, name, uriString, iconFileName, supportsAllMetadata, supportsAllMetadata, supportsAllMetadata, supportsAllMetadata, supportsAllMetadata)
        {

        }

        protected RepositoryBase(IWebClient webClient, string name, string uriString, string iconFileName, bool supportsCover, bool supportsAuthor, bool supportsLastUpdateTime, bool supportsTags, bool supportsDescription)
        {
            WebClient = webClient;
            BrowsingContext = new BrowsingContext(Configuration.Default.WithDefaultLoader().WithDefaultCookies());

            Name = name;
            RootUri = new Uri(uriString, UriKind.Absolute);

            IconFileName = iconFileName;

            icon = new Lazy<byte[]>(LoadIcon);

            SupportsCover = supportsCover;
            SupportsAuthor = supportsAuthor;
            SupportsLastUpdateTime = supportsLastUpdateTime;
            SupportsTags = supportsTags;
            SupportsDescription = supportsDescription;
        }

        public Task<IReadOnlyList<ISeries>> GetSeriesAsync()
        {
            using (var cts = new CancellationTokenSource())
            {
                return GetSeriesAsync(cts.Token);
            }
        }

        public virtual Task<IReadOnlyList<ISeries>> GetSeriesAsync(CancellationToken token)
        {
            return Task.FromResult<IReadOnlyList<ISeries>>(new ISeries[0]);
        }

        public Task<IReadOnlyList<ISeries>> SearchSeriesAsync(string query)
        {
            using (var cts = new CancellationTokenSource())
            {
                return SearchSeriesAsync(query, cts.Token);
            }
        }

        public virtual async Task<IReadOnlyList<ISeries>> SearchSeriesAsync(string query, CancellationToken token)
        {
            if (AvailableSeries == null)
            {
                AvailableSeries = await GetSeriesAsync(token);
            }

            var lowerQuery = query.ToLowerInvariant();
            return AvailableSeries.Where(d => d.Title.Contains(lowerQuery)).OrderBy(d => d.Title).ToArray();
        }

        internal virtual async Task<byte[]> GetImageAsync(ISeries input, CancellationToken token)
        {
            var output = default(byte[]);
            if (!SupportsCover)
            {
                return null;
            }

            if (input.CoverImageUri == null)
            {
                await input.GetChaptersAsync(token);
            }

            if (input.CoverImageUri != null && !token.IsCancellationRequested)
            {
                output = await WebClient.GetByteArrayAsync(input.CoverImageUri, input.SeriesPageUri, token);
            }

            return output;
        }

        public override string ToString()
        {
            return Name;
        }

        private byte[] LoadIcon()
        {
            var iconPath = string.Format("MangaScrapeLib.Resources.{0}", IconFileName);
            var currentAssembly = typeof(RepositoryBase).GetTypeInfo().Assembly;
            using (var stream = currentAssembly.GetManifestResourceStream(iconPath))
            using (var memStream = new MemoryStream())
            {
                stream.CopyTo(memStream);
                return memStream.ToArray();
            }
        }
    }
}
