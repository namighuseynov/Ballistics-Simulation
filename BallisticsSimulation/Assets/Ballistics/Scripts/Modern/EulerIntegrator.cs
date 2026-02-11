using System.Collections.Generic;

namespace BallisticsSimulation
{
    public class EulerIntegrator : IIntegrator
    {
        public List<State> Calculate(in State initState,
            float step,
            int maxSteps,
            BallisticsHandler handler,
            double eps = 1e-4,
            double hMin = 1e-4,
            double hMax = 1)
        {
            List<State> trajectory = new List<State>();
            int counter = 0;

            State state = initState;
            trajectory.Add(state);

            while (counter < maxSteps && state.Position.y >= -1.0)
            {
                State a = handler.Derivatives(state);

                State delta = a * step;
                state = state + delta;
                trajectory.Add(state);

                counter++;
            }
            return trajectory;
        }
    }
}