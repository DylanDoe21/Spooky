using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;

namespace Spooky.Content.Items
{
	public class SentientRarity : ModRarity
    {
        public override Color RarityColor => Color.Lerp(new Color(48, 120, 255), new Color(255, 0, 0), (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.5f / 2.5f * 5f)) / 2f + 0.5f);
    }

    public class BloomSpringRarity : ModRarity
    {
        public override Color RarityColor => new Color(91, 255, 83);
    }

    public class BloomSummerRarity : ModRarity
    {
        public override Color RarityColor => new Color(228, 255, 82);
    }

    public class BloomFallRarity : ModRarity
    {
        public override Color RarityColor => new Color(255, 152, 84);
    }

    public class BloomWinterRarity : ModRarity
    {
        public override Color RarityColor => new Color(139, 202, 255);
    }

    public class BloomDivaRarity : ModRarity
    {
        public override Color RarityColor => new Color(255, 59, 215);
    }
}