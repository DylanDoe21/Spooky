using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

using Spooky.Content.Tiles.SpookyBiome;

namespace Spooky.Content.Items.SpookyBiome.Armor
{
	[AutoloadEquip(EquipType.Head)]
	public class OldWoodHead : ModItem
	{
		public override void SetDefaults() 
		{
			Item.defense = 1;
			Item.width = 26;
			Item.height = 28;
			Item.rare = ItemRarityID.White;
		}

		public override bool IsArmorSet(Item head, Item body, Item legs) 
		{
			return body.type == ModContent.ItemType<OldWoodBody>() && legs.type == ModContent.ItemType<OldWoodLegs>();
		}
		
		public override void UpdateArmorSet(Player player) 
		{
			LocalizedText Description = Language.GetOrRegister("Mods.Spooky.ArmorSetBonus.OldWoodArmor");
			player.setBonus = Description.Value;
			player.statDefense += 1;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<SpookyWoodItem>(), 20)
            .AddTile(TileID.WorkBenches)
            .Register();
        }
	}
}