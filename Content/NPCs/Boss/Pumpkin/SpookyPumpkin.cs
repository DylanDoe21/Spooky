using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using System.Collections.Generic;

using Spooky.Content.NPCs.Boss.Pumpkin.Projectiles;

namespace Spooky.Content.NPCs.Boss.Pumpkin
{
    //[AutoloadBossHead]
    public class SpookyPumpkin : ModNPC
    {
        public bool Phase2 = false;
        public bool Transition = false;
        public bool Phase2Sprite = false;

        public static int secondStageHeadSlot = -1;

        public static readonly SoundStyle FlyBuzz = new SoundStyle("Spooky/Content/Sounds/FlyBuzzing", SoundType.Sound);
        
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rot-Gourd");
            Main.npcFrameCount[NPC.type] = 12;
            NPCID.Sets.TrailCacheLength[NPC.type] = 7;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 1420;
            NPC.damage = 40;
            NPC.defense = 10;
            NPC.width = 138;
            NPC.height = 128;
            NPC.knockBackResist = 0f;
            NPC.lavaImmune = true;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.netAlways = true;
            NPC.boss = true;
            NPC.HitSound = SoundID.NPCHit7;
			NPC.DeathSound = SoundID.NPCDeath1;
            Music = MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/PumpkinBoss");
            NPC.aiStyle = -1;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = 1850;
            NPC.damage = 55;
            NPC.defense = 10;
        }

