﻿using MangaScrapeLib.Repository;

namespace MangaScrapeLib
{
    public static class Repositories
    {
        private static EatMangaRepository eatManga = new EatMangaRepository();
        public static IRepository EatManga => eatManga;

        private static MangaEdenRepository mangaEden = new MangaEdenRepository();
        public static IRepository MangaEden => mangaEden;

        private static MangaHereRepository mangaHere = new MangaHereRepository();
        public static IRepository MangaHere => mangaHere;

        private static MangaStreamRepository mangaStream = new MangaStreamRepository();
        public static IRepository MangaStream => mangaStream;

        private static MyMangaRepository myManga = new MyMangaRepository();
        public static IRepository MyManga => myManga;
    }
}
