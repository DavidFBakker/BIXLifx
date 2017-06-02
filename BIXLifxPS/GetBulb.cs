using System.Collections.Generic;
using System.Management.Automation;
using System.Net.Http;
using LIFXDevices;
using Newtonsoft.Json;

namespace BIXLifxPS
{
    [Cmdlet(VerbsCommon.Get, "BIXBulb")]
    [OutputType(typeof(LightBulb))]
    public class GetBIXBulb : Cmdlet
    {
        private static string _baseUrl = Config.BaseUrl;

        private static readonly HttpClient Client = new HttpClient();

        [Parameter]
        public string BaseUrl
        {
            get => _baseUrl;
            set => _baseUrl = value;
        }


        [Parameter(Position = 0, ValueFromPipelineByPropertyName = true, ValueFromPipeline = true)]
        [Alias("Bulb", "Label")]
        public string BulbLabel { get; set; }

        private List<LightBulb> Bulbs { get; set; }

        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            Bulbs = new List<LightBulb>();
            if (string.IsNullOrEmpty(BulbLabel))
                BulbLabel = "all";

            var cmd = $"Light={BulbLabel}&Json";
            var page = $"{_baseUrl}{cmd}";
            WriteVerbose($"Running: {page}");
            var response = Client.GetAsync(page).Result;
            ;
            var contents = response.Content.ReadAsStringAsync().Result;
            Bulbs = JsonConvert.DeserializeObject<List<LightBulb>>(contents);
        }

        protected override void ProcessRecord()
        {
            foreach (var bulb in Bulbs)
            {
                bulb.Label = "Shit";
                WriteObject(bulb);
            }
        }
    }
}