        /*
        public override void Load() 
        {
			string texture = BossHeadTexture + "_P2";
			secondStageHeadSlot = Mod.AddBossHeadTexture(texture, -1);
		}

		public override void BossHeadSlot(ref int index) 
        {
			int slot = secondStageHeadSlot;
			if (Phase2Sprite && slot != -1) 
            {
				index = slot;
			}
		}
        */

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
			if (NPC.velocity != Vector2.Zero) 
			{
                Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
				Vector2 drawOrigin = new Vector2(tex.Width * 0.5f, (NPC.height * 0.5f));
				for (int k = 0; k < NPC.oldPos.Length; k++)
				{
					var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
					Vector2 drawPos = NPC.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, NPC.gfxOffY + 4);
					Color color = NPC.GetAlpha(Color.Brown) * (float)(((float)(NPC.oldPos.Length - k) / (float)NPC.oldPos.Length) / 2);
					spriteBatch.Draw(tex, drawPos, new Microsoft.Xna.Framework.Rectangle?(NPC.frame), color, NPC.rotation, drawOrigin, NPC.scale, effects, 0f);
				}
			}
            
            return true;
		}

        public override void FindFrame(int frameHeight)
        {
            if (!Phase2Sprite)
            {
                if (NPC.velocity.Y >= -0.1 && NPC.velocity.Y <= 0.1)
                {
                    NPC.frameCounter += 1;
                    if (NPC.frameCounter > 6)
                    {
                        NPC.frame.Y = NPC.frame.Y + frameHeight;
                        NPC.frameCounter = 0.0;
                    }
                    if (NPC.frame.Y >= frameHeight * 4)
                    {
                        NPC.frame.Y = frameHeight * 0;
                    }
                }
                if (NPC.velocity.Y <= -0.1)
                {
                    NPC.frame.Y = frameHeight * 4;
                }
                if (NPC.velocity.Y >= 0.1)
                {
                    NPC.frame.Y = frameHeight * 5;
                }
            }
            
            if (Phase2Sprite)
            {
                if (NPC.ai[3] == 120)
                {
                    NPC.frame.Y = frameHeight * 7;
                }
                
                if (NPC.velocity.Y >= -0.1 && NPC.velocity.Y <= 0.1)
                {
                    NPC.frameCounter += 1;
                    if (NPC.frameCounter > 6)
                    {
                        NPC.frame.Y = NPC.frame.Y + frameHeight;
                        NPC.frameCounter = 0.0;
                    }
                    if (NPC.frame.Y >= frameHeight * 10)
                    {
                        NPC.frame.Y = frameHeight * 6;
                    }
                }
                if (NPC.velocity.Y <= -0.1)
                {
                    NPC.frame.Y = frameHeight * 10;
                }
                if (NPC.velocity.Y >= 0.1)
                {
                    NPC.frame.Y = frameHeight * 11;
                }
			}
        }

        public override void AI()
        {
            NPC.spriteDirection = NPC.direction;
            NPC.rotation = NPC.velocity.X * 0.04f;

            int Damage = Main.expertMode ? 12 : 20;

            NPC.TargetClosest(true);
            NPC.netUpdate = true;
            Player player = Main.player[NPC.target];

            //fade away and despawn when all players die or if you leave the biome
            if (Main.player[NPC.target].dead || !player.InModBiome(ModContent.GetInstance<Content.Biomes.SpookyBiome>()))
            {   
                NPC.alpha += 2;

                if (NPC.alpha >= 255)
                {
                    NPC.active = false;
                }
            }
            else
            {
                if (NPC.alpha >= 0)
                {
                    NPC.alpha -= 2;
                }
            }

            //spawn swarm of flies when spawned
            if (NPC.ai[2] <= 0)
            {
                for (int i = 0; i < 20; i++)
                {
                    Vector2 vector2_2 = Vector2.UnitY.RotatedByRandom(1.57079637050629f) * new Vector2(5f, 3f);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, vector2_2.X, vector2_2.Y, 
                    ModContent.ProjectileType<Fly>(), 0, 0.0f, Main.myPlayer, 0.0f, (float)NPC.whoAmI);
                }

                NPC.netUpdate = true;

                NPC.ai[2] = 1;
            }

            //for the transition when its pumpkin shell begins to break
            if (NPC.life < (NPC.lifeMax / 2) && !Phase2)
            {
                Transition = true;

                NPC.immortal = true;
                NPC.dontTakeDamage = true;
                NPC.noTileCollide = false;
                NPC.noGravity = false;

                NPC.velocity.X *= 0;

                NPC.ai[3]++;

                if (NPC.ai[3] == 120)
                {
                    Phase2Sprite = true;
                }

                if (NPC.ai[3] >= 121)
                {
                    Gore.NewGore(NPC.GetSource_FromThis(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/PumpkinShardGore1").Type);
                    Gore.NewGore(NPC.GetSource_FromThis(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/PumpkinShardGore2").Type);
                    Gore.NewGore(NPC.GetSource_FromThis(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/PumpkinShardGore3").Type);

                    Phase2 = true;
                    Transition = false;

                    NPC.immortal = false;
                    NPC.dontTakeDamage = false;

                    NPC.localAI[0] = 0;
                    NPC.localAI[1] = 0;
                    NPC.localAI[2] = 0;
                    NPC.ai[0] = 0;
                }
            }

            if (!Transition)
            {
                switch ((int)NPC.ai[0])
                {
                    //Jump 3 times towards the player
                    case 0:
                    {
                        NPC.localAI[0]++;
                        
                        if (NPC.localAI[1] < 3)
                        {
                            Vector2 JumpTo = new Vector2(player.Center.X, player.Center.Y - 400);
                            Vector2 velocity = JumpTo - NPC.Center;

                            //actual jumping
                            if (NPC.localAI[0] == 60)
                            {
                                float speed = MathHelper.Clamp(velocity.Length() / 36, 6, 18);
                                velocity.Normalize();
                                velocity.Y -= 0.18f;
                                velocity.X *= 1.1f;
                                NPC.velocity = velocity * speed * 1.1f;
                            }

                            //set no tile collide to true so he can jump through blocks
                            if (NPC.localAI[0] >= 60 && NPC.localAI[0] < 115)
                            {
                                NPC.noTileCollide = true;
                            }

                            //charge down after jumping while in the air
                            if (NPC.localAI[0] == 115)
                            {
                                SoundEngine.PlaySound(SoundID.DD2_JavelinThrowersAttack, NPC.position);

                                NPC.noTileCollide = false;
                                NPC.noGravity = true;

                                int FallSpeed = Phase2 ? 20 : 15;

                                Vector2 ChargeDirection = NPC.velocity;
                                ChargeDirection.Normalize();
                                        
                                ChargeDirection.X *= 0;
                                ChargeDirection.Y = FallSpeed;  
                                NPC.velocity.X = ChargeDirection.X;
                                NPC.velocity.Y = ChargeDirection.Y;
                            }

                            //slam the ground
                            if (NPC.localAI[0] > 115 && NPC.localAI[2] == 0 && NPC.velocity.Y <= 0.1f)
                            {
                                NPC.noGravity = false;

                                NPC.velocity.X *= 0;

                                /*
                                foreach (Player Player in Main.player.Where(Player => Vector2.Distance(Player.Center, NPC.Center) <= 1000))
                                {
                                    Player.GetModPlayer<Core.SpookyPlayer>().Shake = (350 - (int)Vector2.Distance(Player.Center, NPC.Center)) / 12;
                                }
                                */

                                if (player.velocity.Y == 0)
                                {
                                    player.velocity.Y -= 8f;
                                }

                                SoundEngine.PlaySound(SoundID.Item14, NPC.position);

                                //make cool dust effect when slamming the ground
                                for (int i = 0; i < 65; i++)
                                {                                                                                  
                                    int num = Dust.NewDust(NPC.position, NPC.width, NPC.height / 5, DustID.Dirt, 0f, -2f, 0, default(Color), 1.5f);
                                    Main.dust[num].noGravity = true;
                                    Main.dust[num].position.X -= Main.rand.Next(-50, 51) * .05f - 1.5f;
                                    Main.dust[num].position.Y += 104;
                                    Main.dust[num].scale = 3f;
                                    
                                    if (Main.dust[num].position != NPC.Center)
                                    {
                                        Main.dust[num].velocity = NPC.DirectionTo(Main.dust[num].position) * 2f;
                                    }
                                }
                                
                                //in phase 2 shoot random spreads of bolts upward
                                if (Phase2)
                                {
                                    SoundEngine.PlaySound(SoundID.Item70, NPC.position);

                                    int NumProjectiles = Main.rand.Next(10, 15);
                                    for (int i = 0; i < NumProjectiles; ++i)
                                    {
                                        float Spread = (float)Main.rand.Next(-1500, 1500) * 0.01f;

                                        if (Main.netMode != NetmodeID.MultiplayerClient)
                                        {
                                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y + 20, 0 + Spread, Main.rand.Next(-18, -13), 
                                            ModContent.ProjectileType<RootThorn>(), Damage, 1, Main.myPlayer, 0, 0);
                                        }
                                    }
                                }
                                
                                //"complete" the jump attack
                                NPC.localAI[2] = 1; 
                            }

                            //only loop attack if the jump has been completed
                            if (NPC.localAI[0] >= 185 && NPC.localAI[2] > 0)
                            {
                                NPC.localAI[0] = 0;
                                NPC.localAI[2] = 0;
                                NPC.localAI[1]++;
                            }
                        }
                        else
                        {
                            NPC.localAI[0] = 0;
                            NPC.localAI[1] = 0;
                            NPC.localAI[2] = 0;
                            NPC.ai[0]++;
                        }

                        break;
                    }

                    //jump, float above the player, then use super smash
                    case 1:
                    {
                        NPC.localAI[0]++;

                        //fly above the player for a second
                        if (NPC.localAI[0] >= 0 && NPC.localAI[0] < 60)
                        {
                            NPC.noTileCollide = true;

                            NPC.direction = Math.Sign(player.Center.X - NPC.Center.X);	
                            Vector2 GoTo = player.Center;
                            NPC.spriteDirection = NPC.direction;
                            GoTo.X += 0f;
                            GoTo.Y -= 450;

                            float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 18, 25);
                            NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
                        }

                        if (NPC.localAI[0] == 60)
                        {
                            NPC.velocity.X *= 0;
                        }

                        //attempt to crush the player
                        if (NPC.localAI[0] == 70)
                        {
                            SoundEngine.PlaySound(SoundID.DD2_JavelinThrowersAttack, NPC.position);
                            
                            int FallSpeed = Phase2 ? 30 : 25;
                            
                            NPC.noGravity = true;

                            Vector2 ChargeDirection = Main.player[NPC.target].Center - NPC.Center;
                            ChargeDirection.Normalize();
                                    
                            ChargeDirection.X = ChargeDirection.X * 0;
                            ChargeDirection.Y = ChargeDirection.Y * FallSpeed;  
                            NPC.velocity.X = ChargeDirection.X;
                            NPC.velocity.Y = ChargeDirection.Y;
                        }

                        if (NPC.localAI[0] > 70)
                        {
                            if (NPC.position.Y >= player.Center.Y - 150)
                            {
                                NPC.noTileCollide = false;
                            }
                        }

                        //make pumpkin collide with tiles after flying
                        if (NPC.localAI[0] > 70 && NPC.velocity.Y <= 0.1f && NPC.localAI[2] == 0)
                        {
                            NPC.noGravity = false;

                            NPC.velocity.X *= 0;
                            
                            /*
                            foreach (Player Player in Main.player.Where(Player => Vector2.Distance(Player.Center, NPC.Center) <= 1000))
                            {
                                Player.GetModPlayer<Core.SpookyPlayer>().Shake = (450 - (int)Vector2.Distance(Player.Center, NPC.Center)) / 12;
                            }
                            */
                            
                            if (player.velocity.Y == 0)
                            {
                                player.velocity.Y -= 10f;
                            }
                            
                            //make cool dust effect when slamming the ground
                            for (int i = 0; i < 65; i++)
                            {                                                                                  
                                int num = Dust.NewDust(NPC.position, NPC.width, NPC.height / 5, DustID.Dirt, 0f, -2f, 0, default(Color), 1.5f);
                                Main.dust[num].noGravity = true;
                                Main.dust[num].position.X -= Main.rand.Next(-50, 51) * .05f - 1.5f;
                                Main.dust[num].position.Y += 104;
                                Main.dust[num].scale = 3f;
                                
                                if (Main.dust[num].position != NPC.Center)
                                {
                                    Main.dust[num].velocity = NPC.DirectionTo(Main.dust[num].position) * 2f;
                                }
                            }

                            SoundEngine.PlaySound(SoundID.Item14, NPC.position);

                            //in phase 2, shoot root spikes from the ground
                            if (Phase2)
                            {
                                SoundEngine.PlaySound(SoundID.Item69, NPC.position);

                                for (int j = 1; j <= 5; j++)
                                {
                                    for (int i = -1; i <= 1; i += 2) 
                                    {
                                        Vector2 center = new Vector2(NPC.Center.X, NPC.Center.Y + NPC.height / 4);
                                        center.X += j * 50 * i; //50 is the distance between each one
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

                                        if (Main.netMode != NetmodeID.MultiplayerClient)
                                        {
                                            Projectile.NewProjectile(NPC.GetSource_FromThis(), center.X, center.Y, 0, 0, 
                                            ModContent.ProjectileType<RootTelegraph>(), Damage, 1, Main.myPlayer, 0, 0);
                                        }
                                    }
                                }
                            }

                            //"complete" the jump attack
                            NPC.localAI[2] = 1;
                        }

                        if (NPC.localAI[0] >= 180 && NPC.localAI[2] > 0)
                        {
                            NPC.localAI[0] = 0;
                            NPC.localAI[2] = 0; //reset for the projectile smash in phase 2
                            NPC.ai[0]++;
                        }

                        break;
                    }

                    //fire a barrage of pumpkin seeds
                    case 2:
                    {
                        NPC.localAI[0]++;

                        if (NPC.localAI[0] > 20 && NPC.localAI[0] < 110)
                        {
                            NPC.velocity *= 0;

                            if (Main.rand.Next(7) == 0)
                            {
                                SoundEngine.PlaySound(SoundID.Item42, NPC.position);

                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X + Main.rand.Next(-100, 100), NPC.Center.Y + Main.rand.Next(-100, 100), 
                                    Main.rand.Next(-1, 1), Main.rand.Next(-4, -2), ModContent.ProjectileType<PumpkinSeed>(), Damage, 1, Main.myPlayer, 0, 0);
                                }
                            }
                        }

                        if (NPC.localAI[0] > 200)
                        {
                            NPC.localAI[0] = 0;
                            NPC.ai[0]++;
                        }
                        
                        break;
                    }

                    //jump across the player, then release flies mid air and then fall and slow down
                    case 3:
                    {
                        NPC.localAI[0]++;

                        Vector2 JumpTo = NPC.Center.X > player.Center.X ? new Vector2(player.Center.X - 600, player.Center.Y - 1000) : new Vector2(player.Center.X + 600, player.Center.Y - 1000);
                        Vector2 velocity = JumpTo - NPC.Center;

                        //actual jumping
                        if (NPC.localAI[0] == 60)
                        {
                            float speed = MathHelper.Clamp(velocity.Length() / 36, 6, 18);
                            velocity.Normalize();
                            velocity.Y -= 0.25f;
                            velocity.X *= 1.2f;
                            NPC.velocity = velocity * speed * 1.1f;
                        }

                        //set no tile collide to true so he can jump through blocks
                        if (NPC.localAI[0] >= 60 && NPC.localAI[0] <= 110)
                        {
                            NPC.noTileCollide = true;
                        }

                        if (NPC.localAI[0] == 80)
                        {
                            SoundEngine.PlaySound(FlyBuzz, NPC.position);
                        }

                        if (NPC.localAI[0] >= 70 && NPC.localAI[0] <= 130)
                        {
                            float rotation = (float)Math.Atan2(NPC.Center.Y - player.Center.Y, NPC.Center.X - player.Center.X);

                            if (Main.rand.Next(5) == 0)
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X + Main.rand.Next(-60, 60), NPC.Center.Y + Main.rand.Next(-60, 60), Main.rand.NextFloat(-8f, 8f), 
                                    Main.rand.NextFloat(-8f, 8f), ModContent.ProjectileType<FlyProj>(), Damage, 1, Main.myPlayer, 0, 0);	
                                }
                            }
                        }

                        if (NPC.localAI[0] > 130)
                        {
                            NPC.velocity.X *= 0.92f;
                            NPC.noTileCollide = false;
                        }

                        if (NPC.localAI[0] >= 320)
                        {
                            if (!Phase2)
                            {
                                NPC.localAI[0] = 0;
                                NPC.localAI[1] = 0; //reset for the jumping attack loop
                                NPC.ai[0] = 0; //reset back to 0 when in phase 1 so it doesnt use any phase 2 attacks yet
                            }

                            if (Phase2)
                            {
                                NPC.localAI[0] = 0;
                                NPC.localAI[1] = 0; //reset for the jumping attack loop
                                NPC.ai[0]++; //go to new attacks in phase 2
                            }
                        }

                        break;
                    }

                    //go to the players side and charge at them
                    case 4:
                    {
                        NPC.localAI[0]++;

                        if (NPC.localAI[0] >= 0 && NPC.localAI[0] < 60)
                        {
                            NPC.noTileCollide = true;
                            NPC.noGravity = true;

                            NPC.direction = Math.Sign(player.Center.X - NPC.Center.X);	
                            Vector2 GoTo = player.Center;
                            NPC.spriteDirection = NPC.direction;
                            GoTo.X += (NPC.Center.X < player.Center.X) ? -320 : 320;
                            GoTo.Y -= 175;

                            float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 12, 25);
                            NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
                        }

                        //Charge at the player
                        if (NPC.localAI[0] == 60)
                        {
                            SoundEngine.PlaySound(SoundID.DD2_JavelinThrowersAttack, NPC.position);

                            Vector2 ChargeDirection = Main.player[NPC.target].Center - NPC.Center;
                            ChargeDirection.Normalize();
                                    
                            ChargeDirection.X = ChargeDirection.X * 22;
                            ChargeDirection.Y = ChargeDirection.Y * 1;  
                            NPC.velocity.X = ChargeDirection.X;
                            NPC.velocity.Y = ChargeDirection.Y;
                        }

                        if (NPC.localAI[0] >= 90)
                        {
                            NPC.noTileCollide = false;
                            NPC.noGravity = false;
                            NPC.velocity.X *= 0.95f;                        
                        }

                        if (NPC.localAI[0] >= 160)
                        {
                            NPC.localAI[0] = 0;
                            NPC.ai[0]++;
                        }

                        break;
                    }

                    //jump above the player, shoot rotten shards at the player 3 times
                    case 5:
                    {
                        NPC.localAI[0]++;

                        Vector2 JumpTo = NPC.Center.X > player.Center.X ? new Vector2(player.Center.X - 600, player.Center.Y - 1200) : new Vector2(player.Center.X + 600, player.Center.Y - 1200);
                        Vector2 velocity = JumpTo - NPC.Center;

                        //actual jumping
                        if (NPC.localAI[0] == 60)
                        {
                            float speed = MathHelper.Clamp(velocity.Length() / 36, 6, 18);
                            velocity.Normalize();
                            velocity.Y -= 0.25f;
                            velocity.X *= 1.2f;
                            NPC.velocity = velocity * speed * 1.1f;
                        }

                        //set no tile collide to true so he can jump through blocks
                        if (NPC.localAI[0] >= 60 && NPC.localAI[0] <= 100)
                        {
                            NPC.noTileCollide = true;
                        }

                        //shoot spreads of root spikes at the player
                        if (NPC.localAI[0] == 60 || NPC.localAI[0] == 80 || NPC.localAI[0] == 100) 
                        {
                            SoundEngine.PlaySound(SoundID.Item70, NPC.position);

                            float Spread = (float)Main.rand.Next(-1000, 1000) * 0.01f;
                            
                            for (int j = 0; j < 3; j++)
                            {
                                Vector2 ShootSpeed = Main.player[NPC.target].Center - NPC.Center;
                                ShootSpeed.Normalize();
                                ShootSpeed.X *= 6f;
                                ShootSpeed.Y *= 6f;
                                
                                float SpreadX = (float)Main.rand.Next(-200, 200) * 0.01f;
                                float SpreadY = (float)Main.rand.Next(-200, 200) * 0.01f;
                                
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, ShootSpeed.X + SpreadX, 
                                    ShootSpeed.Y + SpreadY, ModContent.ProjectileType<RootThorn2>(), Damage, 1, NPC.target, 0, 0);
                                }
                            }
                        }

                        if (NPC.localAI[0] > 125)
                        {
                            NPC.velocity.X *= 0.92f;

                            NPC.noTileCollide = false;
                        }

                        if (NPC.localAI[0] >= 180)
                        {
                            NPC.velocity.X *= 0f;

                            NPC.localAI[0] = 0;
                            NPC.localAI[1] = 0; //reset for the jumping attack loop
                            NPC.ai[0] = 0; //reset attack pattern
                        }

                        break;
                    }
                }
            }
		}

        public override bool CheckDead()
        {
            if (NPC.life <= 0)
			{
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/PumpkinShardGore4").Type);
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/PumpkinShardGore5").Type);
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/PumpkinShardGore6").Type);

                NPC.damage = 0;
                NPC.life = -1;
                NPC.dontTakeDamage = true;
                NPC.netUpdate = true;
                
                int Phase2 = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y + 60, ModContent.NPCType<SpookyPumpkinP2>());

                //net update so it doesnt vanish on multiplayer
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.SyncNPC, number: Phase2);
                }

                NPC.active = false;

                return false;
            }
            
            return true;
        }
    }
}