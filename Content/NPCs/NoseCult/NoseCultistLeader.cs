using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Items.BossSummon;
using Spooky.Content.Items.SpookyHell;
using Spooky.Content.NPCs.NoseCult.Projectiles;

namespace Spooky.Content.NPCs.NoseCult
{
	public class NoseCultistLeader : ModNPC
	{
        int CurrentFrameX = 0; //0 = idle flying animation  1 = sneezing animation  2 = casting animation
        int SaveDirection;

        bool Sneezing = false;
        bool Casting = false;
        bool Charging = false;
        bool HasSpawnedEnemies = false;
        bool hasCollidedWithWall = false;
        
        Vector2 SavePosition;

        private static Asset<Texture2D> NPCTexture;

        public static readonly SoundStyle SneezeSound = new("Spooky/Content/Sounds/Moco/MocoSneeze1", SoundType.Sound);

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 9;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;

            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Position = new Vector2(0f, 30f),
                PortraitPositionXOverride = 0f,
                PortraitPositionYOverride = 10f
            };
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
			//vector2
			writer.WriteVector2(SavePosition);

			//bools
			writer.Write(HasSpawnedEnemies);
            writer.Write(hasCollidedWithWall);

            //floats
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
			//vector2
			SavePosition = reader.ReadVector2();

            //bools
            HasSpawnedEnemies = reader.ReadBoolean();
            hasCollidedWithWall = reader.ReadBoolean();

