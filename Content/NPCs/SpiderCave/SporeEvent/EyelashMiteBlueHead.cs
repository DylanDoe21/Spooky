using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Collections.Generic;

using Spooky.Content.Items.SpiderCave.Misc;
using Spooky.Content.Tiles.Blooms;

namespace Spooky.Content.NPCs.SpiderCave.SporeEvent
{
    public class EyelashMiteBlueHead : ModNPC
    {
        private bool segmentsSpawned;

        private static Asset<Texture2D> NPCTexture;

        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                CustomTexturePath = "Spooky/Content/NPCs/NPCDisplayTextures/EyelashMiteBlueBestiary",
                Position = new Vector2(35f, 0f),
                PortraitPositionXOverride = 25f,
                PortraitPositionYOverride = 0f
            };

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            //bools
            writer.Write(segmentsSpawned);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            //bools
            segmentsSpawned = reader.ReadBoolean();
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 800;
            NPC.damage = 45;
            NPC.defense = 10;
            NPC.width = 30;
            NPC.height = 40;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 1, 0);
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit22 with { Pitch = -0.5f };
			NPC.DeathSound = SoundID.NPCDeath16 with { Pitch = -0.5f };
            NPC.aiStyle = -1;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SporeEventBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.EyelashMiteBlue"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SporeEventBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            NPCTexture ??= ModContent.Request<Texture2D>(Texture);

            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Rectangle frame = new Rectangle(0, NPC.frame.Y, NPCTexture.Width(), NPCTexture.Height() / Main.npcFrameCount[NPC.type]);
            Vector2 origin = new Vector2(NPCTexture.Width() * 0.5f, NPCTexture.Height() / Main.npcFrameCount[NPC.type] * 0.5f);
            Main.spriteBatch.Draw(NPCTexture.Value, NPC.Center - Main.screenPosition, frame, drawColor, NPC.rotation, origin, NPC.scale, effects, 0);

            return false;
        }
        
        public override void AI()
        {
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            NPC.spriteDirection = NPC.velocity.X > 0 ? -1 : 1;

            NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X) + 1.57f;

            //Create the worm itself
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (!segmentsSpawned)
                {
                    NPC.realLife = NPC.whoAmI;
                    int latestNPC = NPC.whoAmI;

                    latestNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + (NPC.width / 2), (int)NPC.Center.Y + (NPC.height / 2), 
                    ModContent.NPCType<EyelashMiteBlueBody1>(), NPC.whoAmI, 0, latestNPC);
                    Main.npc[latestNPC].lifeMax = NPC.lifeMax;
                    Main.npc[latestNPC].realLife = NPC.whoAmI;
                    Main.npc[latestNPC].ai[3] = NPC.whoAmI;
                    NetMessage.SendData(MessageID.SyncNPC, number: latestNPC);

                    int MaxSegments = Main.rand.Next(1, 7);
                    for (int numSegment = 0; numSegment < MaxSegments; numSegment++)
                    {
                        latestNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + (NPC.width / 2), (int)NPC.Center.Y + (NPC.height / 2), 
                        ModContent.NPCType<EyelashMiteBlueBody2>(), NPC.whoAmI, 0, latestNPC);
                        Main.npc[latestNPC].lifeMax = NPC.lifeMax;
                        Main.npc[latestNPC].realLife = NPC.whoAmI;
                        Main.npc[latestNPC].ai[3] = NPC.whoAmI;
                        NetMessage.SendData(MessageID.SyncNPC, number: latestNPC);
                    }
                    
                    latestNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + (NPC.width / 2), (int)NPC.Center.Y + (NPC.height / 2), 
                    ModContent.NPCType<EyelashMiteBlueTail>(), NPC.whoAmI, 0, latestNPC);                   
                    Main.npc[latestNPC].lifeMax = NPC.lifeMax;
                    Main.npc[latestNPC].realLife = NPC.whoAmI;
                    Main.npc[latestNPC].ai[3] = NPC.whoAmI;
                    NetMessage.SendData(MessageID.SyncNPC, number: latestNPC);

                    segmentsSpawned = true;
                    NPC.netUpdate = true;
                }
            }

            if (NPC.Distance(player.Center) > 40f)
            {
                Vector2 desiredVelocity = NPC.DirectionTo(player.Center) * 5;
                NPC.velocity = Vector2.Lerp(NPC.velocity, desiredVelocity, 1f / 20);
            }

            float WaveIntensity = 4f;
            float Wave = 16f;

            NPC.ai[0]++;
            if (NPC.ai[1] == 0)
            {
                if (NPC.ai[0] > Wave * 0.5f)
                {
                    NPC.ai[0] = 0;
                    NPC.ai[1] = 1;
                }
                else
                {
                    Vector2 perturbedSpeed = new Vector2(NPC.velocity.X, NPC.velocity.Y).RotatedBy(MathHelper.ToRadians(-WaveIntensity));
                    NPC.velocity = perturbedSpeed;
                }
            }
            else
            {
                if (NPC.ai[0] <= Wave)
                {
                    Vector2 perturbedSpeed = new Vector2(NPC.velocity.X, NPC.velocity.Y).RotatedBy(MathHelper.ToRadians(WaveIntensity));
                    NPC.velocity = perturbedSpeed;
                }
                else
                {
                    Vector2 perturbedSpeed = new Vector2(NPC.velocity.X, NPC.velocity.Y).RotatedBy(MathHelper.ToRadians(-WaveIntensity));
                    NPC.velocity = perturbedSpeed;
                }
                if (NPC.ai[0] >= Wave * 2)
                {
                    NPC.ai[0] = 0;
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<MiteMandibles>(), 3, 1, 3));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<FungusSeed>(), 120));
        }

        public override bool CheckDead()
        {
            if (Main.netMode != NetmodeID.Server) 
            {
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity / 2, ModContent.Find<ModGore>("Spooky/EyelashMiteBlueHeadGore").Type);
            }

            return true;
        }
    }
}