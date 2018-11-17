using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Asteroids.Standard.Base;
using Asteroids.Standard.Screen;

namespace Asteroids.Standard.Components
{
    /// <summary>
    /// Summary description for Explosions.
    /// </summary>
    class Explosions : CommonOps
    {
        private IList<Explosion> _explosions;
        private object _updateExplosionLock;

        public Explosions() : base()
        {
            _updateExplosionLock = new object();
            _explosions = new List<Explosion>();
        }

        public int Count()
        {
            lock(_updateExplosionLock)
                return _explosions.Count;
        }

        public void AddExplosion(Point ptExplosion)
        {
            AddExplosion(ptExplosion, 1);
        }

        public void AddExplosions(IList<Explosion> explosions)
        {
            lock (_updateExplosionLock)
                foreach (var explosion in explosions)
                    _explosions.Add(explosion);
        }

        public void AddExplosion(Point ptExplosion, double timeFactor)
        {
            var explosion = new Explosion(ptExplosion, timeFactor);

            lock(_updateExplosionLock)
                _explosions.Add(explosion);
        }

        public IList<Explosion> GetExplosions()
        {
            lock (_updateExplosionLock)
                return _explosions.ToList();
        }

        public void SetExplosions(IList<Explosion> explosions)
        {
            lock (_updateExplosionLock)
                _explosions = explosions;
        }

        //public void Move()
        //{
        //    var explosions = new List<Explosion>();

        //    lock (_updateExplosionLock)
        //        explosions.AddRange(_explosions);

        //    for (int i = explosions.Count - 1; i >= 0; i--)
        //        if (!explosions[i].Move())
        //            explosions.RemoveAt(i);

        //    _explosions = explosions;
        //}

        //public void Draw()
        //{

        //    var explosions = new List<Explosion>();

        //    lock (_updateExplosionLock)
        //        explosions.AddRange(_explosions);

        //    foreach (var explosion in explosions)
        //        explosion.Draw();
        //}
    }
}
