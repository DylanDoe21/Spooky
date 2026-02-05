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
using System.Linq;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Biomes;
using Spooky.Content.Items.BossBags;
using Spooky.Content.NPCs.Boss.OldHunter.Projectiles;
using Spooky.Content.NPCs.Friendly;
using Spooky.Content.Tiles.Relic;

namespace Spooky.Content.NPCs.Boss.OldHunter
{
    [AutoloadBossHead]
	public class OldHunterBoss : ModNPC
	{
        int CurrentFrameX = 0;
        int AmmoType = 0;

        bool Phase2 = false;

        public enum AnimationState
		{
			Idle, Walking, WalkBackwards, Jumping, HoldAmmo, Shoot, ShotProjectile, JumpShoot
		}

        private AnimationState CurrentAnimation
        {
			get => (AnimationState)NPC.ai[3];
			set => NPC.ai[3] = (float)value;
		}

        private static Asset<Texture2D> NPCTexture;
        private static Asset<Texture2D> ArmShootFrontTexture;
        private static Asset<Texture2D> ArmShotProjFrontTexture;
        private static Asset<Texture2D> ArmShootBackTexture;
        private static Asset<Texture2D> AmmoHoldTexture;

        public static readonly SoundStyle UseSound = new("Spooky/Content/Sounds/SlingshotDraw", SoundType.Sound);
        public static readonly SoundStyle ShootSound = new("Spooky/Content/Sounds/SlingshotShoot", SoundType.Sound);

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 9;

            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Position = new Vector2(0f, 0f),
                PortraitPositionXOverride = 0f,
                PortraitPositionYOverride = 22f,
				Velocity = 1f
			};
		}

        public override void SendExtraAI(BinaryWriter writer)
        {
            //bools
            writer.Write(Phase2);

            //floats
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
            writer.Write(NPC.localAI[2]);
            writer.Write(NPC.localAI[3]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            //bools
            Phase2 = reader.ReadBoolean();

            //floats
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            NPC.localAI[2] = reader.ReadSingle();
            NPC.localAI[3] = reader.ReadSingle();
        }

		public override void SetDefaults()
		{
            NPC.lifeMax = 25500;
            NPC.damage = 52;
			NPC.defense = 5;
			NPC.width = 50;
			NPC.height = 56;
            NPC.npcSlots = 8f;
			NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 6, 0, 0);
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = false;
            NPC.netAlways = true;
            NPC.boss = true;
			NPC.HitSound = SoundID.NPCHit2;
			NPC.aiStyle = -1;
            Music = MusicID.Boss1;
			SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpiderCaveBiome>().Type };
		}

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
		{
			NPC.lifeMax = (int)(NPC.lifeMax * 0.75f * balance * bossAdjustment);
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.OldHunter"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpiderCaveBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            NPCTexture ??= ModContent.Request<Texture2D>(Texture);
            ArmShootFrontTexture ??= ModContent.Request<Texture2D>(Texture + "SlingArmFront");
            ArmShotProjFrontTexture ??= ModContent.Request<Texture2D>(Texture + "SlingArmFrontShot");
            ArmShootBackTexture ??= ModContent.Request<Texture2D>(Texture + "SlingArmBack");
            AmmoHoldTexture ??= ModContent.Request<Texture2D>(Texture + "AmmoHold");

            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            //rotation stuff
            Player player = Main.player[NPC.target];
            Vector2 vector = new Vector2(NPC.Center.X, NPC.Center.Y);
            float RotateX = player.Center.X - vector.X;
            float RotateY = player.Center.Y - vector.Y;
			float ArmRotation = (float)Math.Atan2((double)RotateY, (double)RotateX) + (NPC.spriteDirection == 1 ? MathHelper.Pi : MathHelper.TwoPi);

			if (CurrentFrameX == 1 && CurrentAnimation == AnimationState.Shoot)
            {
                spriteBatch.Draw(ArmShootBackTexture.Value, NPC.Center - screenPos + new Vector2(0, NPC.gfxOffY + 4), 
				NPC.frame, NPC.GetAlpha(drawColor), ArmRotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            }

            spriteBatch.Draw(NPCTexture.Value, NPC.Center - screenPos + new Vector2(0, NPC.gfxOffY + 4), NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

            if (CurrentFrameX == 1 && (CurrentAnimation == AnimationState.Shoot || CurrentAnimation == AnimationState.ShotProjectile))
            {
                spriteBatch.Draw(CurrentAnimation == AnimationState.ShotProjectile ? ArmShotProjFrontTexture.Value : ArmShootFrontTexture.Value, 
                NPC.Center - screenPos + new Vector2(0, NPC.gfxOffY + 4), NPC.frame, drawColor, ArmRotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            }

            if (CurrentFrameX == 0 && CurrentAnimation == AnimationState.HoldAmmo)
            {
                Rectangle AmmoFrame = new Rectangle(0, 28 * AmmoType, 28, 28);

                Vector2 drawOrigin = new Vector2(AmmoHoldTexture.Width() * 0.5f, (AmmoHoldTexture.Height() / 7) * 0.5f);

                spriteBatch.Draw(AmmoHoldTexture.Value, NPC.Center - screenPos + new Vector2(25 * -NPC.spriteDirection, NPC.gfxOffY - 4), 
				AmmoFrame, NPC.GetAlpha(drawColor), NPC.rotation, drawOrigin, NPC.scale, effects, 0);
                spriteBatch.Draw(AmmoHoldTexture.Value, NPC.Center - screenPos + new Vector2(25 * -NPC.spriteDirection, NPC.gfxOffY - 4), 
				AmmoFrame, NPC.GetAlpha(Color.White * 0.75f), NPC.rotation, drawOrigin, NPC.scale, effects, 0);
            }

            return false;
        }
        
        public override void FindFrame(int frameHeight)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                NPC.frame.Width = TextureAssets.Npc[NPC.type].Width() / 2;
            }

            NPC.frame.X = (int)(NPC.frame.Width * CurrentFrameX);

            if (CurrentAnimation == AnimationState.Idle)
			{
                NPC.frame.Y = 0;
            }
            else if (CurrentAnimation == AnimationState.Walking)
			{
                NPC.frameCounter++;
                if (NPC.frameCounter > 6 - (NPC.velocity.X > 0 ? NPC.velocity.X : -NPC.velocity.X))
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0;
                }

                if (NPC.frame.Y >= frameHeight * 7)
                {
                    NPC.frame.Y = 1 * frameHeight;
                }
            }
            else if (CurrentAnimation == AnimationState.WalkBackwards)
			{
                NPC.frameCounter++;
                if (NPC.frameCounter > 6 - (NPC.velocity.X > 0 ? NPC.velocity.X : -NPC.velocity.X))
                {
                    NPC.frame.Y = NPC.frame.Y - frameHeight;
                    NPC.frameCounter = 0;
                }

                if (NPC.frame.Y <= frameHeight * 0)
                {
                    NPC.frame.Y = 6 * frameHeight;
                }
            }
            else if (CurrentAnimation == AnimationState.Jumping)
			{
                NPC.frame.Y = 8 * frameHeight;
            }
            else if (CurrentAnimation == AnimationState.HoldAmmo)
			{
                NPC.frame.Y = 7 * frameHeight;
            }
            else if (CurrentAnimation == AnimationState.Shoot || CurrentAnimation == AnimationState.ShotProjectile)
			{
                NPC.frameCounter++;
                if (NPC.frameCounter > 8)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0;
                }

                if (NPC.frame.Y >= frameHeight * 3)
                {
                    NPC.frame.Y = 2 * frameHeight;
                }
            }
            else if (CurrentAnimation == AnimationState.JumpShoot)
            {
                NPC.frameCounter++;
                if (NPC.frameCounter > 8)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0;
                }

                if (NPC.frame.Y < frameHeight * 4)
                {
                    NPC.frame.Y = 3 * frameHeight;
                }

                if (NPC.frame.Y >= frameHeight * 6)
                {
                    NPC.frame.Y = 5 * frameHeight;
                }
            }
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
		{
			return false;
		}

        public override void AI()
        {
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            Vector2 ArenaOriginPosition = Flags.OldHunterPosition;
			Vector2 PlatformOffset1 = new Vector2(-260, -185);
            Vector2 PlatformOffset2 = new Vector2(-25, -185);
            Vector2 PlatformOffset3 = new Vector2(275, -185);

            //despawn and turn back into friendly npc if the player dies or leaves the biome
            if (player.dead || !player.active || !player.InModBiome(ModContent.GetInstance<SpiderCaveBiome>()))
            {
                int OldHunter = NPC.NewNPC(NPC.GetSource_Death(), (int)NPC.Center.X, (int)NPC.Center.Y + (NPC.height / 2), ModContent.NPCType<NPCs.Friendly.OldHunter>());
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.SyncNPC, number: OldHunter);
                }

                NPC.active = false;

                return;
            }

            //enter phase 2 when below half hp
            if (NPC.life <= (NPC.lifeMax / 2) && !Phase2)
            {
                Phase2 = true;

                NPC.localAI[0] = 0;
                NPC.localAI[1] = 0;
                NPC.localAI[2] = 0;
                NPC.localAI[3] = 0;
                NPC.ai[0] = -2;

                NPC.netUpdate = true;
            }

			//old hunter should have gravity but manually because of his jumping behavior
            if (!NPCGlobalHelper.IsCollidingWithFloor(NPC))
            {
                NPC.velocity.Y += 0.3f;
                if (NPC.velocity.Y > 2f)
                {
                    NPC.velocity.Y += 0.6f;
                }
            }

            if (NPC.alpha == 255)
            {
                NPC.alpha = 0;
            }

            switch ((int)NPC.ai[0])
            {
                //phase 2 transition
                case -2:
                {
                    NPC.spriteDirection = -NPC.direction;

                    NPC.localAI[0]++;

                    if (NPC.localAI[0] == 1)
                    {
                        NPC.immortal = true;
                        NPC.dontTakeDamage = true;
                    }

                    if (NPC.localAI[0] == 120)
                    {
                        CombatText.NewText(NPC.getRect(), Color.Red, "Alright.", true);
                    }

                    if (NPC.localAI[0] == 240)
                    {
                        CombatText.NewText(NPC.getRect(), Color.Red, "Its time for phase two.", true);
                    }

                    if (NPC.localAI[0] >= 360)
                    {
                        NPC.immortal = false;
                        NPC.dontTakeDamage = false;

                        NPC.localAI[0] = 0;
                        NPC.ai[0]++;

                        NPC.netUpdate = true;
                    }

                    break;
                }

                //spawn intro
                case -1:
                {
                    NPC.spriteDirection = -NPC.direction;

                    NPC.localAI[0]++;

                    if (NPC.localAI[0] == 1)
                    {
                        NPC.immortal = true;
                        NPC.dontTakeDamage = true;
                    }

                    if (NPC.localAI[0] == 120)
                    {
                        CombatText.NewText(NPC.getRect(), Color.Red, "Hi!", true);
                    }

                    if (NPC.localAI[0] == 240)
                    {
                        CombatText.NewText(NPC.getRect(), Color.Red, "Heres my placeholder spawn intro dialogue.", true);
                    }

                    if (NPC.localAI[0] == 360)
                    {
                        CombatText.NewText(NPC.getRect(), Color.Red, "Ok lets fight.", true);
                    }

                    if (NPC.localAI[0] >= 480)
                    {
                        NPC.immortal = false;
                        NPC.dontTakeDamage = false;

                        NPC.localAI[0] = 0;
                        NPC.ai[0]++;

                        NPC.netUpdate = true;
                    }

                    break;
                }

                //walk at the player
                case 0:
                {
                    NPC.localAI[0]++;

                    CurrentAnimation = AnimationState.Walking;

                    NPC.spriteDirection = NPC.direction = NPC.velocity.X >= 0 ? -1 : 1;

                    WalkTowardsTarget(player.Center, 2f, 0.15f, 50, false);

                    if (NPC.localAI[0] >= 180)
                    {
                        NPC.localAI[0] = 0;
                        NPC.ai[0]++;

                        NPC.netUpdate = true;
                    }

                    break;
                }

                //jump to a platform in the arena and shoot a projectile
                case 1:
                {
                    //jump towards the desired location
                    if (NPC.localAI[1] == 0)
                    {
                        SoundEngine.PlaySound(SoundID.DD2_JavelinThrowersAttack with { Pitch = -0.5f }, NPC.Center);

                        CurrentAnimation = AnimationState.Jumping;
                        
                        NPC.spriteDirection = NPC.direction = NPC.velocity.X >= 0 ? -1 : 1;

                        Vector2 offset = Vector2.Zero;

                        //randomize the platform to jump to depending on where the player is
                        if (player.Center.X >= ArenaOriginPosition.X)
                        {
                            switch (Main.rand.Next(2))
                            {
                                case 0:
                                {
                                    offset = PlatformOffset1;
                                    break;
                                }
                                case 1:
                                {
                                    offset = PlatformOffset2;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            switch (Main.rand.Next(2))
                            {
                                case 0:
                                {
                                    offset = PlatformOffset2;
                                    break;
                                }
                                case 1:
                                {
                                    offset = PlatformOffset3;
                                    break;
                                }
                            }
                        }

                        Vector2 GoTo = ArenaOriginPosition + offset;

                        NPC.velocity = NPCGlobalHelper.GetArcVelocity(NPC, GoTo, 0.3f, 320, 350, maxXvel: 12);

                        NPC.localAI[1]++;

                        NPC.netUpdate = true;
                    }
                    //land and then start shooting behavior
                    if (NPC.localAI[1] == 1)
                    {
                        NPC.spriteDirection = NPC.direction = NPC.velocity.X >= 0 ? -1 : 1;

                        NPC.localAI[2]++;
                        if ((NPCGlobalHelper.IsCollidingWithFloor(NPC) || (NPC.velocity.Y > 0 && NPC.collideY)) && NPC.localAI[2] >= 10)
                        {
                            NPC.spriteDirection = -NPC.direction;

                            AmmoType = Main.rand.Next(2);

                            NPC.velocity = Vector2.Zero;
                            NPC.localAI[1]++;

                            NPC.netUpdate = true;
                        }
                    }
                    //shooting projectile with animations and stuff
                    int Repeats = Phase2 ? 3 : 2;
                    if (NPC.localAI[1] >= Repeats)
                    {
                        NPC.spriteDirection = -NPC.direction;

                        NPC.localAI[0]++;

                        //hold ammo for a second before shooting
                        if (NPC.localAI[0] < 59)
                        {
                            CurrentAnimation = AnimationState.HoldAmmo;
                        }

                        //repet 3 times shooting either an ice or sticky spike ammo
                        if (NPC.localAI[3] <= 2)
                        {
                            //begin shooting animation and sound
                            if (NPC.localAI[0] == 60)
                            {
                                SoundEngine.PlaySound(UseSound, NPC.Center);

                                NPC.frame.Y = 0;
                                CurrentFrameX = 1;
                                CurrentAnimation = AnimationState.Shoot;
                            }

                            //shoot projectile
                            if (NPC.localAI[0] == 140)
                            {
                                SoundEngine.PlaySound(ShootSound, NPC.Center);

                                CurrentFrameX = 1;
                                CurrentAnimation = AnimationState.ShotProjectile;

                                int AmmoToShoot = 0;
                                float ProjSpeed = 0f;

                                switch (AmmoType)
                                {
                                    case 0:
                                    {
                                        AmmoToShoot = ModContent.ProjectileType<SlingshotIceBall>();
                                        ProjSpeed = Phase2 ? 20f : 16f;
                                        break;
                                    }
                                    case 1:
                                    {
                                        AmmoToShoot = ModContent.ProjectileType<SlingshotLingerBall>();
                                        ProjSpeed = Phase2 ? 11f : 8f;
                                        break;
                                    }
                                }

                                Vector2 ShootSpeed = player.Center - NPC.Center;
                                ShootSpeed.Normalize();
                                ShootSpeed *= ProjSpeed;

                                Vector2 Offset = Vector2.Normalize(new Vector2(ShootSpeed.X, ShootSpeed.Y)) * 40f;
                                Vector2 position = NPC.Center;

                                if (Collision.CanHit(position, 0, 0, position + Offset, 0, 0))
                                {
                                    position += Offset;
                                }

                                if (AmmoToShoot == ModContent.ProjectileType<SlingshotLingerBall>())
                                {
                                    for (int numProjs = 0; numProjs <= 3; numProjs++)
                                    {
                                        Vector2 newVelocity = ShootSpeed.RotatedByRandom(MathHelper.ToRadians(42));

                                        NPCGlobalHelper.ShootHostileProjectile(NPC, position, newVelocity, AmmoToShoot, NPC.damage, 4.5f);
                                    }
                                }
                                else
                                {
                                    NPCGlobalHelper.ShootHostileProjectile(NPC, position, ShootSpeed, AmmoToShoot, NPC.damage, 4.5f);
                                }

                                NPC.localAI[0] = 59;
                                NPC.localAI[3]++;

                                NPC.netUpdate = true;
                            }
                        }
                        else
                        {
                            if (NPC.localAI[0] >= 30)
                            {
                                CurrentFrameX = 0;
                                CurrentAnimation = AnimationState.Idle;

                                NPC.localAI[0] = 0;
                                NPC.localAI[1] = 0;
                                NPC.localAI[2] = 0;
                                NPC.localAI[3] = 0;
                                NPC.ai[0]++;
                                
                                NPC.netUpdate = true;
                            }
                        }
                    }

                    break;
                }

                case 2:
                {
                    goto case 0;
                }

                //jump up, then fire at the player directly
                case 3:
                {
                    NPC.spriteDirection = -NPC.direction;

                    //hold up ammo before jumping
                    if (NPC.localAI[1] == 0)
                    {
                        NPC.velocity.X = 0;

                        NPC.localAI[2]++;
                        if (NPC.localAI[2] <= 1)
                        {
                            AmmoType = 2; //bouncy ammo
                        }
                        if (NPC.localAI[2] > 1 && NPC.localAI[2] < 60)
                        {
                            CurrentAnimation = AnimationState.HoldAmmo;
                        }
                        if (NPC.localAI[2] >= 60)
                        {
                            NPC.localAI[1]++;

                            NPC.netUpdate = true;
                        }
                    }
                    //actual jumping
                    if (NPC.localAI[1] == 1)
                    {
                        SoundEngine.PlaySound(SoundID.DD2_JavelinThrowersAttack with { Pitch = -0.5f }, NPC.Center);

                        CurrentFrameX = 1;
                        CurrentAnimation = AnimationState.JumpShoot;

                        NPC.velocity = NPCGlobalHelper.GetArcVelocity(NPC, new Vector2(player.Center.X, ArenaOriginPosition.Y), 0.3f, 325, 350, maxXvel: 12);

                        NPC.localAI[1]++;

                        NPC.netUpdate = true;
                    }
                    //shooting projectile with animations and stuff
                    if (NPC.localAI[1] >= 2)
                    {
                        NPC.localAI[0]++;

                        if (NPC.localAI[0] == 1)
                        {
                            SoundEngine.PlaySound(UseSound, NPC.Center);
                        }

                        if (NPC.localAI[0] < 50)
                        {
                            Vector2 vector = new Vector2(NPC.Center.X, NPC.Center.Y);
                            float RotateX = player.Center.X - vector.X;
                            float RotateY = player.Center.Y - vector.Y;
                            NPC.rotation = (float)Math.Atan2((double)RotateY, (double)RotateX) + (NPC.spriteDirection == 1 ? MathHelper.Pi : MathHelper.TwoPi);
                        }

                        if (NPC.localAI[0] > 30 && NPC.localAI[0] < 50)
                        {
                            NPC.velocity *= 0.98f;
                        }

                        if (NPC.localAI[0] == 50)
                        {
                            SoundEngine.PlaySound(ShootSound, NPC.Center);

                            CurrentFrameX = 0;
                            CurrentAnimation = AnimationState.Jumping;

                            NPC.rotation = 0;

                            int AmmoToShoot = 0;
                            float ProjSpeed = 0f;

                            AmmoToShoot = ModContent.ProjectileType<SlingshotBouncyBall>();

                            Vector2 ShootSpeed = player.Center - NPC.Center;
                            ShootSpeed.Normalize();
                            ShootSpeed *= 12f;

                            Vector2 Offset = Vector2.Normalize(new Vector2(ShootSpeed.X, ShootSpeed.Y)) * 40f;
                            Vector2 position = NPC.Center;

                            if (Collision.CanHit(position, 0, 0, position + Offset, 0, 0))
                            {
                                position += Offset;
                            }

                            NPCGlobalHelper.ShootHostileProjectile(NPC, position, ShootSpeed, AmmoToShoot, NPC.damage, 4.5f);
                        }

                        if (NPC.localAI[0] >= 80)
                        {
                            CurrentAnimation = AnimationState.Idle;

                            NPC.localAI[0] = 0;
                            NPC.localAI[1] = 0;
                            NPC.localAI[2] = 0;
                            NPC.ai[0]++;
                            
                            NPC.netUpdate = true;
                        }
                    }

                    break;
                }

                //fire grenades because its funny
                case 4:
                {
                    //walk at the target for a bit
                    if (NPC.localAI[1] == 0)
                    {   
                        AmmoType = 3; //grenade

                        CurrentAnimation = AnimationState.Walking;

                        NPC.spriteDirection = NPC.direction = NPC.velocity.X >= 0 ? -1 : 1;
                        
                        WalkTowardsTarget(player.Center, 2f, 0.15f, 50, false);

                        NPC.localAI[2]++;
                        if (NPC.localAI[2] >= 45)
                        {
                            NPC.localAI[1]++;

                            NPC.netUpdate = true;
                        }
                    }
                    //shooting projectile with animations and stuff
                    if (NPC.localAI[1] > 0)
                    {
                        NPC.spriteDirection = -NPC.direction;

                        NPC.velocity.X = 0;

                        NPC.localAI[0]++;

                        //hold ammo for a second before shooting
                        if (NPC.localAI[0] < 59)
                        {
                            CurrentAnimation = AnimationState.HoldAmmo;
                        }

                        //repet 2 times shooting a grenade (or grenade bundle in phase 2)
                        if (NPC.localAI[3] <= 3)
                        {
                            //begin shooting animation and sound
                            if (NPC.localAI[0] == 60)
                            {
                                SoundEngine.PlaySound(UseSound, NPC.Center);

                                NPC.frame.Y = 0;
                                CurrentFrameX = 1;
                                CurrentAnimation = AnimationState.Shoot;
                            }

                            //shoot projectile
                            if (NPC.localAI[0] == 110)
                            {
                                SoundEngine.PlaySound(ShootSound, NPC.Center);

                                CurrentFrameX = 1;
                                CurrentAnimation = AnimationState.ShotProjectile;

                                Vector2 ShootSpeed = player.Center - NPC.Center;
                                ShootSpeed.Normalize();
                                ShootSpeed *= 10f;

                                Vector2 Offset = Vector2.Normalize(new Vector2(ShootSpeed.X, ShootSpeed.Y)) * 40f;
                                Vector2 position = NPC.Center;

                                if (Collision.CanHit(position, 0, 0, position + Offset, 0, 0))
                                {
                                    position += Offset;
                                }

                                NPCGlobalHelper.ShootHostileProjectile(NPC, position, ShootSpeed, ModContent.ProjectileType<SlingshotGrenade>(), NPC.damage, 4.5f);

                                NPC.localAI[0] = 59;
                                NPC.localAI[3]++;

                                NPC.netUpdate = true;
                            }
                        }
                        else
                        {
                            if (NPC.localAI[0] >= 30)
                            {
                                CurrentFrameX = 0;
                                CurrentAnimation = AnimationState.Idle;

                                NPC.localAI[0] = 0;
                                NPC.localAI[1] = 0;
                                NPC.localAI[2] = 0;
                                NPC.localAI[3] = 0;
                                NPC.ai[0] = 0;
                                
                                NPC.netUpdate = true;
                            }
                        }
                    }

                    break;
                }

                //jump up to platform, then jump between platforms and fire special ammos
                case 5:
                {
                    //jump towards the desired location
                    if (NPC.localAI[1] == 0)
                    {
                        SoundEngine.PlaySound(SoundID.DD2_JavelinThrowersAttack with { Pitch = -0.5f }, NPC.Center);

                        CurrentAnimation = AnimationState.Jumping;
                        
                        NPC.spriteDirection = NPC.direction = NPC.velocity.X >= 0 ? -1 : 1;

                        Vector2 offset = Vector2.Zero;

                        //randomize the platform to jump to depending on where the player is
                        if (player.Center.X >= ArenaOriginPosition.X)
                        {
                            switch (Main.rand.Next(2))
                            {
                                case 0:
                                {
                                    offset = PlatformOffset1;
                                    break;
                                }
                                case 1:
                                {
                                    offset = PlatformOffset2;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            switch (Main.rand.Next(2))
                            {
                                case 0:
                                {
                                    offset = PlatformOffset2;
                                    break;
                                }
                                case 1:
                                {
                                    offset = PlatformOffset3;
                                    break;
                                }
                            }
                        }

                        Vector2 GoTo = ArenaOriginPosition + offset;

                        NPC.velocity = NPCGlobalHelper.GetArcVelocity(NPC, GoTo, 0.3f, 320, 350, maxXvel: 12);

                        NPC.localAI[1]++;

                        NPC.netUpdate = true;
                    }
                    //land and then start shooting behavior
                    if (NPC.localAI[1] == 1)
                    {
                        NPC.spriteDirection = NPC.direction = NPC.velocity.X >= 0 ? -1 : 1;

                        NPC.localAI[2]++;
                        if ((NPCGlobalHelper.IsCollidingWithFloor(NPC) || (NPC.velocity.Y > 0 && NPC.collideY)) && NPC.localAI[2] >= 10)
                        {
                            NPC.spriteDirection = -NPC.direction;

                            AmmoType = Main.rand.Next(2);

                            NPC.velocity = Vector2.Zero;
                            NPC.localAI[1]++;

                            NPC.netUpdate = true;
                        }
                    }
                    //shooting projectile with animations and stuff
                    if (NPC.localAI[1] >= 2)
                    {
                        NPC.spriteDirection = -NPC.direction;

                        NPC.localAI[0]++;

                        //hold ammo for a second before shooting
                        if (NPC.localAI[0] < 59)
                        {
                            CurrentAnimation = AnimationState.HoldAmmo;
                        }

                        //repet 3 times shooting either an ice or sticky spike ammo
                        if (NPC.localAI[3] <= 2)
                        {
                            //begin shooting animation and sound
                            if (NPC.localAI[0] == 60)
                            {
                                SoundEngine.PlaySound(UseSound, NPC.Center);

                                NPC.frame.Y = 0;
                                CurrentFrameX = 1;
                                CurrentAnimation = AnimationState.Shoot;
                            }

                            //shoot projectile
                            if (NPC.localAI[0] == 140)
                            {
                                SoundEngine.PlaySound(ShootSound, NPC.Center);

                                CurrentFrameX = 1;
                                CurrentAnimation = AnimationState.ShotProjectile;

                                int AmmoToShoot = 0;
                                float ProjSpeed = 0f;

                                switch (AmmoType)
                                {
                                    case 0:
                                    {
                                        AmmoToShoot = ModContent.ProjectileType<SlingshotIceBall>();
                                        ProjSpeed = 18f;
                                        break;
                                    }
                                    case 1:
                                    {
                                        AmmoToShoot = ModContent.ProjectileType<SlingshotLingerBall>();
                                        ProjSpeed = 9f;
                                        break;
                                    }
                                }

                                Vector2 ShootSpeed = player.Center - NPC.Center;
                                ShootSpeed.Normalize();
                                ShootSpeed *= ProjSpeed;

                                Vector2 Offset = Vector2.Normalize(new Vector2(ShootSpeed.X, ShootSpeed.Y)) * 40f;
                                Vector2 position = NPC.Center;

                                if (Collision.CanHit(position, 0, 0, position + Offset, 0, 0))
                                {
                                    position += Offset;
                                }

                                if (AmmoToShoot == ModContent.ProjectileType<SlingshotLingerBall>())
                                {
                                    for (int numProjs = 0; numProjs <= 3; numProjs++)
                                    {
                                        Vector2 newVelocity = ShootSpeed.RotatedByRandom(MathHelper.ToRadians(42));

                                        NPCGlobalHelper.ShootHostileProjectile(NPC, position, newVelocity, AmmoToShoot, NPC.damage, 4.5f);
                                    }
                                }
                                else
                                {
                                    NPCGlobalHelper.ShootHostileProjectile(NPC, position, ShootSpeed, AmmoToShoot, NPC.damage, 4.5f);
                                }

                                NPC.localAI[0] = 59;
                                NPC.localAI[3]++;

                                NPC.netUpdate = true;
                            }
                        }
                        else
                        {
                            if (NPC.localAI[0] >= 30)
                            {
                                CurrentFrameX = 0;
                                CurrentAnimation = AnimationState.Idle;

                                NPC.localAI[0] = 0;
                                NPC.localAI[1] = 0;
                                NPC.localAI[2] = 0;
                                NPC.localAI[3] = 0;
                                NPC.ai[0]++;
                                
                                NPC.netUpdate = true;
                            }
                        }
                    }

                    break;
                }
            }
        }

        public void WalkTowardsTarget(Vector2 Center, float MaxSpeed, float Acceleration, int Distance, bool Backwards)
        {
            Player player = Main.player[NPC.target];

            //prevents the pet from getting stuck on sloped tiles
            Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY);

            Vector2 center2 = NPC.Center;
            Vector2 vector48 = Center - center2;
            float CenterDistance = vector48.Length();

            if ((NPC.velocity.X < 0f && NPC.direction == -1) || (NPC.velocity.X > 0f && NPC.direction == 1))
            {
                if (NPC.velocity.Y == 0 && Collision.SolidTilesVersatile((int)(NPC.Center.X / 16f), (int)(NPC.Center.X + NPC.spriteDirection * 65) / 16, (int)NPC.Top.Y / 16, (int)NPC.Bottom.Y / 16 - 3))
                {
                    NPC.velocity.Y = -10f;
                    NPC.netUpdate = true;
                }
            }

            if (NPC.collideX)
            {
                NPC.velocity.X = -NPC.velocity.X;
            }

            NPC.velocity.Y += 0.15f;

            if (NPC.velocity.Y > 10f)
            {
                NPC.velocity.Y = 10f;
            }

            if (!Backwards)
            {
                if (center2.X < player.Center.X)
                {
                    NPC.velocity.X += Acceleration;
                    if (NPC.velocity.X > MaxSpeed)
                    {
                        NPC.velocity.X = MaxSpeed;
                    }
                }
                else
                {
                    NPC.velocity.X -= Acceleration;
                    if (NPC.velocity.X < -MaxSpeed)
                    {
                        NPC.velocity.X = -MaxSpeed;
                    }
                }
            }
            else
            {
                if (center2.X < player.Center.X)
                {
                    NPC.velocity.X -= Acceleration;
                    if (NPC.velocity.X < -MaxSpeed)
                    {
                        NPC.velocity.X = -MaxSpeed;
                    }
                }
                else
                {
                    NPC.velocity.X += Acceleration;
                    if (NPC.velocity.X > MaxSpeed)
                    {
                        NPC.velocity.X = MaxSpeed;
                    }
                }
            }
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 4; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        //Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/PeacockSpiderBlueGore" + numGores).Type);
                    }
                }
            }
        }

        //Loot and stuff
        public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
            //LeadingConditionRule notExpertRule = new(new Conditions.NotExpert());

			//treasure bag
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<BossBagOldHunter>()));

            //master relic and pet
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<OldHunterRelicItem>()));
        }

        public override void OnKill()
        {
            if (!Flags.downedOldHunter)
			{
				Flags.GuaranteedRaveyard = true;

				if (Main.netMode == NetmodeID.Server)
				{
					NetMessage.SendData(MessageID.WorldData);
				}
			}

            NPC.SetEventFlagCleared(ref Flags.downedOldHunter, -1);

            if (!MenuSaveSystem.hasDefeatedOldHunter)
			{
				MenuSaveSystem.hasDefeatedOldHunter = true;
			}
        }

        public override void BossLoot(ref string name, ref int potionType)
		{
			potionType = ModContent.ItemType<CranberryJelly>();
		}
    }
}