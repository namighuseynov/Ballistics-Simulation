using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace BallisticsSimulation
{
    public class RK4Integrator : IIntegrator
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
            trajectory.Add(new State(state));

            while (counter < maxSteps && state.Y >= 0.0)
            {
                State k1 = handler.Derivatives(state);
                State k2 = handler.Derivatives(state + (k1 * (step * 0.5)));
                State k3 = handler.Derivatives(state + (k2 * (step * 0.5)));
                State k4 = handler.Derivatives(state + (k3 * (step)));

                State delta = (k1 + (k2 * (2))
                                 + (k3 * (2))
                                 + (k4))
                                * (step / 6.0);

                if (state.Y < 0)
                {
                    break;
                }
                state = state.Add(delta);
                trajectory.Add(new State(state));
                counter++;
            }
            return trajectory;
        }
    }
}