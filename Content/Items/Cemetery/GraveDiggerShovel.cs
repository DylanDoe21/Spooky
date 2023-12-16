using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.UI.Chat;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Content.Projectiles.Cemetery;
 
namespace Spooky.Content.Items.Cemetery
{
	public class GraveDiggerShovel : ModItem
	{
		public override void SetDefaults()
		{
			Item.damage = 20;
			Item.DamageType = DamageClass.Ranged;
			Item.noMelee = true;
			Item.autoReuse = true;
			Item.noUseGraphic = true;
			Item.width = 42;           
			Item.height = 42;
			Item.useTime = 12;         
			Item.useAnimation = 12;
			Item.useStyle = ItemUseStyleID.Swing;       
			Item.knockBack = 7;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.buyPrice(gold: 1);
			Item.UseSound = SoundID.Item1;
			Item.shoot = ModContent.ProjectileType<GraveDiggerShovelProj>();
			Item.shootSpeed = 12f;
		}

        public override bool CanUseItem(Player player)
        {
			return true;
		}
	}
}
