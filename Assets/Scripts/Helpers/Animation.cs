using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Helpers
{
    [Serializable]
    public struct Animation
    {
        public string animationName;
        public List<Sprite> animationSprites;
    }
}
