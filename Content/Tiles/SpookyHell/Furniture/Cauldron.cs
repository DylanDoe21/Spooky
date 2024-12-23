using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.DataStructures;
using Terraria.ObjectData;
using Terraria.Enums;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Dusts;

namespace Spooky.Content.Tiles.SpookyHell.Furniture
{
	public class Cauldron : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileLighted[Type] = true;
			Main.tileSolid[Type] = false;
			TileID.Sets.PreventsTileRemovalIfOnTopOfIt[Type] = true;
			TileObjectData.newTile.Width = 4;
			TileObjectData.newTile.Height = 4;
			TileObjectData.newTile.Origin = new Point16(2, 3);
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16 };
			TileObjectData.newTile.StyleWrapLimit = 36;
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.addTile(Type);
			LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(0, 128, 0), name);
			DustType = DustID.Stone;
			HitSound = SoundID.Tink;
		}

		public override bool CanExplode(int i, int j)
		{
			return false;
		}

        public override bool CanKillTile(int i, int j, ref bool blockDamaged)
        {
			return false;
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
			Tile tile = Main.tile[i, j];
			if (tile.TileFrameX == 0 && tile.TileFrameY == 0)
			{
				var existing = Main.projectile.Where(n => n.active && n.type == ModContent.ProjectileType<CauldronDummy>()).FirstOrDefault();
				if (existing == default)
				{
					Projectile.NewProjectile(new EntitySource_TileUpdate(i, j), new Vector2(i + 2, j + 2) * 16, Vector2.Zero, ModContent.ProjectileType<CauldronDummy>(), 0, 0);
				}
			}
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
		{
			noBreak = true;
			return true;
		}
    }

	public class CauldronDummy : ModProjectile
	{
		public int shakeTimer = 0;

		Vector2 scaleVec;

        private Asset<Texture2D> GlowTexture;
        private Asset<Texture2D> ProjTexture;

        public override void SetStaticDefaults()
        {
			Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 64;
			Projectile.height = 66;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.friendly = false;
			Projectile.hostile = false;
			Projectile.hide = true;
		}

		public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
		{
			behindNPCsAndTiles.Add(index);
		}

		public override void AI()
        {
			if (shakeTimer-- > 0)
			{
				float sin = shakeTimer * 0.08971428571f * 2;
				scaleVec = new Vector2(MathF.Sin(sin), -MathF.Sin(sin));
			}
			else
			{
				scaleVec = Vector2.Zero;
			}

			Projectile.frameCounter++;
			if (Projectile.frameCounter % 6 == 0)
			{
				Projectile.frame++;
			}
			
			Projectile.frame %= Main.projFrames[Projectile.type];
        }

        public override bool PreDraw(ref Color lightColor)
        {
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);
			GlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpookyHell/Furniture/CauldronDummyGlow");

			int frameHeight = ProjTexture.Height() / Main.projFrames[Projectile.type];
			Rectangle frameBox = new Rectangle(0, frameHeight * Projectile.frame, ProjTexture.Width(), frameHeight);

			Main.spriteBatch.Draw(ProjTexture.Value, Projectile.Bottom - Main.screenPosition, frameBox, lightColor, Projectile.rotation, new Vector2(ProjTexture.Width() / 2, frameHeight), Projectile.scale * (Vector2.One + (0.1f * scaleVec)), SpriteEffects.None, 0f);

			if (shakeTimer <= 0)
			{
				Main.spriteBatch.Draw(GlowTexture.Value, Projectile.Bottom - Main.screenPosition, frameBox, lightColor, Projectile.rotation, new Vector2(GlowTexture.Width() / 2, frameHeight), Projectile.scale * (Vector2.One + (0.1f * scaleVec)), SpriteEffects.None, 0f);
			}

            return false;
        }
    }

	public class CauldronTransformProj : ModProjectile
	{
		public Texture2D inputTex;

		public Texture2D outputTex;

		public int outputItemID;

		public bool transformed = false;

		int timer = 0;

		Vector2 initialPos = Vector2.Zero;

        public override void SetDefaults()
        {
			Projectile.width = 4;
			Projectile.height = 4;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			Projectile.friendly = false;
			Projectile.hide = true;
			Projectile.hostile = false;
        }

        public override void OnSpawn(IEntitySource source)
        {
			initialPos = Projectile.Center;
        }

        public override void AI()
        {
			timer++;

			if (timer > 70 && timer < 200)
			{
				Projectile.scale *= 0;
			}

			if (timer < 50)
			{
				Projectile.Center = initialPos + (new Vector2(0, -10) * EaseFunction.EaseCircularOut.Ease(timer / 50f));
			}
			else if (timer < 70)
			{
				Vector2 startPos = initialPos + new Vector2(0, -10);
				Vector2 endPos = initialPos + new Vector2(0, 64);
				float progress = (timer - 50) / 20f;
                progress = EaseFunction.EaseCircularIn.Ease(progress);
                Projectile.Center = Vector2.Lerp(startPos, endPos, progress);
			}
			else if (timer > 200)
			{
				transformed = true;
                Vector2 endPos = initialPos + new Vector2(0, -10);
                Vector2 startPos = initialPos + new Vector2(0, 64);
                float progress = (timer - 200) / 20f;

				if (timer >= 220)
				{
					int newItem = Item.NewItem(Projectile.GetSource_DropAsItem(), Projectile.Center, outputItemID);

					if (Main.netMode == NetmodeID.MultiplayerClient && newItem >= 0)
					{
						NetMessage.SendData(MessageID.SyncItem, -1, -1, null, newItem, 1f);
					}

					Projectile.Kill();
					return;
				}

				progress = EaseFunction.EaseCircularOut.Ease(progress);
                Projectile.Center = Vector2.Lerp(startPos, endPos, progress);
            }
			else if (timer % 10 == 9)
			{
                SoundEngine.PlaySound(SoundID.Splash, Projectile.Center);
            }
			else if (timer == 70)
			{
                var existing = Main.projectile.Where(n => n.active && n.type == ModContent.ProjectileType<CauldronDummy>()).FirstOrDefault();
                if (existing != default)
                {
					(existing.ModProjectile as CauldronDummy).shakeTimer = 130;
                }

                SoundEngine.PlaySound(SoundID.Splash, Projectile.Center);
			}
			else if (timer == 200)
			{
				Projectile.scale = 1;

				SpookyPlayer.ScreenShakeAmount = 8;

				SoundEngine.PlaySound(SoundID.DD2_DarkMageSummonSkeleton, Projectile.Center);

                for (int j = 0; j < 16; j++)
				{
					Vector2 vel = Main.rand.NextVector2Circular(2, 4);
					vel.Y = MathF.Abs(vel.Y) * -1;
					Dust NewDust = Dust.NewDustPerfect(Projectile.Top + new Vector2(Main.rand.Next(-24, 24), 0), ModContent.DustType<CauldronBubble>(), vel, 0, Color.White, Main.rand.NextFloat(0.75f, 1.1f));
					NewDust.color = new Color(33, 220, 48);
					NewDust.noGravity = false;
				}
			}
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
			behindNPCsAndTiles.Add(index);
        }

        public override bool PreDraw(ref Color lightColor)
        {
			Texture2D mainTex = transformed ? outputTex : inputTex;
			Vector2 pos = Projectile.Center - Main.screenPosition;
            Main.spriteBatch.Draw(mainTex, pos, null, lightColor, 0f, mainTex.Size() / 2, Projectile.scale, SpriteEffects.None, 0f);
			return false;
        }
    }
}