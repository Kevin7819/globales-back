namespace Api_Orbis_Project.Models
{
    public class BasicInfoDto
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string OfficialName { get; set; }
        public string Region { get; set; }
        public string Subregion { get; set; }
        public long? Population { get; set; }
        public IEnumerable<string>? Languages { get; set; }
        public IEnumerable<string>? Capital { get; set; }
    }
}
