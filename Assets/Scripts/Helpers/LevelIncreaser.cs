﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Helpers
{
    [Serializable]
    public class LevelIncreaser
    {
        public int levelTreshold;
        public int wallTouchesToIncreaseDifficulty;
    }
}
