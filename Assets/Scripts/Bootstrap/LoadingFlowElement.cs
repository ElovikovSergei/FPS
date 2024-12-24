using System.Collections.Generic;
using System;

namespace FPS.Bootstrap
{
    public sealed class LoadingFlowElement
    {
        public Type Segment { get; private set; }
        public List<LoadingFlowElement> Dependencies { get; private set; }

        public LoadingFlowElement(Type loadSegment)
        {
            Segment = loadSegment;
        }

        public LoadingFlowElement AddDependency(LoadingFlowElement dependency)
        {
            Dependencies ??= new List<LoadingFlowElement>();
            Dependencies.Add(dependency);

            return this;
        }
    }
}