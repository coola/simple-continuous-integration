using System.Collections.Generic;

namespace SimpleContinousIntegration.BuildStrategies
{
    public interface IBuilder
    {
        bool BuildSolution();
        List<string> CurrentAssemblyList { get; }
    }
}