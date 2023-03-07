using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

using Spooky.Content.Projectiles.Pets;
using Spooky.Content.Buffs.Pets;

namespace Spooky.Content.Items.Pets
{
	public class FuzzBatWing : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Fuzz Bat Wing");
			Tooltip.SetDefault("Summons a goofy fuzz bat pet");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.Fish);
			Item.width = 20;
			Item.height = 20;
			Item.shoot = ModContent.ProjectileType<FuzzBatPet>();
			Item.buffType = ModContent.BuffType<FuzzBatPetBuff>();
		}

        public override bool? UseItem(Player player)
        {
			player.AddBuff(Item.buffType, 2);
			return true;
        }

        public override bool CanUseItem(Player player)
		{
			return player.miscEquips[0].IsAir;
		}
	}
}