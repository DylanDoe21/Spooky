using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

using Spooky.Core;
using Spooky.Content.Projectiles.SpookyBiome;

namespace Spooky.Content.Items.SpookyBiome.Armor
{
	[AutoloadEquip(EquipType.Head)]
	public class SpookyHead : ModItem
	{
		public override void SetStaticDefaults() 
		{
			DisplayName.SetDefault("Spooky Horseman's Pumpkin");
			Tooltip.SetDefault("2% increased critical strike chance"
			+ "\nEnemies are more likely to target you");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults() 
		{
			Item.defense = 3;
			Item.width = 18;
			Item.height = 18;
			Item.value = Item.buyPrice(gold: 2);
			Item.rare = 1;
		}

		public override bool IsArmorSet(Item head, Item body, Item legs) 
		{
			return body.type == ModContent.ItemType<SpookyBody>() && legs.type == ModContent.ItemType<SpookyLegs>();
		}
		
		public override void UpdateArmorSet(Player player) 
		{
			player.setBonus = "\nYour own head will now fight for you!";
			player.GetModPlayer<SpookyPlayer>().SpookySet = true;

			bool NotSpawned = player.ownedProjectileCounts[ModContent.ProjectileType<PumpkinHead>()] <= 0;
			if (NotSpawned && player.whoAmI == Main.myPlayer)
			{
				//leave the source as null for right now
				Projectile.NewProjectile(null, player.position.X + (float)(player.width / 2), player.position.Y + (float)(player.height / 2), 0f, 0f, 
				ModContent.ProjectileType<PumpkinHead>(), 0, 0f, player.whoAmI, 0f, 0f);
			}
		}

		public override void ArmorSetShadows(Player player)
		{
			player.armorEffectDrawShadow = true;
		}

		public override void UpdateEquip(Player player) 
		{
			player.GetCritChance<GenericDamageClass>() += 2;
			player.aggro += 100;
		}
	}
}
