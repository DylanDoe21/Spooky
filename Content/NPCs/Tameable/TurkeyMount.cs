using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System;
using System.Linq;

using Spooky.Core;
using Spooky.Content.Dusts;
using Spooky.Content.Buffs.Mounts;

namespace Spooky.Content.NPCs.Tameable
{
	public class TurkeyMount : ModMount
	{
		int PeckingTimer = 0;
		int SoundTimer = 0;
		int FallTimer = 0;

		bool PeckingGround = false;

		public static readonly SoundStyle GobbleSound1 = new("Spooky/Content/Sounds/TurkeyGobble1", SoundType.Sound) { PitchVariance = 0.5f };
		public static readonly SoundStyle GobbleSound2 = new("Spooky/Content/Sounds/TurkeyGobble2", SoundType.Sound) { PitchVariance = 0.5f };
		public static readonly SoundStyle FlapSound = new("Spooky/Content/Sounds/TurkeyFlap", SoundType.Sound) { Volume = 0.2f };

		public override void SetStaticDefaults()
        {
			MountData.runSpeed = 7f; //this is slightly faster than hermes boots
			MountData.flightTimeMax = 0;
			MountData.fallDamage = 0f;
			MountData.jumpHeight = 16;
			MountData.acceleration = 0.035f;
			MountData.jumpSpeed = 7f;
			MountData.swimSpeed = 3f;
			MountData.xOffset = 8;
			MountData.yOffset = 13;
			MountData.playerHeadOffset = 16;
			MountData.heightBoost = 16;
			MountData.totalFrames = 18;
			MountData.playerYOffsets = Enumerable.Repeat(16, MountData.totalFrames).ToArray();
			MountData.buff = ModContent.BuffType<TurkeyMountBuff>();
			MountData.spawnDust = ModContent.DustType<TurkeyFeatherDust>();

			//standing still frames
			MountData.standingFrameStart = 5; //starts at frame 5 (6)
			MountData.standingFrameCount = 1; //has 1 frames total
			MountData.standingFrameDelay = 12;
			//running frames
			MountData.runningFrameStart = 14; //starts at frame 14 (15)
			MountData.runningFrameCount = 4;
			MountData.runningFrameDelay = 5;
			//floating frames
			MountData.inAirFrameStart = 10; //starts at frame 10 (11)
			MountData.inAirFrameCount = 4;
			MountData.inAirFrameDelay = 3;
			//swim frames
			MountData.swimFrameStart = MountData.inAirFrameStart;
			MountData.swimFrameCount = MountData.inAirFrameCount;
			MountData.swimFrameDelay = 12;

			if (Main.netMode != NetmodeID.Server)
            {
                MountData.textureWidth = MountData.backTexture.Width();
                MountData.textureHeight = MountData.backTexture.Height();
            }
        }

		public override void Dismount(Player player, ref bool skipDust)
		{
			//spawn turkey when dismounting
			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				int Turkey = NPC.NewNPC(player.GetSource_ReleaseEntity(), (int)player.Center.X, (int)player.Center.Y + 26, ModContent.NPCType<Turkey>());
				Main.npc[Turkey].GetGlobalNPC<NPCGlobal>().NPCTamed = true;

				if (Main.netMode == NetmodeID.Server)
				{
					NetMessage.SendData(MessageID.SyncNPC, number: Turkey);
				}
			}
		}

		public override void UpdateEffects(Player player)
		{
			if (Main.rand.NextBool(500))
            {
				if (Main.rand.NextBool())
				{
					SoundEngine.PlaySound(GobbleSound1, player.Center);
				}
				else
				{
					SoundEngine.PlaySound(GobbleSound2, player.Center);
				}
			}

			if (player.velocity.Y > 3 && !player.controlDown)
			{
				player.velocity.Y = 3;
			}

			if (player.velocity.Y != 0)
			{
				FallTimer++;
				if (FallTimer > 20)
				{
					if (SoundTimer == 0)
					{
						SoundEngine.PlaySound(FlapSound, player.Center);
						SoundTimer = 15;
					}
					else
					{
						SoundTimer--;
					}
				}
			}
			else
			{
				FallTimer = 0;
				SoundTimer = 0;
			}
		}

		public override bool UpdateFrame(Player mountedPlayer, int state, Vector2 velocity)
        {
            bool GoingFast = Math.Abs(velocity.X) > mountedPlayer.mount.RunSpeed / 1.25f;
            float direction = (float)Math.Sign(mountedPlayer.velocity.X);

			//if not moving fast enough use default walking animation
            if (!GoingFast)
            {
				MountData.runningFrameStart = 0; //starts at frame 0 (1)
				MountData.runningFrameCount = 5;
				MountData.runningFrameDelay = 12;
			}
			//running animation for when its going fast enough
            else
            {
				MountData.runningFrameStart = 14; //starts at frame 14 (15)
				MountData.runningFrameCount = 4;
				MountData.runningFrameDelay = 12;
			}

			//randomly do ground pecking animation while idle
			if (Main.rand.NextBool(200) && velocity.X == 0 && !PeckingGround)
			{
				PeckingGround = true;
			}

			if (PeckingGround && velocity.X == 0)
			{
				PeckingTimer++;
				if (PeckingTimer < 20)
				{
					MountData.standingFrameStart = 5; //starts at frame 5 (6)
					MountData.standingFrameCount = 5;
					MountData.standingFrameDelay = 5;
				}
				else
				{
					MountData.standingFrameStart = 8; //starts at frame 8 (9)
					MountData.standingFrameCount = 2;
					MountData.standingFrameDelay = 4;
				}

				if (PeckingTimer >= 70)
				{
					PeckingTimer = 0;
					PeckingGround = false;
				}
			}
			else
			{
				MountData.standingFrameStart = 5; //starts at frame 5 (6)
				MountData.standingFrameCount = 1; //has 1 frames total
				MountData.standingFrameDelay = 5;

				PeckingTimer = 0;
				PeckingGround = false;
			}

            return true;
        }
	}

	public class TurkeyMountScene : ModSceneEffect
	{
		public override int Music => MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/TurkeyTime");

		public override SceneEffectPriority Priority => SceneEffectPriority.Event;

		public override bool IsSceneEffectActive(Player player) => player.HasBuff(ModContent.BuffType<TurkeyMountBuff>()) && !Main.gameMenu;
	}
}
