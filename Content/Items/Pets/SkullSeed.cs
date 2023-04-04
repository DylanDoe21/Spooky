using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

using Spooky.Content.Projectiles.Pets;
using Spooky.Content.Buffs.Pets;

namespace Spooky.Content.Items.Pets
{
	public class SkullSeed : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Skull Seed");
			/* Tooltip.SetDefault("Summons a baby bone to provide light"
			+ "\n'Will it grow into a plant, or a skull?'"); */
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.Fish);
			Item.width = 30;
			Item.height = 30;
			Item.master = true;
			Item.rare = ItemRarityID.Master;
			Item.value = Item.buyPrice(gold: 5);
			Item.shoot = ModContent.ProjectileType<BigBonePet>();
			Item.buffType = ModContent.BuffType<BigBonePetBuff>();
		}

		public override bool? UseItem(Player player)
        {
			player.AddBuff(Item.buffType, 2);
			return true;
        }
	}
}