using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms.TimeSeries;
using PFE.Framework.Training.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Text;


namespace PFE.Framework.Training
{
    class RandomForest
    {
        public string rootDir;
        public string dbFilePath;
        public string modelPath;
        public MLContext mlContext;
        public string connectionString;
        public DatabaseLoader loader;
        public string query;
        public IDataView firstYearData;
        public IDataView secondYearData;
        public SsaForecastingTransformer forecaster;
        private RandomForest() 
        {
            rootDir = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../"));
            dbFilePath = Path.Combine(rootDir, "Data", "DailyDemand.mdf");
            modelPath = Path.Combine(rootDir, "MLModel.zip");
            mlContext = new MLContext();

            connectionString = $"Data Source=LAPTOP-O68NSLJF\\LOCALDB;Database=PFE;Integrated Security=True;Connect Timeout=30;";
            loader = mlContext.Data.CreateDatabaseLoader<DataModel>();

            query = "SELECT ConsumptionDate, CAST(Year as REAL) as Year, CAST(TotalConsumption as REAL) as TotalConsumption FROM Consumption";
            DatabaseSource dbSource = new DatabaseSource(SqlClientFactory.Instance,
                                connectionString,
                                query);

            IDataView dataView = loader.Load(dbSource);

            firstYearData = mlContext.Data.FilterRowsByColumn(dataView, "Year", upperBound: 1);
            secondYearData = mlContext.Data.FilterRowsByColumn(dataView, "Year", lowerBound: 1);

            var forecastingPipeline = mlContext.Forecasting.ForecastBySsa(
                outputColumnName: "ForecastedConnection",
                inputColumnName: "TotalConnection",
                windowSize: 7,
                seriesLength: 30,
                trainSize: 365,
                horizon: 7,
                confidenceLevel: 0.95f,
                confidenceLowerBoundColumn: "LowerBoundConnection",
                confidenceUpperBoundColumn: "UpperBoundConnection");

            SsaForecastingTransformer forecaster = forecastingPipeline.Fit(firstYearData);
        }

        public void EvaluateModel()
        {
            var forecastEngine = forecaster.CreateTimeSeriesEngine<DataModel, ModelOutput>(mlContext);
            forecastEngine.CheckPoint(mlContext, modelPath);
        }

        void Forecast(IDataView testData, int horizon, TimeSeriesPredictionEngine<DataModel, ModelOutput> forecaster, MLContext mlContext)
        {
            ModelOutput forecast = forecaster.Predict();

        }

        private RandomForest _instance;
        public RandomForest GetInstance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new RandomForest();
                }

                return _instance;
            }
        }


    }
}