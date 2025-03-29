using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;
using Spooky.Content.Projectiles.SpookyBiome;

namespace Spooky.Content.Buffs.Debuff
{
	public class GourdDecay : ModBuff
	{
		public override string Texture => "Spooky/Content/Buffs/Debuff/DebuffPlaceholder";

		private bool initializeStats;
        private int storedDefense;

		public override void SetStaticDefaults()
		{
			Main.debuff[Type] = true;
		}

		public override void Update(NPC npc, ref int buffIndex)
        {
			if (!npc.friendly)
            {
				//fly visuals
				if (Main.rand.NextBool(50))
				{
					if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center.X, npc.Center.Y, Main.rand.Next(-2, 2), Main.rand.Next(-3, -1), 
                        ModContent.ProjectileType<DecayDebuffFly>(), 0, 0f, npc.target, 0f, (float)npc.whoAmI);
					}
				}

				//damage over time
                if (npc.lifeRegen > 0)
                {
                    npc.lifeRegen = 0;
                }

                npc.lifeRegen -= 5;

				if (!initializeStats)
				{
					storedDefense = npc.defense;
					npc.defense = (int)(npc.defense * 0.8f);

					initializeStats = true;
				}

				if (npc.buffTime[buffIndex] < 5)
				{
					npc.defense = storedDefense;
					initializeStats = false;
				}
			}
		}
    }
}
