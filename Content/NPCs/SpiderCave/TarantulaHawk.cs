using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Collections.Generic;

namespace Spooky.Content.NPCs.SpiderCave
{
    public class TarantulaHawk1 : ModNPC
    {
        int EnemyBeingCarried = 0;

        bool InitializedEnemy = false;
        bool CarryingEnemy = true;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 4;
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
            NPC.lifeMax = 400;
            NPC.damage = 45;
            NPC.defense = 15;
            NPC.width = 66;
            NPC.height = 58;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 5, 0);
            NPC.noGravity = true;
            NPC.noTileCollide = false;
            NPC.HitSound = SoundID.NPCHit32;
			NPC.DeathSound = SoundID.NPCDeath38;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpiderCaveBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.TarantulaHawk1"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpiderCaveBiome>().ModBiomeBestiaryInfoElement)
			});
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter > 3)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0;
            }
            if (NPC.frame.Y >= frameHeight * 4)
            {
                NPC.frame.Y = 0 * frameHeight;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (CarryingEnemy)
            {
                Texture2D tex = ModContent.Request<Texture2D>("Spooky/Content/NPCs/SpiderCave/TarantulaHawkEnemyCarry").Value;

                Vector2 drawPosition = new Vector2(NPC.Center.X, NPC.Center.Y) - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 20);

                var effects = NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

                spriteBatch.Draw(tex, drawPosition, new Rectangle(0, EnemyBeingCarried * NPC.frame.Height, NPC.frame.Width, NPC.frame.Height), drawColor, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0f);
            }

            return true;
        }

        public override void AI()
		{
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(true);
            
            NPC.spriteDirection = NPC.direction;

            if (!InitializedEnemy)
            {
                EnemyBeingCarried = Main.rand.Next(6);
                InitializedEnemy = true;
            }

            switch ((int)NPC.localAI[0])
            {
                //fly towards the player
                case 0:
                {
                    NPC.aiStyle = 5;
			        AIType = NPCID.Moth;

                    NPC.localAI[1]++;

                    if (NPC.localAI[1] > 300)
                    {
                        //do not charge at the player if they are too far or they are not within line of sight
                        if (Vector2.Distance(player.Center, NPC.Center) <= 250f)
                        {
                            NPC.localAI[1] = 0;
                            NPC.localAI[0] = CarryingEnemy ? 1 : 2;

                            NPC.netUpdate = true;
                        }
                    }

                    break;
                }

                //drop carried enemy on the player
                case 1:
                {
                    NPC.aiStyle = -1;
                    NPC.rotation = 0;

                    NPC.localAI[1]++;

                    //go above player
                    if (NPC.localAI[1] <= 75)
                    {
                        Vector2 GoTo = player.Center;
                        GoTo.Y -= 250;

                        float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 6, 12);
                        NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
                    }
                    else
                    {
                        NPC.velocity *= 0.95f;
                    }

                    //drop the enemy and reset ai
                    if (NPC.localAI[1] >= 100)
                    {
                        int DroppedEnemy = 0;

                        switch (EnemyBeingCarried)
                        {
                            case 0:
                            {   
                                DroppedEnemy = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y + 50, ModContent.NPCType<JumpingSpider1>());
                                break;
                            }
                            case 1:
                            {   
                                DroppedEnemy = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y + 50, ModContent.NPCType<JumpingSpider2>());
                                break;
                            }
                            case 2:
                            {   
                                DroppedEnemy = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y + 50, ModContent.NPCType<OrbWeaver3>());
                                break;
                            }
                            case 3:
                            {   
                                DroppedEnemy = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y + 50, ModContent.NPCType<OrbWeaver1>());
                                break;
                            }
                            case 4:
                            {   
                                DroppedEnemy = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y + 50, ModContent.NPCType<OrbWeaver2>());
                                break;
                            }
                            case 5:
                            {   
                                DroppedEnemy = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y + 50, ModContent.NPCType<TrapdoorSpider1>());
                                break;
                            }
                        }

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {   
                            NetMessage.SendData(MessageID.SyncNPC, number: DroppedEnemy);
                        }

                        NPC.localAI[1] = 0;
                        NPC.localAI[0] = 0;
                        
                        CarryingEnemy = false;
                        NPC.netUpdate = true;
                    }

                    break;
                }

                //charge at the player
                case 2:
                {
                    NPC.aiStyle = -1;
                    NPC.rotation = 0;

                    NPC.localAI[1]++;

                    if (NPC.localAI[1] < 60)
                    {
                        Vector2 GoTo = player.Center;
                        GoTo.X += (NPC.Center.X < player.Center.X) ? -170 : 170;

                        float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 8, 12);
                        NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
                    }

                    if (NPC.localAI[1] == 60)
                    {
                        NPC.velocity *= 0;
                    }

                    if (NPC.localAI[1] == 65)
                    {
                        SoundEngine.PlaySound(SoundID.Zombie77, NPC.Center);

                        Vector2 ChargeDirection = player.Center - NPC.Center;
                        ChargeDirection.Normalize();
                                
                        ChargeDirection.X = ChargeDirection.X * 30;
                        ChargeDirection.Y = ChargeDirection.Y * 5;
                        NPC.velocity.X = ChargeDirection.X;
                        NPC.velocity.Y = ChargeDirection.Y;
                    }

                    if (NPC.localAI[1] >= 65)
                    {
                        NPC.spriteDirection = NPC.direction = NPC.velocity.X < 0 ? -1 : 1;
                    }

                    if (NPC.localAI[1] >= 90)
                    {
                        NPC.velocity *= 0.65f;
                    }

                    if (NPC.localAI[1] >= 120)
                    {   
                        NPC.localAI[1] = 0;
                        NPC.localAI[0] = 0;

                        NPC.netUpdate = true;
                    }

                    break;
                }
            }
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 6; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/TarantulaHawkBrownGore" + numGores).Type);
                    }
                }
            }
        }
    }

    public class TarantulaHawk2 : TarantulaHawk1
    {   
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.TarantulaHawk2"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpiderCaveBiome>().ModBiomeBestiaryInfoElement)
			});
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 6; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/TarantulaHawkOrangeGore" + numGores).Type);
                    }
                }
            }
        }
    }

    public class TarantulaHawk3 : TarantulaHawk1
    {
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.TarantulaHawk3"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpiderCaveBiome>().ModBiomeBestiaryInfoElement)
			});
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 6; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/TarantulaHawkGrayGore" + numGores).Type);
                    }
                }
            }
        }
    }
}