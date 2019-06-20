using System;
using System.IO;
using Xunit;
using Xunit.Abstractions;

namespace rf.tests
{
    public abstract class TestsBase
    {
        protected readonly ITestOutputHelper output;
        private static string outputFilename = null;

        public TestsBase(ITestOutputHelper output)
        {
            // HACK: xunit creates a new instance on every test so we just store a static var with the
            // filename to catch all the output in one place when we run more than one test.
            if (outputFilename == null)
            {
                const string outputPath = "../../../../../test-output";
                if (!Directory.Exists(outputPath))
                {
                    Directory.CreateDirectory(outputPath);
                }
                string filename = DateTime.UtcNow.ToString("yyyyMMdd-HHmmss") + ".test-output";
                outputFilename = Path.Combine(outputPath, filename);
            }
            this.output = new FileWritingTestOutputHelper(outputFilename, output);
        }

        private class FileWritingTestOutputHelper : ITestOutputHelper
        {
            private readonly ITestOutputHelper inner;
            private readonly string filename;

            public FileWritingTestOutputHelper(string filename, ITestOutputHelper main)
            {
                inner = main;
                this.filename = filename;
            }

            public void WriteLine(string message)
            {
                inner.WriteLine(message);
                File.AppendAllText(filename, $"[{DateTime.UtcNow}] {message}\n");
            }

            public void WriteLine(string format, params object[] args)
            {
                inner.WriteLine(format, args);
                var message = string.Format(format, args);
                File.AppendAllText(filename, $"[{DateTime.UtcNow}] {message}\n");
            }
        }
    }
}