            //floats
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 4500;
            NPC.damage = 50;
            NPC.defense = 5;
            NPC.width = 122;
			NPC.height = 128;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 1, 0, 0);
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit48 with { Pitch = -0.5f };
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = -1;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.NoseTempleBiome>().Type };
        }

        //uses boss hp scaling so that it scales based on the amount of players
        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * bossAdjustment);
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
            bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[Type], quickUnlock: true);

			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.NoseCultistLeader"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.NoseTempleBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void FindFrame(int frameHeight)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                NPC.frame.Width = TextureAssets.Npc[NPC.type].Width() / 3;
            }

            NPC.frame.X = (int)(NPC.frame.Width * CurrentFrameX);

            NPC.frameCounter++;

            //sneezing animation, first 6 frames on the second vertical row
            if (Sneezing)
            {
                if (NPC.frameCounter > 5)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0;
                }
                if (NPC.frame.Y >= frameHeight * 6)
                {
                    NPC.frame.Y = 3 * frameHeight;
                }
            }
            //casting animation, third vertical row
            else if (Casting)
            {
                if (NPC.frameCounter > 7)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0;
                }
                if (NPC.frame.Y >= frameHeight * 6)
                {
                    NPC.frame.Y = 3 * frameHeight;
                }
            }
            //charging animation, last 3 frames on the second vertical row 
            else if (Charging)
            {
                if (NPC.frame.Y < frameHeight * 7)
                {
                    NPC.frame.Y = 6 * frameHeight;
                }

                int frameSpeed = CurrentFrameX == 2 ? 5 : 2;

                if (NPC.frameCounter > frameSpeed)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0;
                }
                if (NPC.frame.Y >= frameHeight * 9)
                {
                    NPC.frame.Y = 6 * frameHeight;
                }
            }
            //default idle animation, first vertical row
            else
            {
                if (NPC.frameCounter > 2)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0;
                }
                if (NPC.frame.Y >= frameHeight * 9)
                {
                    NPC.frame.Y = 0 * frameHeight;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            NPCTexture ??= ModContent.Request<Texture2D>(Texture);

            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(NPCTexture.Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            
            return false;
        }

        public override bool CheckActive()
        {
            Player player = Main.player[NPC.target];

            return !player.InModBiome(ModContent.GetInstance<Biomes.NoseTempleBiome>());
        }

        public override void AI()
		{
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            NPC Parent = Main.npc[(int)NPC.ai[3]];
            
            NPC.spriteDirection = NPC.direction;

            //set to transition
            if (NPC.life < (NPC.lifeMax / 2) && !HasSpawnedEnemies && NPC.ai[0] != 5)
            {
                NPC.noGravity = true;
                NPC.noTileCollide = true;

                Sneezing = false;
                Charging = false;
                Casting = false;
                hasCollidedWithWall = false;

                CurrentFrameX = 0;

                NPC.localAI[0] = 0;
                NPC.localAI[1] = 0;
                NPC.ai[0] = 5;

                NPC.netUpdate = true;
            }

            if (NPC.alpha == 255)
            {
                NPC.alpha = 0;
            }

            switch ((int)NPC.ai[0])
            {
                //fly around to a location around the shrine, then switch to a random attack
                case 0:
                {
                    CurrentFrameX = 0;

                    NPC.localAI[0]++;

                    if (NPC.localAI[0] == 5)
                    {
                        SavePosition = new Vector2(Parent.Center.X + Main.rand.Next(-350, 350), Parent.Center.Y - Main.rand.Next(50, 150));

                        NPC.netUpdate = true;
                    }

                    if (NPC.localAI[0] > 5 && NPC.localAI[0] < 60)
                    {
                        Vector2 GoTo = SavePosition;

                        float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 6, 12);
                        NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
                    }
                    else
                    {
                        NPC.velocity *= 0.92f;
                    }

                    if (NPC.localAI[0] >= 120)
                    {
                        NPC.localAI[0] = 0;
                        NPC.localAI[1]++;

                        if (NPC.localAI[1] > 3 && !NPC.AnyNPCs(ModContent.NPCType<NoseBallPurple>()) && !NPC.AnyNPCs(ModContent.NPCType<NoseBallRed>()))
                        {
                            NPC.ai[0] = 4;
                        }
                        else
                        {
                            NPC.ai[0] = Main.rand.Next(1, 4);
                        }

                        NPC.netUpdate = true;
                    }

                    break;
                }

                //go to the top of the arena and sneeze out a stream of boogers
                case 1:
                {
                    NPC.localAI[0]++;

                    if (NPC.localAI[0] < 60)
                    {
                        CurrentFrameX = 0;
                    }

                    if (NPC.localAI[0] < 60)
                    {
                        Vector2 GoTo = new Vector2(Parent.Center.X, Parent.Center.Y - 550);

                        float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 6, 12);
                        NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
                    }

                    if (NPC.localAI[0] == 60)
                    {
                        Sneezing = true;

                        NPC.frame.Y = 0;
                    }

                    if (NPC.localAI[0] >= 60 && NPC.localAI[0] <= 240)
                    {
                        CurrentFrameX = 1;

                        NPC.velocity *= 0.1f;

                        if (NPC.frame.Y >= 3 * NPC.height && NPC.localAI[0] % 10 == 0)
                        {
                            SoundEngine.PlaySound(SneezeSound, NPC.Center);

                            Vector2 ShootSpeed = player.Center - NPC.Center;
                            ShootSpeed.Normalize();
                            ShootSpeed.X *= Main.rand.NextFloat(12f, 17f);
                            ShootSpeed.Y *= Main.rand.NextFloat(12f, 17f);

                            Projectile.NewProjectile(NPC.GetSource_FromAI(), new Vector2(NPC.Center.X, NPC.Center.Y - 50), ShootSpeed, 
                            ModContent.ProjectileType<NoseCultistGruntSnot>(), NPC.damage / 3, 0f, Main.myPlayer);
                        }
                    }

                    if (NPC.localAI[0] > 240)
                    {
                        CurrentFrameX = 0;
                    }

                    if (NPC.localAI[0] >= 300)
                    {
                        Sneezing = false;

                        NPC.localAI[0] = 0;
                        NPC.ai[0] = 0;

                        NPC.netUpdate = true;
                    }

                    break;
                }

                //cast orbiting snot balls and then fly toward the player slowly, after a few seconds launch the oribiting boogers
                case 2:
                {
                    NPC.localAI[0]++;

                    if (NPC.localAI[0] < 60)
                    {
                        CurrentFrameX = 0;
                    }

                    if (NPC.localAI[0] < 60)
                    {
                        Vector2 GoTo = new Vector2(Parent.Center.X, Parent.Center.Y - 85);

                        float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 6, 12);
                        NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
                    }

                    if (NPC.localAI[0] == 60)
                    {
                        Casting = true;
                        
                        CurrentFrameX = 2;

                        NPC.frame.Y = 0;
                    }

                    if (NPC.localAI[0] == 160)
                    {
                        for (int numOrbiters = 0; numOrbiters < 4; numOrbiters++)
                        {
                            int distance = 360 / 4;
                            NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<OrbitingBooger>(), NPC.whoAmI, NPC.whoAmI, numOrbiters * distance);
                        }
                    }

                    if ((NPC.localAI[0] > 60 && NPC.localAI[0] < 180) || NPC.localAI[0] >= 330)
                    {
                        NPC.velocity *= 0.1f;
                    }

                    if (NPC.localAI[0] >= 180 && NPC.localAI[0] < 330)
                    {   
                        Casting = false;

                        CurrentFrameX = 0;

                        Vector2 GoTo = player.Center;

                        float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 1, 2);
                        NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
                    }

                    if (NPC.localAI[0] >= 550)
                    {
                        Casting = false;

                        NPC.localAI[0] = 0;
                        NPC.ai[0] = 0;

                        NPC.netUpdate = true;
                    }

                    break;
                }

                //charge at the player and get stunned after hitting a wall
                case 3:
                {
                    NPC.localAI[0]++;

                    if (NPC.localAI[0] < 70) 
                    {
                        NPC.noTileCollide = false;

                        Vector2 GoTo = player.Center;
                        GoTo.X += (NPC.Center.X < player.Center.X) ? -420 : 420;
                        GoTo.Y -= 20;

                        float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 6, 10);
                        NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
                    }

                    if (NPC.localAI[0] == 70)
                    {
                        NPC.velocity *= 0f;

                        SaveDirection = NPC.direction;
                    }

                    if (NPC.localAI[0] == 75)
                    {
                        SoundEngine.PlaySound(SoundID.DD2_JavelinThrowersAttack, NPC.Center);

                        CurrentFrameX = 1;

                        Charging = true;

                        int ChargeSpeed = 18;

                        Vector2 ChargeDirection = player.Center - NPC.Center;
                        ChargeDirection.Normalize();
                                
                        ChargeDirection.X *= ChargeSpeed;
                        ChargeDirection.Y *= ChargeSpeed / 5f;
                        NPC.velocity.X = ChargeDirection.X;
                        NPC.velocity.Y = ChargeDirection.Y;
                    }

                    if (NPC.localAI[0] >= 75)
                    {
                        NPC.spriteDirection = SaveDirection;
                    }

                    if (NPC.localAI[0] >= 85)
                    {
                        //collide with walls and play a sound
                        if (!hasCollidedWithWall && (NPC.oldVelocity.X >= 5 || NPC.oldVelocity.X <= -5) && (NPC.collideX || NPC.velocity == Vector2.Zero))
                        {
                            SoundEngine.PlaySound(SoundID.NPCDeath43 with { Volume = SoundID.NPCDeath43.Volume * 0.35f }, NPC.Center);

                            CurrentFrameX = 2;

                            SpookyPlayer.ScreenShakeAmount = 8;

                            NPC.velocity *= 0;

                            NPC.noGravity = false;

                            hasCollidedWithWall = true;
                        }
                    }

                    if (NPC.localAI[0] >= 240)
                    {
                        Charging = false;
                        hasCollidedWithWall = false;

                        NPC.localAI[0] = 0;
                        NPC.ai[0] = 0;

                        NPC.noGravity = true;
                        NPC.noTileCollide = true;

                        NPC.netUpdate = true;
                    }

                    break;
                }

                //special attack, summon a nose amalgam, and summon multiple after reaching half hp
                case 4:
                {
                    NPC.localAI[0]++;

                    if (NPC.localAI[0] < 60)
                    {
                        CurrentFrameX = 0;
                    }

                    if (NPC.localAI[0] < 60)
                    {
                        Vector2 GoTo = new Vector2(Parent.Center.X, Parent.Center.Y - 150);

                        float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 6, 12);
                        NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
                    }
                    else
                    {
                        NPC.velocity *= 0.1f;
                    }

                    if (NPC.localAI[0] == 60)
                    {
                        Casting = true;
                        
                        CurrentFrameX = 2;

                        NPC.frame.Y = 0;
                    }

                    if (NPC.localAI[0] == 120 || NPC.localAI[0] == 180)
                    {
                        SoundEngine.PlaySound(SneezeSound, NPC.Center);

                        int[] Types = new int[] { ModContent.ProjectileType<NoseBallPurpleProj>(), ModContent.ProjectileType<NoseBallRedProj>() };

						if (Main.netMode != NetmodeID.MultiplayerClient)
						{
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + Main.rand.Next(-35, 36), NPC.Center.Y - 50, 
							Main.rand.Next(-10, 11), Main.rand.Next(-12, -2), Main.rand.Next(Types), 0, 0f, Main.myPlayer);
						}
                    }

                    if (NPC.localAI[0] >= 200)
                    {
                        Casting = false;

                        CurrentFrameX = 0;
                    }

                    if (NPC.localAI[0] >= 350)
                    {
                        NPC.localAI[0] = 0;
                        NPC.localAI[1] = 0;
                        NPC.ai[0] = 0;

                        NPC.netUpdate = true;
                    }
                
                    break;
                }

                //spawn cultists to assist once half health is reached
                case 5:
                {
                    NPC.localAI[0]++;

                    if (NPC.localAI[0] < 60)
                    {
                        CurrentFrameX = 0;

                        Vector2 GoTo = new Vector2(Parent.Center.X, Parent.Center.Y - 150);

                        float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 6, 12);
                        NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
                        
                    }
                    else
                    {
                        NPC.velocity *= 0.95f;
                    }

                    if (NPC.localAI[0] == 60)
                    {
                        Casting = true;
                        
                        CurrentFrameX = 2;

                        NPC.frame.Y = 0;
                    }

                    if (NPC.localAI[0] == 120 || NPC.localAI[0] == 150 || NPC.localAI[0] == 180 || NPC.localAI[0] == 210 || NPC.localAI[0] == 240)
                    {
                        SoundEngine.PlaySound(SoundID.Item167, NPC.Center);

                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + Main.rand.Next(-35, 36), NPC.Center.Y - 50, 
                        Main.rand.Next(-6, 7), Main.rand.Next(-8, -3), ModContent.ProjectileType<NoseCultistEnemySpawner>(), 0, 0f, Main.myPlayer);
                    }

                    if (NPC.localAI[0] >= 360)
                    {
                        Casting = false;

                        HasSpawnedEnemies = true;

                        NPC.localAI[0] = 0;
                        NPC.ai[0] = 0;

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
                for (int numGores = 1; numGores <= 8; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/NoseCultistLeaderGore" + numGores).Type);
                    }
                }

                for (int numGores = 1; numGores <= 2; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/NoseCultistLeaderWingGore" + numGores).Type);
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/NoseCultistLeaderWingGore" + numGores).Type);
                    }
                }

                for (int numGores = 1; numGores <= 6; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/NoseCultistLeaderClothGore" + Main.rand.Next(1, 3)).Type);
                    }
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SnotWings>()));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CottonSwab>()));
        }

        public override void OnKill()
        {
            if (!Flags.MinibossBarrierOpen)
            {
                Flags.MinibossBarrierOpen = true;

                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.WorldData);
                }
            }

            NPC.SetEventFlagCleared(ref Flags.downedMocoIdol6, -1);
        }
    }
}