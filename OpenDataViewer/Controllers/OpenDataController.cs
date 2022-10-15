using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using OpenDataViewer.Models;
using System.Text.Json;

namespace OpenDataViewer.Controllers
{   
    public class OpenDataController : Controller
    {
        public async Task<JObject> CreateJsonObjectFromUri(string requestUri, int? returnedRecordLimit = null)
        {
            string uri;

            // Specifying final URI.
            if (returnedRecordLimit != null && !requestUri.Contains("&limit="))
                uri = requestUri + $"&limit={returnedRecordLimit}";
            else 
                uri = requestUri;

            // Getting JSON string from api and parsing it into a Json Object.
            HttpClient client = new();
            HttpResponseMessage response = await client.GetAsync(uri);
            string json = await response.Content.ReadAsStringAsync();
            JObject jObj = JObject.Parse(json);

            return jObj;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Show()
        {
            // Hardcoded open data resource URI.
            string uri = "https://data.gov.lv/dati/lv/api/3/action/datastore_search?resource_id=58e7bbf1-c296-41c9-b45f-e2dd67fc9f1d";

            try
            {
                JObject jObj = await CreateJsonObjectFromUri(uri);  // Creating a JsonObject from uri.

                int totalRecords = (int)jObj["result"]!["total"]!;  // Number of all records in the selected dataset.
                int recCount = jObj["result"]!["records"]!.Count(); // Number of records that are returned by api call.

                // NOTE: data.gov.lv API returns only 100 records of pre-selected dataset (because of pagination).
                // If there are more then 100 records in the pre-selected dataset,
                // set 'limit' attribute in URI to total records in the dataset.
                if (totalRecords > 100)
                {
                    jObj = await CreateJsonObjectFromUri(uri, totalRecords);
                    recCount = totalRecords;
                }

                // CREATE A DYNAMIC OBJECT
                
                
                
                // Iterating through each dataset record, deserializing it into
                // a 'RegObjStat' class object and adding it to a list.
                var records = jObj["result"]!["records"]!;
                List<RegObjStatRecord> datasetRecords = new();
                for (int i = 0; i < recCount; i++)
                {
                    string recordAsJson = records[i]!.ToString();
                    RegObjStatRecord regObjStat = JsonSerializer.Deserialize<RegObjStatRecord>(recordAsJson)!;
                    datasetRecords.Add(regObjStat);
                }

                // Saving column names in a list.
                var fields = jObj["result"]!["fields"]!;
                List<string> columnNames = new();
                foreach (var field in fields)
                    columnNames.Add(field["id"]!.ToString());
                ViewBag.ColumnNames = columnNames;    // Passing column names to view for creating table header.

                // Passing a list of 'RegObjStat' type objects to view.
                return View(datasetRecords);
            }
            catch (Exception)
            {
                return View();
            }
        }
    }
}