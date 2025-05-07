using System.Collections.Generic;

namespace BallisticsSimulation
{
    public interface IIntegrator
    {
        public List<State> Calculate(
            State initState, 
            double step, 
            int maxSteps, 
            BallisticsHandler handler,
            double eps = 1e-4, 
            double hMin = 1e-4, 
            double hMax = 1
            );
    }
}