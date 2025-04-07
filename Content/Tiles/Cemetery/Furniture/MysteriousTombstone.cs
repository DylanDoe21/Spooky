using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.Enums;
using ReLogic.Content;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Content.Biomes;
using Spooky.Content.NPCs.Cemetery;
using Spooky.Content.NPCs.Cemetery.Projectiles;

namespace Spooky.Content.Tiles.Cemetery.Furniture
{
    public class MysteriousTombstone : ModTile
    {
        private static Asset<Texture2D> GlowTexture;

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
			Main.tileLighted[Type] = true;
			Main.tileSolid[Type] = false;
			TileObjectData.newTile.Width = 2;
			TileObjectData.newTile.Height = 3;	
			TileObjectData.newTile.Origin = new Point16(0, 2);
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 };
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.addTile(Type);
			LocalizedText name = CreateMapEntryName();
			AddMapEntry(new Color(244, 84, 16), name);
            DustType = DustID.Stone;
            HitSound = SoundID.Tink;
			MinPick = int.MaxValue;
        }

		public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            GlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/Cemetery/Furniture/MysteriousTombstoneOutline");

            Tile tile = Framing.GetTileSafely(i, j);
			Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);

			float glowspeed = Main.GameUpdateCount * 0.02f;
			float glowbrightness = Main.LocalPlayer.InModBiome(ModContent.GetInstance<RaveyardBiome>()) ? 1f : (float)MathF.Sin(j / 10f - glowspeed);

			spriteBatch.Draw(GlowTexture.Value, new Vector2(i * 16, j * 16 + 2) - Main.screenPosition + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16),
			(Main.LocalPlayer.InModBiome(ModContent.GetInstance<RaveyardBiome>()) ? Main.DiscoColor : Color.OrangeRed) * glowbrightness);
		}

		public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
		{
			Player player = Main.LocalPlayer;

			Tile tile = Framing.GetTileSafely(i, j);

			int left = i - tile.TileFrameX / 18 % 2;
			int top = j - tile.TileFrameY / 18 % 3;

			bool CanSpawn = true;

			foreach (var Proj in Main.ActiveProjectiles)
			{
				for (int x = left; x < left + 2; x++)
				{
					for (int y = top; y < top + 3; y++)
					{
						if (Proj.type == ModContent.ProjectileType<MistGhostSpawn>() && Proj.Hitbox.Intersects(new Rectangle(x * 16, y * 16, 16, 16)))
						{
							CanSpawn = false;
							break;
						}
					}
				}
			}

			if (CanSpawn && !NPC.AnyNPCs(ModContent.NPCType<MistGhost>()) && !NPC.AnyNPCs(ModContent.NPCType<MistGhostFaces>()) && !NPC.AnyNPCs(ModContent.NPCType<MistGhostWiggle>()))
			{
				int SpawnX = (left * 16) + 16;
				int SpawnY = (top * 16) + 20;

				//spawn a mist ghost ambush
				Projectile.NewProjectile(new EntitySource_TileInteraction(player, SpawnX, SpawnY), new Vector2(SpawnX, SpawnY),
				Vector2.Zero, ModContent.ProjectileType<MistGhostSpawn>(), 0, 0);
			}
		}
    }
}