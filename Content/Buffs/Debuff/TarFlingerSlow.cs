using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Core;

namespace Spooky.Content.Buffs.Debuff
{
	public class TarFlingerSlow : ModBuff
	{
		public override string Texture => "Spooky/Content/Buffs/Debuff/DebuffPlaceholder";

		private bool initializeStats;
        Color storedColor;

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (!npc.boss && !npc.IsTechnicallyBoss())
            {
                npc.velocity.X *= 0.9f;
                npc.velocity.Y += !npc.noTileCollide ? 0.10f : 0.01f;
            }

            if (!initializeStats && npc.buffTime[buffIndex] >= 5)
            {
                storedColor = npc.color;

                initializeStats = true;
            }

            if (npc.buffTime[buffIndex] < 5)
            {
                npc.color = storedColor;
            }
            else
            {
                Color color = npc.GetAlpha(Color.Black);
                npc.color = color;
            }
        }
    }
}
