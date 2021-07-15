using amadeus;
using amadeus.exceptions;
using amadeus.resources;
using ApiKey;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using travel_app.src.Dijkstra;

namespace travel_app.src
{
    public class TravelSearch
    {
        private const int limit = 1000;
        private char[] alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

        private Amadeus amadeus;
        private List<Location> locationFrom = new List<Location>();
        private List<Location> locationTo = new List<Location>();

        public TravelSearch()
        {
            ConnectToApi();
        }

        private void ConnectToApi()
        {
            var apiKey = AmadeusKey.Key;
            var apiSecret = AmadeusKey.Secret;

            amadeus = Amadeus
                .builder(apiKey, apiSecret)
                .build();
        }

        public ObservableCollection<string> GetLocations(string inputString, Direction direction)
        {
            var locationViewList = new ObservableCollection<string>();
            var locationList = GetLocationList(direction);

            if ((inputString != "") && (locationList != null)) 
            {
                locationList.Clear();
                try 
                {
                    locationList.AddRange(amadeus.referenceData.locations.get(Params
                        .with("keyword", inputString)
                        .and("subType", resources.referenceData.Locations.ANY)).ToList());

                    foreach (Location location in locationList) 
                    {
                        locationViewList.Add($"{location.subType}: {location.detailedName}");
                    }
                } 
                catch (ResponseException e) 
                {
                    locationViewList.Add(e.Message);
                }

            } 
            else 
            {
                locationViewList = null;
            }
            return locationViewList;
        }

        private List<Location> GetLocationList(Direction direction)
        {
            List<Location> locationList = null;
            switch (direction) 
            {
                case Direction.From:
                    locationList = locationFrom;
                    break;
                case Direction.To:
                    locationList = locationTo;
                    break;
            }
            return locationList;
        }

        public ObservableCollection<string> GetFlights(int ixFrom, int ixTo, DateTime flightDate)
        {
            ObservableCollection<string> flightsViewList = new ObservableCollection<string>();
            FlightOffer[] flightOffers;

            try 
            {
                flightOffers = amadeus.shopping.flightOffers.get(Params
                    .with("origin", locationFrom[ixFrom].iataCode)
                    .and("destination", locationTo[ixTo].iataCode)
                    .and("departureDate", flightDate.ToString("yyyy-MM-dd")));
            } 
            catch (ResponseException e) 
            {
                flightsViewList.Add(e.Message);
                return flightsViewList;
            }

            FormatedView.CreateFormatedFlightsView(flightsViewList, flightOffers);
            return flightsViewList;
        }

        public ObservableCollection<string> GetDijkstraFlights(int ixFrom, int ixTo, DateTime depDate)
        {
            ObservableCollection<string> flightsViewList = new ObservableCollection<string>();
            DijkstraSrch dijkstraSrch;

            try 
            {
                dijkstraSrch = new DijkstraSrch(amadeus, locationFrom[ixFrom].iataCode, locationTo[ixTo].iataCode,
                depDate.ToString("yyyy-MM-dd"));
            } catch (ResponseException e) {
                flightsViewList.Add(e.Message);
                return flightsViewList;
            }

            FormatedView.CreateFormatedDijkstraFlightsView(flightsViewList, dijkstraSrch);
            return flightsViewList;
        }
    }
}
