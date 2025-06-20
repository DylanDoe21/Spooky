using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Items.Food;
using Spooky.Content.Items.SpookyHell;
using Spooky.Content.Items.SpookyHell.Misc;
using Spooky.Content.NPCs.SpookyHell.Projectiles;

namespace Spooky.Content.NPCs.SpookyHell
{
    public class ManHoleFleshy : ModNPC  
    {
        public ushort destinationX = 0;
		public ushort destinationY = 0;

        bool FoundPosition = false;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 11;

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            //bools
			writer.Write(FoundPosition);

            //floats
            writer.Write((ushort)destinationX);
            writer.Write((ushort)destinationY);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            //bools
			FoundPosition = reader.ReadBoolean();

            //floats
            destinationX = reader.ReadUInt16();
            destinationY = reader.ReadUInt16();
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 140;
            NPC.damage = 35;
            NPC.defense = 10;
            NPC.width = 78;
            NPC.height = 48;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 1, 0);
            NPC.noTileCollide = false;
            NPC.noGravity = false;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath5;
            NPC.aiStyle = -1;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpookyHellBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement>
            {
                new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.ManHoleFleshy"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyHellBiome>().ModBiomeBestiaryInfoElement)
			});
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;

            if (NPC.ai[0] < 300)
            {
                if (NPC.frameCounter > 4)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0;
                }
                if (NPC.frame.Y >= frameHeight * 6)
                {
                    NPC.frame.Y = 0 * frameHeight;
                }
            }
            else
            {   
                if (NPC.ai[0] == 300)
                {
                    NPC.frame.Y = 6 * frameHeight;
                }

                if (NPC.frameCounter > 4)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0;
                }
                if (NPC.frame.Y >= frameHeight * 11)
                {
                    NPC.frame.Y = 11 * frameHeight;
                }
            }
        }

        public override void AI()
        {
            NPC.TargetClosest(true);
			Player player = Main.player[NPC.target];

            if (NPC.Distance(player.Center) <= 700f || NPC.ai[0] >= 230)
            {
                NPC.ai[0]++;
            }

            if (NPC.ai[0] == 240)
            {   
                SoundEngine.PlaySound(SoundID.NPCDeath13, NPC.Center);

                NPCGlobalHelper.ShootHostileProjectile(NPC, new Vector2(NPC.Center.X, NPC.Center.Y + 5), 
                new Vector2(player.Center.X < NPC.Center.X ? Main.rand.Next(-10, -2) : Main.rand.Next(2, 10), -10), ModContent.ProjectileType<ManHoleBloodBall>(), NPC.damage, 4.5f);
            }

            if (NPC.ai[0] >= 300)
            {
                //Teleport has to be before finding spot so that it syncs the necessary info before it happens
				if (destinationX != 0 && destinationY != 0)
				{
                    NPC.ai[1]++;

                    if (NPC.ai[1] <= 60)
                    {
                        Dust dust = Dust.NewDustDirect(new Vector2((destinationX * 16f) - 30, (destinationY * 16f) - 20), NPC.width, NPC.height, DustID.Blood, Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(-12f, -8f), 50, Color.White, 2.5f);
                        dust.noGravity = true;
                    }
                    else
                    {
                        SoundEngine.PlaySound(SoundID.NPCDeath12, NPC.Center);

                        NPC.position.X = destinationX * 16f - (float)(NPC.width / 2) + 8f;
                        NPC.position.Y = destinationY * 16f - (float)NPC.height;
                        NPC.velocity.X = 0f;
                        NPC.velocity.Y = 0f;
                        NPC.netOffset *= 0f;
                        destinationX = 0;
                        destinationY = 0;

                        NPC.ai[0] = 0;
                        NPC.ai[1] = 0;
                        NPC.netUpdate = true;
                    }
				}

				if (NPC.ai[0] >= 360 && destinationX == 0 && destinationY == 0 && Main.netMode != NetmodeID.MultiplayerClient)
				{
					Point point = Main.player[NPC.target].Center.ToTileCoordinates();
					Vector2 chosenTile = Vector2.Zero;
					if (NPC.AI_AttemptToFindTeleportSpot(ref chosenTile, point.X, point.Y, 20, 5, 1, solidTileCheckCentered: false, teleportInAir: false))
					{
						destinationX = (ushort)chosenTile.X;
						destinationY = (ushort)chosenTile.Y;
					}
				}
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CreepyChunk>(), 3, 1, 2));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<MonsterBloodVial>(), 100));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<GoofyPretzel>(), 100));
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                for (int Repeats = 1; Repeats <= 2; Repeats++)
                {
                    for (int numGores = 1; numGores <= 3; numGores++)
                    {
                        if (Main.netMode != NetmodeID.Server) 
                        {
                            Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/ManHoleFleshyGore" + numGores).Type);
                        }
                    }
                }
            }
        }
    }
}