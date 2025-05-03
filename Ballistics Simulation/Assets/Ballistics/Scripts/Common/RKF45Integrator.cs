using System;
using System.Collections.Generic;

namespace BallisticsSimulation
{
    public class RKF45Integrator : IIntegrator
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

            double h = step;

            while (counter < maxSteps && state.Y >= 0.0)
            {
                State k1 = handler.Derivatives(state);

                State k2 = handler.Derivatives(state.Add(
                           k1.Dot(h * 0.25)));

                State k3 = handler.Derivatives(state.Add(
                           k1.Dot(h * 3.0 / 32.0)
                      .Add(k2.Dot(h * 9.0 / 32.0))));

                State k4 = handler.Derivatives(state.Add(
                           k1.Dot(h * 1932.0 / 2197.0)
                      .Add(k2.Dot(h * -7200.0 / 2197.0))
                      .Add(k3.Dot(h * 7296.0 / 2197.0))));

                State k5 = handler.Derivatives(state.Add(
                           k1.Dot(h * 439.0 / 216.0)
                      .Add(k2.Dot(h * -8.0))
                      .Add(k3.Dot(h * 3680.0 / 513.0))
                      .Add(k4.Dot(h * -845.0 / 4104.0))));

                State k6 = handler.Derivatives(state.Add(
                           k1.Dot(h * -8.0 / 27.0)
                      .Add(k2.Dot(h * 2.0))
                      .Add(k3.Dot(h * -3544.0 / 2565.0))
                      .Add(k4.Dot(h * 1859.0 / 4104.0))
                      .Add(k5.Dot(h * -11.0 / 40.0))));

                State y4 = state.Add(
                           k1.Dot(h * 25.0 / 216.0)
                      .Add(k3.Dot(h * 1408.0 / 2565.0))
                      .Add(k4.Dot(h * 2197.0 / 4104.0))
                      .Add(k5.Dot(h * -1.0 / 5.0)));

                State y5 = state.Add(
                           k1.Dot(h * 16.0 / 135.0)
                      .Add(k3.Dot(h * 6656.0 / 12825.0))
                      .Add(k4.Dot(h * 28561.0 / 56430.0))
                      .Add(k5.Dot(h * -9.0 / 50.0))
                      .Add(k6.Dot(h * 2.0 / 55.0)));

                double err = y5.Sub(y4).Magnitude();

                if (err <= eps || h <= hMin)
                {
                    state = y5;
                    trajectory.Add(new State(state));
                    counter++;
                }

                double s = 0.9 * Math.Pow(eps / Math.Max(err, 1e-15), 0.2);
                h = Math.Clamp(h * s, hMin, hMax);

                if (h < hMin) break;
            }
            return trajectory;
        }
    }
}