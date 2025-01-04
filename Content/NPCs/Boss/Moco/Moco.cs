using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Items.BossBags;
using Spooky.Content.Items.Costume;
using Spooky.Content.Items.Pets;
using Spooky.Content.Items.SpookyHell;
using Spooky.Content.Items.SpookyHell.Misc;
using Spooky.Content.NPCs.Boss.Moco.Projectiles;
using Spooky.Content.Tiles.Relic;
using Spooky.Content.Tiles.Trophy;

namespace Spooky.Content.NPCs.Boss.Moco
{
    [AutoloadBossHead]
    public class Moco : ModNPC
    {
        int CurrentFrameX = 0;

        float ShakeIntensityMult = 1f;

        bool Phase2 = false;
        bool Transition = false;
        bool Sneezing = false;
        bool FinishedSneezing = false;
        bool EyesPoppedOut = false;
        bool SwitchedSides = false;
        bool AfterImages = false;

        Vector2 SaveNPCPosition;
        Vector2 SavePlayerPosition;

        private static Asset<Texture2D> NPCTexture;
        private static Asset<Texture2D> GlowTexture;
        private static Asset<Texture2D> AuraTexture1;
        private static Asset<Texture2D> AuraTexture2;

        public static readonly SoundStyle FlyingSound = new("Spooky/Content/Sounds/Moco/MocoFlying", SoundType.Sound);
        public static readonly SoundStyle EyePopSound = new("Spooky/Content/Sounds/Moco/MocoEyePop", SoundType.Sound) { PitchVariance = 0.75f };
        public static readonly SoundStyle SneezeSound1 = new("Spooky/Content/Sounds/Moco/MocoSneeze1", SoundType.Sound) { PitchVariance = 0.6f };
        public static readonly SoundStyle SneezeSound2 = new("Spooky/Content/Sounds/Moco/MocoSneeze2", SoundType.Sound) { PitchVariance = 0.6f };
        public static readonly SoundStyle SneezeSound3 = new("Spooky/Content/Sounds/Moco/MocoSneeze3", SoundType.Sound) { PitchVariance = 0.6f };
        public static readonly SoundStyle AngrySound = new("Spooky/Content/Sounds/Moco/MocoAngry", SoundType.Sound);
        
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 10;
            NPCID.Sets.TrailCacheLength[NPC.type] = 7;
            NPCID.Sets.TrailingMode[NPC.type] = 1;

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            //vector2
            writer.WriteVector2(SavePlayerPosition);
            writer.WriteVector2(SaveNPCPosition);

            //ints
            writer.Write(CurrentFrameX);

            //bools
            writer.Write(Phase2);
            writer.Write(Transition);
            writer.Write(SwitchedSides);
            writer.Write(AfterImages);

            //floats
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            //vector2
            SavePlayerPosition = reader.ReadVector2();
            SaveNPCPosition = reader.ReadVector2();

            //ints
            CurrentFrameX = reader.ReadInt32();

            //bools
            Phase2 = reader.ReadBoolean();
            Transition = reader.ReadBoolean();
            SwitchedSides = reader.ReadBoolean();
            AfterImages = reader.ReadBoolean();

