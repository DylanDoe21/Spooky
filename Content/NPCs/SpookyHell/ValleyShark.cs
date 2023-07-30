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

using Spooky.Core;
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
            NPCID.Sets.TrailCacheLength[NPC.type] = 8;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;
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
            NPC.width = 72;
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

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
			if (NPC.localAI[0] == 1 && NPC.localAI[1] >= 350 && NPC.localAI[2] == 0)
			{
                Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
				Vector2 drawOrigin = new(tex.Width * 0.5f, (NPC.height * 0.5f));

				for (int oldPos = 0; oldPos < NPC.oldPos.Length; oldPos++)
				{
					var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
					Vector2 drawPos = NPC.oldPos[oldPos] - Main.screenPosition + drawOrigin + new Vector2(-10f, NPC.gfxOffY + 4);
					Color color = NPC.GetAlpha(Color.Red) * (float)(((float)(NPC.oldPos.Length - oldPos) / (float)NPC.oldPos.Length) / 2);
					spriteBatch.Draw(tex, drawPos, new Microsoft.Xna.Framework.Rectangle?(NPC.frame), color, NPC.rotation, drawOrigin, NPC.scale, effects, 0f);
				}
			}
            
            return true;
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
                    NPC.frameCounter = 0;
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
                    if (NPC.localAI[1] > 320 && NPC.localAI[1] < 350)
                    {
                        NPC.frame.Y = 0 * frameHeight;
                    }
                    if (NPC.localAI[1] >= 350)
                    {
                        NPC.frame.Y = 7 * frameHeight;
                    }
                }

                //nose dive frames
                if (NPC.localAI[0] == 1)
                {
                    if (NPC.localAI[1] >= 320 && NPC.localAI[1] < 350)
                    {
                        NPC.frame.Y = 0 * frameHeight;
                    }
                    if (NPC.localAI[1] >= 390 && NPC.localAI[2] == 0)
                    {
                        NPC.frame.Y = 10 * frameHeight;
                    }
                    if (NPC.localAI[2] > 0)
                    {
                        NPC.frame.Y = 0 * frameHeight;
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
                    NPC.frameCounter = 0;
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

                            if (NPC.localAI[1] <= 320)
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
                        if (NPC.localAI[1] >= 320)
                        {
                            NPC.velocity.X *= 0.2f;
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
                            NPC.localAI[2] = 0;
                            NPC.localAI[0]++;

                            NPC.netUpdate = true;
                        }

                        break;
                    }

                    //slow down, jump in the air, then nose dive into the ground and create blood thorns on impact
                    case 1:
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
                                NPC.aiStyle = -1;
                            }
                        }

                        //slow down before jumping
                        if (NPC.localAI[1] >= 320 && NPC.localAI[1] < 350)
                        {
                            NPC.velocity.X *= 0.35f;
                        }

                        //jumping velocity
                        Vector2 JumpTo = new Vector2(player.Center.X, player.Center.Y - 500);

                        Vector2 velocity = JumpTo - NPC.Center;

                        //jump towards the player
                        if (NPC.localAI[1] == 350)
                        {
                            NPC.noTileCollide = true;

                            SoundEngine.PlaySound(SoundID.DD2_JavelinThrowersAttack, NPC.Center);

                            float speed = MathHelper.Clamp(velocity.Length() / 36, 8, 20);
                            velocity.Normalize();
                            velocity.Y -= 0.18f;
                            velocity.X *= 1.1f;
                            NPC.velocity = velocity * speed * 1.1f;
                        }

                        //stop mid air
                        if (NPC.localAI[1] >= 380 && NPC.localAI[1] < 390)
                        {
                            NPC.velocity *= 0.9f;
                        }

                        //charge down at the player
                        if (NPC.localAI[1] == 390)
                        {
                            NPC.noTileCollide = false;

                            SoundEngine.PlaySound(SoundID.DD2_WyvernScream with { Pitch = SoundID.DD2_WyvernScream.Pitch - 0.5f }, NPC.Center);

                            NPC.noGravity = true;
                            NPC.velocity.Y = 35;
                        }

                        //set tile collide to true once it gets to the players level to prevent cheesing
                        if (NPC.localAI[1] > 390 && NPC.localAI[2] == 0)
                        {	
                            NPC.direction = NPC.spriteDirection = NPC.velocity.X > 0f ? 1 : -1;
                
                            NPC.rotation = NPC.velocity.ToRotation();

                            if (NPC.spriteDirection == -1)
                            {
                                NPC.rotation += MathHelper.Pi;
                            }
                        }

                        //slam the ground
                        if (NPC.localAI[1] > 390 && NPC.localAI[2] == 0 && NPC.velocity.Y <= 0.1f)
                        {
                            NPC.noGravity = false;

                            NPC.velocity *= 0;

                            SpookyPlayer.ScreenShakeAmount = 8;

                            SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact, NPC.Center);

                            for (int j = 1; j <= 5; j++)
                            {
                                for (int i = -1; i <= 1; i += 2)
                                {
                                    Vector2 center = new Vector2(NPC.Center.X, NPC.Center.Y + NPC.height / 4);
                                    center.X += j * 45 * i; //45 is the distance between each one
                                    int numtries = 0;
                                    int x = (int)(center.X / 16);
                                    int y = (int)(center.Y / 16);
                                    while (y < Main.maxTilesY - 10 && Main.tile[x, y] != null && !WorldGen.SolidTile2(x, y) && 
                                    Main.tile[x - 1, y] != null && !WorldGen.SolidTile2(x - 1, y) && Main.tile[x + 1, y] != null && !WorldGen.SolidTile2(x + 1, y)) 
                                    {
                                        y++;
                                        center.Y = y * 16;
                                    }
                                    while ((WorldGen.SolidOrSlopedTile(x, y) || WorldGen.SolidTile2(x, y)) && numtries < 10) 
                                    {
                                        numtries++;
                                        y--;
                                        center.Y = y * 16;
                                    }

                                    if (numtries >= 10)
                                    {
                                        break;
                                    }

                                    Vector2 lineDirection = new Vector2(Main.rand.Next(-15, 15), 16);

                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), center.X, center.Y + 25, 0, 0, ModContent.ProjectileType<ValleySharkThorn>(), 
                                    Damage, 0, Main.myPlayer, lineDirection.ToRotation() + MathHelper.Pi, -16 * 60);
                                }
                            }

                            //complete the slam attack
							NPC.localAI[2] = 1; 
                            NPC.localAI[1] = 420;
                        }

                        //only loop attack if the slam attack has been completed
                        if (NPC.localAI[2] > 0)
                        {
                            if (NPC.localAI[1] >= 500)
                            {
                                NPC.localAI[1] = 0;
                                NPC.localAI[2] = 0;
                                NPC.localAI[0] = 0;

                                NPC.netUpdate = true;
                            }
                        }

                        break;
                    }
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            //haemorrhaxe
            npcLoot.Add(ItemDropRule.Common(ItemID.BloodHamaxe, 3));

            //blood thorn
            npcLoot.Add(ItemDropRule.Common(ItemID.SharpTears, 3));

            //sanguine staff
            npcLoot.Add(ItemDropRule.Common(ItemID.SanguineStaff, 3));

            //chum buckets
            npcLoot.Add(ItemDropRule.Common(ItemID.ChumBucket, 1, 5, 10));
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