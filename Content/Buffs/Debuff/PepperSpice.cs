using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Content.Dusts;
using Spooky.Content.Projectiles.Blooms;

namespace Spooky.Content.Buffs.Debuff
{
	public class PepperSpice : ModBuff
	{
        public override string Texture => "Spooky/Content/Buffs/Debuff/DebuffPlaceholder";

        private int FlameTimer = 0;

		private bool initializeStats;
        Color storedColor;

		public override void SetStaticDefaults()
		{
			Main.debuff[Type] = true;
		}

		public override void Update(NPC npc, ref int buffIndex)
		{
            if (Main.rand.NextBool(10))
            {
				Color[] colors = new Color[] { Color.LightGray, Color.Gray, Color.DarkGray };

				int DustEffect = Dust.NewDust(npc.position, npc.width, 3, ModContent.DustType<SmokeEffect>(), 0f, 0f, 100, Main.rand.Next(colors) * 0.5f, Main.rand.NextFloat(0.2f, 0.5f));
				Main.dust[DustEffect].velocity.X = 0;
                Main.dust[DustEffect].velocity.Y = -2;
				Main.dust[DustEffect].alpha = 100;
			}

			if (!initializeStats && npc.buffTime[buffIndex] >= 5)
            {
                storedColor = npc.color;

                initializeStats = true;
            }

            if (npc.buffTime[buffIndex] < 5)
            {
                npc.color = storedColor;
                npc.buffTime[buffIndex] = 0;
            }
            else
            {
                npc.color = Color.Firebrick;

				FlameTimer++;

				if (FlameTimer > 120 && FlameTimer % 10 == 0)
				{
					for (int i = 0; i < Main.maxNPCs; i++)
					{
						NPC NPC = Main.npc[i];
						if (NPC.active && !NPC.friendly && !NPC.dontTakeDamage && NPC.whoAmI != npc.whoAmI &&
						!NPCID.Sets.CountsAsCritter[NPC.type] && Vector2.Distance(npc.Center, NPC.Center) <= 550f)
						{
							SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, npc.Center);

							Vector2 ShootSpeed = NPC.Center - npc.Center;
							ShootSpeed.Normalize();
							ShootSpeed *= 6f;

							int FinalDamage = NPC.damage > 250 ? 250 : NPC.damage;

							Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, ShootSpeed, ModContent.ProjectileType<PepperFlame>(), FinalDamage, 0f, npc.target, ai1: npc.whoAmI);

							break;
						}
					}
				}

				if (FlameTimer >= 210)
				{
					FlameTimer = 0;
				}
            }
		}
    }
}
