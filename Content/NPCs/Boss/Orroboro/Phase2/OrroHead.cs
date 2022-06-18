using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Items.BossBags;
using Spooky.Content.Items.SpookyHell;
using Spooky.Content.Items.SpookyHell.Boss;
using Spooky.Content.NPCs.Boss.Orroboro.Projectiles;

namespace Spooky.Content.NPCs.Boss.Orroboro.Phase2
{
    public class OrroHead : ModNPC
    {
        //bools for animation
        public bool Chomp = false;
        public bool OpenMouth = false;
        public bool Synced = false;

        public static readonly SoundStyle CrunchSound = new SoundStyle("Spooky/Content/Sounds/OrroboroCrunch", SoundType.Sound);
        public static readonly SoundStyle GrowlSound = new SoundStyle("Spooky/Content/Sounds/OrroboroGrowl1", SoundType.Sound);

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Orro");
            Main.npcFrameCount[NPC.type] = 5;

            var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                CustomTexturePath = "Spooky/Content/BossTextures/OrroBestiary",
                Position = new Vector2(40f, 24f),
                PortraitPositionXOverride = 0f,
                PortraitPositionYOverride = 20f
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifier);
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 10000;
            NPC.damage = 55;
            NPC.defense = 30;
            NPC.width = 62;
            NPC.height = 62;
            NPC.knockBackResist = 0f;
            NPC.npcSlots = 1f;
            NPC.boss = true;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.behindTiles = true;
            NPC.netAlways = true;
            NPC.HitSound = SoundID.NPCHit9;
            NPC.DeathSound = SoundID.NPCDeath5; //mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/OrroboroSplit").WithVolume(3f);
            Music = MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/Orroboro");
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = 12500;
            NPC.damage = 70;
            NPC.defense = 20;
        }
        
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new MoonLordPortraitBackgroundProviderBestiaryInfoElement(), //Plain black background
				new FlavorTextBestiaryInfoElement("The two parasites constantly grow as they devour each other's flesh. A symbiotic relationship, and a very odd one.")
			});
		}

        //rotate the bosses map icon to the NPCs direction
        public override void BossHeadRotation(ref float rotation)
        {
            rotation = NPC.rotation;
        }

        public override void FindFrame(int frameHeight)
        {
            if (!Chomp)
            {
                if (!OpenMouth)
                {
                    NPC.frame.Y = frameHeight * 0;
                }
                if (OpenMouth)
                {
                    NPC.frame.Y = frameHeight * 3;
                }
            }
            if (Chomp)
            {
                NPC.frameCounter += 1;
                if (NPC.frameCounter > 4)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0.0;
                }
                if (NPC.frame.Y >= frameHeight * 5)
                {
                    SoundEngine.PlaySound(CrunchSound, NPC.position);

                    NPC.frame.Y = 0;
                }
            }
        }

        public override bool CheckDead()
        {
            if (NPC.AnyNPCs(ModContent.NPCType<BoroHead>()))
            {
                //if boro hasnt "died"
                if (Main.npc[NPCGlobal.Boro].ai[3] <= 0)
                {
                    NPC.ai[3] = 1;

                    NPC.life = 1;
                    NPC.immortal = true;
                    NPC.dontTakeDamage = true;

                    return false;
                }
                //if orro has "died"
                else if (Main.npc[NPCGlobal.Boro].ai[3] > 0)
                {
                    Main.NewText("Boro has been defeated!", 171, 64, 255);

                    Gore.NewGore(NPC.GetSource_Death(), Main.npc[NPCGlobal.Boro].position, Main.npc[NPCGlobal.Boro].velocity / 5, ModContent.Find<ModGore>("Spooky/BoroHeadGore1").Type);
                    Gore.NewGore(NPC.GetSource_Death(), Main.npc[NPCGlobal.Boro].position, Main.npc[NPCGlobal.Boro].velocity / 5, ModContent.Find<ModGore>("Spooky/BoroHeadGore2").Type);

                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("Spooky/OrroHeadGore1").Type);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("Spooky/OrroHeadGore2").Type);

                    NPC.life = 0;
                    Main.npc[NPCGlobal.Boro].life = 0;
                    NPC.active = false;

                    return true;
                }
            }
            
            return true;
        }

        public override void HitEffect (int hitDirection, double damage)
		{
            //if boro has "died", spawn gores normally
            if (!NPC.AnyNPCs(ModContent.NPCType<BoroHead>()) && NPC.life <= 0)
            {
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("Spooky/OrroHeadGore1").Type);
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("Spooky/OrroHeadGore2").Type);
            }
		}

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            //if boro is in its death state
            if (NPC.ai[3] > 0)
            {
                float num108 = 4;
                float num107 = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.4f / 2.4f * 6.28318548f)) / 2f + 0.5f;
                float num106 = 0f;

                Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;

                Vector2 vector33 = new Vector2(NPC.Center.X, NPC.Center.Y) - Main.screenPosition + new Vector2(0, NPC.gfxOffY) - NPC.velocity;
                Color color29 = new Color(127 - NPC.alpha, 127 - NPC.alpha, 127 - NPC.alpha, 0).MultiplyRGBA(Color.LimeGreen);
                for (int num103 = 0; num103 < 4; num103++)
                {
                    Color color28 = color29;
                    color28 = NPC.GetAlpha(color28);
                    color28 *= 1f - num107;
                    Vector2 vector29 = new Vector2(NPC.Center.X, NPC.Center.Y) + ((float)num103 / (float)num108 * 6.28318548f + NPC.rotation + num106).ToRotationVector2() * (4f * num107 + 2f) - Main.screenPosition + new Vector2(0, NPC.gfxOffY) - NPC.velocity * (float)num103;
                    Main.EntitySpriteDraw(tex, vector29, NPC.frame, color28, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale * 1.5f, SpriteEffects.None, 0);
                }
            }

            return true;
        }

        public override bool PreAI()
        {
            NPCGlobal.Orro = NPC.whoAmI;

            if (NPC.CountNPCS(ModContent.NPCType<OrroHead>()) > 1)
            {
                NPC.active = false;
            }

            Player player = Main.player[NPC.target];

            NPC.TargetClosest(true);

            int Damage = Main.expertMode ? 28 : 40;

            NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X) + 1.57f;

            //despawn if all players are dead or not in the biome
            if (Main.player[NPC.target].dead || !player.InModBiome(ModContent.GetInstance<Content.Biomes.SpookyHellBiome>()))
            {
                NPC.localAI[3]++;
                if (NPC.localAI[3] >= 120)
                {
                    NPC.velocity.Y = 25;
                }

                if (NPC.localAI[3] >= 180)
                {
                    NPC.active = false;
                }
            }
            else
            {
                NPC.localAI[3] = 0;
            }

            //Make the worm itself
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                //use ai 1 to track if the segments are spawned or not
                if (NPC.ai[1] == 0)
                {
                    NPC.realLife = NPC.whoAmI;
                    int latestNPC = NPC.whoAmI;

                    for (int i = 0; i < 2; ++i)
                    {
                        latestNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.position.X + (NPC.width / 2), (int)NPC.position.Y + (NPC.height / 2), ModContent.NPCType<OrroBody>(), NPC.whoAmI, 0, latestNPC);                   
                        Main.npc[(int)latestNPC].realLife = NPC.whoAmI;
                        Main.npc[(int)latestNPC].ai[2] = NPC.whoAmI;
                    }

                    latestNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.position.X + (NPC.width / 2), (int)NPC.position.Y + (NPC.height / 2), ModContent.NPCType<OrroTail>(), NPC.whoAmI, 0, latestNPC);
                    Main.npc[(int)latestNPC].realLife = NPC.whoAmI;
                    Main.npc[(int)latestNPC].ai[2] = NPC.whoAmI;

                    NPC.ai[1] = 1;
                    NPC.netUpdate = true;
                }
            }

            //attacks
            switch ((int)NPC.ai[0])
            {
                //chase the player while chomping
                case 0:
                {
                    NPC.localAI[0]++;
                    Chomp = true;

                    if (NPC.localAI[1] < 3)
                    {
                        ChaseMovement(player, 8f, 0.20f);
                        
                        if (NPC.localAI[0] >= 150)
                        {
                            NPC.localAI[0] = 0;
                            NPC.localAI[1]++;
                        }
                    }
                    else
                    {
                        Chomp = false;
                        //sync boros ai here because their ai had a weird issue where it was slightly off
                        if (!Synced && NPC.AnyNPCs(ModContent.NPCType<BoroHead>()))
                        {
                            Main.npc[NPCGlobal.Boro].localAI[0] = 0;
                            Main.npc[NPCGlobal.Boro].localAI[1] = 0;
                            Main.npc[NPCGlobal.Boro].ai[0]++;
                            Synced = true;
                        }
                        NPC.localAI[0] = 0;
                        NPC.localAI[1] = 0;
                        NPC.ai[0]++;
                    }
                    
                    break;
                }

                //charge from the top/bottom while boro dashes
                case 1:
                {
                    NPC.localAI[0]++;

                    if (NPC.localAI[1] < 3)
                    {
                        if (NPC.localAI[0] < 80)
                        {
                            Vector2 GoTo = player.Center;
                            GoTo.X += 0;
                            GoTo.Y += (NPC.Center.Y < player.Center.Y) ? -750 : 750;

                            float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 12, 25);
                            NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
                        }

                        if (NPC.localAI[0] == 80)
                        {
                            NPC.velocity *= 0.02f;

                            NPC.position.X = player.position.X;
                            NPC.position.Y = (NPC.Center.Y < player.Center.Y) ? player.position.Y - 750 : player.position.Y + 750;

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), player.Center.X, (NPC.Center.Y < player.Center.Y) ? player.Center.Y - 250 : player.Center.Y + 250, 
                                0, 0, ModContent.ProjectileType<TelegraphOrange>(), 0, 0f, 0);
                            }
                        }

                        if (NPC.localAI[0] == 100)
                        {
                            SoundEngine.PlaySound(GrowlSound, NPC.position);

                            Vector2 ChargeDirection = Main.player[NPC.target].Center - NPC.Center;
                            ChargeDirection.Normalize();
                                    
                            ChargeDirection.X = ChargeDirection.X * 0;
                            ChargeDirection.Y = ChargeDirection.Y * 30;  
                            NPC.velocity.X = ChargeDirection.X;
                            NPC.velocity.Y = ChargeDirection.Y;
                        }

                        if (NPC.localAI[0] > 150)
                        {
                            NPC.localAI[0] = 0;
                            NPC.localAI[1]++;
                        }
                    }
                    else
                    {
                        if (NPC.localAI[0] >= 60)
                        {
                            NPC.localAI[0] = 0;
                            NPC.localAI[1] = 0;
                            NPC.ai[0]++;
                        }
                    }

                    break;
                }

                //go to players side, charge and then curve and spit projectiles in sync with boro
                case 2:
                {
                    NPC.localAI[0]++;
                    
                    if (NPC.localAI[0] < 100)
                    {
                        //this is slightly offset so its even with the other worm in game
                        Vector2 GoTo = player.Center;
                        GoTo.X -= 1250;
                        GoTo.Y += 0;

                        float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 12, 25);
                        NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
                    }
                    
                    //set exact position right before so the curling up is always even
                    if (NPC.localAI[0] == 100)
                    {
                        NPC.velocity *= 0.02f;

                        //this is slightly offset so its even with the other worm in game
                        NPC.position.X = player.Center.X - 1250;
                        NPC.position.Y = player.Center.Y + 0;

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), player.Center.X - 550, player.Center.Y, 0, 0,
                            ModContent.ProjectileType<TelegraphOrange>(), 0, 0f, 0);
                        }
                    }

                    if (NPC.localAI[0] == 120)
                    {
                        SoundEngine.PlaySound(GrowlSound, NPC.position);

                        NPC.velocity.X = 40;
                        NPC.velocity.Y *= 0;
                    }

                    if (NPC.localAI[0] >= 155 && NPC.localAI[0] <= 200)
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

                        NPC.localAI[1] = Math.Sign(angle);
                        NPC.velocity = Vector2.Normalize(NPC.velocity) * 32;

                        NPC.velocity = NPC.velocity.RotatedBy(MathHelper.ToRadians(4f) * NPC.localAI[1]);

                        if (NPC.localAI[0] == 160 || NPC.localAI[0] == 170 || NPC.localAI[0] == 180 || NPC.localAI[0] == 190 || NPC.localAI[0] == 200)
                        {
                            Vector2 ShootSpeed = player.Center - NPC.Center;
                            ShootSpeed.Normalize();
                            ShootSpeed.X *= 3f;
                            ShootSpeed.Y *= 3f;

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, ShootSpeed.X, ShootSpeed.Y, 
                                ModContent.ProjectileType<EyeSpit>(), Damage, 1, Main.myPlayer, 0, 0);  
                            }
                        }
                    }   

                    if (NPC.localAI[0] == 200)
                    {
                        NPC.velocity *= 0.25f;
                    }

                    if (NPC.localAI[0] > 300)
                    {
                        NPC.localAI[0] = 0;
                        NPC.localAI[1] = 0;
                        NPC.ai[0]++;
                    }

                    break;
                }

                //fly above player and drop projectiles down while boro uses acid breath
                case 3:
                {
                    NPC.localAI[0]++;

                    if (NPC.localAI[1] < 4)
                    {
                        Vector2 GoTo = player.Center;

                        GoTo.X += 0;
                        GoTo.Y -= 600;

                        //go from side to side
                        if (NPC.localAI[0] < 120)
                        {
                            GoTo.X += -1000;
                        }
                        if (NPC.localAI[0] > 120)
                        {
                            GoTo.X += 1000;
                        }
                        
                        float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 17, 25);
                        NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);

                        if (NPC.localAI[0] % 20 == 5)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, 0, 5, 
                                ModContent.ProjectileType<EyeSpit>(), Damage, 0f, Main.myPlayer, 0, 0);
                            }
                        }

                        if (NPC.localAI[0] >= 240)
                        {
                            NPC.velocity *= 0.25f;
                            NPC.localAI[0] = 0;
                            NPC.localAI[1]++;
                        }
                    }
                    else
                    {
                        if (NPC.localAI[0] >= 60)
                        {
                            NPC.velocity *= 0.25f;
                            NPC.localAI[0] = 0;
                            NPC.localAI[1] = 0; 
                            NPC.ai[0]++; 
                        }
                    }
                    
                    break;
                }

                //spit acid bolts everywhere and chase player while boro summons tentacle pillars
                case 4:
                {
                    NPC.localAI[0]++;

                    if (NPC.localAI[1] < 3)
                    {
                        //use chase movement
                        ChaseMovement(player, 6.5f, 0.18f);

                        //Shoot toxic spit when nearby the player
                        if (NPC.localAI[0] > 140 && NPC.localAI[0] < 200) 
                        {
                            NPC.velocity *= 0.95f;

                            OpenMouth = true;

                            if (Main.rand.Next(2) == 0)
                            {
                                if (Main.rand.Next(2) == 0)
                                {
                                    SoundEngine.PlaySound(SoundID.NPCDeath13, NPC.position);
                                }
                                
                                Vector2 position = new Vector2(NPC.position.X + (NPC.width / 2), NPC.position.Y + (NPC.height / 2));

                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromThis(), position.X, position.Y, NPC.velocity.X * 0.2f + Main.rand.NextFloat(-5f, 5f) * 1, 
                                    NPC.velocity.Y * 0.2f + Main.rand.NextFloat(-5f, 5f) * 1, ModContent.ProjectileType<EyeSpit>(), Damage, 0f, 0);
                                }
                            }  
                        }
                        else
                        {
                            OpenMouth = false;
                        }
                        
                        if (NPC.localAI[0] >= 240)
                        {
                            NPC.localAI[0] = 0;
                            NPC.localAI[1]++;
                        }
                    }
                    else
                    {
                        OpenMouth = false;
                        NPC.localAI[0] = 0;
                        NPC.localAI[1] = 0; 
                        NPC.ai[0]++; 
                    }

                    break;
                }

                //fly to corner, close in, charge down in the middle
                case 5:
                {
                    NPC.localAI[0]++;

                    if (NPC.localAI[0] > 60 && NPC.localAI[0] < 180)
                    {
                        //this is slightly offset so its even with the other worm in game
                        Vector2 GoTo = player.Center;
                        GoTo.X -= 1600;
                        GoTo.Y -= 750;

                        float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 12, 25);
                        NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
                    }

                    if (NPC.localAI[0] == 180)
                    {
                        NPC.velocity *= 0.02f;

                        //this is slightly offset so its even with the other worm in game
                        NPC.position.X = player.Center.X - 1600;
                        NPC.position.Y = player.Center.Y - 750;

                        for (int i = 150; i <= 750; i += 150)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), player.Center.X - i, player.Center.Y - 175, 0, 0,
                                ModContent.ProjectileType<TelegraphPurple>(), 0, 0f, 0);
                            }
                        }
                    }

                    if (NPC.localAI[0] == 200)
                    {
                        SoundEngine.PlaySound(GrowlSound, NPC.position);

                        NPC.velocity.X = 20;
                        NPC.velocity.Y *= 0;
                    }

                    if (NPC.localAI[0] > 200 && NPC.localAI[0] < 270)
                    {
                        if (Main.rand.Next(2) == 0)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, Main.rand.Next(-1, 1), 8, 
                                ModContent.ProjectileType<EyeSpit>(), Damage, 0f, Main.myPlayer, 0, 0);
                            }
                        }
                    }

                    if (NPC.localAI[0] == 275)
                    {
                        NPC.velocity *= 0.2f;
                    }

                    if (NPC.localAI[0] == 300)
                    {
                        NPC.velocity *= 0f;

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y + 450, 0, 0, 
                            ModContent.ProjectileType<TelegraphOrange>(), 0, 0f, Main.myPlayer, 0, 0);
                        }
                    }

                    if (NPC.localAI[0] == 350)
                    {
                        SoundEngine.PlaySound(GrowlSound, NPC.position);

                        NPC.velocity.X *= 0;
                        NPC.velocity.Y = 35;
                    }

                    if (NPC.localAI[0] >= 420)
                    {
                        NPC.velocity *= 0.5f;
                        NPC.localAI[0] = 0;
                        NPC.localAI[1] = 0;
                        NPC.ai[0]++;
                    }

                    break;
                }

                //circle and chase other worm
                case 6:
                {
                    NPC.localAI[0]++;

                    if (NPC.localAI[0] < 119)
                    {
                        Vector2 GoTo = player.Center;
                        GoTo.X -= 1000;
                        GoTo.Y += 750;

                        float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 12, 25);
                        NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
                    }
                    
                    //set exact position right before so it is even
                    if (NPC.localAI[0] == 119)
                    {
                        NPC.position.X = player.Center.X - 1000;
                        NPC.position.Y = player.Center.Y + 750;
                    }

                    if (NPC.localAI[0] == 120)
                    {
                        SoundEngine.PlaySound(GrowlSound, NPC.position);

                        Vector2 ChargeDirection = Main.player[NPC.target].Center - NPC.Center;
                        ChargeDirection.Normalize();
                                
                        ChargeDirection.X = ChargeDirection.X * 40;
                        ChargeDirection.Y = ChargeDirection.Y * 0;  
                        NPC.velocity.X = ChargeDirection.X;
                        NPC.velocity.Y = ChargeDirection.Y;
                    }

                    if (NPC.localAI[0] >= 151 && NPC.localAI[0] <= 500)
                    {
                        if (NPC.localAI[0] >= 250 && NPC.localAI[0] <= 380)
                        {
                            if (NPC.localAI[0] % 20 == 5)
                            {
                                Vector2 ShootSpeed = player.Center - NPC.Center;
                                ShootSpeed.Normalize();
                                ShootSpeed.X *= 2f;
                                ShootSpeed.Y *= 2f;

                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, ShootSpeed.X, ShootSpeed.Y, 
                                    ModContent.ProjectileType<EyeSpit>(), Damage, 1, Main.myPlayer, 0, 0);  
                                }
                            }

                            NPC.localAI[2] += 0.025f;
                        }

                        double angle = NPC.velocity.ToRotation();

                        //always subtract angle so it goes in a set circle
                        angle -= 3.5 * Math.PI;

                        NPC.localAI[1] = Math.Sign(angle);
                        NPC.velocity = Vector2.Normalize(NPC.velocity) * 50;

                        NPC.velocity = NPC.velocity.RotatedBy(MathHelper.ToRadians(3.5f + NPC.localAI[2]) * NPC.localAI[1]);
                    }

                    if (NPC.localAI[0] >= 500)
                    {
                        NPC.velocity *= 0.25f;
                        NPC.localAI[0] = 0;
                        NPC.localAI[1] = 0;
                        NPC.localAI[2] = 0;
                        NPC.ai[0] = 0;
                    }

                    break;
                }
            }

            return false;
        }

        private void ChaseMovement(Player player, float maxSpeed, float accel)
        {
            int minTilePosX = (int)(NPC.position.X / 16.0) - 1;
            int maxTilePosX = (int)((NPC.position.X + NPC.width) / 16.0) + 2;
            int minTilePosY = (int)(NPC.position.Y / 16.0) - 1;
            int maxTilePosY = (int)((NPC.position.Y + NPC.height) / 16.0) + 2;
            if (minTilePosX < 0)
            {
                minTilePosX = 0;
            }
            if (maxTilePosX > Main.maxTilesX)
            {
                maxTilePosX = Main.maxTilesX;
            }
            if (minTilePosY < 0)
            {
                minTilePosY = 0;
            }
            if (maxTilePosY > Main.maxTilesY)
            {
                maxTilePosY = Main.maxTilesY;
            }

            bool collision = false;
            float speed = maxSpeed;
            float acceleration = accel;

            if (!collision)
            {
                int maxDistance = 12;
                bool playerCollision = true;
                Rectangle rectangle1 = new Rectangle((int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height);
                
                for (int index = 0; index < 255; ++index)
                {
                    if (Main.player[index].active)
                    {
                        Rectangle rectangle2 = new Rectangle((int)Main.player[index].position.X - maxDistance, 
                        (int)Main.player[index].position.Y - maxDistance, maxDistance * 2, maxDistance * 2);
                        if (rectangle1.Intersects(rectangle2))
                        {
                            playerCollision = false;
                        
                            break;
                        }
                    }
                }

                if (playerCollision)
                {
                    collision = true;
                }
            }

            Vector2 NPCCenter = new Vector2(NPC.position.X + NPC.width * 0.5f, NPC.position.Y + NPC.height * 0.5f);
            float targetXPos = Main.player[NPC.target].position.X + (Main.player[NPC.target].width / 2);
            float targetYPos = Main.player[NPC.target].position.Y + (Main.player[NPC.target].height / 2);

            float targetRoundedPosX = (float)((int)(targetXPos / 16.0) * 16);
            float targetRoundedPosY = (float)((int)(targetYPos / 16.0) * 16);
            NPCCenter.X = (float)((int)(NPCCenter.X / 16.0) * 16);
            NPCCenter.Y = (float)((int)(NPCCenter.Y / 16.0) * 16);
            float dirX = targetRoundedPosX - NPCCenter.X;
            float dirY = targetRoundedPosY - NPCCenter.Y;
            Vector2 vector168 = NPC.Center;
            float length = (float)Math.Sqrt(dirX * dirX + dirY * dirY);
            
            if (!collision)
            {
                NPC.TargetClosest(true);

                if (NPC.velocity.Y > speed)
                {
                    NPC.velocity.Y = speed;
                }
                if (Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y) < speed * 0.4)
                {
                    if (NPC.velocity.X < 0.0)
                    {
                        NPC.velocity.X = NPC.velocity.X - acceleration * 1.1f;
                    }
                    else
                    {
                        NPC.velocity.X = NPC.velocity.X + acceleration * 1.1f;
                    }
                }

                else if (NPC.velocity.Y == speed)
                {
                    if (NPC.velocity.X < dirX)
                    {
                        NPC.velocity.X = NPC.velocity.X + acceleration;
                    }
                    else if (NPC.velocity.X > dirX)
                    {
                        NPC.velocity.X = NPC.velocity.X - acceleration;
                    }
                }
                else if (NPC.velocity.Y > 4.0)
                {
                    if (NPC.velocity.X < 0.0)
                    {
                        NPC.velocity.X = NPC.velocity.X + acceleration * 1f;
                    }
                    else
                    {
                        NPC.velocity.X = NPC.velocity.X - acceleration * 1f; 
                    }
                }
            }

            if (!collision)
            {
                NPC.TargetClosest(true);
                NPC.velocity.Y = NPC.velocity.Y + 0.11f;

                if (NPC.velocity.Y > speed)
                    NPC.velocity.Y = speed;
                if (Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y) < speed * 1.0) //was 0.5
                {
                    if (NPC.velocity.X < 0.0)
                        NPC.velocity.X = NPC.velocity.X - acceleration * 1.1f;
                    else
                        NPC.velocity.X = NPC.velocity.X + acceleration * 1.4f;
                }
                
                if (NPC.velocity.Y > 5.0) 
                {
                    if (NPC.velocity.X < 0.0)
                        NPC.velocity.X = NPC.velocity.X + acceleration * 0.9f;
                    else
                        NPC.velocity.X = NPC.velocity.X - acceleration * 0.9f;
                }
            }
            else if (collision)
            {
                float absDirX = Math.Abs(dirX);
                float absDirY = Math.Abs(dirY);
                float newSpeed = speed / length;
                dirX = dirX * newSpeed;
                dirY = dirY * newSpeed;
                if (NPC.velocity.X > 0.0 && dirX > 0.0 || NPC.velocity.X < 0.0 && dirX < 0.0 || (NPC.velocity.Y > 0.0 && dirY > 0.0 || NPC.velocity.Y < 0.0 && dirY < 0.0))
                {
                    if (NPC.velocity.X < dirX)
                        NPC.velocity.X = NPC.velocity.X + acceleration;
                    else if (NPC.velocity.X > dirX)
                        NPC.velocity.X = NPC.velocity.X - acceleration;
                    if (NPC.velocity.Y < dirY)
                        NPC.velocity.Y = NPC.velocity.Y + acceleration;
                    else if (NPC.velocity.Y > dirY)
                        NPC.velocity.Y = NPC.velocity.Y - acceleration;
                    if (Math.Abs(dirY) < speed * 0.2 && (NPC.velocity.X > 0.0 && dirX < 0.0 || NPC.velocity.X < 0.0 && dirX > 0.0))
                    {
                        if (NPC.velocity.Y > 0.0)
                            NPC.velocity.Y = NPC.velocity.Y + acceleration * 2f;
                        else
                            NPC.velocity.Y = NPC.velocity.Y - acceleration * 2f;
                    }
                    if (Math.Abs(dirX) < speed * 0.2 && (NPC.velocity.Y > 0.0 && dirY < 0.0 || NPC.velocity.Y < 0.0 && dirY > 0.0))
                    {
                        if (NPC.velocity.X > 0.0)
                            NPC.velocity.X = NPC.velocity.X + acceleration * 2f;
                        else
                            NPC.velocity.X = NPC.velocity.X - acceleration * 2f;
                    }
                }
                else if (absDirX > absDirY)
                {
                    if (NPC.velocity.X < dirX)
                        NPC.velocity.X = NPC.velocity.X + acceleration * 1.1f;
                    else if (NPC.velocity.X > dirX)
                        NPC.velocity.X = NPC.velocity.X - acceleration * 1.1f;
                    if (Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y) < speed * 0.5)
                    {
                        if (NPC.velocity.Y > 0.0)
                            NPC.velocity.Y = NPC.velocity.Y + acceleration;
                        else
                            NPC.velocity.Y = NPC.velocity.Y - acceleration;
                    }
                }
                else
                {
                    if (NPC.velocity.Y < dirY)
                        NPC.velocity.Y = NPC.velocity.Y + acceleration * 1.1f;
                    else if (NPC.velocity.Y > dirY)
                        NPC.velocity.Y = NPC.velocity.Y - acceleration * 1.1f;
                    if (Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y) < speed * 0.5)
                    {
                        if (NPC.velocity.X > 0.0)
                            NPC.velocity.X = NPC.velocity.X + acceleration;
                        else
                            NPC.velocity.X = NPC.velocity.X - acceleration;
                    }
                }
            }

            //Some netupdate stuff
            if (collision)
            {
                if (NPC.ai[2] != 1)
                {
                    NPC.netUpdate = true;
                }
                NPC.ai[2] = 1f;
            }
            else
            {
                if (NPC.ai[2] != 0.0)
                {
                    NPC.netUpdate = true;
                }
                NPC.ai[2] = 0.0f;
            }
            if ((NPC.velocity.X > 0.0 && NPC.oldVelocity.X < 0.0 || NPC.velocity.X < 0.0 && NPC.oldVelocity.X > 0.0 || 
            (NPC.velocity.Y > 0.0 && NPC.oldVelocity.Y < 0.0 || NPC.velocity.Y < 0.0 && NPC.oldVelocity.Y > 0.0)) && !NPC.justHit)
            {
                NPC.netUpdate = true;
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());
            
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<BossBagOrroboro>()));

            int[] MainItem = new int[] { ModContent.ItemType<EyeFlail>(), ModContent.ItemType<EyeRocketLauncher>(), 
            ModContent.ItemType<LeechStaff>(), ModContent.ItemType<MouthFlamethrower>(), ModContent.ItemType<Scycler>() };

            notExpertRule.OnSuccess(ItemDropRule.Common(MainItem[Main.rand.Next(5)]));

            if (Main.rand.Next(5) == 0)
            {
                notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<OrroboroHeart>()));
            }

            int itemType = ModContent.ItemType<MonsterChunk>();
            var parameters = new DropOneByOne.Parameters() 
            {
                ChanceNumerator = 1,
                ChanceDenominator = 1,
                MinimumStackPerChunkBase = 1,
                MaximumStackPerChunkBase = 1,
                MinimumItemDropsCount = 12,
                MaximumItemDropsCount = 25,
            };

			notExpertRule.OnSuccess(new DropOneByOne(itemType, parameters));
        }

        public override void OnKill()
        {
            NPC.SetEventFlagCleared(ref Flags.downedOrroboro, -1);
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.GreaterHealingPotion;
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            scale = 1.2f;
            return null;
        }
    }
}