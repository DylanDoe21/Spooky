using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles.Catacomb;
 
namespace Spooky.Content.Items.Catacomb
{
	public class BigBoneBow : ModItem
	{
		public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<BigBoneStaff>();
        }

		public override void SetDefaults()
		{
			Item.damage = 100;    
			Item.DamageType = DamageClass.Ranged;  
			Item.noMelee = true;
			Item.autoReuse = true;       
			Item.width = 30;         
			Item.height = 72;        
			Item.useTime = 10;
			Item.useAnimation = 10;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 5;
			Item.rare = ItemRarityID.Yellow;  
			Item.value = Item.buyPrice(gold: 25);
			Item.UseSound = SoundID.Item5;
			Item.shoot = ProjectileID.PurificationPowder;
			Item.shootSpeed = 12f;
			Item.useAmmo = AmmoID.Arrow;
		}

        public override Vector2? HoldoutOffset()
		{
			return new Vector2(-5, 0);
		}

		public override bool AltFunctionUse(Player player)
		{
			return player.ownedProjectileCounts[ModContent.ProjectileType<BowThornFlower>()] < 1;
		}

		public override bool CanUseItem(Player player)
		{
			if (player.altFunctionUse == 2)
			{
				Item.useTime = 50;
				Item.useAnimation = 50;
				Item.UseSound = SoundID.Item5;
			}
			else
			{
				Item.useTime = 10;
				Item.useAnimation = 10;
				Item.UseSound = SoundID.Item5;
			}

			return true;
		}

		public override bool CanConsumeAmmo(Item ammo, Player player)
		{
			return !Main.rand.NextBool(10);
		}

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) 
		{	
			if (player.altFunctionUse == 2)
			{
				type = ModContent.ProjectileType<BowThornFlower>();
			}
			else
			{
				type = ModContent.ProjectileType<BowFlower>();
			}
		}
	}
}
