using System.Collections.Generic;

namespace BallisticsSimulation
{
    public class RK4Integrator : IIntegrator
    {
        public List<State> Calculate(State initState,
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
                State k2 = handler.Derivatives(state.Add(k1.Dot(step * 0.5)));
                State k3 = handler.Derivatives(state.Add(k2.Dot(step * 0.5)));
                State k4 = handler.Derivatives(state.Add(k3.Dot(step)));

                State delta = (k1.Add(k2.Dot(2))
                                 .Add(k3.Dot(2))
                                 .Add(k4))
                               .Dot(step / 6.0);

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