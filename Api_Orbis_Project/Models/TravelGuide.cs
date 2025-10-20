namespace Api_Orbis_Project.Models
{
    public class QuickGuideResponse
    {
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public string Summary { get; set; }
        public string[] KeyPoints { get; set; }
        public int ReadingTimeMinutes { get; set; }
        public GuideContent Content { get; set; }
    }
    public class SafetyGuide
    {
        public string CountryCode { get; set; }
        public List<Risk> MainRisks { get; set; } 
        public List<Area> SafeAreas { get; set; }   
        public List<Area> AvoidAreas { get; set; }  
        public List<Contact> EmergencyContacts { get; set; }
        public List<string> PracticalTips { get; set; }
        public int ReadingTimeMinutes { get; set; }
    }


    public class HealthGuide
    {
        public string CountryCode { get; set; }
        public List<Vaccine> RequiredVaccines { get; set; }
        public List<HealthRisk> HealthRisks { get; set; }
        public List<string> HygieneTips { get; set; }
        public List<MedicalService> MedicalServices { get; set; }
        public List<string> InsuranceTips { get; set; }
        public int ReadingTimeMinutes { get; set; }
    }

    public class CultureGuide
    {
        public string CountryCode { get; set; }
        public List<EtiquetteRule> BasicEtiquette { get; set; }
        public List<ClothingRule> ClothingRules { get; set; }
        public List<Custom> LocalCustoms { get; set; }
        public List<string> TimingExpectations { get; set; }
        public List<string> TippingPractices { get; set; }
        public int ReadingTimeMinutes { get; set; }
    }

    public class GuideContent { public string Introduction { get; set; } public string Details { get; set; } }
    public class Risk { public string Type { get; set; } public string Description { get; set; } }
    public class Area { public string Name { get; set; } public string Description { get; set; } }
    public class Contact { public string Name { get; set; } public string Phone { get; set; } }
    public class Vaccine { public string Name { get; set; } public string Description { get; set; } }
    public class HealthRisk { public string Type { get; set; } public string Prevention { get; set; } }
    public class MedicalService { public string Name { get; set; } public string Address { get; set; } }
    public class EtiquetteRule { public string Rule { get; set; } public string Description { get; set; } }
    public class ClothingRule { public string Situation { get; set; } public string Recommendation { get; set; } }
    public class Custom { public string Name { get; set; } public string Description { get; set; } }
}
