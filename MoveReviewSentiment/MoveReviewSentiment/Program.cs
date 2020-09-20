using System;
using Microsoft.ML;
using SentimentAnalysisConsoleApp.DataStructures;
using static Microsoft.ML.DataOperationsCatalog;

namespace SentimentAnalysisConsoleApp
{
    internal static class Program
    {
        private static readonly string BaseDatasetsRelativePath = @"../../../../Data";
        private static readonly string DataRelativePath = $"{BaseDatasetsRelativePath}/imdbdataset.csv";
        private static readonly string DataPath = Helper.GetAbsolutePath(DataRelativePath);

        static void Main()
        {
            // STEP 1: Create MLContext with seed for deterministic results
            var mlContext = new MLContext(seed: 1);

            // STEP 2: Load data
            IDataView dataView = mlContext.Data.LoadFromTextFile<SentimentReview>(DataPath, hasHeader: true, separatorChar: ',', allowQuoting: true);

            // STEP 3: Split data into training and testing sets
            TrainTestData trainTestSplit = mlContext.Data.TrainTestSplit(dataView, testFraction: 0.2);
            IDataView trainingData = trainTestSplit.TrainSet;
            IDataView testData = trainTestSplit.TestSet;

            // STEP 4: Define table used to map from string values in our csv to bool values that our model can work with  
            var lookupData = new[] {
                new LookupMap { Value = "negative", Category = false },
                new LookupMap { Value = "positive", Category = true }
            };

            var lookupIdvMap = mlContext.Data.LoadFromEnumerable(lookupData);

            // STEP 5: Make pipeline (by applying the table from step 4)
            var dataProcessPipeline = mlContext.Transforms.Conversion.MapValue(outputColumnName: "Label", lookupMap: lookupIdvMap, lookupIdvMap.Schema["Value"], lookupIdvMap.Schema["Category"], inputColumnName: nameof(SentimentReview.Sentiment))
                .Append(mlContext.Transforms.Text.FeaturizeText(outputColumnName: "Features", inputColumnName: nameof(SentimentReview.Review)));

            // STEP 6: Set the training algorithm and it to the pipeline                          
            var trainer = mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(labelColumnName: "Label", featureColumnName: "Features");
            var trainingPipeline = dataProcessPipeline.Append(trainer);

            // STEP 7: Train the model (fitting to the trainingData)
            Console.WriteLine("Please wait. The model is currently being trained (and tested)...");
            ITransformer trainedModel = trainingPipeline.Fit(trainingData);

            // STEP 8: Evaluate the model on the test data
            var predictions = trainedModel.Transform(testData);
            var metrics = mlContext.BinaryClassification.Evaluate(data: predictions, labelColumnName: "Label", scoreColumnName: "Score");

            // STEP 9: Print the results of the evaluation
            Helper.PrintMetrics(metrics);

            // STEP 10: Create a prediction engine using the trained model
            var predEngine = mlContext.Model.CreatePredictionEngine<SentimentReview, SentimentPrediction>(trainedModel);

            // STEP 11: Create some example reviews (for testing the prediction engine)
            SentimentReview badReview = new SentimentReview { Review = "I hate this movie! It is terrible!" };
            SentimentReview goodReview = new SentimentReview { Review = "I love this movie! It is great!" };
            SentimentReview neutralReview = new SentimentReview { Review = "I don't know how I feel about this movie. It is OK, I guess." };
            
            // STEP 12: Predict whether each example review has a positive or negative sentiment
            var predBadReview = predEngine.Predict(badReview);
            var predGoodReview = predEngine.Predict(goodReview);
            var predNeutralReview = predEngine.Predict(neutralReview);

            // STEP 13: Print the predictions
            Helper.PrintPredictions(new[] { (badReview, predBadReview), (goodReview, predGoodReview), (neutralReview, predNeutralReview) });

            // STEP 14: Finish off
            Helper.PrintEnd();
        }
    }
}