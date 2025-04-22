using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

using Spooky.Core;
using Spooky.Content.Items.Minibiomes.Vegetable;

namespace Spooky.Content.Items.Minibiomes.Armor
{
	[AutoloadEquip(EquipType.Head)]
	public class BroccoliHead : ModItem
	{
		public override void SetDefaults() 
		{
			Item.defense = 3;
			Item.width = 26;
			Item.height = 28;
			Item.rare = ItemRarityID.Blue;
		}

		public override bool IsArmorSet(Item head, Item body, Item legs) 
		{
			return body.type == ModContent.ItemType<BroccoliBody>() && legs.type == ModContent.ItemType<BroccoliLegs>();
		}
		
		public override void UpdateArmorSet(Player player) 
		{
			player.setBonus = Language.GetTextValue("Mods.Spooky.ArmorSetBonus.BroccoliArmor");
			player.GetModPlayer<SpookyPlayer>().BroccoliSet = true;
		}

		public override void UpdateEquip(Player player) 
		{
			player.maxMinions += 1;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<PlantMulch>(), 32)
            .AddTile(TileID.Anvils)
            .Register();
        }
	}
}