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
using Spooky.Content.Items.Pets;
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

		public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
			Player player = Main.LocalPlayer;

			//always drop some swamp rock
			Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 16, ModContent.ItemType<CemeteryStoneItem>(), Main.rand.Next(3, 10));

			//guaranteed goodie bags during a raveyard
			if (player.InModBiome<RaveyardBiome>())
			{
				Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 16, ItemID.GoodieBag);
			}
			//spawn a mist ghost sometimes
            else if (Main.rand.NextBool(4))
            {
				int x = i;
                int y = j;
                while (Main.tile[x, y].TileType == Type) x--;
                x++;
                while (Main.tile[x, y].TileType == Type) y--;
                y++;

                int SpawnX = x * 16;
                int SpawnY = y * 16;

				Projectile.NewProjectile(new EntitySource_TileInteraction(player, x * 16, y * 16), 
                SpawnX, SpawnY, 0, 0, ModContent.ProjectileType<MistGhostSpawn>(), 0, 0, Main.myPlayer);
            }
			//otherwise drop random stuff
			else
			{
				switch (Main.rand.Next(5))
				{
					//cobwebs
					case 0:
					{
						Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 16, ItemID.Cobweb, Main.rand.Next(10, 21));
						break;
					}
					//torches
					case 1:
					{
						Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 16, ItemID.Torch, Main.rand.Next(3, 9));
						break;
					}
					//maggots
					case 2:
					{
						Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 16, ItemID.Maggot, Main.rand.Next(1, 4));
						break;
					}
				}

				if (Main.rand.NextBool(30))
				{
					Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 16, ModContent.ItemType<DissolvedBone>(), 1);
				}
			}
        }
    }
}