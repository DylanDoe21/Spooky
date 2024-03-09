using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

using Spooky.Core;
using Spooky.Content.Projectiles.SpookyHell;
using Spooky.Content.Tiles.SpookyHell;

namespace Spooky.Content.Items.SpookyHell.Armor
{
	[AutoloadEquip(EquipType.Head)]
	public class EyeHead : ModItem, IHelmetGlowmask
	{
		public string GlowmaskTexture => "Spooky/Content/Items/SpookyHell/Armor/EyeHead_Glow";

		public override void SetDefaults() 
		{
			Item.defense = 3;
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

			bool NotSpawned = player.ownedProjectileCounts[ModContent.ProjectileType<EyeArmorMinion>()] <= 0;
			if (NotSpawned)
			{
				var realDamage = 20;
                var scaledDamage = (int)player.GetTotalDamage<SummonDamageClass>().ApplyTo(20);

				int smallEye = Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.Center.X, 
				player.position.Y, 0, 0 ,ModContent.ProjectileType<EyeArmorMinion>(), scaledDamage, 0, player.whoAmI);

				Main.projectile[smallEye].originalDamage = realDamage;
			}
		}

		public override void UpdateEquip(Player player) 
		{
			player.GetDamage(DamageClass.Summon) += 0.03f;
			player.GetDamage(DamageClass.SummonMeleeSpeed) += 0.03f;
			player.maxMinions += 1;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
			.AddIngredient(ItemID.DemoniteBar, 5)
            .AddIngredient(ModContent.ItemType<EyeBlockItem>(), 25)
			.AddIngredient(ModContent.ItemType<LivingFleshItem>(), 50)
            .AddTile(TileID.Anvils)
            .Register();

			CreateRecipe()
			.AddIngredient(ItemID.CrimtaneBar, 5)
            .AddIngredient(ModContent.ItemType<EyeBlockItem>(), 25)
			.AddIngredient(ModContent.ItemType<LivingFleshItem>(), 50)
            .AddTile(TileID.Anvils)
            .Register();
        }
	}
}
