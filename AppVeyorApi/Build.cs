namespace AppVeyorApi
{
    internal class Build
    {
        public string Version { get; set; }
        public string Status { get; set; }
        public Jobs[] Jobs { get; set; }
    }
}