using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;
using System.IO;

using Spooky.Core;
using Spooky.Content.NPCs.Minibiomes.TarPits.Projectiles;

namespace Spooky.Content.NPCs.Minibiomes.TarPits
{
    public class TarSlimeSpiked : ModNPC  
    {
		public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 3;
        }

		public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.localAI[0]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.localAI[0] = reader.ReadSingle();
        }

        public override void SetDefaults()
		{
            NPC.lifeMax = 200;
            NPC.damage = 40;
            NPC.defense = 5;
            NPC.width = 44;
			NPC.height = 38;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0.25f;
            NPC.value = Item.buyPrice(0, 0, 1, 0);
			NPC.noGravity = false;
			NPC.noTileCollide = false;
			NPC.HitSound = SoundID.Item95 with { Volume = 0.8f, Pitch = 1f };
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.aiStyle = 1;
			AIType = NPCID.GreenSlime;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.TarPitsBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.TarSlimeSpiked"),
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

			bool lineOfSight = Collision.CanHitLine(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height);
			if (lineOfSight && NPC.Distance(player.Center) <= 300f)
			{
				if (NPC.wet)
                {
                    NPC.aiStyle = 1;
                }
				else
				{
					NPC.aiStyle = 0;
				}

				NPC.localAI[0]++;
				if (NPC.localAI[0] >= 60)
				{
					SoundEngine.PlaySound(SoundID.Item17, NPC.Center);

					Vector2 ShootSpeed = player.Center - NPC.Center;
					ShootSpeed.Normalize();
					ShootSpeed *= 8;

					NPCGlobalHelper.ShootHostileProjectile(NPC, NPC.Top + new Vector2(0, 8), ShootSpeed, ModContent.ProjectileType<TarSpike>(), NPC.damage, 1f);

					NPC.localAI[0] = 0;
				}
			}
			else
			{
				NPC.aiStyle = 1;
				AIType = NPCID.GreenSlime;
			}
        }

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
}