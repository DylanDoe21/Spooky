using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles.Pets;
using Spooky.Content.Buffs.Pets;

namespace Spooky.Content.Items.Pets
{
	public class PetscopTool1 : ModItem
	{
		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.Fish);
			Item.width = 36;
			Item.height = 36;
			Item.shoot = ModContent.ProjectileType<PetscopPet>();
			Item.buffType = ModContent.BuffType<PetscopBuff>();
		}

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
			if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
            {
                player.AddBuff(Item.buffType, 2, true);
            }
        }
    }

	public class PetscopTool2 : ModItem
	{
		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.Fish);
			Item.width = 36;
			Item.height = 36;
			Item.shoot = ModContent.ProjectileType<PetscopMarvinPet>();
			Item.buffType = ModContent.BuffType<PetscopMarvinBuff>();
		}

        public override bool? UseItem(Player player)
        {
			player.AddBuff(Item.buffType, 2);
			return true;
        }
    }

	public class PetscopTool3 : ModItem
	{
		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.Fish);
			Item.width = 36;
			Item.height = 36;
			Item.shoot = ModContent.ProjectileType<PetscopTiaraPet>();
			Item.buffType = ModContent.BuffType<PetscopTiaraBuff>();
		}

        public override bool? UseItem(Player player)
        {
			player.AddBuff(Item.buffType, 2);
			return true;
        }
    }
}