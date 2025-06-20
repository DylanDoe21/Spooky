using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Dusts;

namespace Spooky.Content.Buffs.Debuff
{
	public class CavefishStunned : ModBuff
	{
        public override string Texture => "Spooky/Content/Buffs/Debuff/DebuffPlaceholder";

		public override void SetStaticDefaults()
		{
			Main.debuff[Type] = true;
		}

		public override void Update(NPC npc, ref int buffIndex)
		{
            if (!npc.boss && !npc.IsTechnicallyBoss())
            {
                npc.velocity *= 0.85f;
			}

            if (Main.rand.NextBool(10))
            {
				int newDust = Dust.NewDust(npc.position, npc.width, npc.height / 4, ModContent.DustType<CartoonStar>(), 0f, -2f, 0, default, 0.55f);
				Main.dust[newDust].velocity.X = Main.rand.NextFloat(-1.5f, 1.5f);
				Main.dust[newDust].velocity.Y = Main.rand.NextFloat(-1.5f, -0.2f);
				Main.dust[newDust].alpha = Main.rand.Next(0, 2);
				Main.dust[newDust].noGravity = true;
			}
		}
    }
}
