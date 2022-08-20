using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using static Terraria.ModLoader.ModContent;

namespace Spooky.Core
{
    public static class TrailHelper
    {
        public static Vector3 Vec3(this Vector2 vector) => new Vector3(vector.X, vector.Y, 0);

        public static T[] FastUnion<T>(this T[] front, T[] back)
        {
            T[] combined = new T[front.Length + back.Length];

            Array.Copy(front, combined, front.Length);
            Array.Copy(back, 0, combined, front.Length, back.Length);

            return combined;
        }

    }
}

