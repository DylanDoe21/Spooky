using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Tiles.NoseTemple;

namespace Spooky.Content.Items
{
    public class BrickColorChanger3 : ModItem
    {
        public override string Texture => "Spooky/Content/Items/BossSummon/CowBell";

        public override void SetDefaults()
        {                
            Item.width = 20;
            Item.height = 28;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 3;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(gold: 1);
        }

        public override bool? UseItem(Player player)
        {
            for (int i = 0; i < Main.maxTilesX; i++)
            {
                for (int j = 0; j < Main.maxTilesY; j++)
                {
                    Tile tile = Main.tile[i, j];

                    //regular bricks
                    if (tile.TileType == ModContent.TileType<NoseTempleBrickRed>())
                    {
                        Main.tile[i, j].TileType = (ushort)ModContent.TileType<NoseTempleBrickPurple>();
                    }

                    //fancy bricks
                    if (tile.TileType == ModContent.TileType<NoseTempleFancyBrickRed>())
                    {
                        Main.tile[i, j].TileType = (ushort)ModContent.TileType<NoseTempleFancyBrickPurple>();
                    }

                    //walls
                    if (tile.WallType == ModContent.WallType<NoseTempleWallRed>())
                    {
                        Main.tile[i, j].WallType = (ushort)ModContent.WallType<NoseTempleWallPurple>();
                    }

                    //fancy walls
                    if (tile.WallType == ModContent.WallType<NoseTempleFancyWallRed>())
                    {
                        Main.tile[i, j].WallType = (ushort)ModContent.WallType<NoseTempleFancyWallPurple>();
                    }

                    //BG walls
                    if (tile.WallType == ModContent.WallType<NoseTempleWallBGRed>())
                    {
                        Main.tile[i, j].WallType = (ushort)ModContent.WallType<NoseTempleWallBGPurple>();
                    }
                }
            }

            return true;
        }
    }
}