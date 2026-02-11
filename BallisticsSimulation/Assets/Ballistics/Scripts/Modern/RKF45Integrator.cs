using System;
using System.Collections.Generic;
using UnityEngine;

namespace BallisticsSimulation
{
    public class RKF45Integrator : IIntegrator
    {
        public List<State> Calculate(
            in State initState,
            float step,         
            int maxSteps,
            BallisticsHandler handler,
            double eps = 1e-4,
            double hMin = 1e-4,  
            double hMax = 1.0)   
        {
            List<State> trajectory = new List<State>();
            State state = initState;
            trajectory.Add(state);

            double h = step;
            int counter = 0;

            while (counter < maxSteps && state.Position.y >= -1.0)
            {
                State k1 = handler.Derivatives(state);
                State k2 = handler.Derivatives(state + k1 * (h * (1.0 / 4.0)));
                State k3 = handler.Derivatives(state + k1 * (h * (3.0 / 32.0)) + k2 * (h * (9.0 / 32.0)));
                State k4 = handler.Derivatives(state + k1 * (h * (1932.0 / 2197.0)) + k2 * (h * (-7200.0 / 2197.0)) + k3 * (h * (7296.0 / 2197.0)));
                State k5 = handler.Derivatives(state + k1 * (h * (439.0 / 216.0)) + k2 * (h * (-8.0)) + k3 * (h * (3680.0 / 513.0)) + k4 * (h * (-845.0 / 4104.0)));
                State k6 = handler.Derivatives(state + k1 * (h * (-8.0 / 27.0)) + k2 * (h * (2.0)) + k3 * (h * (-3544.0 / 2565.0)) + k4 * (h * (1859.0 / 4104.0)) + k5 * (h * (-11.0 / 40.0)));

                State y4 = state + k1 * (h * (25.0 / 216.0)) + k3 * (h * (1408.0 / 2565.0)) + k4 * (h * (2197.0 / 4104.0)) + k5 * (h * (-1.0 / 5.0));

                State y5 = state + k1 * (h * (16.0 / 135.0)) + k3 * (h * (6656.0 / 12825.0)) + k4 * (h * (28561.0 / 56430.0)) + k5 * (h * (-9.0 / 50.0)) + k6 * (h * (2.0 / 55.0));


                Vector3 errorVec = y5.Position - y4.Position;
                double err = errorVec.magnitude;

                if (err <= eps || h <= hMin)
                {
                    state = y5;
                    trajectory.Add(state);
                    counter++;

                    if (state.Position.y < -1.0f) break;
                }

                double s = (err < 1e-15) ? 2.0 : 0.84 * Math.Pow(eps / err, 0.2);
                h = Math.Clamp(h * s, hMin, hMax);

                if (h < hMin * 0.5) break;
            }

            return trajectory;
        }
    }
}