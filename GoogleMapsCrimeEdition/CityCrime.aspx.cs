using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;
using Newtonsoft.Json;

namespace GoogleMapsCrimeEdition
{
    public partial class CityCrime : System.Web.UI.Page
    {
        private const string Path =
             (@"C:\Users\DanCu\OneDrive\Documents\Visual Studio 2015\WebSites\GoogleMapPlotTest\CSVFiles\5b743788-99b9-4814-8f29-3cf75735023e.csv");

        /// <summary>
        /// Point object used to contain longitude and latitude on a map.
        /// </summary>
        public class Point
        {
            /// <summary>
            /// Latitude of map point
            /// </summary>
            public string Lat { get; set; }

            /// <summary>
            /// Longitute of map point
            /// </summary>
            public string Log { get; set; }
        }

        /// <summary>
        /// List of map points 
        /// </summary>
        public List<Point> MapPoints { get; set; }

        /// <summary>
        /// CSV crime object.
        /// Maps to row in CSV file.
        /// </summary>
        public class Crime
        {
            public string CrimeId { get; set; }
            public string Month { get; set; }
            public string Latitude { get; set; }
            public string Longitude { get; set; }
            public string Location { get; set; }
            public string Category { get; set; }
            public string OutcomeStatus { get; set; }
        }

        /// <summary>
        /// List of map points 
        /// </summary>
        public List<Crime> Crimes { get; set; }


        /// <summary>
        /// Page load event.
        /// </summary>
        private void Page_Load()
        {
            if (!Page.IsPostBack)
            {
                List<Crime> lstCrimeCsv = GetCrimes(Path);
                BindComboBoxes(lstCrimeCsv);
            }
        }

        /// <summary>s
        /// Gets crimes from CSV file and returns them 
        /// as a list of T as well as sends them back to client side via hidden field.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private List<Crime> GetCrimes(string path)
        {
            List<Crime> crimes = ReadCsv(path);
            HiddenField.Value = SerialiseList(crimes);

            return crimes;
        }

        private void BindComboBoxes(List<Crime> crimes)
        {
            List<Crime> distinctTypes = crimes.GroupBy(type => type.Category)
                .Select(grp => grp.First())
                .ToList();

            var crimeTypes = distinctTypes.Select(c => c.Category)
                .Where(c => c != "")
                .ToList();

            cmbCrimeType.DataSource = crimeTypes;
            cmbCrimeType.DataBind();
            cmbCrimeType.Items.Insert(0, "All Crime");

            List<Crime> distinctStatuses = crimes.GroupBy(type => type.OutcomeStatus)
                .Select(grp => grp.First())
                .ToList();

            var crimeStatus = distinctStatuses.Select(c => c.OutcomeStatus)
                .Where(c => c != "")
                .ToList();

            cmbCrimeStatus.DataSource = crimeStatus;
            cmbCrimeStatus.DataBind();
            cmbCrimeStatus.Items.Insert(0, "All Statuses");
        }

        protected void btnrefresh_OnServerClick(object sender, EventArgs e)
        {
            List<Crime> crimes = ReadCsv(Path);
            List<Crime> filteredResult = crimes;

            string crimeStatusFilter = cmbCrimeStatus.Text;
            string crimeTypeFilter = cmbCrimeType.Text;

            if (cmbCrimeStatus.SelectedIndex != 0)
            {
                filteredResult =
                    crimes.Where(p => crimes.All(t => p.OutcomeStatus.Contains(crimeStatusFilter))).ToList();
            }
            if (cmbCrimeType.SelectedIndex != 0)
            {
                filteredResult = filteredResult.Where(p => crimeTypeFilter.All(t => p.Category.Contains(t))).ToList();
            }

            HiddenField.Value = SerialiseList(filteredResult);
        }

        /// <summary>
        /// Serialise lise to string.
        /// </summary>
        /// <param name="lstCrimeCsv"></param>
        /// <returns></returns>
        private string SerialiseList(List<Crime> lstCrimeCsv)
        {
            return JsonConvert.SerializeObject(lstCrimeCsv);
        }

        /// <summary>
        /// Reads CSV file using CsvHelper.
        /// Ref:https://joshclose.github.io/CsvHelper/
        /// </summary>
        /// <param name="path"></param>
        private List<Crime> ReadCsv(string path)
        {
            List<Crime> crimes = new List<Crime>();

            TextReader reader = File.OpenText(path);
            var csv = new CsvReader(reader);
            while (csv.Read())
            {
                Crime crime = new Crime();
                crime.CrimeId = csv.GetField<string>("Crime ID");
                crime.Month = csv.GetField<string>("Month");
                crime.Latitude = csv.GetField<string>("Latitude");
                crime.Longitude = csv.GetField<string>("Longitude");
                crime.Location = csv.GetField<string>("Location");
                crime.Category = csv.GetField<string>("Category");
                crime.OutcomeStatus = csv.GetField<string>("Outcome status");
                crimes.Add(crime);
            }

            return crimes;
        }
    }
}
