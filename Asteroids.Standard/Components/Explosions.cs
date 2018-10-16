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
        private IList<Explosion> _explosions;
        private object _updateExplosionLock;

        public Explosions()
        {
            _updateExplosionLock = new object();
            _explosions = new List<Explosion>();
        }

        public int Count()
        {
            return _explosions.Count;
        }

        public void AddExplosion(Point ptExplosion)
        {
            AddExplosion(ptExplosion, 1);
        }

        public void AddExplosion(Point ptExplosion, double timeFactor)
        {
            var explosion = new Explosion(ptExplosion, timeFactor);

            lock(_updateExplosionLock)
                _explosions.Add(explosion);
        }

        public void Move()
        {
            var explosions = new List<Explosion>();

            lock (_updateExplosionLock)
                explosions.AddRange(_explosions);

            for (int i = explosions.Count - 1; i >= 0; i--)
                if (!explosions[i].Move())
                    explosions.RemoveAt(i);

            _explosions = explosions;
        }

        public void Draw(ScreenCanvas sc, int iPictX, int iPictY)
        {

            var explosions = new List<Explosion>();

            lock (_updateExplosionLock)
                explosions.AddRange(_explosions);

            foreach (var explosion in explosions)
                explosion.Draw(sc, iPictX, iPictY);
        }
    }
}