            //floats
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 7000;
            NPC.damage = 40;
            NPC.defense = 25;
            NPC.width = 78;
            NPC.height = 128;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 5, 0, 0);
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.netAlways = true;
            NPC.boss = true;
            NPC.HitSound = SoundID.NPCHit22;
			NPC.DeathSound = SoundID.DD2_BetsyWindAttack;
            NPC.aiStyle = -1;
            Music = MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/Moco");
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpookyHellBiome>().Type };
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
		{
			NPC.lifeMax = (int)(NPC.lifeMax * 0.75f * balance * bossAdjustment);
		}

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
                new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.Moco"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyHellBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void FindFrame(int frameHeight)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                NPC.frame.Width = TextureAssets.Npc[NPC.type].Width() / 4;
            }

            NPC.frame.X = (int)(NPC.frame.Width * CurrentFrameX);

            NPC.frameCounter++;

            if (NPC.frameCounter > 2)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0;
            }

            //sneezing animation
            if (Sneezing)
            {
                //stop on the frame where mocos eyes are fully popped out by default
                if (!FinishedSneezing)
                {
                    if (NPC.frame.Y >= frameHeight * 6)
                    {
                        NPC.frame.Y = 4 * frameHeight;
                    }
                }
                //if FinishedSneezing is true and the animation is finished playing, reset it
                else
                {
                    if (NPC.frame.Y >= frameHeight * 9)
                    {
                        NPC.frame.Y = 0 * frameHeight;
                    }
                }
            }
            //eyes popped out animation for his eye pop attack, bottom 2 frames of the second and fourth vertical columns
            else if (EyesPoppedOut)
            {
                if (NPC.frame.Y < frameHeight * 9)
                {
                    NPC.frame.Y = 8 * frameHeight;
                }

                if (NPC.frame.Y >= frameHeight * 10)
                {
                    NPC.frame.Y = 8 * frameHeight;
                }
            }
            //default idle animation, first vertical row
            else
            {
                if (NPC.frame.Y >= frameHeight * 10)
                {
                    NPC.frame.Y = 0 * frameHeight;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            NPCTexture ??= ModContent.Request<Texture2D>(Texture);
            GlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/Moco/MocoGlow");
            AuraTexture1 ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/Moco/MocoPowerGlow1");
            AuraTexture2 ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/Moco/MocoPowerGlow2");

            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            if (AfterImages)
            {
				for (int oldPos = 0; oldPos < NPC.oldPos.Length; oldPos++)
				{
					Vector2 drawPos = NPC.oldPos[oldPos] - screenPos + NPC.frame.Size() / 2 + new Vector2(-25f, NPC.gfxOffY);
					Color color = NPC.GetAlpha(drawColor) * (float)(((float)(NPC.oldPos.Length - oldPos) / (float)NPC.oldPos.Length) / 2);

					spriteBatch.Draw(NPCTexture.Value, drawPos, NPC.frame, color, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0f);
				}
            }

            bool IsAngry = Transition && Sneezing && !FinishedSneezing && NPC.localAI[0] >= 100;

            Color NPCDrawColor = IsAngry ? NPC.GetAlpha(Color.Red) : NPC.GetAlpha(drawColor);

            spriteBatch.Draw(NPCTexture.Value, NPC.Center - screenPos, NPC.frame, NPCDrawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            spriteBatch.Draw(GlowTexture.Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(Color.White), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

            if (!Transition && Phase2)
            {
                float fade1 = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.4f / 2.4f * 6f)) / 2f + 0.5f;
                float fade2 = (float)Math.Sin((double)(Main.GlobalTimeWrappedHourly % 2.4f / 2.4f * 6f)) / 2f + 0.5f;

                spriteBatch.Draw(AuraTexture1.Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(Color.Lime * 0.5f) * fade1, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
                spriteBatch.Draw(AuraTexture2.Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(Color.Lime * 0.5f) * fade2, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            }
            
            return false;
        }

        public override void AI()
        {   
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            NPC.rotation = NPC.velocity.X * 0.005f;
            NPC.spriteDirection = NPC.direction;

            //despawn if the player dies or leaves the biome
            if (player.dead || !player.InModBiome(ModContent.GetInstance<Biomes.SpookyHellBiome>()))
            {
                NPC.ai[0] = -2;
            }

            //set to transition
            if (NPC.life < (NPC.lifeMax / 2) && NPC.ai[0] != -1 && !Phase2)
            {
                NPC.ai[0] = -1;
                NPC.localAI[0] = 0;
                NPC.localAI[1] = 0;
            }

            if (NPC.alpha == 255)
            {
                NPC.alpha = 0;
            }

			switch ((int)NPC.ai[0])
			{
				//despawning
				case -2:
				{
					AfterImages = true;
					NPC.velocity.Y = -25;
					NPC.EncourageDespawn(10);

					break;
				}

				//phase transition
				case -1:
				{
                    NPC.localAI[0]++;

                    NPC.velocity *= 0.92f;

                    if (NPC.localAI[0] == 1)
                    {
                        NPC.rotation = 0;

                        AfterImages = false;
                        Sneezing = false;
                        FinishedSneezing = false;
                        EyesPoppedOut = false;
                        Transition = true;

                        CurrentFrameX = 2;

                        NPC.immortal = true;
                        NPC.dontTakeDamage = true;
                    }

                    if (NPC.localAI[0] == 60)
                    {
                        SaveNPCPosition = NPC.Center;

                        NPC.netUpdate = true;
                    }

                    if (NPC.localAI[0] >= 120 && NPC.localAI[0] < 240)
                    {
                        if (NPC.localAI[0] == 120)
                        {
                            SoundEngine.PlaySound(AngrySound, NPC.Center);
                        }

                        Sneezing = true;

                        CurrentFrameX = 3;

                        NPC.Center = SaveNPCPosition;
                        NPC.Center += Main.rand.NextVector2Square(-8, 8);

                        int Steam1 = Gore.NewGore(NPC.GetSource_FromThis(), new Vector2(NPC.Center.X - 3, NPC.Center.Y + 20), default, Main.rand.Next(61, 64), 0.5f);
                        Main.gore[Steam1].velocity.X *= 0f;
                        Main.gore[Steam1].velocity.Y *= -2f;
                        Main.gore[Steam1].alpha = 125;

                        int Steam2 = Gore.NewGore(NPC.GetSource_FromThis(), new Vector2(NPC.Center.X - 34, NPC.Center.Y + 20), default, Main.rand.Next(61, 64), 0.5f);
                        Main.gore[Steam2].velocity.X *= 0f;
                        Main.gore[Steam2].velocity.Y *= -2f;
                        Main.gore[Steam2].alpha = 125;
                    }

                    //set this to true so the sneezing animation can finish playing
                    if (NPC.localAI[0] == 250)
                    {
                        FinishedSneezing = true;
                    }

                    //once the sneezing animation is done, then set its animation back to idle
                    if (NPC.localAI[0] > 250 && NPC.frame.Y >= 8 * NPC.height)
                    {
                        Sneezing = false;
                        FinishedSneezing = false;

                        CurrentFrameX = 2;

                        NPC.netUpdate = true;
                    }

                    if (NPC.localAI[0] >= 300)
                    {
                        //summon mocling minions
                        SpawnMoclings(12);

                        Phase2 = true;
                        Transition = false;
                        NPC.immortal = false;
                        NPC.dontTakeDamage = false;

                        NPC.localAI[0] = 0;
                        NPC.localAI[1] = 0;
                        NPC.ai[0] = 0;

                        NPC.netUpdate = true;
                    }

					break;
				}

                //dash towards the player twice quickly
                case 0:
				{
                    NPC.localAI[0]++;

                    CurrentFrameX = 0;

                    if (NPC.localAI[1] < 2)
                    {
                        //go to the side of the player
                        if (NPC.localAI[0] >= 60 && NPC.localAI[0] < 100) 
                        {
                            Vector2 GoTo = player.Center;
                            GoTo.X += (NPC.Center.X < player.Center.X) ? -420 : 420;
                            GoTo.Y -= 20;

                            float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 25, 50);
                            NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
                        }

                        //stop before charging
                        if (NPC.localAI[0] == 100)
                        {
                            NPC.velocity *= 0f;
                        }

                        //charge
                        if (NPC.localAI[0] == 105)
                        {
                            AfterImages = true;

                            SoundEngine.PlaySound(FlyingSound, NPC.Center);

                            Vector2 ChargeDirection = player.Center - NPC.Center;
                            ChargeDirection.Normalize();
                                    
                            ChargeDirection.X *= 22f;
                            ChargeDirection.Y *= 22f / 2.5f;
                            NPC.velocity.X = ChargeDirection.X;
                            NPC.velocity.Y = ChargeDirection.Y;
                        }

                        //loop attack
                        if (NPC.localAI[0] >= 135)
                        {
                            AfterImages = false;

                            NPC.localAI[1]++;
                            NPC.localAI[0] = 70;

                            NPC.netUpdate = true;
                        }
                    }
                    //go to next attack
                    else
                    {
                        NPC.velocity *= 0.95f;

                        AfterImages = false;

                        if (NPC.localAI[0] >= 135)
                        {
                            NPC.localAI[0] = 0;
                            NPC.localAI[1] = 0;
                            NPC.ai[0]++;

                            NPC.netUpdate = true;
                        }
                    }

                    break;
                }

                //zip above the player and shoot out snot globs that turn into puddles
                case 1:
				{
                    NPC.localAI[0]++;

                    //zip to the players location
                    if (NPC.localAI[0] >= 60 && NPC.localAI[0] < 70)
                    {
                        AfterImages = true;

                        if (NPC.localAI[0] == 60)
                        {
                            SoundEngine.PlaySound(FlyingSound, NPC.Center);
                        }

                        CurrentFrameX = 2;

                        //make moco go to an offset x-position to reach the player a bit more quickly
                        MoveToPlayer(player, player.Center.X < NPC.Center.X ? -225f : 225f, -280f);
                    }
                    else
                    {
                        NPC.velocity *= 0.85f;
                    }

                    //save position for shaking
                    if (NPC.localAI[0] == 90)
                    {
                        AfterImages = false;

                        SaveNPCPosition = NPC.Center;

                        NPC.netUpdate = true;
                    }

                    //shake
                    if (NPC.localAI[0] > 90 && NPC.localAI[0] < 120)
                    {
                        NPC.Center = SaveNPCPosition;
                        NPC.Center += Main.rand.NextVector2Square(-5, 5);
                    }

					if (NPC.localAI[0] == 120)
					{
						NPC.frame.Y = 0;
					}

                    int Frequency = Phase2 ? 7 : 15;

                    //fire out nose globs that land on the ground and add upward recoil when each one is shot
                    if (NPC.localAI[0] >= 120 && NPC.localAI[0] < 270 && NPC.localAI[0] % Frequency == 0)
                    {
                        SoundEngine.PlaySound(SneezeSound1, NPC.Center);

						Sneezing = true;

                        CurrentFrameX = 3;

                        NPC.velocity.Y = -4;

                        int VelocityX = Phase2 ? Main.rand.Next(-10, 11) : Main.rand.Next(-7, 8);

                        NPCGlobalHelper.ShootHostileProjectile(NPC, new Vector2(NPC.Center.X, NPC.Center.Y + 35), 
                        new Vector2(VelocityX, Main.rand.Next(2, 4)), ModContent.ProjectileType<LingeringSnotBall>(), NPC.damage, 4.5f);
                    }
                    else
                    {
                        if (NPC.localAI[0] >= 120)
                        {
                            NPC.velocity *= 0.1f;
                        }
                    }

                    //set this to true so the sneezing animation can finish playing
                    if (NPC.localAI[0] == 270)
                    {
                        FinishedSneezing = true;
                    }

                    //once the sneezing animation is done, then set its animation back to idle
                    if (NPC.localAI[0] > 270 && NPC.frame.Y >= 8 * NPC.height)
                    {
                        Sneezing = false;
                        FinishedSneezing = false;

                        CurrentFrameX = 2;
                    }
                    
                    //go to next attack
                    if (NPC.localAI[0] >= 340)
                    {
                        CurrentFrameX = 0;

                        NPC.localAI[0] = 0;
						NPC.ai[0]++;

                        NPC.netUpdate = true;
                    }
 
                    break;
                }

                //zip above the player and shoot out a giant bouncing snot glob
                case 2:
				{
                    NPC.localAI[0]++;

                    if (NPC.localAI[0] >= 60 && NPC.localAI[0] <= 155)
                    {
                        //EoC rotation
                        Vector2 vector = new Vector2(NPC.Center.X, NPC.Center.Y);
                        float RotateX = player.Center.X - vector.X;
                        float RotateY = player.Center.Y - vector.Y;
                        NPC.rotation = (float)Math.Atan2((double)RotateY, (double)RotateX) + 4.71f;
                    }

                    //zip to the players location
                    if (NPC.localAI[0] >= 60 && NPC.localAI[0] < 70)
                    {
                        AfterImages = true;

                        if (NPC.localAI[0] == 60)
                        {
                            SoundEngine.PlaySound(FlyingSound, NPC.Center);
                        }

                        //make moco go to an offset x-position to reach the player a bit more quickly
                        MoveToPlayer(player, player.Center.X < NPC.Center.X ? -225f : 225f, -280f);
                    }
                    
                    if (NPC.localAI[0] >= 70 && NPC.localAI[0] <= 90)
                    {
                        NPC.velocity *= 0.85f;
                    }

                    //save position for shaking
                    if (NPC.localAI[0] == 90)
                    {
                        AfterImages = false;

                        SaveNPCPosition = NPC.Center;

                        NPC.netUpdate = true;
                    }

                    //shake
                    if (NPC.localAI[0] > 90 && NPC.localAI[0] < 120)
                    {
                        NPC.Center = SaveNPCPosition;
                        NPC.Center += Main.rand.NextVector2Square(-5, 5);
                    }

                    //fire out bouncy booger
                    if (NPC.localAI[0] == 120)
                    {
                        SoundEngine.PlaySound(SneezeSound2, NPC.Center);

						Sneezing = true;
                        FinishedSneezing = true;

                        NPC.frame.Y = 0;
                        CurrentFrameX = 1;

                        NPC.velocity = Vector2.Zero;

                        Vector2 ShootSpeed = player.Center - new Vector2(NPC.Center.X, NPC.Center.Y + 35);
                        ShootSpeed.Normalize();
                        ShootSpeed *= 12;

                        Vector2 muzzleOffset = Vector2.Normalize(new Vector2(ShootSpeed.X, ShootSpeed.Y)) * 60f;
                        Vector2 position = new Vector2(NPC.Center.X, NPC.Center.Y);

                        if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
                        {
                            position += muzzleOffset;
                        }
                            
                        NPCGlobalHelper.ShootHostileProjectile(NPC, position, ShootSpeed, ModContent.ProjectileType<GiantSnot>(), NPC.damage, 4.5f, ai0: Phase2 ? 1 : 0);
                    }
                    
                    //after shooting the booger, zip towards the player quickly if the get too far away
                    if (NPC.localAI[0] >= 155)
                    {
                        //quickly move to a location above the player if they are too far away
                        if (NPC.Distance(player.Center) >= 750f)
                        {
                            SoundEngine.PlaySound(FlyingSound, NPC.Center);

                            AfterImages = true;

                            //make moco go to an offset x-position to reach the player a bit more quickly
                            MoveToPlayer(player, 0f, -300f);
                        }
                        //if moco is close enough, then slow down its velocity
                        else
                        {
                            AfterImages = false;

                            NPC.velocity *= 0.85f;
                        }
                    }

                    //once the sneezing animation is done, then set its animation back to idle
                    if (NPC.frame.Y >= 8 * NPC.height)
                    {
                        Sneezing = false;
                        FinishedSneezing = false;

                        CurrentFrameX = 0;
                    }
                    
                    //go to next attack
                    if (NPC.localAI[0] >= 420)
                    {
                        AfterImages = false;

                        CurrentFrameX = 0;

                        NPC.velocity = Vector2.Zero;

                        NPC.localAI[0] = 0;
                        NPC.localAI[1] = 0;
						NPC.ai[0]++;

                        NPC.netUpdate = true;
                    }

                    break;
                }

                //zip to the side of the player and rapid fire boogers
                case 3:
				{
                    NPC.localAI[0]++;

                    if (NPC.localAI[0] >= 60)
                    {
                        //EoC rotation
                        Vector2 vector = new Vector2(NPC.Center.X, NPC.Center.Y);
                        float RotateX = player.Center.X - vector.X;
                        float RotateY = player.Center.Y - vector.Y;
                        NPC.rotation = (float)Math.Atan2((double)RotateY, (double)RotateX) + 4.71f;
                    }

                    //zip to the players location
                    if (NPC.localAI[0] >= 60 && NPC.localAI[0] < 70)
                    {
                        AfterImages = true;

                        if (NPC.localAI[0] == 60)
                        {
                            SoundEngine.PlaySound(FlyingSound, NPC.Center);
                        }

                        CurrentFrameX = 2;

                        MoveToPlayer(player, player.Center.X > NPC.Center.X ? -250f : 250f, -100f);
                    }

                    if (NPC.localAI[0] >= 70 && NPC.localAI[0] <= 90)
                    {
                        NPC.velocity *= 0.85f;
                    }

                    //save position for shaking
                    if (NPC.localAI[0] == 90)
                    {
                        AfterImages = false;

                        SaveNPCPosition = NPC.Center;

                        NPC.netUpdate = true;
                    }

                    //shake
                    if (NPC.localAI[0] > 90 && NPC.localAI[0] < 120)
                    {
                        NPC.Center = SaveNPCPosition;
                        NPC.Center += Main.rand.NextVector2Square(-5, 5);
                    }

					if (NPC.localAI[0] == 120)
					{
						NPC.frame.Y = 0;
					}

                    //rapid fire boogers
                    if (NPC.localAI[0] >= 120 && NPC.localAI[0] < 300 && NPC.localAI[0] % 10 == 0)
                    {
                        SoundEngine.PlaySound(SneezeSound1, NPC.Center);

						Sneezing = true;

                        CurrentFrameX = 3;

                        Vector2 Recoil = player.Center - NPC.Center;
                        Recoil.Normalize();
                        Recoil *= 2;
                        NPC.velocity -= Recoil;

                        Vector2 ShootSpeed = player.Center - NPC.Center;
                        ShootSpeed.Normalize();
                        ShootSpeed.X *= 10 + Main.rand.Next(-5, 6);
                        ShootSpeed.Y *= 10 + Main.rand.Next(-5, 6);

                        Vector2 muzzleOffset = Vector2.Normalize(new Vector2(ShootSpeed.X, ShootSpeed.Y)) * 60f;
                        Vector2 position = new Vector2(NPC.Center.X, NPC.Center.Y);

                        if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
                        {
                            position += muzzleOffset;
                        }

                        NPCGlobalHelper.ShootHostileProjectile(NPC, position, ShootSpeed, ModContent.ProjectileType<SnotBall>(), NPC.damage, 4.5f);
                    }

                    if (NPC.localAI[0] >= 120 && NPC.localAI[0] < 300)
                    {
                        //quickly move towards the player if they try and run away
                        if (NPC.Distance(player.Center) >= 750f)
                        {
                            SoundEngine.PlaySound(FlyingSound, NPC.Center);

                            AfterImages = true;

                            MoveToPlayer(player, player.Center.X > NPC.Center.X ? -200f : 200f, -100f);
                        }
                        //if moco is close enough, then slow down its velocity
                        else
                        {
                            AfterImages = false;

                            NPC.velocity *= 0.85f;
                        }
                    }

                    if (NPC.localAI[0] >= 300)
                    {
                        NPC.velocity *= 0.85f;
                    }

                    //set this to true so the sneezing animation can finish playing
                    if (NPC.localAI[0] == 300)
                    {
                        FinishedSneezing = true;
                    }

                    //once the sneezing animation is done, then set its animation back to idle
                    if (NPC.localAI[0] > 300 && NPC.frame.Y >= 8 * NPC.height)
                    {
                        Sneezing = false;
                        FinishedSneezing = false;

                        CurrentFrameX = 2;
                    }
                    
                    //go to next attack
                    if (NPC.localAI[0] >= 340)
                    {
                        CurrentFrameX = 0;

                        NPC.localAI[0] = 0;
						NPC.ai[0] = Phase2 ? 5 : 4;

                        NPC.netUpdate = true;
                    }
 
                    break;
                }

                //go nearby the player, then shoot out eyes 
                case 4:
				{
                    NPC.localAI[0]++;

                    if (NPC.localAI[0] >= 60 && NPC.localAI[0] <= 120)
                    {
                        //EoC rotation
                        Vector2 vector = new Vector2(NPC.Center.X, NPC.Center.Y);
                        float RotateX = player.Center.X - vector.X;
                        float RotateY = player.Center.Y - vector.Y;
                        NPC.rotation = (float)Math.Atan2((double)RotateY, (double)RotateX) + 4.71f;
                    }

                    //zip to the players location
                    if (NPC.localAI[0] >= 60 && NPC.localAI[0] < 70)
                    {
                        AfterImages = true;

                        if (NPC.localAI[0] == 60)
                        {
                            SoundEngine.PlaySound(FlyingSound, NPC.Center);
                        }

                        CurrentFrameX = 2;

                        MoveToPlayer(player, 0f, -75f);
                    }

					if (NPC.localAI[0] >= 70 && NPC.localAI[0] <= 90)
                    {
                        NPC.velocity *= 0.85f;
                    }

					//save position for shaking
                    if (NPC.localAI[0] == 90)
                    {
                        AfterImages = false;

                        SaveNPCPosition = NPC.Center;

                        NPC.netUpdate = true;
                    }

                    //shake
                    if (NPC.localAI[0] > 90 && NPC.localAI[0] < 120)
                    {
                        NPC.Center = SaveNPCPosition;
                        NPC.Center += Main.rand.NextVector2Square(-5, 5);
                    }

                    if (NPC.localAI[0] == 100)
                    {
                        NPC.localAI[1] = NPC.rotation;

                        SavePlayerPosition = player.Center;

                        NPC.netUpdate = true;
                    }

                    if (NPC.localAI[0] > 100)
                    {
                        NPC.rotation = NPC.localAI[1];
					}

					//pop out eyes
                    if (NPC.localAI[0] == 120)
                    {
                        SoundEngine.PlaySound(EyePopSound, NPC.Center);
                        SoundEngine.PlaySound(SneezeSound2, NPC.Center);

						EyesPoppedOut = true;

                        CurrentFrameX = 3;

                        NPC.velocity = Vector2.Zero;

                        for (int numProjectiles = -1; numProjectiles <= 1; numProjectiles += 2)
                        {
                            Vector2 ShootFrom = 35f * NPC.DirectionTo(SavePlayerPosition).RotatedBy(MathHelper.ToRadians(23) * numProjectiles);

                            Vector2 muzzleOffset = Vector2.Normalize(new Vector2(ShootFrom.X, ShootFrom.Y)) * 38f;
                            Vector2 position = new Vector2(NPC.Center.X, NPC.Center.Y);

                            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
                            {
                                position += muzzleOffset;
                            }

                            Vector2 ShootSpeed = SavePlayerPosition - position;
                            ShootSpeed.Normalize();
                            ShootSpeed *= 28;

                            NPCGlobalHelper.ShootHostileProjectile(NPC, position, ShootSpeed, ModContent.ProjectileType<MocoEye>(), NPC.damage, 4.5f, ai1: NPC.whoAmI);
                        }

                        NPC.netUpdate = true;
                    }

                    //set this to true so the sneezing animation can finish playing
                    if (NPC.localAI[0] >= 250)
                    {
                        CurrentFrameX = 2;

                        EyesPoppedOut = false;

                        NPC.netUpdate = true;
                    }

                    //go to next attack
                    if (NPC.localAI[0] >= 320)
                    {
                        CurrentFrameX = 0;

                        NPC.localAI[0] = 0;
                        NPC.localAI[1] = 0;
						NPC.ai[0] = 0;

                        NPC.netUpdate = true;
                    }

                    break;
                }

                //summon projectile moclings that charge at the player and die on impact
                case 5:
				{
                    NPC.localAI[0]++;

                    //zip to the players location
                    if (NPC.localAI[0] >= 60 && NPC.localAI[0] < 70)
                    {
                        AfterImages = true;

                        if (NPC.localAI[0] == 60)
                        {
                            SoundEngine.PlaySound(FlyingSound, NPC.Center);
                        }

                        MoveToPlayer(player, 0f, -300f);
                    }

                    if (NPC.localAI[0] >= 70 && NPC.localAI[0] <= 100)
                    {
                        AfterImages = false;

                        NPC.velocity *= 0.85f;
                    }

                    //save position for shaking
                    if (NPC.localAI[0] == 90)
                    {
                        SaveNPCPosition = NPC.Center;

                        NPC.netUpdate = true;
                    }

                    //shake
                    if (NPC.localAI[0] > 90 && NPC.localAI[0] < 140)
                    {
                        NPC.Center = SaveNPCPosition;
                        NPC.Center += Main.rand.NextVector2Square(-5, 5);
                    }

                    if (NPC.localAI[0] >= 150 && NPC.localAI[0] < 320)
                    {
                        //quickly move towards the player if they try and run away
                        if (NPC.Distance(player.Center) >= 750f)
                        {
                            SoundEngine.PlaySound(FlyingSound, NPC.Center);

                            AfterImages = true;

                            MoveToPlayer(player, player.Center.X > NPC.Center.X ? -200f : 200f, -100f);
                        }
                        //if moco is close enough, then slow down its velocity
                        else
                        {
                            AfterImages = false;

                            NPC.velocity *= 0.85f;
                        }
                    }

                    //spawn mocling projectiles from offscreen
                    if (NPC.localAI[0] >= 150 && NPC.localAI[0] < 280 && NPC.localAI[0] % 10 == 0)
                    {
                        int SpawnX = Main.rand.NextBool() ? (int)player.Center.X - 1200 : (int)player.Center.X + 1200;
                        int SpawnY = (int)NPC.Center.Y + Main.rand.Next(-100, 100);

                        SoundEngine.PlaySound(FlyingSound, new Vector2(SpawnX, SpawnY));

                        NPCGlobalHelper.ShootHostileProjectile(NPC, new Vector2(SpawnX, SpawnY), Vector2.Zero, ModContent.ProjectileType<MoclingProjectile>(), NPC.damage, 4.5f);
                    }

                    if (NPC.localAI[0] >= 320)
                    {
                        NPC.velocity *= 0.85f;
                    }

                    if (NPC.localAI[0] >= 380)
                    {
                        NPC.localAI[0] = 0;
                        NPC.ai[0]++;

                        NPC.netUpdate = true;
                    }

                    break;
                }

                //prepare and shoot giant booger spread at the player, then come back on the opposite side of the screen
                case 6:
				{
                    NPC.localAI[0]++;

                    //zip to the side of the player for a while
                    if (NPC.localAI[0] >= 30 && NPC.localAI[0] < 140)
                    {
                        CurrentFrameX = 2;

                        if (NPC.localAI[0] == 60)
                        {
                            SoundEngine.PlaySound(FlyingSound, NPC.Center);
                        }

                        MoveToPlayer(player, player.Center.X > NPC.Center.X ? -550f : 550f, 0f);
                    }

                    if (NPC.localAI[0] >= 130 && NPC.localAI[0] <= 160)
                    {
                        NPC.velocity *= 0.85f;
                    }

                    if (NPC.localAI[0] == 100)
                    {
                        Sneezing = true;

                        CurrentFrameX = 3;
                    }

                    if (NPC.localAI[0] >= 100 && NPC.localAI[0] <= 140)
                    {
                        SaveNPCPosition = NPC.Center;

                        NPC.netUpdate = true;
                    }

                    //shake
                    if (NPC.localAI[0] > 100 && NPC.localAI[0] < 160)
                    {
                        NPC.Center = SaveNPCPosition;
                        NPC.Center += Main.rand.NextVector2Square(-3 * ShakeIntensityMult, 3 * ShakeIntensityMult);

                        ShakeIntensityMult += 0.05f;
                    }

                    if (NPC.localAI[0] > 30 && NPC.localAI[0] <= 160)
                    {
                        NPC.rotation = (NPC.Center.X < player.Center.X) ? -1.55f: 1.55f;
                    }

                    if (NPC.localAI[0] > 160)
                    {
                        NPC.rotation = (NPC.velocity.X < 0) ? -1.55f : 1.55f;
                    }

                    //shot snot spread
                    if (NPC.localAI[0] == 160)
                    {
                        Sneezing = true;

                        AfterImages = true;

                        SoundEngine.PlaySound(SneezeSound3, NPC.Center);

                        NPC.velocity.X = (NPC.Center.X < player.Center.X) ? -35 : 35;
                        NPC.velocity.Y *= 0;

                        int ShootTowards = (NPC.Center.X < player.Center.X) ? 100 : -100;

                        for (int numProjectiles = 0; numProjectiles <= 15; numProjectiles++)
                        {
                            Vector2 ShootSpeed = new Vector2(NPC.Center.X + ShootTowards, NPC.Center.Y) - NPC.Center;
                            ShootSpeed.Normalize();
                            ShootSpeed.X *= Main.rand.Next(12, 20);

                            int RandomVelocityY = Main.rand.Next(-25, 26);

                            Vector2 muzzleOffset = Vector2.Normalize(new Vector2(ShootSpeed.X, RandomVelocityY)) * 60f;
                            Vector2 position = new Vector2(NPC.Center.X, NPC.Center.Y);

                            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
                            {
                                position += muzzleOffset;
                            }

                            NPCGlobalHelper.ShootHostileProjectile(NPC, position, new Vector2(ShootSpeed.X, RandomVelocityY), ModContent.ProjectileType<SnotBall>(), NPC.damage, 4.5f);
                        }
                    }

                    //handle moco switching sides
                    if (NPC.localAI[0] >= 160)
                    {
                        if (NPC.Distance(player.Center) >= 2000f && !SwitchedSides) 
                        {
                            NPC.position.X = (NPC.Center.X < player.Center.X) ? player.Center.X + 2000 : player.Center.X - 2000;
                            NPC.localAI[1] = 1; //used so the moclings teleport to moco after switching sides
                            FinishedSneezing = true;
                            SwitchedSides = true;
                        }
                    }

                    //once the sneezing animation is done, then set its animation back to idle
                    if (SwitchedSides && NPC.frame.Y >= 8 * NPC.height)
                    {
                        Sneezing = false;
                        FinishedSneezing = false;

                        CurrentFrameX = 2;
                    }

                    if (NPC.localAI[0] >= 220)
                    {
                        NPC.velocity *= 0.97f;
                    }

                    if (NPC.localAI[0] >= 300)
                    {
                        ShakeIntensityMult = 1f;

                        SwitchedSides = false;
                        AfterImages = false;
                        Sneezing = false;

                        NPC.localAI[0] = 0;
                        NPC.localAI[1] = 0;
                        NPC.ai[0]++;
                        NPC.netUpdate = true;
                    }

                    break;
                }

                //spawn moclings if there arent any, and then command them to attack the player directly
                case 7:
				{
                    NPC.localAI[0]++;

                    //zip to the players location
                    if (NPC.localAI[0] >= 60 && NPC.localAI[0] < 70)
                    {
                        AfterImages = true;

                        CurrentFrameX = 2;

                        if (NPC.localAI[0] == 60)
                        {
                            SoundEngine.PlaySound(FlyingSound, NPC.Center);
                        }

                        MoveToPlayer(player, 0f, -300f);
                    }

                    if (NPC.localAI[0] >= 70 && NPC.localAI[0] <= 90)
                    {
                        AfterImages = true;

                        NPC.velocity *= 0.85f;
                    }

                    //save position for shaking
                    if (NPC.localAI[0] == 90)
                    {
                        AfterImages = false;

                        SaveNPCPosition = NPC.Center;

                        NPC.netUpdate = true;

                        //summon moclings if there isnt any
                        int Amount = 12 - NumberOfMoclings();

                        SpawnMoclings(Amount);
                    }

                    //shake
                    if (NPC.localAI[0] > 90 && NPC.localAI[0] < 140)
                    {
                        NPC.Center = SaveNPCPosition;
                        NPC.Center += Main.rand.NextVector2Square(-5, 5);
                    }

                    if (NPC.localAI[0] >= 140 && NPC.localAI[0] < 500)
                    {
                        //quickly move towards the player if they try and run away
                        if (NPC.Distance(player.Center) >= 750f)
                        {
                            SoundEngine.PlaySound(FlyingSound, NPC.Center);

                            AfterImages = true;

                            MoveToPlayer(player, player.Center.X > NPC.Center.X ? -200f : 200f, -300f);
                        }
                        //if moco is close enough, then slow down its velocity
                        else
                        {
                            AfterImages = false;

                            NPC.velocity *= 0.85f;
                        }
                    }

                    if (NPC.localAI[0] >= 420)
                    {
                        NPC.velocity *= 0.85f;
                    }

                    if (NPC.localAI[0] >= 450)
                    {
                        NPC.localAI[0] = 0;
                        NPC.ai[0] = 0;

                        NPC.netUpdate = true;
                    }

                    break;
                }
			}
		}

        //spawn mocling minions with the provided amount
        public void SpawnMoclings(int Amount)
        {
            Player player = Main.player[NPC.target];

            for (int numFlies = 1; numFlies <= Amount; numFlies++)
            {
                int SpawnX = Main.rand.NextBool() ? (int)player.Center.X - 1200 : (int)player.Center.X + 1200;
                int SpawnY = (int)NPC.Center.Y + Main.rand.Next(-300, 300);

                int NewMocling = NPC.NewNPC(NPC.GetSource_FromAI(), SpawnX, SpawnY, ModContent.NPCType<MoclingMinion>(), ai1: NPC.whoAmI);

                if (Main.netMode != NetmodeID.SinglePlayer)
                {
                    NetMessage.SendData(MessageID.SyncNPC, number: NewMocling);
                }
            }

            NPC.netUpdate = true;
        }

        public int NumberOfMoclings()
        {
            int numMoclings = 0;

            for (int i = 0; i <= Main.maxNPCs; i++)
            {
                NPC Mocling = Main.npc[i];

                if (Mocling.active && Mocling.type == ModContent.NPCType<MoclingMinion>())
                {
                    numMoclings++;

                    //if moco has maximum moclings, then just return that value instantly without looping further
                    if (numMoclings >= 12)
                    {
                        return numMoclings;
                    }
                }
            }

            return numMoclings;
        }

        //super fast movement to create a fly-like movement effect
        public void MoveToPlayer(Player target, float TargetPositionX, float TargetPositionY)
        {
            Vector2 GoTo = target.Center + new Vector2(TargetPositionX, TargetPositionY);

            if (NPC.Distance(GoTo) >= 200f)
            { 
                GoTo -= NPC.DirectionTo(GoTo) * 100f;
            }

            Vector2 GoToVelocity = GoTo - NPC.Center;

            float lerpValue = Utils.GetLerpValue(100f, 600f, GoToVelocity.Length(), false);

            float velocityLength = GoToVelocity.Length();

            if (velocityLength > 18f)
            { 
                velocityLength = 18f;
            }

            NPC.velocity = Vector2.Lerp(GoToVelocity.SafeNormalize(Vector2.Zero) * velocityLength, GoToVelocity / 6f, lerpValue);
            NPC.netUpdate = true;
        }

        //Loot and stuff
        public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
            LeadingConditionRule notExpertRule = new(new Conditions.NotExpert());

            //treasure bag
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<BossBagMoco>()));

            //master relic and pet
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<MocoRelicItem>()));
            npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<MocoTissue>(), 4));

            //weapon drops
            int[] MainItem = new int[]
            { 
                ModContent.ItemType<BoogerFlail>(), 
                ModContent.ItemType<BoogerBlaster>(), 
                ModContent.ItemType<BoogerBook>(),
                ModContent.ItemType<BoogerStaff>()
            };

            notExpertRule.OnSuccess(ItemDropRule.OneFromOptions(1, MainItem));

            //material
			notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<SnotGlob>(), 1, 12, 20));

            //drop boss mask
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<MocoMask>(), 7));

            //trophy always drops directly from the boss
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<MocoTrophyItem>(), 10));

            npcLoot.Add(notExpertRule);
        }

        public override void OnKill()
        {
            //drop a sentient heart for each active player in the world
            if (!Flags.downedMoco)
            {
                for (int numPlayer = 0; numPlayer <= Main.maxPlayers; numPlayer++)
                {
                    if (Main.player[numPlayer].active)
                    {
                        int newItem = Item.NewItem(NPC.GetSource_DropAsItem(), NPC.Hitbox, ModContent.ItemType<SentientHeart>());

                        if (Main.netMode == NetmodeID.MultiplayerClient && newItem >= 0)
                        {
                            NetMessage.SendData(MessageID.SyncItem, -1, -1, null, newItem, 1f);
                        }
                    }
                }

                Flags.GuaranteedRaveyard = true;

                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.WorldData);
                }
            }

            NPC.SetEventFlagCleared(ref Flags.downedMoco, -1);

            if (!MenuSaveSystem.hasDefeatedMoco)
            {
                MenuSaveSystem.hasDefeatedMoco = true;
            }

            //spawn particles
            for (int numParticles = 0; numParticles <= 12; numParticles++)
            {
                ParticleOrchestrator.RequestParticleSpawn(clientOnly: false, ParticleOrchestraType.TrueNightsEdge, new ParticleOrchestraSettings
                {
                    PositionInWorld = NPC.Center + new Vector2(Main.rand.Next(-55, 56), Main.rand.Next(-55, 56))
                });
            }

            //spawn little mocling outro
            int MoclingOutro = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<MocoOutro>());

            if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.SendData(MessageID.SyncNPC, number: MoclingOutro);
            }
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.HealingPotion;
        }
    }
}