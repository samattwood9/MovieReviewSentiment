using Microsoft.ML.Data;
using SentimentAnalysisConsoleApp.DataStructures;
using System;
using System.Collections.Generic;
using System.IO;

namespace SentimentAnalysisConsoleApp
{
    public static class Helper
    {
        public static string GetAbsolutePath(string relativePath)
        {
            FileInfo _dataRoot = new FileInfo(typeof(Program).Assembly.Location);
            string assemblyFolderPath = _dataRoot.Directory.FullName;

            string fullPath = Path.Combine(assemblyFolderPath, relativePath);

            return fullPath;
        }

        public static void PrintMetrics(CalibratedBinaryClassificationMetrics metrics)
        {
            Console.WriteLine();
            Console.WriteLine($"************************************************************");
            Console.WriteLine($"*       Metrics");
            Console.WriteLine($"*-----------------------------------------------------------");
            Console.WriteLine($"*       Accuracy: {metrics.Accuracy:P2}");
            Console.WriteLine($"*       Area Under Curve:      {metrics.AreaUnderRocCurve:P2}");
            Console.WriteLine($"*       Area under Precision recall Curve:  {metrics.AreaUnderPrecisionRecallCurve:P2}");
            Console.WriteLine($"*       F1Score:  {metrics.F1Score:P2}");
            Console.WriteLine($"*       LogLoss:  {metrics.LogLoss:#.##}");
            Console.WriteLine($"*       LogLossReduction:  {metrics.LogLossReduction:#.##}");
            Console.WriteLine($"*       PositivePrecision:  {metrics.PositivePrecision:#.##}");
            Console.WriteLine($"*       PositiveRecall:  {metrics.PositiveRecall:#.##}");
            Console.WriteLine($"*       NegativePrecision:  {metrics.NegativePrecision:#.##}");
            Console.WriteLine($"*       NegativeRecall:  {metrics.NegativeRecall:P2}");
            Console.WriteLine($"************************************************************");

            return;
        }

        public static void PrintPredictions(IEnumerable<(SentimentReview, SentimentPrediction)> predictions)
        {
            Console.WriteLine();
            Console.WriteLine("************************************************************");
            Console.WriteLine("*       Example Predictions      ");
            Console.WriteLine("*-----------------------------------------------------------");

            foreach ((SentimentReview, SentimentPrediction) prediction in predictions)
                Console.WriteLine($"*       Review: {prediction.Item1.Review} | Predicted Sentiment: {(Convert.ToBoolean(prediction.Item2.Prediction) ? "Postive" : "Negative")} ({prediction.Item2.Probability}) ");

            Console.WriteLine("************************************************************");

            return;
        }

        public static void PrintEnd()
        {
            Console.WriteLine();
            Console.WriteLine("************************************************************");
            Console.WriteLine("End of Process. Hit any key to exit.");
            Console.WriteLine("************************************************************");
            Console.ReadLine();

            return;
        }
    }
}
