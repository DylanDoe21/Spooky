using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Collections.Generic;

namespace Spooky.Content.NPCs.EggEvent
{
    public class EarWormHead : ModNPC
    {
        private bool segmentsSpawned;

        private static Asset<Texture2D> NPCTexture;

        public override void SetStaticDefaults()
        {
            NPCID.Sets.CantTakeLunchMoney[Type] = true;

            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
			{
                CustomTexturePath = "Spooky/Content/NPCs/NPCDisplayTextures/EarWormBestiary",
                Position = new Vector2(2f, 20f),
                PortraitPositionXOverride = 0f,
                PortraitPositionYOverride = 20f
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
            NPC.lifeMax = 1000;
            NPC.damage = 45;
            NPC.defense = 10;
            NPC.width = 20;
            NPC.height = 20;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 2, 0);
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit9;
            NPC.DeathSound = SoundID.Zombie40;
            NPC.aiStyle = -1;
            SpawnModBiomes = new int[2] { ModContent.GetInstance<Biomes.SpookyHellBiome>().Type, ModContent.GetInstance<Biomes.SpookyHellEventBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement>
            {
                new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.EarWormHead"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyHellBiome>().ModBiomeBestiaryInfoElement),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyHellEventBiome>().ModBiomeBestiaryInfoElement)
            });
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
			NPCTexture ??= ModContent.Request<Texture2D>(Texture);

			spriteBatch.Draw(NPCTexture.Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0);

			return false;
        }

        public override void AI()
        {
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X) + 1.57f;

            //Make the worm itself
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (!segmentsSpawned)
                {
                    NPC.realLife = NPC.whoAmI;
                    int latestNPC = NPC.whoAmI;

                    int randomLength = Main.rand.Next(10, 25);

                    for (int Segment = 0; Segment < randomLength; Segment++)
                    {
                        int Type = Main.rand.NextBool(5) && Segment > 1 ? (Main.rand.NextBool() ? ModContent.NPCType<EarWormBodyEye1>() : ModContent.NPCType<EarWormBodyEye2>()) : ModContent.NPCType<EarWormBody>();

                        latestNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + (NPC.width / 2), (int)NPC.Center.Y + (NPC.height / 2), Type, NPC.whoAmI, 0, latestNPC);
                        Main.npc[latestNPC].lifeMax = NPC.lifeMax;
                        Main.npc[latestNPC].realLife = NPC.whoAmI;
                        Main.npc[latestNPC].ai[2] = NPC.whoAmI;
                        NetMessage.SendData(MessageID.SyncNPC, number: latestNPC);
                    }

                    latestNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + (NPC.width / 2), (int)NPC.Center.Y + (NPC.height / 2), ModContent.NPCType<EarWormTail>(), NPC.whoAmI, 0, latestNPC);
                    Main.npc[latestNPC].lifeMax = NPC.lifeMax;         
                    Main.npc[latestNPC].realLife = NPC.whoAmI;
                    Main.npc[latestNPC].ai[2] = NPC.whoAmI;
                    NetMessage.SendData(MessageID.SyncNPC, number: latestNPC);

                    segmentsSpawned = true;
                    NPC.netUpdate = true;
                }
            }

            NPC.ai[0]++;

            //go below the player
            if (NPC.ai[0] < 120)
            {
                Vector2 GoTo = player.Center;
                GoTo.Y += 750;

                float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 10, 22);
                NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
            }

            //teleport below the player, then create telegraph
            if (NPC.ai[0] == 120)
            {
                NPC.velocity *= 0;

                NPC.position.X = player.Center.X - 20;
                NPC.position.Y = player.Center.Y + 750;
            }

            //charge up
            if (NPC.ai[0] == 135)
            {
                NPC.velocity.X *= 0;
                NPC.velocity.Y = -25;
            }

            //turn around after vertically passing the player
            if (NPC.ai[0] >= 135 && NPC.ai[0] <= 220)
            {
                double angle = NPC.DirectionTo(player.Center).ToRotation() - NPC.velocity.ToRotation();
                while (angle > Math.PI)
                {
                    angle -= 2.0 * Math.PI;
                }
                while (angle < -Math.PI)
                {
                    angle += 2.0 * Math.PI;
                }

                if (Math.Abs(angle) > Math.PI / 2)
                {
                    NPC.ai[1] = Math.Sign(angle);
                    NPC.velocity = Vector2.Normalize(NPC.velocity) * 30;
                }

                NPC.velocity = NPC.velocity.RotatedBy(MathHelper.ToRadians(4.5f) * NPC.ai[1]);
            }

            if (NPC.ai[0] > 185)
            {
                NPC.velocity *= 0.95f;
            }

            if (NPC.ai[0] > 245)
            {
                NPC.ai[0] = 0;
                NPC.ai[1] = 0;
                NPC.netUpdate = true;
            }
        }

        public override bool CheckDead()
        {
            if (Main.netMode != NetmodeID.Server) 
            {
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity / 2, ModContent.Find<ModGore>("Spooky/EarWormGore1").Type);
            }

            return true;
        }
    }
}