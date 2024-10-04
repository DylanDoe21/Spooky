using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Core;

namespace Spooky.Content.Buffs.Debuff
{
	public class BlueberryFrost : ModBuff
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
                npc.velocity.X *= 0.75f;
                npc.velocity.Y += !npc.noTileCollide ? 0.10f : 0.01f;
            }

            if (Main.rand.NextBool(10))
            {
                int dust = Dust.NewDust(npc.position, npc.width, npc.height, DustID.SnowflakeIce, npc.velocity.X * 0.5f, npc.velocity.Y * 0.5f, 100, default, 1.5f);
				Main.dust[dust].noGravity = true;
            }

			if (!initializeStats)
            {
                storedColor = npc.color;

                initializeStats = true;
            }

            if (npc.buffTime[buffIndex] <= 1)
            {
                npc.color = storedColor;
            }
            else
            {
                npc.color = Color.Cyan;
            }
		}
    }
}
