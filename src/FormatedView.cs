using amadeus.resources;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using travel_app.src.Dijkstra;

namespace travel_app.src
{
    internal static class FormatedView
    {
        internal static void CreateFormatedFlightsView(ObservableCollection<string> flightsViewList, FlightOffer[] flightOffers)
        {
            foreach (FlightOffer offer in flightOffers) {
                foreach (FlightOffer.OfferItem item in offer.offerItems) {
                    string flight = $"Price {item.price.total}| Taxes {item.price.totalTaxes}| ";

                    foreach (FlightOffer.Service service in item.services) {
                        foreach (FlightOffer.Segment segment in service.segments) {
                            flight += $"{segment.flightSegment.departure.iataCode}({segment.flightSegment.departure.at}) " +
                                $"-> { segment.flightSegment.arrival.iataCode}({segment.flightSegment.arrival.at})| ";
                        }
                    }

                    flightsViewList.Add(flight);
                }
            }
        }

        internal static void CreateFormatedDijkstraFlightsView(ObservableCollection<string> flightsViewList, DijkstraSrch dijkstraSrch)
        {
            flightsViewList.Add($"Price {dijkstraSrch.ShortestPathCost}| Checked locations {dijkstraSrch.NodeVisits}| ");
            List<Node> path = dijkstraSrch.ShortestPath;
            for (int i = 0; i < (path.Count - 1); i++) {
                flightsViewList.Add($"{path[i].Name} -> {path[i + 1].Name}");
            }
        }
    }
}
