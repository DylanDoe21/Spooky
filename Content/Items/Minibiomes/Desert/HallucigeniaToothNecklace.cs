using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.Shaders;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Core;

namespace Spooky.Content.Items.Minibiomes.Desert
{
	public class HallucigeniaToothNecklace : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 26;
			Item.height = 42;
			Item.value = Item.buyPrice(gold: 20);
			Item.rare = ItemRarityID.LightPurple;
			Item.accessory = true;
		}

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
			player.GetModPlayer<SpookyPlayer>().HallucigeniaSpine = true;
        }

		public override void AddRecipes()
        {
            CreateRecipe()
			.AddIngredient(ItemID.SharkToothNecklace)
            .AddIngredient(ModContent.ItemType<HallucigeniaSpine>())
            .AddTile(TileID.TinkerersWorkbench)
            .Register();
        }
	}
}