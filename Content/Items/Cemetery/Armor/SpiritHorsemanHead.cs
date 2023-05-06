using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

using Spooky.Core;
using Spooky.Content.Projectiles.Cemetery;

namespace Spooky.Content.Items.Cemetery.Armor
{
	[AutoloadEquip(EquipType.Head)]
	public class SpiritHorsemanHead : ModItem
	{
		public override void SetDefaults() 
		{
			Item.defense = 4;
			Item.width = 26;
			Item.height = 28;
			Item.rare = ItemRarityID.Green;
			Item.value = Item.buyPrice(gold: 2);
		}

		public override bool IsArmorSet(Item head, Item body, Item legs) 
		{
			return body.type == ModContent.ItemType<SpiritHorsemanBody>() && legs.type == ModContent.ItemType<SpiritHorsemanLegs>();
		}
		
		public override void UpdateArmorSet(Player player) 
		{
			player.setBonus = Language.GetTextValue("Mods.Spooky.ArmorSetBonus.SpiritHorsemanArmor");
			player.GetModPlayer<SpookyPlayer>().HorsemanSet = true;

			bool NotSpawned = player.ownedProjectileCounts[ModContent.ProjectileType<PumpkinHead>()] <= 0;
			if (NotSpawned)
			{
				//leave the source as null for right now
				Projectile.NewProjectile(null, player.position.X + (float)(player.width / 2), player.position.Y + (float)(player.height / 2), 0f, 0f, 
				ModContent.ProjectileType<PumpkinHead>(), 0, 0f, player.whoAmI, 0f, 0f);
			}
		}

		public override void UpdateEquip(Player player) 
		{
			player.GetCritChance(DamageClass.Generic) += 5;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<SpookyPlasma>(), 10)
			.AddIngredient(ItemID.Silk, 12)
            .AddTile(TileID.Anvils)
            .Register();
        }
	}
}