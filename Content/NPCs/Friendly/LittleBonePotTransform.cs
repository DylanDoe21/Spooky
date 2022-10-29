using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.NPCs.Friendly
{
	public class LittleBonePotTransform : ModNPC
	{
		Vector2 SaveNPCPosition;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Little Bone");
			NPCID.Sets.TownCritter[NPC.type] = true;
			NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
		}

		public override void SetDefaults()
		{
			NPC.lifeMax = 150;
			NPC.damage = 0;
			NPC.defense = 25;
            NPC.width = 26;
			NPC.height = 20;
			NPC.friendly = true;
			NPC.immortal = true;
			NPC.dontTakeDamage = true;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.knockBackResist = 0f;
			NPC.rarity = 1;
            NPC.aiStyle = -1;
		}

		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
		{
			return false;
		}

		public override void AI()
		{
			NPC.ai[0]++;

			if (NPC.ai[0] == 2)
			{
				SaveNPCPosition = NPC.Center;
			}

			if (NPC.ai[0] > 2 && NPC.ai[0] < 120)
			{
				NPC.Center = new Vector2(SaveNPCPosition.X, SaveNPCPosition.Y);
				NPC.Center += Main.rand.NextVector2Square(-4, 4);
			}

			if (NPC.ai[0] == 120)
			{
				SoundEngine.PlaySound(SoundID.NPCDeath58, NPC.Center);

				NPC.Transform(ModContent.NPCType<LittleBone>());

				for (int numDusts = 0; numDusts < 20; numDusts++)
                {
                    int DustGore = Dust.NewDust(NPC.Center, NPC.width / 2, NPC.height / 2, 222, 0f, 0f, 100, default, 2f);
                    Main.dust[DustGore].noGravity = true;
					Main.dust[DustGore].scale = 0.5f;
                    Main.dust[DustGore].velocity *= 1.2f;

                    if (Main.rand.Next(2) == 0)
                    {
                        Main.dust[DustGore].scale = 0.5f;
                        Main.dust[DustGore].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
			}
        }
    }
}