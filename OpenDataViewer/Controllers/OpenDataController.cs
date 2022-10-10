using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using OpenDataViewer.Models;
using System.Text.Json;

namespace OpenDataViewer.Controllers
{   
    public class OpenDataController : Controller
    {
        public async Task<JObject?> CreateJsonObjectFromUri(string requestUri, int? limitAmount = null)
        {
            string uri;
            JObject jObj;

            if (limitAmount != null && !requestUri.Contains("&limit="))
                uri = requestUri + $"&limit={limitAmount}";
            else 
                uri = requestUri;

            // Get JSON string from api and parse it into a Json Object
            HttpClient client = new();
            HttpResponseMessage response = await client.GetAsync(uri);
            if (!response.IsSuccessStatusCode)  // Check if response from api is successful
                return null;

            string json = await response.Content.ReadAsStringAsync();
            if (json == null || json == "")     // Exception handling for json string being empty or null
                return null;

            jObj = JObject.Parse(json);
            return jObj;
        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Show()
        {
            string uri = "https://data.gov.lv/dati/lv/api/3/action/datastore_search?resource_id=58e7bbf1-c296-41c9-b45f-e2dd67fc9f1d";

            try
            {
                JObject? jObj = await CreateJsonObjectFromUri(uri);     // Creating a JObject from uri.
                int totalRecords = (int)jObj["result"]!["total"]!;      // Count of all the records in the selected dataset.
                int recCount = jObj["result"]!["records"]!.Count();    // Count of records that are returned by api call.

                // NOTE: data.gov.lv API returns only 100 records in pre-selected dataset.
                // If there are more then 100 records in the pre-selected dataset,
                // set 'limit' attribute in URI to total records in dataset.
                if (totalRecords > 100)
                {
                    jObj = await CreateJsonObjectFromUri(uri, totalRecords);
                    recCount = totalRecords;
                }

                // Iterate through each datarow, deserialize it into
                // 'RegObjStat' class object and add to list.
                var records = jObj["result"]!["records"]!;
                List<RegObjStat> datasetRecords = new();
                for (int i = 0; i < recCount; i++)
                {
                    string recordAsJson = records[i]!.ToString();
                    RegObjStat regObjStat = JsonSerializer.Deserialize<RegObjStat>(recordAsJson)!;
                    datasetRecords.Add(regObjStat);
                }

                // Save column names in a List.
                var fields = jObj["result"]!["fields"]!;
                List<string> columnNames = new();
                foreach (var field in fields)
                    columnNames.Add(field["id"]!.ToString());
                ViewBag.ColumnNames = columnNames;    // Pass column names to view for creating table header.

                // Pass a list of 'RegObjStat' type objects to view.
                return View(datasetRecords);
            }
            catch (Exception)
            {
                return View();
            }
        }
    }
}