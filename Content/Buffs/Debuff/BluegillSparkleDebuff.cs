using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Projectiles.Blooms;

namespace Spooky.Content.Buffs.Debuff
{
	public class BluegillSparkleDebuff : ModBuff
	{
        public override string Texture => "Spooky/Content/Buffs/Debuff/DebuffPlaceholder";

        int SparkleTimer = 0;

		public override void SetStaticDefaults()
		{
			Main.debuff[Type] = true;
		}

		public override void Update(NPC npc, ref int buffIndex)
		{
            SparkleTimer++;
            if (SparkleTimer == 60)
            {
				Vector2 Velocity = new Vector2(0, -3).RotatedByRandom(MathHelper.ToRadians(360));

				if (Main.rand.NextBool())
				{
					Projectile.NewProjectile(null, npc.Center, Velocity, ModContent.ProjectileType<BluegillSparkle>(), npc.damage, 0, Main.myPlayer);
				}
				else
				{
					Projectile.NewProjectile(null, npc.Center, Velocity, ModContent.ProjectileType<BluegillSparkleSmall>(), npc.damage, 0, Main.myPlayer);
				}

                SparkleTimer = 0;
            }
		}
    }
}
