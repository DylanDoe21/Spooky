using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;

namespace Spooky.Content.Items.SpookyHell.Sentient
{
	public class SentientRarity : ModRarity
	{
        public override Color RarityColor => Color.Lerp(new Color(75, 80, 250), new Color(255, 0, 0), (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.5f / 2.5f * 6.28318548f)) / 2f + 0.5f);
        //new Color(0, 255, 75);
	}
}