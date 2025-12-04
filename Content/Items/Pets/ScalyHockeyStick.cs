using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles.Pets;
using Spooky.Content.Buffs.Pets;

namespace Spooky.Content.Items.Pets
{
	public class ScalyHockeyStick : ModItem
	{
		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.Fish);
			Item.width = 26;
			Item.height = 30;
			Item.shoot = ModContent.ProjectileType<LongisquamaPet>();
			Item.buffType = ModContent.BuffType<LongisquamaPetBuff>();
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