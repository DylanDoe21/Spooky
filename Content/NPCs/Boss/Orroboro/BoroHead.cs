using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
using Terraria.DataStructures;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Items.BossBags;
using Spooky.Content.Items.Pets;
using Spooky.Content.Items.SpookyHell.Boss;
using Spooky.Content.NPCs.Boss.Orroboro.Projectiles;
using Spooky.Content.Tiles.Relic;

namespace Spooky.Content.NPCs.Boss.Orroboro
{
    //[AutoloadBossHead]
    public class BoroHead : ModNPC
    {
        Vector2 SavePoint;
        public bool Enraged = false;
        private bool spawned;

        public static readonly SoundStyle GrowlSound = new("Spooky/Content/Sounds/SpookyHell/OrroboroGrowl2", SoundType.Sound);
        public static readonly SoundStyle HitSound = new("Spooky/Content/Sounds/SpookyHell/EnemyHit", SoundType.Sound);
        public static readonly SoundStyle DeathSound = new("Spooky/Content/Sounds/SpookyHell/OrroboroDeath", SoundType.Sound);

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Boro");

            var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                CustomTexturePath = "Spooky/Content/NPCs/Boss/Orroboro/BoroBestiary",
                Position = new Vector2(2f, -24f),
                PortraitPositionXOverride = 0f,
                PortraitPositionYOverride = -24f
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifier);

