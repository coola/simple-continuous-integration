using System.Collections.Generic;

namespace SimpleContinousIntegration.Builder
{
    public interface IBuilder
    {
        bool BuildSolution();
        List<string> CurrentAssemblyList { get; }
    }
}