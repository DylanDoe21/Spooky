using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

using Spooky.Core;
using Spooky.Content.Projectiles.SpookyHell;
using Spooky.Content.Tiles.SpookyHell;

namespace Spooky.Content.Items.SpookyHell.Armor
{
	[AutoloadEquip(EquipType.Head)]
	public class EyeHead : ModItem
	{
		public override void SetStaticDefaults() 
		{
			// DisplayName.SetDefault("Eye Mask");
			/* Tooltip.SetDefault("3% increased summon damage"
			+ "\nIncreases your max minions by 1"); */
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults() 
		{
			Item.defense = 3;
			Item.width = 24;
			Item.height = 22;
			Item.rare = ItemRarityID.Green;
			Item.value = Item.buyPrice(gold: 2);
		}

		public override bool IsArmorSet(Item head, Item body, Item legs) 
		{
			return body.type == ModContent.ItemType<EyeBody>() && legs.type == ModContent.ItemType<EyeLegs>();
		}
		
		public override void UpdateArmorSet(Player player) 
		{
			player.setBonus = "Summons a tiny floating eye to protect you"
			+ "\nThe eye does not take up any minion slots";
			player.GetModPlayer<SpookyPlayer>().EyeArmorSet = true;

			bool NotSpawned = player.ownedProjectileCounts[ModContent.ProjectileType<EyeArmorMinion>()] <= 0;
			if (NotSpawned)
			{
				var realDamage = 20;
                var scaledDamage = (int)player.GetTotalDamage<SummonDamageClass>().ApplyTo(20);

				int smallEye = Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.position.X + (float)(player.width / 2), 
				player.position.Y + (float)(player.height / 2), 0f, 0f,ModContent.ProjectileType<EyeArmorMinion>(), scaledDamage, 0f, player.whoAmI, 0f, 0f);

				Main.projectile[smallEye].originalDamage = realDamage;
			}
		}

		public override void ArmorSetShadows(Player player)
		{
			player.armorEffectDrawOutlines = true;
		}

		public override void UpdateEquip(Player player) 
		{
			player.GetDamage(DamageClass.Summon) += 0.03f;
			player.maxMinions += 1;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<EyeBlockItem>(), 25)
			.AddIngredient(ModContent.ItemType<LivingFleshItem>(), 50)
            .AddTile(TileID.Anvils)
            .Register();
        }
	}
}
