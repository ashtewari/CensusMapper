using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CensusMapper.Models;

namespace CensusMapper.ViewModels
{
    internal class Mappings
    {

        internal static string CountyNameToFips(CountyFips counties, string stateAbr, string countyName)
        {
            if (counties == null) return string.Empty;

            string name = countyName.Trim();
            if (name.EndsWith("Co."))
            {
                name = name.Substring(0, name.Length - 3);
            }
            else if (name.EndsWith("County"))
            {
                name = name.Substring(0, name.Length - 6);
            }

            var result = counties.table.rows.FirstOrDefault((row) => row[1] == string.Format("{0}, {1}", stateAbr, name.Trim()));

            if (result == null) return string.Empty;

            return result[0];
        }

        internal static string StateAbbreviationToFips(string abbreviation)
        {
            string name = StateAbbreviationToName(abbreviation);
            if (string.IsNullOrEmpty(name)) return string.Empty;

            var states = GetStatesList();
            var state = states.FirstOrDefault(s => s.Value.Name.ToLowerInvariant().CompareTo(name.ToLowerInvariant()) == 0);

            if (string.IsNullOrEmpty(state.Key)) return string.Empty;

            return state.Key;
        }

        internal static string StateAbbreviationToName(string abbreviation)
        {
            Dictionary<string, string> states = new Dictionary<string, string>();

            states.Add("AL", "Alabama");
            states.Add("AK", "Alaska");
            states.Add("AZ", "Arizona");
            states.Add("AR", "Arkansas");
            states.Add("CA", "California");
            states.Add("CO", "Colorado");
            states.Add("CT", "Connecticut");
            states.Add("DE", "Delaware");
            states.Add("DC", "District of Columbia");
            states.Add("FL", "Florida");
            states.Add("GA", "Georgia");
            states.Add("HI", "Hawaii");
            states.Add("ID", "Idaho");
            states.Add("IL", "Illinois");
            states.Add("IN", "Indiana");
            states.Add("IA", "Iowa");
            states.Add("KS", "Kansas");
            states.Add("KY", "Kentucky");
            states.Add("LA", "Louisiana");
            states.Add("ME", "Maine");
            states.Add("MD", "Maryland");
            states.Add("MA", "Massachusetts");
            states.Add("MI", "Michigan");
            states.Add("MN", "Minnesota");
            states.Add("MS", "Mississippi");
            states.Add("MO", "Missouri");
            states.Add("MT", "Montana");
            states.Add("NE", "Nebraska");
            states.Add("NV", "Nevada");
            states.Add("NH", "New Hampshire");
            states.Add("NJ", "New Jersey");
            states.Add("NM", "New Mexico");
            states.Add("NY", "New York");
            states.Add("NC", "North Carolina");
            states.Add("ND", "North Dakota");
            states.Add("OH", "Ohio");
            states.Add("OK", "Oklahoma");
            states.Add("OR", "Oregon");
            states.Add("PA", "Pennsylvania");
            states.Add("RI", "Rhode Island");
            states.Add("SC", "South Carolina");
            states.Add("SD", "South Dakota");
            states.Add("TN", "Tennessee");
            states.Add("TX", "Texas");
            states.Add("UT", "Utah");
            states.Add("VT", "Vermont");
            states.Add("VA", "Virginia");
            states.Add("WA", "Washington");
            states.Add("WV", "West Virginia");
            states.Add("WI", "Wisconsin");
            states.Add("WY", "Wyoming");
            if (states.ContainsKey(abbreviation))
                return (states[abbreviation]);

            return string.Empty;
        }

