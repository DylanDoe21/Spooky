using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using System.Linq;

using Spooky.Core;
using Spooky.Content.Dusts;
using Spooky.Content.Buffs.Mounts;
using Spooky.Content.NPCs.SpookyBiome;

namespace Spooky.Content.Projectiles.Pets
{
	public class TurkeyMount : ModMount
	{
		int PeckingTimer = 0;
		bool PeckingGround = false;

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
				Main.npc[Turkey].GetGlobalNPC<NPCGlobal>().TurkeyTamed = true;

				if (Main.netMode == NetmodeID.Server)
				{
					NetMessage.SendData(MessageID.SyncNPC, number: Turkey);
				}
			}
		}

		public override void UpdateEffects(Player player)
		{
			if (player.velocity.Y > 3 && !player.controlDown)
			{
				player.velocity.Y = 3;
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

			if (GoingFast && velocity.Y == 0f)
			{
				if (mountedPlayer.cMount == 0)
				{
					mountedPlayer.position += new Vector2(direction * 24f, 0f);
					mountedPlayer.FloorVisuals(true);
					mountedPlayer.position -= new Vector2(direction * 24f, 0f);
				}
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
