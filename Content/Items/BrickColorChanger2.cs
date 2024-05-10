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
    public class BrickColorChanger2 : ModItem
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
                    if (tile.TileType == ModContent.TileType<NoseTempleBrickGreen>())
                    {
                        Main.tile[i, j].TileType = (ushort)ModContent.TileType<NoseTempleBrickRed>();
                    }

                    //fancy bricks
                    if (tile.TileType == ModContent.TileType<NoseTempleFancyBrickGreen>())
                    {
                        Main.tile[i, j].TileType = (ushort)ModContent.TileType<NoseTempleFancyBrickRed>();
                    }

                    //walls
                    if (tile.WallType == ModContent.WallType<NoseTempleWallGreen>())
                    {
                        Main.tile[i, j].WallType = (ushort)ModContent.WallType<NoseTempleWallRed>();
                    }

                    //fancy walls
                    if (tile.WallType == ModContent.WallType<NoseTempleFancyWallGreen>())
                    {
                        Main.tile[i, j].WallType = (ushort)ModContent.WallType<NoseTempleFancyWallRed>();
                    }

                    //BG walls
                    if (tile.WallType == ModContent.WallType<NoseTempleWallBGGreen>())
                    {
                        Main.tile[i, j].WallType = (ushort)ModContent.WallType<NoseTempleWallBGRed>();
                    }
                }
            }

            return true;
        }
    }
}