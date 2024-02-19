using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Collections.Generic;

using Spooky.Content.Items.SpookyBiome;
using Spooky.Content.NPCs.SpookyBiome.Projectiles;

namespace Spooky.Content.NPCs.SpookyBiome
{
	public class Chungus : ModNPC
	{
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 11;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
        }

		public override void SetDefaults()
		{
            NPC.lifeMax = 50;
            NPC.damage = 20;
            NPC.defense = 0;
            NPC.width = 64;
            NPC.height = 50;
			NPC.knockBackResist = 0.4f;
			NPC.value = Item.buyPrice(0, 0, 1, 0);
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = 1;
			AIType = NPCID.BlueSlime;
			SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpookyBiomeUg>().Type };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.Chungus"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyBiomeUg>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void FindFrame(int frameHeight)
        {
            if (NPC.velocity.Y == 0 && NPC.localAI[0] < 200)
            {
                NPC.frameCounter++;
                if (NPC.frameCounter > 6)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0;
                }
                if (NPC.frame.Y >= frameHeight * 5)
                {
                    NPC.frame.Y = 0 * frameHeight;
                }
            }

            if (NPC.localAI[0] == 230)
            {
                NPC.localAI[1] = 1;
                NPC.frame.Y = 5 * frameHeight;
            }

            if (NPC.localAI[0] > 230)
            {
                NPC.frameCounter++;
                if (NPC.frameCounter > 4)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0;
                }
                if (NPC.frame.Y >= frameHeight * 11)
                {
                    NPC.frame.Y = 0 * frameHeight;
                    NPC.localAI[0] = 0;
                }
            }
        }

		public override void AI()
		{
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

			NPC.spriteDirection = NPC.direction;

            if (NPC.velocity.Y == 0)
            {
                NPC.localAI[0]++;

                if (NPC.localAI[0] >= 200)
                {
                    NPC.aiStyle = -1;
                }
                else
                {
                    NPC.aiStyle = 1;
			        AIType = NPCID.HoppinJack;
                }
            }

            if (NPC.localAI[1] == 1)
            {
                SoundEngine.PlaySound(SoundID.NPCDeath9, NPC.Center);

                for (int numProjectiles = 0; numProjectiles < 3; numProjectiles++)
                {
                    Vector2 ShootSpeed = player.Center - NPC.Center;
                    ShootSpeed.Normalize();
                    ShootSpeed.X *= Main.rand.NextFloat(1.5f, 2.5f);
                    ShootSpeed.Y *= Main.rand.NextFloat(1.5f, 2.5f);

                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, ShootSpeed.X, 
                    ShootSpeed.Y, ModContent.ProjectileType<ChungusSpore>(), NPC.damage / 5, 1, NPC.target);
                }

                NPC.localAI[1] = 0;

                NPC.netUpdate = true;
            }
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BustlingGlowshroom>(), 30));
        }

		public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                if (Main.netMode != NetmodeID.Server) 
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/ChungusGore").Type);
                }
            }
        }
	}
}