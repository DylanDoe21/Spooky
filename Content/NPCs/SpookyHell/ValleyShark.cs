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

using Spooky.Content.NPCs.SpookyHell.Projectiles;

namespace Spooky.Content.NPCs.SpookyHell
{
    public class ValleyShark : ModNPC  
    {
        public static readonly SoundStyle HitSound = new("Spooky/Content/Sounds/EggEvent/EnemyHit", SoundType.Sound);
        public static readonly SoundStyle DeathSound = new("Spooky/Content/Sounds/EggEvent/EnemyDeath", SoundType.Sound);

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 11;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            //bools
            //writer.Write(HasShotMouth);

            //floats
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
            writer.Write(NPC.localAI[2]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            //bools
            //HasShotMouth = reader.ReadBoolean();

            //floats
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            NPC.localAI[2] = reader.ReadSingle();
        }
        
        public override void SetDefaults()
		{
            NPC.lifeMax = 6000;
            NPC.damage = 70;
            NPC.defense = 20;
            NPC.width = 94;
			NPC.height = 72;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 1, 50, 0);
            NPC.HitSound = HitSound;
			NPC.DeathSound = DeathSound;
            SpawnModBiomes = new int[2] { ModContent.GetInstance<Biomes.SpookyHellBiome>().Type, ModContent.GetInstance<Biomes.SpookyHellLake>().Type };
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.9f * bossAdjustment);
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.ValleyShark"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyHellBiome>().ModBiomeBestiaryInfoElement),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyHellLake>().ModBiomeBestiaryInfoElement)
			});
		}
        
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>("Spooky/Content/NPCs/SpookyHell/ValleySharkGlow").Value;

            var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(tex, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), 
            NPC.frame, Color.White, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0);
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 1;

            if (!NPC.wet)
            {
                //running animation
                if (NPC.frameCounter > 4)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0.0;
                }
                if (NPC.frame.Y >= frameHeight * 6)
                {
                    NPC.frame.Y = 0 * frameHeight;
                }

                //jumping frame
                if (NPC.velocity.Y > 0 || NPC.velocity.Y < 0)
                {
                    NPC.frame.Y = 6 * frameHeight;
                }

                //mouth eject frames
                if (NPC.localAI[0] == 0)
                {
                    if (NPC.localAI[1] > 300 && NPC.localAI[1] < 350)
                    {
                        NPC.frame.Y = 0 * frameHeight;
                    }
                    if (NPC.localAI[1] >= 350)
                    {
                        NPC.frame.Y = 7 * frameHeight;
                    }
                }
            }
            else
            {
                //swimming animations
                if (NPC.frame.Y < frameHeight * 9)
                {
                    NPC.frame.Y = 8 * frameHeight;
                }

                if (NPC.frameCounter > 4)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0.0;
                }
                if (NPC.frame.Y >= frameHeight * 11)
                {
                    NPC.frame.Y = 8 * frameHeight;
                }
            }
        }

        public override void AI()
		{
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(true);

			int Damage = Main.masterMode ? 70 / 3 : Main.expertMode ? 50 / 2 : 35;

            if (NPC.wet)
            {
                NPC.aiStyle = 3;
                AIType = NPCID.ZombieMerman;

                NPC.direction = NPC.spriteDirection = NPC.velocity.X > 0f ? 1 : -1;
                
                NPC.rotation = NPC.velocity.ToRotation();

                if (NPC.spriteDirection == -1)
                {
                    NPC.rotation += MathHelper.Pi;
                }
            }
            else
            {
                NPC.spriteDirection = NPC.direction;

                NPC.rotation = 0;

                switch ((int)NPC.localAI[0])
                {
                    //walk at the player, use mouth eject attack if the player gets within a certain rectangular distance
                    case 0:
                    {
                        if (!NPC.wet)
                        {
                            NPC.localAI[1]++;

                            if (NPC.localAI[1] <= 300)
                            {
                                NPC.aiStyle = 3;
                                AIType = NPCID.PirateCorsair;
                            }
                            else
                            {
                                NPC.aiStyle = 0;
                            }
                        }

                        //slow down before ejecting its mouth
                        if (NPC.localAI[1] >= 300)
                        {
                            NPC.velocity.X *= 0;
                        }

                        //shoot out mouth
                        if (NPC.localAI[1] == 350)
                        {
                            NPC.localAI[2] = NPC.direction;

                            SoundEngine.PlaySound(SoundID.Item171, NPC.Center);

                            NPC.ai[3] = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<ValleySharkMouth>(), ai3: NPC.whoAmI);
                    
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                NetMessage.SendData(MessageID.SyncNPC, number: (int)NPC.ai[3]);
                            }
                        }

                        if (NPC.localAI[1] > 350)
                        {
                            NPC.direction = (int)NPC.localAI[2];
                            NPC.spriteDirection = (int)NPC.localAI[2];
                        }

                        if (NPC.localAI[1] > 350 && !NPC.AnyNPCs(ModContent.NPCType<ValleySharkMouth>()))
                        {
                            NPC.localAI[1] = 0;
                            //NPC.localAI[0]++;

                            NPC.netUpdate = true;
                        }

                        break;
                    }

                    //slow down, jump in the air, then slam down and create blood thorns out of the ground where it lands
                    case 1:
                    {
                        break;
                    }
                }
            }
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
            //dont run on multiplayer
			if (Main.netMode == NetmodeID.Server) 
            {
				return;
			}

			if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 7; numGores++)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/ValleySharkGore" + numGores).Type);
                }
            }
        }
    }
}