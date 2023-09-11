namespace SapODataClient.Model
{
    public class Picklist
    {
        public Metadata __metadata { get; set; }
        
        public string picklistId { get; set; }
        
        public Nav picklistOptions { get; set; }
    }

    public class PicklistOption
    {
        public Metadata __metadata { get; set; }
        public string id { get; set; }
        public string minValue { get; set; }
        public string externalCode { get; set; }
        public string maxValue { get; set; }
        public string optionValue { get; set; }
        public int sortOrder { get; set; }
        public string mdfExternalCode { get; set; }
        public string status { get; set; }
        public Nav parentPicklistOption { get; set; }
        public Nav picklistLabels { get; set; }
        public Picklist picklist { get; set; }
        public Nav childPicklistOptions { get; set; }
    }

    public class PicklistLabels
    {
        public Metadata __metadata { get; set; }
        public string optionId { get; set; }
        public string locale { get; set; }
        public string id { get; set; }
        public string label { get; set; }
        public Nav picklistOption { get; set; }
    }
}