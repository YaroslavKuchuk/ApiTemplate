using System;

namespace Common.Helpers
{
    public class RandomUtility
    {
        public Random Rand { get; }

        private static volatile object _lock = new object();

        private static RandomUtility _instance;

        public static RandomUtility GetInstance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if(_instance == null)
                        {
                            _instance = new RandomUtility();
                        }
                    }
                }

                return _instance;
            }
        }

        private RandomUtility()
        {
            Rand = new Random();
        }
    }
}
