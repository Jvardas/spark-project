using Microsoft.Spark.Sql;
using System;
using System.Diagnostics;
using static Microsoft.Spark.Sql.Functions;

namespace ChicagoIncidentsSpark
{
    class Program
    {
        static void Main(string[] args)
        {

            var spark = SparkSession.Builder()
                .Config("spark.jars.packages", "org.mongodb.spark:mongo-spark-connector_2.11:2.4.0,org.mongodb:mongo-java-driver:3.4.3")
                .GetOrCreate();

            //Debugger.Launch();
            Stopwatch sw = Stopwatch.StartNew();

            //reading dataset from csvs and import them in DataFrames
            DataFrame vehicles = spark.Read().Format("csv").Option("header", "true").Option("inferSchema", true).Csv(@"../../../Csvs/abandoned-vehicles.csv");
            DataFrame alleyLightsOut = spark.Read().Format("csv").Option("header", "true").Option("inferSchema", true).Csv(@"../../../Csvs/alley-lights-out.csv");
            DataFrame garbage = spark.Read().Format("csv").Option("header", "true").Option("inferSchema", true).Csv(@"../../../Csvs/garbage-carts.csv");
            DataFrame graffiti = spark.Read().Format("csv").Option("header", "true").Option("inferSchema", true).Csv(@"../../../Csvs/graffiti-removal.csv");
            DataFrame potholes = spark.Read().Format("csv").Option("header", "true").Option("inferSchema", true).Csv(@"../../../Csvs/pot-holes-reported.csv");
            DataFrame rodentBaiting = spark.Read().Format("csv").Option("header", "true").Option("inferSchema", true).Csv(@"../../../Csvs/rodent-baiting.csv");
            DataFrame sanitation = spark.Read().Format("csv").Option("header", "true").Option("inferSchema", true).Csv(@"../../../Csvs/sanitation-code.csv");
            DataFrame streetLightsOut = spark.Read().Format("csv").Option("header", "true").Option("inferSchema", true).Csv(@"../../../Csvs/street-lights-all-out.csv");
            DataFrame streetLightsOneOut = spark.Read().Format("csv").Option("header", "true").Option("inferSchema", true).Csv(@"../../../Csvs/street-lights-one-out.csv");
            DataFrame treeDebris = spark.Read().Format("csv").Option("header", "true").Option("inferSchema", true).Csv(@"../../../Csvs/tree-debris.csv");
            DataFrame treeTrims = spark.Read().Format("csv").Option("header", "true").Option("inferSchema", true).Csv(@"../../../Csvs/tree-trims.csv");

            Console.WriteLine($"\tRead CSVs in: {sw.Elapsed}");
            sw.Stop();
            sw.Reset();

            sw = Stopwatch.StartNew();

            //Find the total requests per day for each request type
            var reqCreationDates = vehicles.Select("Creation Date", "Service Request Number", "Type of Service Request").Union(
                                           alleyLightsOut.Select("Creation Date", "Service Request Number", "Type of Service Request")).Union(
                                           garbage.Select("Creation Date", "Service Request Number", "Type of Service Request")).Union(
                                           graffiti.Select("Creation Date", "Service Request Number", "Type of Service Request")).Union(
                                           potholes.Select("Creation Date", "Service Request Number", "Type of Service Request")).Union(
                                           rodentBaiting.Select("Creation Date", "Service Request Number", "Type of Service Request")).Union(
                                           sanitation.Select("Creation Date", "Service Request Number", "Type of Service Request")).Union(
                                           streetLightsOut.Select("Creation Date", "Service Request Number", "Type of Service Request")).Union(
                                           streetLightsOneOut.Select("Creation Date", "Service Request Number", "Type of Service Request")).Union(
                                           treeDebris.Select("Creation Date", "Service Request Number", "Type of Service Request")).Union(
                                           treeTrims.Select("Creation Date", "Service Request Number", "Type of Service Request")
                                           ).Filter(Col("Service Request Number").IsNotNull());

            var reqPerTypePerDay = reqCreationDates.WithColumn("Creation Date", DateFormat(reqCreationDates["Creation Date"], "yyyy-MM-dd"))
                .GroupBy("Creation Date", "Type of Service Request").Agg(Count("Service Request Number").As("Total Requests"))
                .Sort(Desc("Total Requests"));

            //saving the result into the MongoDB collection
            reqPerTypePerDay
                .Write()
                .Format("com.mongodb.spark.sql.DefaultSource")
                .Option("uri", "mongodb://127.0.0.1/sparkOutputs.totalRequestsPerTypeAndDay")
                .Mode(SaveMode.Overwrite)
                .Save();

            //Completed Requests per Day
            var reqCompletionDates = vehicles.Select("Completion Date", "Service Request Number", "Type of Service Request", "Status").Union(
                                           alleyLightsOut.Select("Completion Date", "Service Request Number", "Type of Service Request", "Status")).Union(
                                           garbage.Select("Completion Date", "Service Request Number", "Type of Service Request", "Status")).Union(
                                           graffiti.Select("Completion Date", "Service Request Number", "Type of Service Request", "Status")).Union(
                                           potholes.Select("Completion Date", "Service Request Number", "Type of Service Request", "Status")).Union(
                                           rodentBaiting.Select("Completion Date", "Service Request Number", "Type of Service Request", "Status")).Union(
                                           sanitation.Select("Completion Date", "Service Request Number", "Type of Service Request", "Status")).Union(
                                           streetLightsOut.Select("Completion Date", "Service Request Number", "Type of Service Request", "Status")).Union(
                                           streetLightsOneOut.Select("Completion Date", "Service Request Number", "Type of Service Request", "Status")).Union(
                                           treeDebris.Select("Completion Date", "Service Request Number", "Type of Service Request", "Status")).Union(
                                           treeTrims.Select("Completion Date", "Service Request Number", "Type of Service Request", "Status")
                                           ).Filter(Col("Service Request Number").IsNotNull()).Filter(Col("Status").StartsWith("Compl"));
            Debugger.Launch();
            var completedRequestsPerDay = reqCompletionDates.WithColumn("Completion Date", DateFormat(reqCompletionDates["Completion Date"], "yyyy-MM-dd"))
                .GroupBy("Completion Date").Count().WithColumnRenamed("count", "Completed Requests Num").Sort(Desc("Completion Date"));

            //saving the result into the MongoDB collection
            completedRequestsPerDay
                .Write()
                .Format("com.mongodb.spark.sql.DefaultSource")
                .Option("uri", "mongodb://127.0.0.1/sparkOutputs.completedRequestsPerDay")
                .Mode(SaveMode.Overwrite)
                .Save();
           
            //Number of violation for each license plates of vehicles
            var lp = vehicles
                .Select("License Plate", "Creation Date", "Service Request Number").Filter(Col("License Plate").IsNotNull())
                .GroupBy("License Plate").Count().WithColumnRenamed("count", "Number of complaints").Sort(Desc("Number of complaints"));//Agg(Count(vehicles.Col("License Plate") > 1));

            ////saving the result into the MongoDB collection licensePlates
            lp
                .Write()
                .Format("com.mongodb.spark.sql.DefaultSource")
                .Option("uri", "mongodb://127.0.0.1/sparkOutputs.licensePlates")
                .Mode(SaveMode.Overwrite)
                .Save();


            //SSAs in regards of requests per day
            var top5Ssa = vehicles.Select("SSA", "Creation Date", "Service Request Number").Union(
                               graffiti.Select("SSA", "Creation Date", "Service Request Number")).Union(
                               potholes.Select("SSA", "Creation Date", "Service Request Number")).Union(
                               garbage.Select("SSA", "Creation Date", "Service Request Number")).Filter(Col("SSA").IsNotNull());

            var top5SsasResult = top5Ssa.WithColumn("Creation Date", DateFormat(top5Ssa["Creation Date"], "yyyy-MM-dd"))
                .GroupBy("Creation Date", "SSA").Agg(Count(top5Ssa.Col("Service Request Number")).As("Service Request Count"))
                .Sort(Desc("Service Request Count"));

            //saving the result into the MongoDB collection
            top5SsasResult
            .Write()
            .Format("com.mongodb.spark.sql.DefaultSource")
            .Option("uri", "mongodb://127.0.0.1/sparkOutputs.top5Ssa")
            .Mode(SaveMode.Overwrite)
            .Save();
            
            Console.WriteLine($"\tFinished exporting results in MongoDB in: {sw.Elapsed}");
            sw.Stop();

        }
    }
}