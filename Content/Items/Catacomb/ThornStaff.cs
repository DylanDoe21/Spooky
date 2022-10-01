using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

using Spooky.Content.Projectiles.Catacomb;
 
namespace Spooky.Content.Items.Catacomb
{
	public class ThornStaff : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Rod of Ensnaring");
			Tooltip.SetDefault("Casts a lump of thorns that can sometimes ensnare non boss enemies"
			+ "\nEnsnared enemies will not be able to move, and will be poisoned"
			+ "\nEnsnared enemies cannot be ensnared again for 10 seconds");
			Item.staff[Item.type] = true;
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.damage = 25;
			Item.mana = 10;
			Item.DamageType = DamageClass.Magic;
			Item.autoReuse = true;
			Item.noMelee = true;
			Item.width = 54;           
			Item.height = 58;
			Item.useTime = 30;         
			Item.useAnimation = 30;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 2;
			Item.rare = ItemRarityID.Green;
			Item.value = Item.buyPrice(gold: 2);
			Item.UseSound = SoundID.Item17;     
			Item.shoot = ModContent.ProjectileType<ThornStaffBall>();
			Item.shootSpeed = 10f;
		}
	}
}
