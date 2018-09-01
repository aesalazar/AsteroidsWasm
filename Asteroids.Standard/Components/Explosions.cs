using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Asteroids.Standard.Base;
using Asteroids.Standard.Screen;

namespace Asteroids.Standard.Components
{
    /// <summary>
    /// Summary description for Explosions.
    /// </summary>
    public class Explosions : CommonOps
    {
        private IList<Explosion> explosions;

        public Explosions()
        {
            explosions = new List<Explosion>();
        }

        public int Count()
        {
            return explosions.Count;
        }

        public void AddExplosion(Point ptExplosion)
        {
            AddExplosion(ptExplosion, 1);
        }

        public void AddExplosion(Point ptExplosion, double timeFactor)
        {
            var explosion = new Explosion(ptExplosion, timeFactor);
            explosions.Add(explosion);
        }

        public void Move()
        {
            for (int i = explosions.Count - 1; i >= 0; i--)
                if (!explosions[i].Move())
                    explosions.RemoveAt(i);
        }

        public void Draw(ScreenCanvas sc, int iPictX, int iPictY)
        {
            foreach (var explosion in explosions)
                explosion.Draw(sc, iPictX, iPictY);
        }
    }
}
