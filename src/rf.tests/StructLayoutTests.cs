using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Xunit;
using Xunit.Abstractions;

namespace rf.tests
{
    public class StructLayoutTests : TestsBase
    {
        public StructLayoutTests(ITestOutputHelper output) : base(output) 
        {
        }

        [Fact]
        public void TestOffsets()
        {
            OutputOffsets(typeof(DecisionTreeNodeBad));
            OutputOffsets(typeof(DecisionTreeNodeBetter));
            //OutputOffsets(typeof(DecisionTreeNodeAuto)); // cannot marshal this object
        }

        private void OutputOffsets(Type type)
        {
            output.WriteLine("\n" + type.Name);
            var fieldOffsets = GetFieldOffsets(type);
            foreach(var f in fieldOffsets)
            {
                output.WriteLine($"  {f.Name,15}: {f.Offset}");
            }
        }

        private (string Name, IntPtr Offset)[] GetFieldOffsets(Type type)
        {
            return type.GetFields()
                .Select(f => (Name: f.Name, Offset: Marshal.OffsetOf(type, f.Name) ))
                .ToArray();
        }

        [Fact]
        public void TestActualSizes()
        {
            OutputActualSize<DecisionTreeNodeBad>();
            OutputActualSize<DecisionTreeNodeBetter>();
            OutputActualSize<DecisionTreeNodeAuto>();
        }

        private void OutputActualSize<T>() where T : struct
        {
            var x = GC.GetAllocatedBytesForCurrentThread();
            var tmp = new T[1<<20];
            x = GC.GetAllocatedBytesForCurrentThread() - x;
            output.WriteLine("\n" + typeof(T).Name);
            output.WriteLine($"   size per struct: {x / tmp.Length}");
        }

        [Fact]
        public unsafe void TestSizeOfs()
        {
            output.WriteLine($"{nameof(DecisionTreeNodeBad)}: {sizeof(DecisionTreeNodeBad)}");
            output.WriteLine($"{nameof(DecisionTreeNodeBetter)}: {sizeof(DecisionTreeNodeBetter)}");
            output.WriteLine($"{nameof(DecisionTreeNodeAuto)}: {sizeof(DecisionTreeNodeAuto)}");
        }
    }
}