using System.Collections.Generic;

namespace BallisticsSimulation
{
    public class EulerIntegrator : IIntegrator
    {
        public List<State> Calculate(in State initState,
            double step,
            int maxSteps,
            BallisticsHandler handler,
            double eps = 1e-4,
            double hMin = 1e-4,
            double hMax = 1)
        {
            List<State> trajectory = new List<State>();
            int counter = 0;

            State state = new State(initState);
            trajectory.Add(state);

            while (counter < maxSteps && state.Y >= 0.0)
            {
                State a = handler.Derivatives(state);

                State delta = a * step;
                state = state + delta;

                if (state.Y < 0)
                {
                    break;
                }
                trajectory.Add(new State(state));
                counter++;
            }
            return trajectory;
        }
    }
}