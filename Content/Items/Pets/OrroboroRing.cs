using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles.Pets;
using Spooky.Content.Buffs.Pets;

namespace Spooky.Content.Items.Pets
{
	public class OrroboroRing : ModItem
	{
		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.Fish);
			Item.width = 32;
			Item.height = 38;
			Item.master = true;
			Item.rare = ItemRarityID.Master;
			Item.value = Item.buyPrice(gold: 5);
			Item.shoot = ModContent.ProjectileType<OrroboroPet>();
			Item.buffType = ModContent.BuffType<OrroboroPetBuff>();
		}

		public override bool? UseItem(Player player)
		{
			if (player.whoAmI == Main.myPlayer)
            {
                player.AddBuff(Item.buffType, 2, true);
            }

			return true;
        }
	}
}