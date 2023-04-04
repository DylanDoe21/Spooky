using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

using Spooky.Core;
using Spooky.Content.Projectiles.Cemetery;

namespace Spooky.Content.Items.Cemetery.Armor
{
	[AutoloadEquip(EquipType.Head)]
	public class SpiritHorsemanHead : ModItem
	{
		public override void SetStaticDefaults() 
		{
			// DisplayName.SetDefault("Spirit Horseman's Pumpkin");
			// Tooltip.SetDefault("5% increased critical strike chance");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

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
			player.setBonus = "Your own head will now fight with you!";
			player.GetModPlayer<SpookyPlayer>().SpookySet = true;

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
	}
}