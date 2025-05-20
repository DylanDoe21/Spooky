using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Items.Minibiomes.Vegetable;
using Spooky.Content.Projectiles.Minibiomes.Vegetable;

namespace Spooky.Content.Items.Minibiomes.Armor
{
	[AutoloadEquip(EquipType.Head)]
	public class HazmatHead : ModItem
	{
		public override void SetDefaults() 
		{
			Item.defense = 10;
			Item.width = 26;
			Item.height = 30;
			Item.rare = ItemRarityID.Pink;
		}

		public override bool IsArmorSet(Item head, Item body, Item legs) 
		{
			return body.type == ModContent.ItemType<HazmatBody>() && legs.type == ModContent.ItemType<HazmatLegs>();
		}
		
		public override void UpdateArmorSet(Player player) 
		{
			player.setBonus = Language.GetTextValue("Mods.Spooky.ArmorSetBonus.HazmatArmor");
			player.GetModPlayer<SpookyPlayer>().HazmatSet = true;

			int MaxMinions = 0;

			foreach (string var in player.GetModPlayer<BloomBuffsPlayer>().BloomBuffSlots)
			{
				if (var != string.Empty)
				{
					MaxMinions++;
				}
			}

			if (player.ownedProjectileCounts[ModContent.ProjectileType<HazmatArmorMinion>()] < MaxMinions)
			{
				Projectile.NewProjectile(null, player.Center, Vector2.Zero, ModContent.ProjectileType<HazmatArmorMinion>(), 60, 0f, player.whoAmI);
			}
		}

		public override void UpdateEquip(Player player) 
		{
		}

		public override void AddRecipes()
        {
            CreateRecipe()
			.AddIngredient(ItemID.HallowedBar, 12)
            .AddIngredient(ModContent.ItemType<PlantMulch>(), 40)
            .AddTile(TileID.MythrilAnvil)
            .Register();
        }
	}
}