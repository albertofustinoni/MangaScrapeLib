﻿using AngleSharp.Parser.Html;
using MangaScrapeLib.Tools;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace MangaScrapeLib.Repository
{
    internal abstract class RepositoryBase : IRepository
    {
        protected readonly IWebClient WebClient;

        internal abstract Task<IChapter[]> GetChaptersAsync(ISeries input, CancellationToken token);
        internal abstract Task<IPage[]> GetPagesAsync(IChapter input, CancellationToken token);
        internal abstract Task<byte[]> GetImageAsync(IPage input, CancellationToken token);

        private ISeries[] AvailableSeries { get; set; }

        protected static readonly HtmlParser Parser = new HtmlParser();

        private readonly string IconFileName;
        private readonly Lazy<byte[]> icon;
        public byte[] Icon { get { return icon.Value; } }

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

        public virtual Task<ISeries[]> GetSeriesAsync(CancellationToken token)
        {
            return Task.FromResult(new ISeries[0]);
        }

        public virtual async Task<ISeries[]> SearchSeriesAsync(string query, CancellationToken token)
        {
            if (AvailableSeries == null)
            {
                AvailableSeries = await GetSeriesAsync(token);
            }

            var lowerQuery = query.ToLowerInvariant();
            return AvailableSeries.Where(d => d.Title.Contains(lowerQuery)).OrderBy(d => d.Title).ToArray();
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
