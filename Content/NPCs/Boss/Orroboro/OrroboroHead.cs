using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System;

using Spooky.Content.NPCs.Boss.Orroboro.Projectiles;
using Spooky.Content.NPCs.Boss.Orroboro.Phase2;

namespace Spooky.Content.NPCs.Boss.Orroboro
{
    [AutoloadBossHead]
    public class OrroboroHead : ModNPC
    {
        //bools for animation
        public bool Chomp = false;
        public bool OpenMouth = false;
        private bool spawned;

        public static readonly SoundStyle CrunchSound = new("Spooky/Content/Sounds/OrroboroCrunch", SoundType.Sound);
        public static readonly SoundStyle GrowlSound = new("Spooky/Content/Sounds/OrroboroGrowl1", SoundType.Sound);
        public static readonly SoundStyle SplitSound = new("Spooky/Content/Sounds/OrroboroSplit", SoundType.Sound);
        public static readonly SoundStyle SplitRoarSound = new("Spooky/Content/Sounds/OrroboroSplitRoar", SoundType.Sound);

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Orro-Boro");
            Main.npcFrameCount[NPC.type] = 5;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = Main.masterMode ? 50000 / 3 : Main.expertMode ? 45000 / 2 : 35000;
            NPC.damage = 60;
            NPC.defense = 35;
            NPC.width = 62;
            NPC.height = 62;
            NPC.npcSlots = 25f;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 5, 0, 0);
            NPC.boss = true;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.behindTiles = true;
            NPC.netAlways = true;
            NPC.HitSound = SoundID.NPCHit9;
            NPC.aiStyle = -1;
            Music = MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/Orroboro");
        }

        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
            float Divide = 1.5f;

			if (projectile.penetrate <= -1)
			{
				damage /= (int)Divide;
			}
			else if (projectile.penetrate >= 3)
			{
				damage /= (int)Divide;
			}
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
                    SoundEngine.PlaySound(CrunchSound, NPC.Center);

                    NPC.frame.Y = 0;
                }
            }
        }
        
        public override bool PreAI()
        {
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(true);

            int Damage = Main.expertMode ? 28 : 40;

            NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X) + 1.57f;

            //despawn if all players are dead or not in the biome
            if (player.dead || !player.InModBiome(ModContent.GetInstance<Content.Biomes.SpookyHellBiome>()))
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

            //Create the worm itself
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (!spawned)
                {
                    NPC.realLife = NPC.whoAmI;
                    int latestNPC = NPC.whoAmI;

                    for (int Segment1 = 0; Segment1 < 2; Segment1++)
                    {
                        latestNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + (NPC.width / 2), (int)NPC.Center.Y + (NPC.height / 2), ModContent.NPCType<OrroboroBody1>(), NPC.whoAmI, 0, latestNPC);                   
                        Main.npc[latestNPC].realLife = NPC.whoAmI;
                        Main.npc[latestNPC].ai[3] = NPC.whoAmI;
                        Main.npc[latestNPC].netUpdate = true;
                    }
                    
                    latestNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + (NPC.width / 2), (int)NPC.Center.Y + (NPC.height / 2), ModContent.NPCType<OrroboroBody2>(), NPC.whoAmI, 0, latestNPC);                   
                    Main.npc[latestNPC].realLife = NPC.whoAmI;
                    Main.npc[latestNPC].ai[3] = NPC.whoAmI;
                    Main.npc[latestNPC].netUpdate = true;

                    for (int Segment2 = 0; Segment2 < 2; Segment2++)
                    {
                        latestNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + (NPC.width / 2), (int)NPC.Center.Y + (NPC.height / 2), ModContent.NPCType<OrroboroBody1>(), NPC.whoAmI, 0, latestNPC);                   
                        Main.npc[latestNPC].realLife = NPC.whoAmI;
                        Main.npc[latestNPC].ai[3] = NPC.whoAmI;
                        Main.npc[latestNPC].netUpdate = true;
                    }

                    latestNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + (NPC.width / 2), (int)NPC.Center.Y + (NPC.height / 2), ModContent.NPCType<OrroboroTail>(), NPC.whoAmI, 0, latestNPC);
                    Main.npc[latestNPC].realLife = NPC.whoAmI;
                    Main.npc[latestNPC].ai[3] = NPC.whoAmI;
                    Main.npc[latestNPC].netUpdate = true;

                    NPC.netUpdate = true;
                    spawned = true;
                }
            }

            //splitting transition
            if (NPC.life <= NPC.lifeMax / 2)
			{
                //set ai to -1 to not run during transition
                NPC.ai[0] = -1;

                NPC.immortal = true;
                NPC.dontTakeDamage = true;
                NPC.netUpdate = true;
                NPC.velocity *= 0.97f;

                OpenMouth = true; //mouth open during split transition
                Chomp = false; //set chomping anim to false to prevent animation conflictions

                NPC.ai[2]++;
                
                if (NPC.ai[2] == 2)
                {
                    SoundEngine.PlaySound(SplitRoarSound, NPC.Center);
                }
                
                if (NPC.ai[2] < 180)
                {
                    NPC.Center = new Vector2(NPC.Center.X, NPC.Center.Y);
                    NPC.Center += Main.rand.NextVector2Square(-2, 2);
                }

                if (NPC.ai[2] >= 180)
                {
                    SoundEngine.PlaySound(SplitSound, NPC.Center);

                    int Orro = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<OrroHead>());

                    //net update so the worms dont vanish on multiplayer
                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, Orro);
                    }

                    NPC.active = false;
                }
            }

            //attacks
            switch ((int)NPC.ai[0])
            {
                //basic movement
                case 0:
                {
                    NPC.localAI[0]++;

                    Movement(player, 18f, 0.38f, false);
                        
                    if (NPC.localAI[0] > 500)
                    {
                        NPC.localAI[0] = 0;
                        NPC.ai[0]++;
                    }

                    break;
                }

                //chase the player directly
                case 1:
                {
                    NPC.localAI[0]++;

                    Chomp = true;

                    //use chase movement
                    Movement(player, 8f, 0.20f, true);

                    if (NPC.localAI[0] > 600)
                    {
                        Chomp = false;
                        NPC.localAI[0] = 0;
                        NPC.ai[0]++;
                    }

                    break;
                }

                //go below player, dash up, then curve back down
                case 2:
                {
                    NPC.localAI[0]++;
                    if (NPC.localAI[0] < 100)
                    {
                        Vector2 GoTo = player.Center;
                        GoTo.X += 0;
                        GoTo.Y += 750;

                        float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 12, 25);
                        NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
                    }

                    if (NPC.localAI[0] == 100)
                    {
                        NPC.velocity *= 0.02f;

                        NPC.position.X = player.Center.X;
                        NPC.position.Y = player.Center.Y + 750;

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), player.Center.X, player.Center.Y + 225, 0, 0,
                            ModContent.ProjectileType<TelegraphRed>(), 0, 0f, 0);
                        }
                    }

                    if (NPC.localAI[0] == 120)
                    {
                        SoundEngine.PlaySound(GrowlSound, NPC.Center);

                        Vector2 ChargeDirection = player.Center - NPC.Center;
                        ChargeDirection.Normalize();
                                
                        ChargeDirection.X *= 0;
                        ChargeDirection.Y *= 35;  
                        NPC.velocity.X = ChargeDirection.X;
                        NPC.velocity.Y = ChargeDirection.Y;
                    }

                    if (NPC.localAI[0] >= 120 && NPC.localAI[0] <= 200)
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

                        if (Math.Abs(angle) > Math.PI / 2) //passed player, turn around
                        {
                            NPC.localAI[1] = Math.Sign(angle);
                            NPC.velocity = Vector2.Normalize(NPC.velocity) * 22;
                        }

                        NPC.velocity = NPC.velocity.RotatedBy(MathHelper.ToRadians(3.2f) * NPC.localAI[1]);
                    }   

                    if (NPC.localAI[0] > 230)
                    {
                        NPC.localAI[0] = 0;
                        NPC.localAI[1] = 0;
                        NPC.ai[0]++;
                    }

                    break;
                }

                //fly around and shoot toxic spit around when near player
                case 3:
                {
                    NPC.localAI[0]++;

                    Movement(player, 16f, 0.36f, false);

                    //Shoot toxic spit when nearby the player
                    if (NPC.Distance(player.Center) <= 450f) 
                    {
                        OpenMouth = true;
                        
                        if (Main.rand.Next(7) == 0)
                        {
                            Vector2 position = new(NPC.Center.X + (NPC.width / 2), NPC.Center.Y + (NPC.height / 2));  

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), position.X, position.Y, NPC.velocity.X * 0.2f + Main.rand.NextFloat(-5f, 5f) * 1, 
                                NPC.velocity.Y * 0.2f + Main.rand.NextFloat(-5f, 5f) * 1, ModContent.ProjectileType<EyeSpit>(), Damage, 0f, 0);
                            }

                            if (Main.rand.Next(2) == 0)
                            {
                                SoundEngine.PlaySound(SoundID.NPCDeath13, NPC.Center);
                            }
                        }
                    }
                    else
                    {
                        OpenMouth = false;
                    }

                    if (NPC.localAI[0] >= 450)
                    {
                        OpenMouth = false;
                        NPC.localAI[0] = 0;
                        NPC.ai[0]++;
                    }

                    break;
                }

                //go to the side of player, dash and circle the player
                case 4:
                {
                    NPC.localAI[0]++;

                    if (NPC.localAI[0] <= 100)
                    {
                        Vector2 GoTo = player.Center;
                        GoTo.X += (NPC.Center.X < player.Center.X) ? -1300 : 1300;
                        GoTo.Y -= 400;

                        float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 12, 25);
                        NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
                    }

                    if (NPC.localAI[0] == 100)
                    {
                        NPC.velocity *= 0.02f;

                        NPC.position.X = (NPC.Center.X < player.Center.X) ? player.Center.X - 1300 : player.Center.X + 1300;
                        NPC.position.Y = player.Center.Y - 400;

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), player.Center.X, player.Center.Y - 400, 0, 0,
                            ModContent.ProjectileType<TelegraphRed>(), 0, 0f, 0);
                        }
                    }

                    if (NPC.localAI[0] == 120)
                    {
                        SoundEngine.PlaySound(GrowlSound, NPC.position);

                        Vector2 ChargeDirection = player.Center - NPC.Center;
                        ChargeDirection.Normalize();
                                
                        ChargeDirection.X *= 45;
                        ChargeDirection.Y *= 0;  
                        NPC.velocity.X = ChargeDirection.X;
                        NPC.velocity.Y = ChargeDirection.Y;
                    }

                    if (NPC.localAI[0] >= 120 && NPC.localAI[0] <= 260)
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

                        if (Math.Abs(angle) > Math.PI / 2) //passed player, turn around
                        {
                            NPC.localAI[1] = Math.Sign(angle);
                            NPC.velocity = Vector2.Normalize(NPC.velocity) * 35;
                        }

                        NPC.velocity = NPC.velocity.RotatedBy(MathHelper.ToRadians(5f) * NPC.localAI[1]);

                        if (NPC.localAI[0] % 20 == 5)
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

                    if (NPC.localAI[0] > 260)
                    {
                        NPC.velocity *= 0.5f;
                        NPC.localAI[0] = 0;
                        NPC.localAI[1] = 0;
                        NPC.ai[0]++;
                    }

                    break;
                }

                //go below player, charge up, and tooth balls upward
                case 5:
                {
                    NPC.localAI[0]++;
                        
                    if (NPC.localAI[0] < 80)
                    {
                        Vector2 GoTo = player.Center;
                        GoTo.X += 0;
                        GoTo.Y += 750;

                        float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 18, 25);
                        NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
                    }

                    if (NPC.localAI[0] == 80)
                    {
                        NPC.velocity *= 0.02f;

                        NPC.position.X = player.Center.X;
                        NPC.position.Y = player.Center.Y + 750;

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), player.Center.X, player.Center.Y + 225, 0, 0,
                            ModContent.ProjectileType<TelegraphRed>(), 0, 0f, 0);
                        }
                    }

                    if (NPC.localAI[0] == 100)
                    {
                        SoundEngine.PlaySound(GrowlSound, NPC.Center);

                        Vector2 ChargeDirection = player.Center - NPC.Center;
                        ChargeDirection.Normalize();
                                
                        ChargeDirection.X *= 0;
                        ChargeDirection.Y *= 25;  
                        NPC.velocity.X = ChargeDirection.X;
                        NPC.velocity.Y = ChargeDirection.Y;
                    }

                    if (NPC.localAI[0] > 110)
                    {
                        NPC.velocity *= 0.95f;
                        OpenMouth = true;
                    }

                    if (NPC.localAI[0] > 120 && NPC.localAI[0] < 180)
                    {
                        NPC.Center = new Vector2(NPC.Center.X, NPC.Center.Y);
                        NPC.Center += Main.rand.NextVector2Square(-3, 3);
                    }

                    if (NPC.localAI[0] == 180 || NPC.localAI[0] == 200 || NPC.localAI[0] == 220)
                    {
                        SoundEngine.PlaySound(SoundID.NPCDeath13, NPC.Center);

                        int NumProjectiles = Main.rand.Next(5, 8);
                        for (int i = 0; i < NumProjectiles; ++i)
                        {
                            float Spread = (float)Main.rand.Next(-1000, 1000) * 0.01f;

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, 0 + Spread, 
                                Main.rand.Next(-10, -5), ModContent.ProjectileType<Toothball>(), Damage, 1, Main.myPlayer, 0, 0);
                            }
                        }
                    }

                    if (NPC.localAI[0] > 320)
                    {
                        OpenMouth = false;
                        NPC.localAI[0] = 0;
                        NPC.ai[0]++;
                    }

                    break;
                }

                //summon thorn pillars 3 times, then charge up at the player
                case 6:
                {
                    NPC.localAI[0]++;

                    if (NPC.localAI[1] < 3)
                    {
                        Vector2 GoTo = player.Center;
                        GoTo.X += 0;
                        GoTo.Y += 700;

                        //go from side to side
                        if (NPC.localAI[0] < 120)
                        {
                            GoTo.X += 1000;
                        }
                        if (NPC.localAI[0] > 120)
                        {
                            GoTo.X += -1000;
                        }
                        
                        float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 17, 25);
                        NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);

                        if (NPC.localAI[0] % 20 == 5)
                        {
                            for (int j = 0; j <= 0; j++) //0 was 1, 1 was 10
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

                                for (int i = -1; i <= 1; i += 2) 
                                {
                                    Vector2 center = new(player.Center.X, player.Center.Y + player.height / 4);
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

                                    if (Main.rand.Next(15) == 0)
                                    {
                                        if (Main.netMode != NetmodeID.MultiplayerClient)
                                        {
                                            Projectile.NewProjectile(NPC.GetSource_FromThis(), center.X, center.Y + 20, 0, 0, 
                                            ModContent.ProjectileType<ThornTelegraph>(), Damage, 1, Main.myPlayer, 0, 0);
                                        }
                                    }
                                }
                            }
                        }

                        if (NPC.localAI[0] >= 240)
                        {
                            NPC.localAI[0] = 0;
                            NPC.localAI[1]++;
                        }
                    }
                    else
                    {
                        NPC.localAI[0] = 0;
                        NPC.localAI[1] = 0; 
                        NPC.ai[0]++;
                    }

                    break;
                }

                //charge up after thorns
                case 7:
                {
                    NPC.localAI[0]++;

                    if (NPC.localAI[0] < 100)
                    {
                        Vector2 GoTo = player.Center;
                        GoTo.X += 0;
                        GoTo.Y += 750;

                        float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 12, 25);
                        NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
                    }

                    if (NPC.localAI[0] == 100)
                    {
                        NPC.velocity *= 0.02f;

                        NPC.position.X = player.Center.X;
                        NPC.position.Y = player.Center.Y + 700;

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), player.Center.X, player.Center.Y + 225, 0, 0,
                            ModContent.ProjectileType<TelegraphRed>(), 0, 0f, 0);
                        }
                    }

                    if (NPC.localAI[0] == 120)
                    {
                        SoundEngine.PlaySound(GrowlSound, NPC.position);

                        Vector2 ChargeDirection = player.Center - NPC.Center;
                        ChargeDirection.Normalize();
                                
                        ChargeDirection.X *= 0;
                        ChargeDirection.Y *= 32;  
                        NPC.velocity.X = ChargeDirection.X;
                        NPC.velocity.Y = ChargeDirection.Y;
                    }

                    if (NPC.localAI[0] == 150)
                    {
                        NPC.localAI[0] = 0;
                        NPC.localAI[1] = 0; 
                        NPC.ai[0] = 0; 
                    }

                    break;
                }
            }

            return false;
        }

        private void Movement(Player player, float maxSpeed, float accel, bool ChaseMovement = false)
        {
            bool collision = false;
            bool FastMovement = false;

            if (Vector2.Distance(NPC.Center, player.Center) >= 650)
            {
                FastMovement = true;
            }
            if (Vector2.Distance(NPC.Center, player.Center) <= 650)           
            {
                FastMovement = false;
            }

            float speed = maxSpeed;
            float acceleration = accel;

            if (!collision)
            {
                Rectangle rectangle1 = new((int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height);
                int maxDistance = 450;

                if (!ChaseMovement)
                {
                    maxDistance = 450;
                }
                if (ChaseMovement)
                {
                    maxDistance = 12;
                }

                bool playerCollision = true;

                for (int index = 0; index < 255; ++index)
                {
                    if (Main.player[index].active)
                    {
                        Rectangle rectangle2 = new((int)Main.player[index].position.X - maxDistance, 
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

            Vector2 NPCCenter = new(NPC.position.X + NPC.width * 0.5f, NPC.position.Y + NPC.height * 0.5f);
            float targetXPos = player.position.X + (player.width / 2);
            float targetYPos = player.position.Y + (player.height / 2);

            float targetRoundedPosX = (int)(targetXPos / 16.0) * 16;
            float targetRoundedPosY = (int)(targetYPos / 16.0) * 16;
            NPCCenter.X = (int)(NPCCenter.X / 16.0) * 16;
            NPCCenter.Y = (int)(NPCCenter.Y / 16.0) * 16;
            float dirX = targetRoundedPosX - NPCCenter.X;
            float dirY = targetRoundedPosY - NPCCenter.Y;
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
                        NPC.velocity.X -= acceleration * 1.1f;
                    }
                    else
                    {
                        NPC.velocity.X += acceleration * 1.1f;
                    }
                }

                else if (NPC.velocity.Y == speed)
                {
                    if (NPC.velocity.X < dirX)
                    {
                        NPC.velocity.X += acceleration;
                    }
                    else if (NPC.velocity.X > dirX)
                    {
                        NPC.velocity.X -= acceleration;
                    }
                }
                else if (NPC.velocity.Y > 4.0)
                {
                    if (NPC.velocity.X < 0.0)
                    {
                        NPC.velocity.X += acceleration * 1f;
                    }
                    else
                    {
                        NPC.velocity.X -= acceleration * 1f; 
                    }
                }
            }

            if (!collision)
            {
                NPC.TargetClosest(true);
                NPC.velocity.Y = NPC.velocity.Y + 0.11f;

                if (NPC.velocity.Y > speed)
                { 
                    NPC.velocity.Y = speed;
                }

                if (Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y) < speed * 1.0)
                {
                    if (NPC.velocity.X < 0.0)
                    {
                        NPC.velocity.X -= acceleration * 1.1f;
                    }
                    else
                    {
                        NPC.velocity.X += acceleration * 1.4f;
                    }
                }
                
                if (NPC.velocity.Y > 5.0) 
                {
                    if (NPC.velocity.X < 0.0)
                    {
                        NPC.velocity.X += acceleration * 0.9f;
                    }
                    else
                    {
                        NPC.velocity.X -= acceleration * 0.9f;
                    }
                }
            }
            else if (collision || FastMovement)
            {
                float absDirX = Math.Abs(dirX);
                float absDirY = Math.Abs(dirY);
                float newSpeed = speed / length;
                dirX *= newSpeed;
                dirY *= newSpeed;

                if (NPC.velocity.X > 0.0 && dirX > 0.0 || NPC.velocity.X < 0.0 && dirX < 0.0 || (NPC.velocity.Y > 0.0 && dirY > 0.0 || NPC.velocity.Y < 0.0 && dirY < 0.0))
                {
                    if (NPC.velocity.X < dirX)
                    {
                        NPC.velocity.X += acceleration;
                    }
                    else if (NPC.velocity.X > dirX)
                    {
                        NPC.velocity.X -= acceleration;
                    }
                    if (NPC.velocity.Y < dirY)
                    {
                        NPC.velocity.Y += acceleration;
                    }
                    else if (NPC.velocity.Y > dirY)
                    {
                        NPC.velocity.Y -= acceleration;
                    }

                    if (Math.Abs(dirY) < speed * 0.2 && (NPC.velocity.X > 0.0 && dirX < 0.0 || NPC.velocity.X < 0.0 && dirX > 0.0))
                    {
                        if (NPC.velocity.Y > 0.0)
                        {
                            NPC.velocity.Y += acceleration * 2f;
                        }
                        else
                        {
                            NPC.velocity.Y -= acceleration * 2f;
                        }
                    }
                    if (Math.Abs(dirX) < speed * 0.2 && (NPC.velocity.Y > 0.0 && dirY < 0.0 || NPC.velocity.Y < 0.0 && dirY > 0.0))
                    {
                        if (NPC.velocity.X > 0.0)
                        {
                            NPC.velocity.X += acceleration * 2f;
                        }
                        else
                        {
                            NPC.velocity.X -= acceleration * 2f;
                        }
                    }
                }
                else if (absDirX > absDirY)
                {
                    if (NPC.velocity.X < dirX)
                    {
                        NPC.velocity.X += acceleration * 1.1f;
                    }
                    else if (NPC.velocity.X > dirX)
                    {
                        NPC.velocity.X -= acceleration * 1.1f;
                    }
                    if (Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y) < speed * 0.5)
                    {
                        if (NPC.velocity.Y > 0.0)
                        {
                            NPC.velocity.Y += acceleration;
                        }
                        else
                        {
                            NPC.velocity.Y -= acceleration;
                        }
                    }
                }
                else
                {
                    if (NPC.velocity.Y < dirY)
                    {
                        NPC.velocity.Y += acceleration * 1.1f;
                    }
                    else if (NPC.velocity.Y > dirY)
                    {
                        NPC.velocity.Y -= acceleration * 1.1f;
                    }

                    if (Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y) < speed * 0.5)
                    {
                        if (NPC.velocity.X > 0.0)
                        {
                            NPC.velocity.X += acceleration;
                        }
                        else
                        {
                            NPC.velocity.X -= acceleration;
                        }
                    }
                }
            }

            //Some netupdate stuff
            if (collision)
            {
                if (NPC.localAI[2] != 1)
                {
                    NPC.netUpdate = true;
                }
                NPC.localAI[2] = 1f;
            }
            else
            {
                if (NPC.localAI[2] != 0.0)
                {
                    NPC.netUpdate = true;
                }
                NPC.localAI[2] = 0.0f;
            }
            if ((NPC.velocity.X > 0.0 && NPC.oldVelocity.X < 0.0 || NPC.velocity.X < 0.0 && NPC.oldVelocity.X > 0.0 || 
            (NPC.velocity.Y > 0.0 && NPC.oldVelocity.Y < 0.0 || NPC.velocity.Y < 0.0 && NPC.oldVelocity.Y > 0.0)) && !NPC.justHit)
            {
                NPC.netUpdate = true;
            }
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.GreaterHealingPotion;
        }
        
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            scale = 1.2f;
            return null;
        }
    }
}