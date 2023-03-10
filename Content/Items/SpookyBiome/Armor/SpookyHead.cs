using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

using Spooky.Core;
using Spooky.Content.Items.SpookyBiome;
using Spooky.Content.Projectiles.SpookyBiome;
using Spooky.Content.Tiles.SpookyBiome;

namespace Spooky.Content.Items.SpookyBiome.Armor
{
	[AutoloadEquip(EquipType.Head)]
	public class SpookyHead : ModItem
	{
		public override void SetStaticDefaults() 
		{
			DisplayName.SetDefault("Rotten Pumpkin Head");
			Tooltip.SetDefault("2% increased critical strike chance");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults() 
		{
			Item.defense = 2;
			Item.width = 24;
			Item.height = 26;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.buyPrice(gold: 2);
		}

		public override bool IsArmorSet(Item head, Item body, Item legs) 
		{
			return body.type == ModContent.ItemType<SpookyBody>() && legs.type == ModContent.ItemType<SpookyLegs>();
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
			player.GetCritChance(DamageClass.Generic) += 2;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<RottenChunk>(), 10)
            .AddTile(TileID.Anvils)
            .Register();
        }
	}
}