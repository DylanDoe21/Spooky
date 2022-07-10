using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Content.Buffs.Pets;
 
namespace Spooky.Content.Projectiles.Pets;

public class OrroboroMount : ModMount
{
    public override void SetStaticDefaults()
    {
        MountData.totalFrames = 4;
        MountData.runSpeed = 7f;
        MountData.dashSpeed = 7f;
        MountData.jumpSpeed = 10f;
        MountData.jumpHeight = 10;
        MountData.heightBoost = 10;
        MountData.fallDamage = 0f;
        MountData.flightTimeMax = 120;
        MountData.fatigueMax = 120;
        MountData.acceleration = 0.3f;
        MountData.spawnDust = DustID.Blood;
        MountData.spawnDustNoGravity = true;
        MountData.blockExtraJumps = true;
        MountData.usesHover = true;

        MountData.buff = ModContent.BuffType<OrroboroMountBuff>();

        int[] array = new int[MountData.totalFrames];
        for (int l = 0; l < array.Length; l++)
        {
            array[l] = 16;
        }
        MountData.playerYOffsets = array;
        MountData.bodyFrame = 3;
        MountData.playerHeadOffset = 22;


        if (Main.netMode != NetmodeID.Server) 
        {
            MountData.textureWidth = MountData.backTexture.Width();
            MountData.textureHeight = MountData.backTexture.Height();
        }
    }

    public override void UpdateEffects(Player player) //this is like mostly just decompiled vanilla flying mount code because using the default flying mount code did not work for custom animation style iirc
		{
            Lighting.AddLight(player.position, 0f, 0.5f, 1f); 
			player.gravity = 0;
			player.fallStart = (int)(player.position.Y / 16.0);
            float num1 = 0.5f;
            float acc = 0.4f;

            float yvelcap = -1f / 10000f;

			if (player.controlUp || player.controlJump)
            {
				yvelcap = -5f;
				player.velocity.Y -= acc * num1;
            }
            else if (player.controlDown)
            {
				player.velocity.Y += acc * num1;

				if (TileID.Sets.Platforms[Framing.GetTileSafely((int)(player.Center.X / 16), (int)((player.MountedCenter.Y + (player.height / 2)) / 16) + 1).TileType])
                {
					player.position.Y += 1;
                }

				yvelcap = 5f;
            }

            if (player.velocity.Y < yvelcap)
            {
                if (yvelcap - player.velocity.Y < acc)
                {
					player.velocity.Y = yvelcap;
                }
                else
                {
					player.velocity.Y += acc * num1;
                }
            }
            else if (player.velocity.Y > yvelcap)
            {
                if (player.velocity.Y - yvelcap < acc)
                {
					player.velocity.Y = yvelcap;
                }
                else
                {
					player.velocity.Y -= acc * num1;
                }
			}
        }

    public override bool UpdateFrame(Player mountedPlayer, int state, Vector2 velocity)
    {
        mountedPlayer.mount._frameCounter += 0.1f;
        mountedPlayer.mount._frame = (int)(mountedPlayer.mount._frameCounter %= 4);

        return false;
    }
}
