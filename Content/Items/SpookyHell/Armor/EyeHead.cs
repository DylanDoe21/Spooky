using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

using Spooky.Core;
using Spooky.Content.Items.SpookyHell.Misc;
using Spooky.Content.Tiles.SpookyHell;

namespace Spooky.Content.Items.SpookyHell.Armor
{
	[AutoloadEquip(EquipType.Head)]
	public class EyeHead : ModItem, ISpecialHelmetDraw
	{
		public string GlowTexture => "Spooky/Content/Items/SpookyHell/Armor/EyeHead_Glow";

		public override void SetDefaults() 
		{
			Item.defense = 4;
			Item.width = 38;
			Item.height = 28;
			Item.rare = ItemRarityID.Green;
			Item.value = Item.buyPrice(gold: 2);
		}

		public override bool IsArmorSet(Item head, Item body, Item legs) 
		{
			return body.type == ModContent.ItemType<EyeBody>() && legs.type == ModContent.ItemType<EyeLegs>();
		}
		
		public override void UpdateArmorSet(Player player) 
		{
			player.setBonus = Language.GetTextValue("Mods.Spooky.ArmorSetBonus.EyeArmor");
			player.GetModPlayer<SpookyPlayer>().EyeArmorSet = true;
			player.whipRangeMultiplier += 0.15f;
		}

		public override void UpdateEquip(Player player) 
		{
			player.GetDamage(DamageClass.Summon) += 0.08f;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
			.AddRecipeGroup("SpookyMod:DemoniteBars", 8)
			.AddIngredient(ModContent.ItemType<CreepyChunk>(), 20)
			.AddIngredient(ModContent.ItemType<LivingFleshItem>(), 50)
            .AddTile(TileID.Anvils)
            .Register();
        }
	}
}
