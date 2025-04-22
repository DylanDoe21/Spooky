using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

using Spooky.Core;
using Spooky.Content.Projectiles.Minibiomes.Ocean;

namespace Spooky.Content.Items.Minibiomes.Armor
{
	[AutoloadEquip(EquipType.Head)]
	public class SharkBoneHead : ModItem
	{
		public override void SetDefaults() 
		{
			Item.defense = 4;
			Item.width = 28;
			Item.height = 18;
			Item.rare = ItemRarityID.Blue;
		}

		public override bool IsArmorSet(Item head, Item body, Item legs) 
		{
			return body.type == ModContent.ItemType<SharkBoneBody>() && legs.type == ModContent.ItemType<SharkBoneLegs>();
		}
		
		public override void UpdateArmorSet(Player player) 
		{
			player.setBonus = Language.GetTextValue("Mods.Spooky.ArmorSetBonus.SharkBoneArmor");
			player.GetModPlayer<SpookyPlayer>().SharkBoneSet = true;

			bool NotSpawned = player.ownedProjectileCounts[ModContent.ProjectileType<SharkArmorFriend>()] <= 0;
			if (NotSpawned && player.whoAmI == Main.myPlayer)
			{
				Projectile.NewProjectile(null, player.position.X + (float)(player.width / 2), player.position.Y - 25, 0f, 0f, ModContent.ProjectileType<SharkArmorFriend>(), 25, 7f, player.whoAmI);
			}
		}

		public override void UpdateEquip(Player player) 
		{
			player.GetDamage(DamageClass.Generic) += 0.08f;
			player.fishingSkill += 4;
		}

		/*
		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<SpookyWoodItem>(), 20)
            .AddTile(TileID.WorkBenches)
            .Register();
        }
		*/
	}
}