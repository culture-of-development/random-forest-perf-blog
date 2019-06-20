using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Xunit;
using Xunit.Abstractions;
using rf.naive;

namespace rf.tests
{
    public class NaiveTests : TestsBase
    {
        public NaiveTests(ITestOutputHelper output) : base(output) 
        {
            const string path = @"../../../../../data";
            model = LoadModel(Path.Combine(path, "xgboost_model_trees.txt"));
            samples = LoadSamples(Path.Combine(path, "xgboost_test_cases.txt"));
        }

        private static RandomForest LoadModel(string path)
        {
            var treesString = File.ReadAllText(path);
            var model = ModelBuilder.CreateXGBoost(treesString);
            return model;
        }

        private static double[][] LoadSamples(string path)
        {
            var samplesString = File.ReadLines(path);
            var samples = new Dictionary<string, double[]>();
            foreach(var line in samplesString.Skip(1))
            {
                var parts = line.Split(',');
                var sampleId = parts[0];
                var featureIndex = int.Parse(parts[1]);
                var value = double.Parse(parts[2]);
                if (!samples.ContainsKey(sampleId))
                {
                    samples.Add(sampleId, new double[1000]);
                }
                samples[sampleId][featureIndex] = value;
            }
            var result = samples.Values.ToArray();
            return result;
        }

        RandomForest model;
        double[][] samples;
        const int probablity_feature_index = 840;
        
        [Fact]
        public void TestCorrectness()
        {
            foreach(var sample in samples)
            {
                var actual = model.EvaluateProbability(sample);
                var expected = sample[probablity_feature_index];
                Assert.InRange(actual, expected - 1e-06, expected + 1e06);
            }
            output.WriteLine("Correctness verified.");
        }

        [Fact]
        public void Timing()
        {
            var timer = Stopwatch.StartNew();
            foreach(var sample in samples)
            {
                model.EvaluateProbability(sample);
            }
            timer.Stop();
            output.WriteLine($"Time taken for {samples.Length} evaluations: {timer.Elapsed.TotalMilliseconds} ms");
        }
    }
}