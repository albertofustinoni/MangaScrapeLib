﻿using MangaScrapeLib.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MangaScrapeLib.TestServices
{
    /// <summary>
    ///This is a test class for EatMangaRepositoryTest and is intended
    ///to contain all EatMangaRepositoryTest Unit Tests
    ///</summary>
    [TestClass]
    public class EatMangaRepositoryTest : MangaRepositoryTestBase
    {
        internal override RepositoryBase GetRepository()
        {
            return EatMangaRepository.Instance;
        }
    }
}
