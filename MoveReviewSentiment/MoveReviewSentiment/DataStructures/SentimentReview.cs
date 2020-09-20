using Microsoft.ML.Data;

namespace SentimentAnalysisConsoleApp.DataStructures
{
    public class SentimentReview
    {
        [LoadColumn(1)]
        public string Sentiment { get; set; }

        [LoadColumn(0)]
        public string Review { get; set; }
    }
}
