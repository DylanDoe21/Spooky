using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;

namespace Spooky.Content.NPCs.Minibiomes.TarPits
{
    public class TarSlime1 : ModNPC  
    {
		bool HasJumped = false;

		public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 3;
        }

        public override void SetDefaults()
		{
            NPC.lifeMax = 80;
            NPC.damage = 25;
            NPC.defense = 5;
            NPC.width = 46;
			NPC.height = 34;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0.25f;
            NPC.value = Item.buyPrice(0, 0, 0, 50);
			NPC.noGravity = false;
			NPC.noTileCollide = false;
			NPC.HitSound = SoundID.Item95 with { Volume = 0.8f, Pitch = 1.3f };
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.aiStyle = 1;
			AIType = NPCID.GreenSlime;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.TarPitsBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.TarSlime1"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.TarPitsBiome>().ModBiomeBestiaryInfoElement)
			});
        }

        public override void FindFrame(int frameHeight)
		{
            if (NPC.velocity.Y == 0)
            {
				NPC.frameCounter++;
				if (NPC.frameCounter > 6)
				{
					NPC.frame.Y = NPC.frame.Y + frameHeight;
					NPC.frameCounter = 0;
				}
				if (NPC.frame.Y >= frameHeight * 2)
				{
					NPC.frame.Y = 0 * frameHeight;
				}
			}
            else
            {
				NPC.frame.Y = 2 * frameHeight;
			}
		}

        public override void AI()
		{
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

			NPC.spriteDirection = NPC.direction;
        }

		/*
		public void JumpToTarget(Player target, int JumpHeight, int TimeBeforeNextJump)
		{
			NPC.ai[0]++;

			//set where the it should be jumping towards
			Vector2 JumpTo = new(target.Center.X, NPC.Center.Y - JumpHeight);

			//set velocity and speed
			Vector2 velocity = JumpTo - NPC.Center;
			velocity.Normalize();

			int JumpSpeed = Main.rand.Next(13, 18);

			float speed = MathHelper.Clamp(velocity.Length() / 36, 10, JumpSpeed);

			NPC.velocity.X *= NPC.velocity.Y <= 0 ? 0.98f : 0.95f;

			//actual jumping
			if (NPC.ai[0] >= TimeBeforeNextJump)
			{
				NPC.ai[1]++;

				if (NPC.velocity == Vector2.Zero || NPC.wet)
				{
					if ((NPC.ai[1] == 10 && !HasJumped) || NPC.wet)
					{
						if (target.Distance(NPC.Center) <= 450f && !NPC.wet)
						{
							SoundEngine.PlaySound(SoundID.Item95 with { Volume = 0.8f, Pitch = 1.05f }, NPC.Center);
						}

						velocity.Y -= 0.25f;

						HasJumped = true;
					}
				}

				if (NPC.ai[1] < 12 && HasJumped)
				{
					NPC.velocity = velocity * speed;
				}
			}

			//loop ai
			if (NPC.ai[0] >= TimeBeforeNextJump + 150)
			{
				HasJumped = false;

				NPC.ai[0] = 0;
				NPC.ai[1] = 0;
			}
		}
		*/

		public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
				for (int numDusts = 0; numDusts < 25; numDusts++)
                {                                                                                  
                    int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Asphalt, 0f, -2f, 0, default, 1f);
                    Main.dust[dust].position.X += Main.rand.Next(-25, 25) * 0.05f - 1.5f;
                    Main.dust[dust].position.Y += Main.rand.Next(-25, 25) * 0.05f - 1.5f;
                }
            }
        }
    }

    public class TarSlime2 : TarSlime1  
    {
		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement>
			{
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.TarSlime2"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.TarPitsBiome>().ModBiomeBestiaryInfoElement)
			});
		}
	}
}