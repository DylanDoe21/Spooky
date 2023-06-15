using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

using Spooky.Core;
using Spooky.Content.Tiles.SpookyBiome;

namespace Spooky.Content.Items.SpookyBiome.Armor
{
	[LegacyName("SpookyHead")]
	[AutoloadEquip(EquipType.Head)]
	public class GourdHead : ModItem
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
			return body.type == ModContent.ItemType<GourdBody>() && legs.type == ModContent.ItemType<GourdLegs>();
		}
		
		public override void UpdateArmorSet(Player player) 
		{
			player.setBonus = Language.GetTextValue("Mods.Spooky.ArmorSetBonus.GourdArmor");
			player.GetModPlayer<SpookyPlayer>().GourdSet = true;
		}

		public override void UpdateEquip(Player player) 
		{
			player.GetCritChance(DamageClass.Melee) += 2;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<RottenChunk>(), 12)
			.AddIngredient(ModContent.ItemType<SpookyWoodItem>(), 20)
            .AddTile(TileID.WorkBenches)
            .Register();
        }
	}
}