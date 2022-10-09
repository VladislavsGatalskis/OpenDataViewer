using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using OpenDataViewer.Models;
using System.Text.Json;

namespace OpenDataViewer.Controllers
{   
    public class OpenDataController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Show()
        {
            // Get JSON string from api and parse it into a Json Object
            string url = "https://data.gov.lv/dati/lv/api/3/action/datastore_search?resource_id=58e7bbf1-c296-41c9-b45f-e2dd67fc9f1d";
            HttpClient client = new HttpClient();
            string json = client.GetStringAsync(url).Result;
            JObject jObj = JObject.Parse(json);

            int totalRecords = (int)jObj["result"]!["total"]!; // Count of all the records in the selected dataset
            int recCount = jObj["result"]!["records"]!.Count(); // Count of records that are returned by api call


            // If there are more records then amount retrieved from api call,
            // set 'limit' attribute in URI to 'totalRecords' in dataset.
            if (totalRecords > recCount)
            {
                string url2 = $"https://data.gov.lv/dati/lv/api/3/action/datastore_search?resource_id=58e7bbf1-c296-41c9-b45f-e2dd67fc9f1d&limit={totalRecords}";
                string json2 = client.GetStringAsync(url2).Result;
                jObj = JObject.Parse(json2);

                recCount = totalRecords;
            }

            // Iterate through each datarow, deserialize it into
            // VARregObj class object and add to list.
            var records = jObj["result"]!["records"]!;
            List<RegObjStat> dataRows = new List<RegObjStat>();
            for (int i = 0; i < recCount; i++)
            {
                string jsonString = records[i]!.ToString();
                RegObjStat regObjStat = JsonSerializer.Deserialize<RegObjStat>(jsonString)!;
                dataRows.Add(regObjStat);
            }

            // Save keys (column names) in a List
            var fields = jObj["result"]!["fields"]!;
            List<string> keys = new List<string>()!;
            foreach (var key in fields)
                keys.Add(key["id"]!.ToString());
            ViewBag.Keys = keys;

            // Pass a list of VARregObj type objects to view
            return View(dataRows);
        }
    }
}