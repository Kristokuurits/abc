using CommunityToolkit.Mvvm.ComponentModel;
using peeter.Models;
using Services;
using System.Collections.ObjectModel;

namespace peeter.ViewModels
{
    public partial class HomeViewModel : ObservableObject
    {
        private readonly TmdbService _tmdbService;
        public HomeViewModel(TmdbService tmdbService)
        {
            _tmdbService = tmdbService;
        }

        [ObservableProperty]
        private Media _trendingMovie;

        public ObservableCollection<Media> Trending { get; set; } = new();
        public ObservableCollection<Media> TopRated { get; set; } = new();
        public ObservableCollection<Media> NetflixOriginals { get; set; } = new();
        public ObservableCollection<Media> ActionMovies { get; set; } = new();

        public async Task InitializeAsync()
        {
            var trendingListTask = _tmdbService.GetTrendingAsync();
            var netflixOriginalsListTask = _tmdbService.GetNetflixOriginalAsync();
            var topRatedListTask = _tmdbService.GetTopRatedAsync();
            var actionLisTaskt = _tmdbService.GetActionAsync();

            var medias = await Task.WhenAll(trendingListTask,
                                        netflixOriginalsListTask,
                                        topRatedListTask,
                                        actionLisTaskt);

            var trendingList = medias[0];
            var netflixOriginalsList = medias[1];
            var topRatedList = medias[2];
            var actionList = medias[3];

            SetMediaCollection(trendingList, Trending);
            SetMediaCollection(netflixOriginalsList, NetflixOriginals);
            SetMediaCollection(topRatedList, TopRated);
            SetMediaCollection(actionList, ActionMovies);

            if (trendingList?.Any() == true)
            {
                foreach (var trending in trendingList)
                {
                    Trending.Add(trending);
                }
            }

            if (netflixOriginalsList?.Any() == true)
            {
                foreach (var original in NetflixOriginals)
                {
                    NetflixOriginals.Add(original);
                }
            }

            TrendingMovie = trendingList.OrderBy(t => Guid.NewGuid())
                                 .FirstOrDefault(t =>
                                 !string.IsNullOrWhiteSpace(t.DisplayTitle)
                                 && !string.IsNullOrWhiteSpace(t.Thumbnail));

        }
        private static void SetMediaCollection(IEnumerable<Media> medias, ObservableCollection<Media> collection)
        {
            collection.Clear();
            foreach (var media in medias)
            {
                collection.Add(media);
            }
        }
    }
}