        internal static Dictionary<string, CensusEntityViewModel> GetStatesList()
        {
            var states = new Dictionary<string, CensusEntityViewModel>();

            states.Add("01", new CensusEntityViewModel("01", "Alabama", 33.001, -86.766233));
            states.Add("02", new CensusEntityViewModel("02", "Alaska", 61.288254, -148.716968));

            states.Add("04", new CensusEntityViewModel("04", "Arizona", 33.373506, -111.828711));
            states.Add("05", new CensusEntityViewModel("05", "Arkansas", 35.080251, -92.576816));
            states.Add("06", new CensusEntityViewModel("06", "California", 35.458606, -119.355165));

            states.Add("08", new CensusEntityViewModel("08", "Colorado", 39.500656, -105.203628));
            states.Add("09", new CensusEntityViewModel("09", "Connecticut", 41.494852, -72.874365));
            states.Add("10", new CensusEntityViewModel("10", "Delaware", 39.397164, -75.561908));
            states.Add("11", new CensusEntityViewModel("11", "District of Columbia", 38.910092, -77.014001));
            states.Add("12", new CensusEntityViewModel("12", "Florida", 27.795850, -81.634622));
            states.Add("13", new CensusEntityViewModel("13", "Georgia", 33.332208, -83.868887));

            states.Add("15", new CensusEntityViewModel("15", "Hawaii", 21.146768, -157.524452));
            states.Add("16", new CensusEntityViewModel("16", "Idaho", 44.242605, -115.133222));
            states.Add("17", new CensusEntityViewModel("17", "Illinois", 41.278216, -88.380238));
            states.Add("18", new CensusEntityViewModel("18", "Indiana", 40.163935, -86.261515));
            states.Add("19", new CensusEntityViewModel("19", "Iowa", 41.960392, -93.049161));
            states.Add("20", new CensusEntityViewModel("20", "Kansas", 38.454303, -96.536052));
            states.Add("21", new CensusEntityViewModel("21", "Kentucky", 37.808159, -85.241819));
            states.Add("22", new CensusEntityViewModel("22", "Louisiana", 30.699270, -91.457133));
            states.Add("23", new CensusEntityViewModel("23", "Maine", 44.313614, -69.719931));
            states.Add("24", new CensusEntityViewModel("24", "Maryland", 39.145653, -76.797396));
            states.Add("25", new CensusEntityViewModel("25", "Massachusetts", 42.271831, -71.363628));
            states.Add("26", new CensusEntityViewModel("26", "Michigan", 42.866412, -84.170753));
            states.Add("27", new CensusEntityViewModel("27", "Minnesota", 45.210782, -93.583003));
            states.Add("28", new CensusEntityViewModel("28", "Mississippi", 32.566420, -89.593164));
            states.Add("29", new CensusEntityViewModel("29", "Missouri", 38.437715, -92.153770));
            states.Add("30", new CensusEntityViewModel("30", "Montana", 46.813302, -111.209708));
            states.Add("31", new CensusEntityViewModel("31", "Nebraska", 41.183753, -97.403875));
            states.Add("32", new CensusEntityViewModel("32", "Nevada", 37.165965, -116.304648));
            states.Add("33", new CensusEntityViewModel("33", "New Hampshire", 43.153046, -71.463342));
            states.Add("34", new CensusEntityViewModel("34", "New Jersy", 40.438458, -74.428055));
            states.Add("35", new CensusEntityViewModel("35", "New Mexico", 34.623012, -106.342108));
            states.Add("36", new CensusEntityViewModel("36", "New York", 41.507548, -74.645228));
            states.Add("37", new CensusEntityViewModel("37", "North Carolina", 35.553334, -79.667654));
            states.Add("38", new CensusEntityViewModel("38", "North Dakota", 47.375168, -99.334736));
            states.Add("39", new CensusEntityViewModel("39", "Ohio", 40.480854, -82.749366));
            states.Add("40", new CensusEntityViewModel("40", "Oklahoma", 35.597940, -96.834653));
            states.Add("41", new CensusEntityViewModel("41", "Oregon", 44.732273, -122.579524));
            states.Add("42", new CensusEntityViewModel("42", "Pennsylvania", 40.463528, -77.075925));

            states.Add("44", new CensusEntityViewModel("44", "Rhode Island", 41.753318, -71.448902));
            states.Add("45", new CensusEntityViewModel("45", "South Carolina", 34.034551, -81.032387));
            states.Add("46", new CensusEntityViewModel("46", "South Dakota", 44.047502, -99.043799));
            states.Add("47", new CensusEntityViewModel("47", "Tennessee", 35.795862, -86.397772));
            states.Add("48", new CensusEntityViewModel("48", "Texas", 30.943149, -97.388631));
            states.Add("49", new CensusEntityViewModel("49", "Utah", 40.438987, -111.900160));
            states.Add("50", new CensusEntityViewModel("50", "Vermont", 44.081127, -72.814309));
            states.Add("51", new CensusEntityViewModel("51", "Virginia", 37.750345, -77.835857));

            states.Add("53", new CensusEntityViewModel("53", "Washington", 47.341728, -121.624501));
            states.Add("54", new CensusEntityViewModel("54", "West Virginia", 38.767195, -80.820221));
            states.Add("55", new CensusEntityViewModel("55", "Wisconsin", 43.728544, -89.001006));
            states.Add("56", new CensusEntityViewModel("56", "Wyoming", 42.675762, -107.008835));

            return states;
        }

        private async Task<string> ReadTextFile(string filename)
        {
            var folder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            var file = await folder.GetFileAsync(filename);
            var text = await Windows.Storage.FileIO.ReadTextAsync(file);
            return text;
        }

    }
}
