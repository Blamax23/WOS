using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WOS.Model
{
    public class WebhookPayloadUPS
    {
        [JsonProperty("trackingNumber")]
        public string TrackingNumber { get; set; }

        [JsonProperty("localActivityDate")]
        public string LocalActivityDate { get; set; }

        [JsonProperty("localActivityTime")]
        public string LocalActivityTime { get; set; }

        [JsonProperty("activityLocation")]
        public ActivityLocation ActivityLocation { get; set; }

        [JsonProperty("activityStatus")]
        public ActivityStatus ActivityStatus { get; set; }

        [JsonProperty("scheduledDeliveryDate")]
        public string ScheduledDeliveryDate { get; set; }

        [JsonProperty("actualDeliveryDate")]
        public string ActualDeliveryDate { get; set; }

        [JsonProperty("actualDeliveryTime")]
        public string ActualDeliveryTime { get; set; }

        [JsonProperty("gmtActivityDate")]
        public string GmtActivityDate { get; set; }

        [JsonProperty("gmtActivityTime")]
        public string GmtActivityTime { get; set; }

        [JsonProperty("deliveryStartTime")]
        public string DeliveryStartTime { get; set; }

        [JsonProperty("deliveryEndTime")]
        public string DeliveryEndTime { get; set; }

        [JsonProperty("deliveryTimeDescription")]
        public string DeliveryTimeDescription { get; set; }
    }

    public class ActivityLocation
    {
        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("stateProvince")]
        public string StateProvince { get; set; }

        [JsonProperty("postalCode")]
        public string PostalCode { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }
    }

    public class ActivityStatus
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("descriptionCode")]
        public string DescriptionCode { get; set; }
    }
}
