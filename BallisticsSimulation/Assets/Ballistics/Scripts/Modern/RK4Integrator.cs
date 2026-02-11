using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace BallisticsSimulation
{
    public class RK4Integrator : IIntegrator
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
                State k1 = handler.Derivatives(state);
                State k2 = handler.Derivatives(state + (k1 * (step * 0.5)));
                State k3 = handler.Derivatives(state + (k2 * (step * 0.5)));
                State k4 = handler.Derivatives(state + (k3 * (step)));

                State delta = (k1 + (k2 * 2.0) + (k3 * 2.0) + k4) * (step / 6.0);

                state = state + delta;
                trajectory.Add(state);

                counter++;
            }
            return trajectory;
        }
    }
}