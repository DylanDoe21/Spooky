using Microsoft.Xna.Framework;

namespace Spooky.Core
{
    public class BezierCurveUtil
    {
        public static Vector2 BezierCurve(Vector2[] bezierPoints, float bezierProgress)
        {
            if (bezierPoints.Length == 1)
            {
                return bezierPoints[0];
            }
            else
            {
                Vector2[] newBezierPoints = new Vector2[bezierPoints.Length - 1];
                for (int i = 0; i < bezierPoints.Length - 1; i++)
                {
                    newBezierPoints[i] = bezierPoints[i] * bezierProgress + bezierPoints[i + 1] * (1 - bezierProgress);
                }
                return BezierCurve(newBezierPoints, bezierProgress);
            }
        }

        public static Vector2 BezierCurveDerivative(Vector2[] bezierPoints, float bezierProgress)
        {
            if (bezierPoints.Length == 2)
            {
                return bezierPoints[0] - bezierPoints[1];
            }
            else
            {
                Vector2[] newBezierPoints = new Vector2[bezierPoints.Length - 1];
                for (int i = 0; i < bezierPoints.Length - 1; i++)
                {
                    newBezierPoints[i] = bezierPoints[i] * bezierProgress + bezierPoints[i + 1] * (1 - bezierProgress);
                }
                return BezierCurveDerivative(newBezierPoints, bezierProgress);
            }
        }
    }
}