            NPCDebuffImmunityData debuffData = new NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[] 
                {
                    BuffID.Confused
                }
            };
            NPCID.Sets.DebuffImmunitySets.Add(Type, debuffData);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            //bools
            writer.Write(Enraged);
            writer.Write(spawned);

            //local ai
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
            writer.Write(NPC.localAI[2]);
            writer.Write(NPC.localAI[3]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            //bools
            Enraged = reader.ReadBoolean();
            spawned = reader.ReadBoolean();

            //local ai
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            NPC.localAI[2] = reader.ReadSingle();
            NPC.localAI[3] = reader.ReadSingle();
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = Main.masterMode ? 18000 / 3 : Main.expertMode ? 14500 / 2 : 10000;
            NPC.damage = 55;
            NPC.defense = 30;
            NPC.width = 54;
            NPC.height = 54;
            NPC.npcSlots = 25f;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 12, 0, 0);
            NPC.boss = true;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.behindTiles = true;
            NPC.netAlways = true;
            NPC.HitSound = HitSound;
            NPC.DeathSound = DeathSound;
            NPC.aiStyle = -1;
            Music = MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/Orroboro");
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Content.Biomes.SpookyHellBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("A blind and aggressive serpent that works together with Orro to defend it's territory. Stories say that valley of eyes is made from the flesh these serpents have torn off each other."),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyHellBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        //rotate the bosses map icon to the NPCs direction
        public override void BossHeadRotation(ref float rotation)
        {
            rotation = NPC.rotation;
        }

        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
			float Divide = 1.8f;

			if (projectile.penetrate <= -1)
			{
				damage /= (int)Divide;
			}
			else if (projectile.penetrate >= 3)
			{
				damage /= (int)Divide;
			}
		}

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            //if orro is in its enraged state
            if (Enraged)
            {
                float fade = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 0.5f / 2.5f * 150f)) / 2f + 0.5f;

                Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;

                Vector2 vector = new Vector2(NPC.Center.X, NPC.Center.Y) + (6.28318548f + NPC.rotation + 0f).ToRotationVector2() * (4f * fade + 2f) - Main.screenPosition + new Vector2(0, NPC.gfxOffY) - NPC.velocity;
                Main.EntitySpriteDraw(tex, vector, NPC.frame, Color.Purple * 0.75f, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale * 1.5f, SpriteEffects.None, 0);
            }

            Texture2D texture =  ModContent.Request<Texture2D>(Texture).Value;
            Rectangle frame = new Rectangle(0, NPC.frame.Y, texture.Width, texture.Height / Main.npcFrameCount[NPC.type]);
            Vector2 origin = new Vector2(texture.Width * 0.5f, texture.Height * 0.5f);
            Main.spriteBatch.Draw(texture, NPC.Center - Main.screenPosition, frame, drawColor, NPC.rotation, origin, NPC.scale, SpriteEffects.None, 0);

            return false;
        }

        public override void AI()
        {
            NPC Orro = Main.npc[(int)NPC.ai[1]];

            Player player = Main.player[NPC.target];
            NPC.TargetClosest(true);

            int Damage = Main.masterMode ? 100 / 3 : Main.expertMode ? 80 / 2 : 50;

            NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X) + 1.57f;

            Enraged = !NPC.AnyNPCs(ModContent.NPCType<OrroHeadP2>());

            //despawn if all players are dead or not in the biome
            if (player.dead || !player.InModBiome(ModContent.GetInstance<Biomes.SpookyHellBiome>()))
            {
                NPC.localAI[3]++;
                if (NPC.localAI[3] >= 75)
                {
                    NPC.velocity.Y = 25;
                    NPC.EncourageDespawn(10);
                }
            }
            else
            {
                NPC.localAI[3] = 0;
            }

            //Make the worm itself
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (!spawned)
                {
                    NPC.realLife = NPC.whoAmI;
                    int LatestNPC = NPC.whoAmI;

                    //spawn the 4 boro segments
                    LatestNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + (NPC.width / 2), (int)NPC.Center.Y + (NPC.height / 2), 
                    ModContent.NPCType<BoroBody1>(), NPC.whoAmI, 0, LatestNPC);                   
                    Main.npc[LatestNPC].realLife = NPC.whoAmI;
                    Main.npc[LatestNPC].ai[3] = NPC.whoAmI;
                    Main.npc[LatestNPC].netUpdate = true;
                    LatestNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + (NPC.width / 2), (int)NPC.Center.Y + (NPC.height / 2), 
                    ModContent.NPCType<BoroBody2>(), NPC.whoAmI, 0, LatestNPC);                   
                    Main.npc[LatestNPC].realLife = NPC.whoAmI;
                    Main.npc[LatestNPC].ai[3] = NPC.whoAmI;
                    Main.npc[LatestNPC].netUpdate = true;
                    LatestNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + (NPC.width / 2), (int)NPC.Center.Y + (NPC.height / 2), 
                    ModContent.NPCType<BoroBody1>(), NPC.whoAmI, 0, LatestNPC);                   
                    Main.npc[LatestNPC].realLife = NPC.whoAmI;
                    Main.npc[LatestNPC].ai[3] = NPC.whoAmI;
                    Main.npc[LatestNPC].netUpdate = true;
                    LatestNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + (NPC.width / 2), (int)NPC.Center.Y + (NPC.height / 2), 
                    ModContent.NPCType<BoroBody2>(), NPC.whoAmI, 0, LatestNPC);                   
                    Main.npc[LatestNPC].realLife = NPC.whoAmI;
                    Main.npc[LatestNPC].ai[3] = NPC.whoAmI;
                    Main.npc[LatestNPC].netUpdate = true;

                    //spawn tail
                    LatestNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + (NPC.width / 2), (int)NPC.Center.Y + (NPC.height / 2), 
                    ModContent.NPCType<BoroTail>(), NPC.whoAmI, 0, LatestNPC);                   
                    Main.npc[LatestNPC].realLife = NPC.whoAmI;
                    Main.npc[LatestNPC].ai[3] = NPC.whoAmI;
                    Main.npc[LatestNPC].netUpdate = true;

                    NPC.netUpdate = true;
                    spawned = true;
                }
            }

            if (!player.dead && player.InModBiome(ModContent.GetInstance<Biomes.SpookyHellBiome>()) && NPC.localAI[3] < 75)
            {
                //attacks
                switch ((int)NPC.ai[0])
                {
                    //charge from the sides while orro chases
                    case 0:
                    {
                        NPC.localAI[0]++;

                        int repeats = Enraged ? 4 : 3;
                        if (NPC.localAI[1] < repeats)
                        {
                            int positionTime = Enraged ? 50 : 60;
                            if (NPC.localAI[0] < positionTime)
                            {
                                Vector2 GoTo = player.Center;
                                GoTo.X += (NPC.Center.X < player.Center.X) ? -1200 : 1200;
                                GoTo.Y -= 0;

                                float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 15, 30);
                                NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
                            }

                            if (NPC.localAI[0] == positionTime)
                            {
                                NPC.velocity *= 0;

                                NPC.position.X = (NPC.Center.X < player.Center.X) ? player.Center.X - 1200 : player.Center.X + 1200;
                                NPC.position.Y = player.Center.Y;

                                Projectile.NewProjectile(NPC.GetSource_FromThis(), (NPC.Center.X < player.Center.X) ? player.Center.X - 400 : player.Center.X + 400, 
                                player.Center.Y, 0, 0, ModContent.ProjectileType<TelegraphRed>(), 0, 0f, 0);
                            }

                            int chargeTime = Enraged ? 65 : 75;
                            if (NPC.localAI[0] == chargeTime)
                            {
                                SoundEngine.PlaySound(GrowlSound, NPC.Center);

                                Vector2 ChargeDirection = player.Center - NPC.Center;
                                ChargeDirection.Normalize();
                                        
                                ChargeDirection.X *= Enraged ? 48 : 40;
                                ChargeDirection.Y *= 0;
                                NPC.velocity.X = ChargeDirection.X;
                                NPC.velocity.Y = ChargeDirection.Y;
                            }

                            int stopTime = Enraged ? 85 : 100;
                            if (NPC.localAI[0] > stopTime)
                            {
                                int check = Enraged ? 4 : 2;
                                if (NPC.localAI[1] == check)
                                {
                                    NPC.velocity *= 0.95f; 
                                }
                                else
                                {
                                    NPC.velocity *= 0.99f;
                                }
                            }

                            int extraTime = Enraged ? 15 : 45;
                            if (NPC.localAI[0] >= stopTime + extraTime)
                            {
                                NPC.localAI[0] = 0;
                                NPC.localAI[1]++;
                                NPC.netUpdate = true;
                            }
                        }
                        else
                        {
                            NPC.velocity *= 0.5f;
                            NPC.localAI[0] = 0;
                            NPC.localAI[1] = 0;
                            NPC.ai[0]++;
                            NPC.netUpdate = true;
                        }
                        
                        break;
                    }

                    //charge at the player 
                    case 1:
                    {
                        NPC.localAI[0]++;
                        
                        NPC.velocity *= 0.975f;

                        if (NPC.localAI[1] < 3)
                        {
                            int time1 = Enraged ? 45 : 45;
                            int time2 = Enraged ? 80 : 95;
                            int time3 = Enraged ? 115 : 145;
                            if (NPC.localAI[0] == time1 || NPC.localAI[0] == time2 || NPC.localAI[0] == time3)
                            {
                                SoundEngine.PlaySound(GrowlSound, NPC.Center);

                                Vector2 ChargeDirection = player.Center - NPC.Center;
                                ChargeDirection.Normalize();
                                        
                                ChargeDirection.X *= (Enraged ? 32 : 23) + Main.rand.Next(-5, 5);
                                ChargeDirection.Y *= (Enraged ? 32 : 23) + Main.rand.Next(-5, 5);
                                NPC.velocity.X = ChargeDirection.X;
                                NPC.velocity.Y = ChargeDirection.Y;
                            }

                            if (NPC.localAI[0] > time3)
                            {
                                NPC.localAI[0] = 0;
                                NPC.localAI[1]++;
                                NPC.netUpdate = true;
                            }
                        }
                        else
                        {
                            if (NPC.localAI[0] >= 60)
                            {
                                NPC.localAI[0] = 0;
                                NPC.localAI[1] = 0;
                                NPC.ai[0]++;
                                NPC.netUpdate = true;
                            }
                        }

                        break;
                    }

                    //go to players side, charge and then curve and spit biomass in sync with orro
                    case 2:
                    {
                        NPC.localAI[0]++;
                        
                        if (NPC.localAI[0] < 75)
                        {
                            //this is slightly offset so its even with the other worm in game
                            Vector2 GoTo = player.Center;
                            GoTo.X += 1200;
                            GoTo.Y += 0;

                            float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 18, 42);
                            NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
                        }
                        
                        //set exact position right before so the curling up is always even
                        if (NPC.localAI[0] == 75)
                        {
                            NPC.velocity *= 0;

                            //this is slightly offset so its even with the other worm in game
                            NPC.position.X = player.Center.X + 1200;
                            NPC.position.Y = player.Center.Y + 0;

                            Projectile.NewProjectile(NPC.GetSource_FromThis(), player.Center.X + 550, player.Center.Y, 0, 0,
                            ModContent.ProjectileType<TelegraphRed>(), 0, 0f, 0);
                        }

                        if (NPC.localAI[0] == 90)
                        {
                            SoundEngine.PlaySound(GrowlSound, NPC.Center);

                            NPC.velocity.X = Enraged ? -50 : -42;
                            NPC.velocity.Y *= 0;
                        }

                        if (NPC.localAI[0] >= 125 && NPC.localAI[0] <= 170)
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

                            if (NPC.localAI[0] == 130 || NPC.localAI[0] == 140 || NPC.localAI[0] == 150 || NPC.localAI[0] == 160 || NPC.localAI[0] == 170)
                            {
                                Vector2 ShootSpeed = player.Center - NPC.Center;
                                ShootSpeed.Normalize();
                                ShootSpeed.X *= Enraged ? 5f : 3f;
                                ShootSpeed.Y *= Enraged ? 5f : 3f;

                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, ShootSpeed.X, ShootSpeed.Y, 
                                    ModContent.ProjectileType<BoroBiomatter>(), Damage, 1, Main.myPlayer, 0, 0);  
                                }
                            }
                        }

                        if (NPC.localAI[0] == 170)
                        {
                            NPC.velocity *= 0.25f;
                        }

                        if (NPC.localAI[0] > 270)
                        {
                            NPC.localAI[0] = 0;
                            NPC.localAI[1] = 0;
                            NPC.ai[0]++;
                            NPC.netUpdate = true;
                        }

                        break;
                    }

                    //use tentacle pillars while orro spits acid and chases
                    case 3:
                    {
                        NPC.localAI[0]++;

                        int repeats = Enraged ? 2 : 3;
                        if (NPC.localAI[1] < repeats)
                        {
                            Vector2 GoTo = player.Center;
                            GoTo.X += 0;
                            GoTo.Y += 600;

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

                            if (((!Enraged && NPC.localAI[0] % 60 == 20) || (Enraged && NPC.localAI[0] % 20 == 5)) && NPC.localAI[1] > 0)
                            {
                                for (int j = 0; j <= 0; j++)
                                {
                                    for (int i = 0; i <= 0; i += 1) 
                                    {
                                        Vector2 center = new(NPC.Center.X, player.Center.Y + player.height / 4);
                                        center.X += j * Main.rand.Next(150, 220) * i; //distance between each spike
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
                                            Projectile.NewProjectile(NPC.GetSource_FromThis(), center.X, center.Y + 20, 0, 0, 
                                            ModContent.ProjectileType<ThornTelegraph>(), Damage, 1, Main.myPlayer, 0, 0);
                                        }
                                    }
                                }
                            }

                            if (NPC.localAI[0] >= 240)
                            {
                                NPC.localAI[0] = 0;
                                NPC.localAI[1]++;
                                NPC.netUpdate = true;
                            }
                        }
                        else
                        {
                            NPC.localAI[0] = 0;
                            NPC.localAI[1] = 0; 
                            NPC.ai[0]++; 
                            NPC.netUpdate = true;
                        }

                        break;
                    }

                    //charge and shoot venom breath while orro drops venom spit
                    case 4:
                    {
                        NPC.localAI[0]++;
                        NPC.velocity *= 0.97f;

                        if (NPC.localAI[1] < 3)
                        {
                            int time1 = Enraged ? 60 : 80;
                            int time2 = Enraged ? 120 : 160;
                            int time3 = Enraged ? 180 : 240;
                            if (NPC.localAI[0] == time1 || NPC.localAI[0] == time2 || NPC.localAI[0] == time3)
                            {
                                Vector2 CenterPoint = player.Center;

                                Projectile.NewProjectile(NPC.GetSource_FromThis(), CenterPoint.X, CenterPoint.Y, 0, 0, 
                                ModContent.ProjectileType<AcidTarget>(), 0, 0f, 0);
                                
                                //use SavePoint to save where the telegraph was
                                SavePoint = CenterPoint;
                            }

                            //charge towards where the telegraphs saved point is
                            if (NPC.localAI[0] == time1 + 15 || NPC.localAI[0] == time2 + 15 || NPC.localAI[0] == time3 + 15)
                            {
                                SoundEngine.PlaySound(GrowlSound, NPC.Center);
                                SoundEngine.PlaySound(SoundID.DD2_BetsyFlameBreath, NPC.Center);

                                Vector2 ChargeDirection = SavePoint - NPC.Center;
                                ChargeDirection.Normalize();
                                        
                                ChargeDirection.X *= Enraged ? 35 : 28;
                                ChargeDirection.Y *= Enraged ? 35 : 28; 
                                NPC.velocity.X = ChargeDirection.X;
                                NPC.velocity.Y = ChargeDirection.Y;
                            }
                            
                            if ((NPC.localAI[0] > time1 + 15 && NPC.localAI[0] < time1 + 35) || (NPC.localAI[0] > time2 + 15 && NPC.localAI[0] < time2 + 35) ||
                            (NPC.localAI[0] > time3 + 15 && NPC.localAI[0] < time3 + 35))
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X + (NPC.velocity.X / 3), NPC.Center.Y + (NPC.velocity.Y / 3), 
                                NPC.velocity.X * 0.5f + Main.rand.NextFloat(-0.2f, 0.2f) * 1, NPC.velocity.Y * 0.5f + Main.rand.NextFloat(-0.2f, 0.2f) * 1, 
                                ModContent.ProjectileType<AcidBreath>(), Damage, 0f, 0);
                            }

                            if (NPC.localAI[0] >= time3 + 35)
                            {
                                NPC.localAI[0] = 20;
                                NPC.localAI[1]++;
                                NPC.netUpdate = true;
                            }
                        }
                        else
                        {
                            NPC.velocity *= 0.25f;
                            NPC.localAI[0] = 0;
                            NPC.localAI[1] = 0; 
                            NPC.ai[0]++;
                            NPC.netUpdate = true;
                        }
                        
                        break;
                    }

                    //circle and chase other worm
                    case 5:
                    {
                        NPC.localAI[0]++;

                        if (NPC.localAI[0] < 119)
                        {
                            Vector2 GoTo = player.Center;
                            GoTo.X += 1000;
                            GoTo.Y -= 750;

                            float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 18, 42);
                            NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
                        }
                        
                        //set exact position right before so it is even
                        if (NPC.localAI[0] == 119)
                        {
                            NPC.velocity *= 0;

                            NPC.position.X = player.Center.X + 1000;
                            NPC.position.Y = player.Center.Y - 750;
                        }

                        if (NPC.localAI[0] == 120)
                        {
                            SoundEngine.PlaySound(GrowlSound, NPC.position);

                            Vector2 ChargeDirection = player.Center - NPC.Center;
                            ChargeDirection.Normalize();
                                    
                            ChargeDirection.X *= Enraged ? 45 : 40;
                            ChargeDirection.Y *= 0;  
                            NPC.velocity.X = ChargeDirection.X;
                            NPC.velocity.Y = ChargeDirection.Y;
                        }

                        if (NPC.localAI[0] >= 151 && NPC.localAI[0] <= 500)
                        {
                            if (NPC.localAI[0] >= 250 && NPC.localAI[0] <= 380)
                            {
                                int frequency = Enraged ? 5 : 8;

                                if (NPC.localAI[0] % 20 == frequency)
                                {
                                    Vector2 ShootSpeed = player.Center - NPC.Center;
                                    ShootSpeed.Normalize();
                                    ShootSpeed.X *= Enraged ? 3f : 2f;
                                    ShootSpeed.Y *= Enraged ? 3f : 2f;

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
                            NPC.netUpdate = true;
                        }

                        break;
                    }
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBagByCondition(new ShouldBoroDropLootExpert(), ModContent.ItemType<BossBagBoro>()));

            npcLoot.Add(ItemDropRule.ByCondition(new ShouldBoroDropLootMaster(), ModContent.ItemType<OrroboroEye>(), 4));
            npcLoot.Add(ItemDropRule.ByCondition(new ShouldBoroDropLootMaster(), ModContent.ItemType<OrroboroRelicItem>()));

            int[] MainItem = new int[] { ModContent.ItemType<EyeFlail>(), ModContent.ItemType<Scycler>(),
            ModContent.ItemType<EyeRocketLauncher>(), ModContent.ItemType<MouthFlamethrower>(),
            ModContent.ItemType<LeechStaff>(), ModContent.ItemType<LeechWhip>() };

            npcLoot.Add(ItemDropRule.ByCondition(new ShouldBoroDropLoot(), Main.rand.Next(MainItem)));

            npcLoot.Add(ItemDropRule.ByCondition(new ShouldBoroDropLoot(), ModContent.ItemType<OrroboroChunk>(), 1, 12, 25));
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

    //i love having to make custom drop rules just so the first worm doesnt drop anything
    //for non expert drops
    public class ShouldBoroDropLoot : IItemDropRuleCondition
    {
        public bool CanDrop(DropAttemptInfo info)
        {
            if (!info.IsInSimulation)
            {
                if (!NPC.AnyNPCs(ModContent.NPCType<OrroHeadP2>()) && !Main.expertMode)
                {
                    return true;
                }
            }

            return false;
        }

        public bool CanShowItemDropInUI()
        {
            return true;
        }

        public string GetConditionDescription()
        {
            return "Drops from Boro";
        }
    }

    //for expert drops
    public class ShouldBoroDropLootExpert : IItemDropRuleCondition
    {
        public bool CanDrop(DropAttemptInfo info)
        {
            if (!info.IsInSimulation)
            {
                if (!NPC.AnyNPCs(ModContent.NPCType<OrroHeadP2>()) && Main.expertMode)
                {
                    return true;
                }
            }

            return false;
        }

        public bool CanShowItemDropInUI()
        {
            return true;
        }

        public string GetConditionDescription()
        {
            return "Drops from Boro";
        }
    }

    //for master drops
    public class ShouldBoroDropLootMaster : IItemDropRuleCondition
    {
        public bool CanDrop(DropAttemptInfo info)
        {
            if (!info.IsInSimulation)
            {
                if (!NPC.AnyNPCs(ModContent.NPCType<OrroHeadP2>()) && Main.masterMode)
                {
                    return true;
                }
            }

            return false;
        }

        public bool CanShowItemDropInUI()
        {
            return true;
        }

        public string GetConditionDescription()
        {
            return "Drops from Boro";
        }
    }
}