using System.Management.Automation;
using System.Net.Http;

namespace BIXLifxPS
{
    [Cmdlet(VerbsCommon.Set, "BIXBulb")]
    [OutputType(typeof(string))]
    public class SetBixBulb : Cmdlet
    {
        private static string _baseUrl = Config.BaseUrl;

        private static readonly HttpClient Client = new HttpClient();

        private static string _response;

        [Parameter]
        public string BaseUrl
        {
            get => _baseUrl;
            set => _baseUrl = value;
        }


        [Parameter(Position = 0, ValueFromPipelineByPropertyName = true, ValueFromPipeline = true, Mandatory = true)]
        [Alias("Bulb", "Label")]
        public string BulbLabel { get; set; }

        [Parameter]
        public ColorEnum Color { get; set; }

        [Parameter]
        [ValidateRange(1, 100)]
        [AllowNull]
        public int? Dim { get; set; }

        [Parameter]
        [ValidateSet("On", "Off", "Toggle", IgnoreCase = true)]
        public string Power { get; set; }

        protected override void BeginProcessing()
        {
            base.BeginProcessing();

            var cmd = $"Light={BulbLabel}";
            if (!string.IsNullOrEmpty(Power))
                cmd = cmd + $"&Power={Power}";

            if (Color != ColorEnum.ZZZZDontSet)
                cmd = cmd + $"&Color={Color}";

            if (Dim != null)
                cmd = cmd + $"&Dim={Dim}";

            var page = $"{_baseUrl}{cmd}";
            WriteVerbose($"Running: {page}");
            var response = Client.GetAsync(page).Result;
            _response = response.Content.ReadAsStringAsync().Result;
        }

        protected override void ProcessRecord()
        {
            WriteObject(_response);
        }
    }
}