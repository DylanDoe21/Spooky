using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Linq;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Tiles.SpookyBiome;
using Spooky.Content.Tiles.SpiderCave;

namespace Spooky.Content.Items.Debug
{
    public class Converter : ModItem
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
            for (int i = (int)(player.Center.X / 16) - 20; i <= (int)(player.Center.X / 16) + 20; i++)
            {
                for (int j = (int)(player.Center.Y / 16) - 20; j <= (int)(player.Center.Y / 16) + 20; j++)
                {
                    Tile tile = Main.tile[i, j];
                    if (tile.WallType == ModContent.WallType<DampGrassWall>())
                    {
                        WorldGen.KillWall(i, j);
                    }
                }
            }

			return true;
		}
    }
}