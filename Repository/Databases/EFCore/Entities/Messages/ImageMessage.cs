namespace Repository
{
    public class ImageMessage : Message
    {
        public string ImageURL { get; set; } = DefaultImageURL;

        // Default Values
        public static string DefaultImageURL = "";
    }
}
